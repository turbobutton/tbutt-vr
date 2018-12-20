using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_GearVRTouchpad : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_GearVRTouchpad _instance;

        public static TBController_Oculus_GearVRTouchpad instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_GearVRTouchpad();
                    _instance.model = VRController.GearVRTouchpad;
                    _instance.name = "Gear VR Touchpad";
                    _instance.fileName = "Maps_Oculus_GearVRTouchpad";
                    _instance.type = TBInput.Controller.ClickRemote;
                    _instance.supportsRumble = false;
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
                    rawButton = OVRInput.RawButton.Start,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Back,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Back Button"},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadUp,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadUp },
                    name = "Swipe Up"},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadDown,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadDown },
                    name = "Swipe Down",
                    supportsTouch = true,
                    supportsAxis1D = true},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadLeft,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadLeft },
                    name = "Swipe Left"},
                 new TBInput.ButtonDef<OVRInput.RawButton>()
                 {
                     rawButton = OVRInput.RawButton.DpadRight,
                     virtualButtons = new TBInput.Button[] { TBInput.Button.DpadRight },
                     name = "Swipe Right"
                 }
            };
        }
    }
}
#endif