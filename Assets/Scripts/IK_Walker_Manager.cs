using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class IK_Walker_Manager : MonoBehaviour {
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public int steps = 0;

    [HideInInspector] public bool useStructValues;
    [HideInInspector] public float stepDistance;
    [HideInInspector] public float stepHeight;
    [HideInInspector] public float lerpTimeToTake;
    [HideInInspector] public Vector3 bezierMidPoint;
    [HideInInspector] public eases ease;

    [HideInInspector] public bool deadZone;
    [HideInInspector] public float deadZoneTimeThreshold = .05f;
    [HideInInspector] public float deadZoneDirectionThreshold = .1f;
    private bool deadZoneTripped;
    private float deadZoneTimer = 0;

    public delegate float easingDelegate(float t);
    public easingDelegate easing;

    public List<IK_FootGroup> footGroups;

    private int totalFeet = 0;
    private int footGroupIndex = 0;
    private List<IK_Walker_Foot> footGroup;

    public enum eases {
        easeLinear,
        easeInQuad, easeOutQuad, easeInOutQuad,
        easeInSine, easeOutSine, easeInOutSine,
        easeInCubic, easeOutCubic, easeInOutCubic,
        easeInExpo, easeOutExpo, easeInOutExpo,
        easeInBounce, easeOutBounce, easeInOutBounce
    }

    private void Awake() {
        AddManagerToFeet();
    }

    private void Start() {
        ApplyValues();
        footGroup = footGroups[footGroupIndex].feet;
    }

    private void Update() {
        CheckDeadZone();
    }

    // Move all feet in currently selected footgroup, swap foot group when all feet have finished stepping
    private void ProcessFootGroups() {
        for (int i = 0; i < footGroup.Count; i++) {
            footGroup[i].GetNewPosition();
        }
        if (steps >= footGroup.Count) {
            SwapFootGroup();
        }
    }

    // Check if in deadzone, reset feet when not moving, process foot groups when not in deadzone
    private void CheckDeadZone() {
        if (deadZone) {
            if (steps >= totalFeet) {
                deadZone = false;
            }
        } else {
            if (direction.magnitude < deadZoneDirectionThreshold) {
                deadZoneTimer += Time.deltaTime;
                if (deadZoneTimer > deadZoneTimeThreshold && !deadZoneTripped) {
                    deadZone = deadZoneTripped = true;
                    ResetAllFeet();
                }
            } else {
                deadZoneTripped = false;
                deadZoneTimer = 0;
                ProcessFootGroups();
            }
        }
    }

    // Adds this manager to all foot selected in footgroups
    private void AddManagerToFeet() {
        for (int i = 0; i < footGroups.Count; i++) {
            List<IK_Walker_Foot> currentGroup = footGroups[i].feet;
            for (int j = 0; j < currentGroup.Count; j++) {
                currentGroup[j].manager = this;
                totalFeet++;
            }
        }
    }

    // Used in ApplyValues() to send a delegate for the desired easing to the feet
    private easingDelegate GetEase(eases newEase) {
        switch (newEase) {
            case 0:
                return Easing.Linear;
            case (eases)1:
                return Easing.Quad.In;
            case (eases)2:
                return Easing.Quad.Out;
            case (eases)3:
                return Easing.Quad.InOut;
            case (eases)4:
                return Easing.Sine.In;
            case (eases)5:
                return Easing.Sine.Out;
            case (eases)6:
                return Easing.Sine.InOut;
            case (eases)7:
                return Easing.Cubic.In;
            case (eases)8:
                return Easing.Cubic.Out;
            case (eases)9:
                return Easing.Cubic.InOut;
            case (eases)10:
                return Easing.Expo.In;
            case (eases)11:
                return Easing.Expo.Out;
            case (eases)12:
                return Easing.Expo.InOut;
            case (eases)13:
                return Easing.Bounce.In;
            case (eases)14:
                return Easing.Bounce.Out;
            case (eases)15:
                return Easing.Bounce.InOut;
            default:
                return Easing.Linear;
        }
    }

    // Changes the currently selected footgroup
    public void SwapFootGroup() {
        steps = 0;
        footGroupIndex++;
        if (footGroupIndex >= footGroups.Count) {
            footGroupIndex = 0;
        }
        footGroup = footGroups[footGroupIndex].feet;
    }

    // Resets the positions of all feet to their default position
    public void ResetAllFeet() {
        footGroupIndex = 0;
        steps = 0;
        for (int i = 0; i < footGroups.Count; i++) {
            List<IK_Walker_Foot> currentGroup = footGroups[i].feet;
            for (int j = 0; j < currentGroup.Count; j++) {
                currentGroup[j].ResetPositions();
            }
        }
    }

    // Applies animation parameters to the values in each foot
    // Selects between using either the manager values or each foot group's values
    public void ApplyValues() {
        IK_FootGroup currentGroup;
        List<IK_Walker_Foot> currentFootList;
        for (int i = 0; i < footGroups.Count; i++) {
            currentGroup = footGroups[i];
            currentFootList = currentGroup.feet;
            for (int j = 0; j < currentFootList.Count; j++) {
                if (useStructValues) {
                    currentFootList[j].stepDistance = currentGroup.stepDistance;
                    currentFootList[j].stepHeight = currentGroup.stepHeight;
                    currentFootList[j].lerpTimeToTake = currentGroup.lerpTimeToTake;
                    currentFootList[j].bezierMidPoint = currentGroup.bezierMidPoint;
                    currentFootList[j].easing = GetEase(currentGroup.ease);
                } else {
                    currentFootList[j].stepDistance = stepDistance;
                    currentFootList[j].stepHeight = stepHeight;
                    currentFootList[j].lerpTimeToTake = lerpTimeToTake;
                    currentFootList[j].bezierMidPoint = bezierMidPoint;
                    currentFootList[j].easing = GetEase(ease);
                }
            }
        }
    }
}
