using System.Collections;
using UnityEngine;

public class IK_Walker_Foot : MonoBehaviour {
    [HideInInspector] public IK_Walker_Manager manager;
    [HideInInspector] public float stepDistance;
    [HideInInspector] public float stepHeight;
    [HideInInspector] public float lerpTimeToTake;
    [HideInInspector] public Vector3 bezierMidPoint;

    public IK_Walker_Manager.easingDelegate easing;
    private bool lerping = false;

    private Vector3 newPosition;
    private Vector3 midPosition;
    private Vector3 oldPosition;
    private Vector3 currentPosition;
    private Transform rayCastPoint;

    private void Awake() {
        rayCastPoint = transform.parent;
        oldPosition = transform.position;
    }

    private void Start() {
        if (manager == null) {
            Debug.LogWarning("Walker foot is not assinged to a footgroup!");
        }
    }

    // Updates position so that foot doesnt get dragged along with parent transforms
    private void Update() {
        transform.position = currentPosition;
    }

    // Draw debug spheres for Bezier lerp mid points
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(midPosition, .1f);
    }

    // Uses raycasts to predict where to place the foot next
    // Starts lerping the foot target position to raycast point when distance between raycast and foot target is large enough
    public void GetNewPosition() {
        Ray ray = new Ray();
        ray = new Ray(rayCastPoint.position + (Vector3.up * stepHeight) + (manager.direction * stepDistance), Vector3.down);
        Debug.DrawRay(rayCastPoint.position + (Vector3.up * stepHeight) + (manager.direction * stepDistance), Vector3.down, new Color(0, 255, 0));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f)) {
            if ((Vector3.Distance(newPosition, hitInfo.point) > stepDistance) && Vector3.Distance(newPosition, rayCastPoint.position) > stepDistance / 2) {
                if (!lerping && !manager.deadZone) {
                    newPosition = hitInfo.point;
                    midPosition = rayCastPoint.position + new Vector3(bezierMidPoint.x * manager.direction.x, bezierMidPoint.y, bezierMidPoint.z * manager.direction.z);
                    StartCoroutine(LerpFoot());
                }
            }
        }
    }

    // Lerps foot from current position back to its default position
    public void ResetPositions() {
        StopAllCoroutines();
        oldPosition = transform.position;
        newPosition = rayCastPoint.position;
        midPosition = rayCastPoint.position + new Vector3(bezierMidPoint.x * manager.direction.x, bezierMidPoint.y, bezierMidPoint.z * manager.direction.z);
        StartCoroutine(LerpFoot());
    }

    // Lerps the foot to new position using easing delegate
    private IEnumerator LerpFoot() {
        lerping = true;
        float time = 0;
        float timeElapsed = 0;
        float perc = 0;
        Vector3 bezierPosition1;
        Vector3 bezierPosition2;
        Vector3 lerpPosition;
        while (time < 1) {
            perc = easing(time);
            bezierPosition1 = Vector3.Lerp(oldPosition, midPosition, perc);
            bezierPosition2 = Vector3.Lerp(midPosition, newPosition, perc);
            lerpPosition = Vector3.Lerp(bezierPosition1, bezierPosition2, perc);
            currentPosition = lerpPosition;
            timeElapsed += Time.deltaTime;
            time = timeElapsed / lerpTimeToTake;
            yield return null;
        }
        oldPosition = newPosition;
        lerping = false;
        manager.steps++;
    }
}
