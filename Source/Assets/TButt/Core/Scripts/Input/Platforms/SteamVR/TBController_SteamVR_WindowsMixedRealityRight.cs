using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR
using Valve.VR;

namespace TButt.Input
{
    public class TBController_SteamVR_WindowsMixedRealityRight: TBControllerBase<EVRButtonId>
    {
        protected static TBController_SteamVR_WindowsMixedRealityRight _instance;

        public static TBController_SteamVR_WindowsMixedRealityRight instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR_WindowsMixedRealityRight();
                    _instance.model = VRController.WMRController;
                    _instance.name = "Windows Mixed Reality (Right)";
                    _instance.fileName = "Maps_SteamVR_WindowsMixedRealityRight";
                    _instance.supportsRumble = true;
                    _instance.type = TBInput.Controller.RHandController;
                    _instance.Initialize();
                }
                return _instance;
            }
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
                    name = "Menu"},
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

        public override EVRButtonId[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new EVRButtonId[] { EVRButtonId.k_EButton_SteamVR_Touchpad };
                case TBInput.Finger.Index:
                    return new EVRButtonId[] { EVRButtonId.k_EButton_SteamVR_Trigger };
                case TBInput.Finger.Grip:
                    return new EVRButtonId[] { EVRButtonId.k_EButton_Grip };
                default:
                    return null;
            }
        }
    }
}
#endif