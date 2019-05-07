using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR
using Valve.VR;

namespace TButt.Input
{
    public class TBController_SteamVR_ViveControllerLeft : TBControllerBase<EVRButtonId>
    {
        protected static TBController_SteamVR_ViveControllerLeft _instance;

        public static TBController_SteamVR_ViveControllerLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR_ViveControllerLeft();
                    _instance.model = VRController.ViveController;
                    _instance.name = "Vive Controller (Left)";
                    _instance.fileName = "Maps_SteamVR_ViveControllerLeft";
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
        }

        public override List<TBInput.ButtonDef<EVRButtonId>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<EVRButtonId>>
            {
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Trigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger",
                    supportsAxis1D = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_ApplicationMenu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2, TBInput.Button.Options },
                    name = "Menu" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1, TBInput.Button.Touchpad },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true }
            };
        }

		protected override HandTrackingOffsets GetDefaultTrackingOffsets ()
		{
			HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets ();
			newTrackingOffsets.positionOffset = new Vector3 (0f, -0.02f, -0.1f);
			newTrackingOffsets.rotationOffset = Vector3.zero;

			return newTrackingOffsets;
		}
    }
}
#endif