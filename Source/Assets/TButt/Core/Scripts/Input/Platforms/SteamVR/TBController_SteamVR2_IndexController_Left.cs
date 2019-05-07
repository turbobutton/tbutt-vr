using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR_2

namespace TButt.Input
{
    public class TBController_SteamVR2_IndexController_Left : TBControllerBase<SteamVRHardwareButton>
    {
        protected static TBController_SteamVR2_IndexController_Left _instance;

        public static TBController_SteamVR2_IndexController_Left instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR2_IndexController_Left();
                    _instance.model = VRController.ValveIndexController;
                    _instance.name = "Index Controller (Left)";
                    _instance.fileName = "Maps_SteamVR_IndexControllerLeft";
                    _instance.supportsRumble = true;
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.Touchpad, SteamVRHardwareButton.Joystick, SteamVRHardwareButton.AX, SteamVRHardwareButton.BY };
            _instance.indexPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.PrimaryTrigger };
            _instance.gripPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.Grip };
        }


        public override List<TBInput.ButtonDef<SteamVRHardwareButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<SteamVRHardwareButton>>
            {
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.PrimaryTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger",
                    supportsAxis1D = true,
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip",
                    supportsAxis1D = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.AX,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "A" ,
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.BY,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2, TBInput.Button.Options },
                    name = "B",
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad },
                    name = "Touchpad",
                    supportsAxis2D = true,
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Joystick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Joystick",
                    supportsAxis2D = true,
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Squeeze,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action3 },
                    name = "Squeeze",
                    supportsAxis1D = true }
            };
        }
    }
}
#endif