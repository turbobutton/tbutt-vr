using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt
{
    public class TBTrackingNodeOculus : TBTrackingNodeBase
    {
        #if TB_OCULUS
        protected OVRInput.Controller _controller;

        protected override void OnEnable()
        {
            base.OnEnable();
            TBCore.Events.OnSystemMenu += UpdateHandedness;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TBCore.Events.OnSystemMenu -= UpdateHandedness;
        }

        /// <summary>
        /// This is an optional code path we might enable at some point to use the native arm model on Gear VR, rather than the custom one.
        /// </summary>
        /// <param name="needed"></param>
        void UpdateHandedness(bool needed)
        {
            if (!needed || (_node != XRNode.GameController))
                return;

            switch (TBInput.Get3DOFHandedness())
            {
                case TBInput.Mobile3DOFHandedness.Left:
                    if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
                        _controller = OVRInput.Controller.LTouch;
                    else
                        _controller = OVRInput.Controller.LTrackedRemote;
                    break;
                case TBInput.Mobile3DOFHandedness.Right:
                    if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
                        _controller = OVRInput.Controller.RTouch;
                    else
                        _controller = OVRInput.Controller.RTrackedRemote;
                    break;
            }
        }

        public override void TrackNode(XRNode node)
        {
            switch(node)
            {
                case XRNode.LeftHand:
                    _controller = OVRInput.Controller.LTouch;
                    break;
                case XRNode.RightHand:
                    _controller = OVRInput.Controller.RTouch;
                    break;
                case XRNode.GameController:
                    UpdateHandedness(true);
                    break;
            }

            base.TrackNode(node);
        }

        protected override void OnUpdatePoses()
        {
            if (!_overridePosition)
                transform.localPosition = OVRInput.GetLocalControllerPosition(_controller);
            else
                transform.position = _positionTarget.position;
            transform.localRotation = OVRInput.GetLocalControllerRotation(_controller);
        }
        #endif
    }
}