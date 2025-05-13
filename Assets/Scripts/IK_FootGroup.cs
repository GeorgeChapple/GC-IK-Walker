using System.Collections.Generic;
using System;
using UnityEngine;

// Struct used to create foot groups for IK walkers
[Serializable]
public struct IK_FootGroup {
    [HideInInspector] public float stepDistance;
    [HideInInspector] public float stepHeight;
    [HideInInspector] public float lerpTimeToTake;
    [HideInInspector] public Vector3 bezierMidPoint;
    [HideInInspector] public IK_Walker_Manager.eases ease;
    public List<IK_Walker_Foot> feet;
}
