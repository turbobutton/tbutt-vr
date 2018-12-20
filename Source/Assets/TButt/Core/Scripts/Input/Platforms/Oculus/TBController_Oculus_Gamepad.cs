using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_Gamepad: TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_Gamepad _instance;

        public static TBController_Oculus_Gamepad instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_Gamepad();
                    _instance.model = VRController.XboxOneGamepad;
                    _instance.name = "Xbox One Controller";
                    _instance.fileName = "Maps_Oculus_Gamepad";
                    _instance.type = TBInput.Controller.Gamepad;
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
                    rawButton = OVRInput.RawButton.A,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "A Button" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.B,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "B Button" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.X,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action3 },
                    name = "X Button" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Y,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action4 },
                    name = "Y Button" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Start,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Start },
                    name = "Start Button" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Back,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Back Button" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.LeftStick, TBInput.Button.Joystick },
                    name = "Left Stick",
                    supportsAxis2D = true },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.RightStick },
                    name = "Right Stick",
                    supportsAxis2D = true },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LShoulder,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.LeftBumper },
                    name = "Left Shoulder" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RShoulder,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.RightBumper },
                    name = "Right Shoulder" },
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.LeftTrigger, TBInput.Button.PrimaryTrigger },
                    name = "Left Trigger",
                    supportsAxis1D = true},
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.RightTrigger, TBInput.Button.SecondaryTrigger },
                    name = "Right Trigger",
                    supportsAxis1D = true},
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadUp,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadUp },
                    name = "Dpad Up"},
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadDown,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadDown },
                    name = "Dpad Down"},
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadLeft,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadLeft },
                    name = "Dpad Left"},
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.DpadRight,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadRight },
                    name = "Dpad Right"},
            };
        }
    }
}
#endif