using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_WINDOWS_MR || TB_STEAM_VR
namespace TButt.Input
{
    public class TBController_XInput_Gamepad: TBControllerBase<TBXInput.TBXInputButton>
    {
        protected static TBController_XInput_Gamepad _instance;

        public static TBController_XInput_Gamepad instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_XInput_Gamepad();
                    _instance.model = VRController.XboxOneGamepad;
                    _instance.name = "Xbox One Controller (XInput)";
                    _instance.fileName = "Maps_XInput_Gamepad";
                    _instance.type = TBInput.Controller.Gamepad;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public override List<TBInput.ButtonDef<TBXInput.TBXInputButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<TBXInput.TBXInputButton>>
            {
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.ButtonA,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "A Button" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.ButtonB,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "B Button" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.ButtonX,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action3 },
                    name = "X Button" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.ButtonY,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action4 },
                    name = "Y Button" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.Start,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Start },
                    name = "Start Button" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.Back,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Back Button" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.LeftStick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.LeftStick, TBInput.Button.Joystick },
                    name = "Left Stick",
                    supportsAxis2D = true },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.RightStick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.RightStick },
                    name = "Right Stick",
                    supportsAxis2D = true },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.LeftBumper,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.LeftBumper },
                    name = "Left Shoulder" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.RightBumper,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.RightBumper },
                    name = "Right Shoulder" },
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.LeftTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.LeftTrigger, TBInput.Button.PrimaryTrigger },
                    name = "Left Trigger",
                    supportsAxis1D = true},
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.RightTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.RightTrigger, TBInput.Button.SecondaryTrigger },
                    name = "Right Trigger",
                    supportsAxis1D = true},
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.DpadUp,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadUp },
                    name = "Dpad Up"},
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.DpadDown,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadDown },
                    name = "Dpad Down"},
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.DpadLeft,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadLeft },
                    name = "Dpad Left"},
                new TBInput.ButtonDef<TBXInput.TBXInputButton>() {
                    rawButton = TBXInput.TBXInputButton.DpadRight,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadRight },
                    name = "Dpad Right"},
            };
        }
    }
}
#endif