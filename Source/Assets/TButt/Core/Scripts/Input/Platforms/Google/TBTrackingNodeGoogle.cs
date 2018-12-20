using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt
{
    public class TBTrackingNodeGoogle : TBTrackingNodeBase
    {
        #if TB_GOOGLE
        protected GvrControllerInputDevice _controller;
        protected Vector3 nodePosition;
        protected Quaternion nodeRotation;

        protected override void OnUpdatePoses()
        {
            switch(_node)
            {
                case XRNode.LeftHand:
                    _controller = GvrControllerInput.GetDevice(GvrControllerHand.Left);
                    break;
                case XRNode.RightHand:
                    _controller = GvrControllerInput.GetDevice(GvrControllerHand.Right);
                    break;
            }

            if (_controller != null)
            {
                nodePosition = _controller.Position;
                nodeRotation = _controller.Orientation;
            }

            if (!_overridePosition)
                transform.localPosition = nodePosition;
            else
                transform.position = _positionTarget.position;

            transform.localRotation = nodeRotation;
        }        
        #endif
    }
}