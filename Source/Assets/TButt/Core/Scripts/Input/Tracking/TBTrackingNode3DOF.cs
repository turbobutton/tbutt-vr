using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace TButt
{
    public class TBTrackingNode3DOF : TBTrackingNodeBase
    {
        protected override void OnUpdatePoses()
        {
            transform.localPosition = TB3DOFArmModel.instance.GetHandTransform().position;
            transform.localRotation = TB3DOFArmModel.instance.GetHandTransform().rotation;
        }
    }
}