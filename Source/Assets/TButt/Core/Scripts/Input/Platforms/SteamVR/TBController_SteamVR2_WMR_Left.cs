using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR_2

namespace TButt.Input
{
    public class TBController_SteamVR2_WMR_Left : TBControllerBase<SteamVRHardwareButton>
    {
        protected static TBController_SteamVR2_WMR_Left _instance;

        public static TBController_SteamVR2_WMR_Left instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR2_WMR_Left();
                    _instance.model = VRController.WMRController;
                    _instance.name = "Windows Mixed Reality (Left)";
                    _instance.fileName = "Maps_SteamVR_WindowsMixedRealityLeft";
                    _instance.supportsRumble = true;
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.Touchpad, SteamVRHardwareButton.Joystick, SteamVRHardwareButton.Menu };
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
                    name = "Select Trigger",
                    supportsAxis1D = true },
                 new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip"},
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Menu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Menu" },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Joystick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Thumbstick",
                    supportsButton = false,
                    supportsAxis2D = true }
            };
        }
    }
}
#endif