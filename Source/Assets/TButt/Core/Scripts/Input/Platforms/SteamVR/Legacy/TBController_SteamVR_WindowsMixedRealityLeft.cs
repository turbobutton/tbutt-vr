using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR
using Valve.VR;

namespace TButt.Input
{
    public class TBController_SteamVR_WindowsMixedRealityLeft: TBControllerBase<EVRButtonId>
    {
        protected static TBController_SteamVR_WindowsMixedRealityLeft _instance;

        public static TBController_SteamVR_WindowsMixedRealityLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR_WindowsMixedRealityLeft();
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
            _instance.thumbPoseButtons = new EVRButtonId[] { EVRButtonId.k_EButton_SteamVR_Touchpad, EVRButtonId.k_EButton_ApplicationMenu };
            _instance.indexPoseButtons = new EVRButtonId[] { EVRButtonId.k_EButton_SteamVR_Trigger };
            _instance.gripPoseButtons = new EVRButtonId[] { EVRButtonId.k_EButton_Grip };
        }

        public override List<TBInput.ButtonDef<EVRButtonId>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<EVRButtonId>>
            {
                 new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Trigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Select Trigger",
                    supportsAxis1D = true },
                 new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip"},
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_ApplicationMenu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Menu" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_Axis2,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Thumbstick",
                    supportsButton = false,
                    supportsAxis2D = true }
            };
        }
    }
}
#endif