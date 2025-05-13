using UnityEngine;
using UnityEditor;
    
[CustomEditor(typeof(IK_Walker_Manager))]
public class IK_Walker_Manager_Editor : Editor {

    private const int spacing = 20;

    private const string
    label_dzTimeOutLimit = "Timeout Threshold", tooltip_dzTimeOutLimit = "How long the IK must be stationary/in a deadzone for all feet to return to default positions.",
    label_dzDirectionLimit = "Moving Threshold", tooltip_dzDirectionLimit = "Deadzone will be triggered if manager direction magnitude is below this value. Recommended value is 0.1",
    label_UseGroups = "Use Foot Group Values", tooltip_UseGroups = "Determines if feet should use animation parameters from the manager or their foot group.",
    label_StepDistance = "Step Distance", tooltip_StepDistance = "How far the foot will raycast/step.",
    label_StepHeight = "Step Height", tooltip_StepHeight = "How high the foot will raycast/step. This doesn't affect the animation implicitly, rather where the foot can step.",
    label_LerpTimeToTake = "Lerp Time", tooltip_LerpTimeToTake = "How long the foot should take to step.",
    label_BezierMidPoint = "Bezier Mid Point", tooltip_BezierMidPoint = "Foot will lerp between its last position/current position to its new position using this value as a mid point to create a curve. This value is in reference from foot's defualt position.",
    label_Ease = "Ease", tooltip_Ease = "Easing for foot lerp animation.";

    public override void OnInspectorGUI() {
        // Draw default inspector to show foot group scructs, many other public variabls need [HideInInspector] attribute because of this
        DrawDefaultInspector();


        IK_Walker_Manager manager = (IK_Walker_Manager)target;

        EditorGUILayout.LabelField("Dead Zone Parameters");
        manager.deadZoneTimeThreshold = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(label_dzTimeOutLimit, tooltip_dzTimeOutLimit), manager.deadZoneTimeThreshold), 0f, float.MaxValue);
        manager.deadZoneDirectionThreshold = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(label_dzDirectionLimit, tooltip_dzDirectionLimit), manager.deadZoneDirectionThreshold), 0f, 1.1f);

        EditorGUILayout.Space(spacing);
        EditorGUILayout.LabelField("Animation Parameters");
        manager.useStructValues = EditorGUILayout.Toggle(new GUIContent(label_UseGroups, tooltip_UseGroups), manager.useStructValues);

        // Chooses to draw fields for anim params between manager or for each foot group struct
        if (manager.useStructValues) {
            for (int i = 0; i < manager.footGroups.Count; i++) {
                EditorGUILayout.Space(spacing);
                EditorGUILayout.LabelField("Foot Group " + i + " Parameters");
                IK_FootGroup currentGroup = manager.footGroups[i];
                currentGroup.stepDistance = EditorGUILayout.FloatField(new GUIContent(label_StepDistance, tooltip_StepDistance), currentGroup.stepDistance);
                currentGroup.stepHeight = EditorGUILayout.FloatField(new GUIContent(label_StepHeight, tooltip_StepHeight), currentGroup.stepHeight);
                currentGroup.lerpTimeToTake = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(label_LerpTimeToTake, tooltip_LerpTimeToTake), currentGroup.lerpTimeToTake), 0f, float.MaxValue);
                currentGroup.bezierMidPoint = EditorGUILayout.Vector3Field(new GUIContent(label_BezierMidPoint, tooltip_BezierMidPoint), currentGroup.bezierMidPoint);
                currentGroup.ease = (IK_Walker_Manager.eases)EditorGUILayout.EnumPopup(new GUIContent(label_Ease, tooltip_Ease), currentGroup.ease);
                manager.footGroups[i] = currentGroup;
            }
        } else {
            EditorGUILayout.Space(spacing);
            EditorGUILayout.LabelField("Manager Parameters");
            manager.stepDistance = EditorGUILayout.FloatField(new GUIContent(label_StepDistance, tooltip_StepDistance), manager.stepDistance);
            manager.stepHeight = EditorGUILayout.FloatField(new GUIContent(label_StepHeight, tooltip_StepHeight), manager.stepHeight);
            manager.lerpTimeToTake = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(label_LerpTimeToTake, tooltip_LerpTimeToTake), manager.lerpTimeToTake), 0f, float.MaxValue);
            manager.bezierMidPoint = EditorGUILayout.Vector3Field(new GUIContent(label_BezierMidPoint, tooltip_BezierMidPoint), manager.bezierMidPoint);
            manager.ease = (IK_Walker_Manager.eases)EditorGUILayout.EnumPopup(new GUIContent(label_Ease, tooltip_Ease), manager.ease);
        }

        // On click, applies all set values in inspector
        // Only needed if values in anim params are changed during runtime
        EditorGUILayout.Space(spacing);
        if (GUILayout.Button(new GUIContent("Apply Values", "Applies new animation parameters to feet. Use this if you are changing values during play mode."))) {
            manager.ApplyValues();
        }

        EditorUtility.SetDirty(manager);
    }
}