using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_ViveControllerLeft : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_ViveControllerLeft _instance;

        public static TBController_Oculus_ViveControllerLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_ViveControllerLeft();
                    _instance.name = "Vive Controller (Left)";
                    _instance.model = VRController.ViveController;
                    _instance.fileName = "Maps_Oculus_ViveControllerLeft";
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<OVRInput.RawButton>>
            {
                  new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger",
                    supportsAxis1D = true},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LHandTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip"},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Y,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "Menu" },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true }
            };
        }

        public override OVRInput.RawButton[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.LThumbstick };
                case TBInput.Finger.Index:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.LIndexTrigger };
                default:
                    return null;
            }
        }
    }
}
#endif