using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_ViveControllerRight : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_ViveControllerRight _instance;

        public static TBController_Oculus_ViveControllerRight instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_ViveControllerRight();
                    _instance.name = "Vive Controller (Right)";
                    _instance.model = VRController.ViveController;
                    _instance.fileName = "Maps_Oculus_ViveControllerRight";
                    _instance.type = TBInput.Controller.RHandController;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.RThumbstick };
            _instance.indexPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.RIndexTrigger };
        }

        public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<OVRInput.RawButton>>
            {
                  new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger",
                    supportsAxis1D = true},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RHandTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip"},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.B,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "Menu" },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true }
            };
        }
    }
}
#endif