using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_TouchLeft : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_TouchLeft _instance;

        public static TBController_Oculus_TouchLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_TouchLeft();
                    switch(TBCore.GetActiveHeadset())
                    {
                        case VRHeadset.OculusRift:
                            _instance.name = "Oculus Touch V1 (Left)";
                            _instance.model = VRController.OculusTouchV1;
                            break;
                        default:
                            _instance.name = "Oculus Touch V2 (Left)";
                            _instance.model = VRController.OculusTouchV2;
                            break;
                    }
                    _instance.fileName = "Maps_Oculus_TouchLeft";
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
        }
        protected override void SetFingerPoseButtons()
        {
            if (_instance.model == VRController.OculusTouchV1)
                _instance.thumbPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.X, OVRInput.RawButton.Y, OVRInput.RawButton.LThumbstick, OVRInput.RawButton.LShoulder };
            else
                _instance.thumbPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.X, OVRInput.RawButton.Y, OVRInput.RawButton.LThumbstick };

            _instance.indexPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.LIndexTrigger };
            _instance.gripPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.LHandTrigger };
        }

        public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<OVRInput.RawButton>>()
            {
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.X,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "X Button",
                    supportsTouch = true},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                     rawButton = OVRInput.RawButton.Y,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "Y Button",
                    supportsTouch = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Joystick",
                    supportsTouch = true,
                    supportsAxis2D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Index Trigger",
                    supportsTouch = true,
                    supportsAxis1D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LHandTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Hand Trigger",
                    supportsTouch = false,
                    supportsAxis1D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Start,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Menu" }
            };
        }

        protected override HandTrackingOffsets GetDefaultTrackingOffsets()
        {
            HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets();
            newTrackingOffsets.positionOffset = new Vector3 (0f, -0.02f, -0.07f);
            newTrackingOffsets.rotationOffset = Vector3.zero;

            return newTrackingOffsets;
        }
    }
}
#endif