using System.Collections;
using UnityEngine;

public class IK_LittleBug : MonoBehaviour
{
    [SerializeField] private float distanceLimit;
    [SerializeField] private float moveTimeToTake;
    [SerializeField] private float rotateTimeToTake;

    private Vector3 lerpVal;
    private Vector3 rotationLerpVal;
    private Vector3 startVal;
    private Vector3 endVal;
    private Vector3 direction;

    private IK_Walker_Manager manager;

    private void Awake() {
        lerpVal = transform.position;
        manager = transform.GetComponentInChildren<IK_Walker_Manager>();
    }

    private void Start() {
        StartCoroutine(RandomWait());
    }

    void Update()
    {
        direction = endVal - transform.position;
        manager.direction = direction.normalized;
        transform.forward = rotationLerpVal;
        transform.position = lerpVal;
    }

    private IEnumerator RandomWait() {
        yield return new WaitForSeconds(Random.Range(4f, 6f));
        startVal = transform.position;
        endVal = startVal + new Vector3(Random.Range(-distanceLimit, distanceLimit), 0, Random.Range(distanceLimit, distanceLimit));
        StartCoroutine(LerpRot());
        StartCoroutine(LerpPos());
    }

    private IEnumerator LerpPos() {
        float time = 0;
        float perc; 
        float timeElapsed = 0;
        while (time < 1) {
            perc = Easing.Sine.InOut(time);
            lerpVal = Vector3.Lerp(startVal, endVal, perc);
            timeElapsed += Time.deltaTime;
            time = timeElapsed / moveTimeToTake;
            yield return null;
        }
        StartCoroutine(RandomWait());
    }
    private IEnumerator LerpRot() {
        float time = 0;
        float perc;
        float timeElapsed = 0;
        Vector3 startRotVal = transform.forward;
        while (time < 1) {
            perc = Easing.Sine.InOut(time);
            rotationLerpVal = Vector3.Lerp(startRotVal, direction, perc);
            timeElapsed += Time.deltaTime;
            time = timeElapsed / rotateTimeToTake;
            yield return null;
        }
    }
}
