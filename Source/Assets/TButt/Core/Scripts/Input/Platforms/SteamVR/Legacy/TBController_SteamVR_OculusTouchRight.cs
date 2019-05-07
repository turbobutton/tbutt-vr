using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR
using Valve.VR;

namespace TButt.Input
{
    public class TBController_SteamVR_OculusTouchRight: TBControllerBase<EVRButtonId>
    {
        protected static TBController_SteamVR_OculusTouchRight _instance;

        public static TBController_SteamVR_OculusTouchRight instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR_OculusTouchRight();
                    _instance.name = "Oculus Touch (Right)";
                    _instance.fileName = "Maps_SteamVR_OculusTouchRight";
                    if (TBCore.GetActiveHeadset() == VRHeadset.OculusRift)
                        _instance.model = VRController.OculusTouchV1;
                    else
                        _instance.model = VRController.OculusTouchV2;
                    _instance.supportsRumble = true;
                    _instance.type = TBInput.Controller.RHandController;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new EVRButtonId[] { EVRButtonId.k_EButton_SteamVR_Touchpad, EVRButtonId.k_EButton_A, EVRButtonId.k_EButton_ApplicationMenu };
            _instance.indexPoseButtons = new EVRButtonId[] { EVRButtonId.k_EButton_SteamVR_Trigger };
            _instance.gripPoseButtons = new EVRButtonId[] { EVRButtonId.k_EButton_Grip };
        }

        public override List<TBInput.ButtonDef<EVRButtonId>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<EVRButtonId>>
            {
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_A,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "A Button",
                    supportsTouch = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_ApplicationMenu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2, TBInput.Button.Options },
                    name = "B Button",
                    supportsTouch = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick, TBInput.Button.Touchpad },
                    name = "Joystick",
                    supportsTouch = true,
                    supportsAxis2D = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Trigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Index Trigger",
                    supportsAxis1D = true,
                    supportsTouch = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Hand Trigger",
                    supportsAxis1D = true }
            };
        }

		protected override HandTrackingOffsets GetDefaultTrackingOffsets ()
		{
			HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets ();
			newTrackingOffsets.positionOffset = new Vector3 (-0.03f, -0.08f, -0.1f);
			newTrackingOffsets.rotationOffset = Vector3.right * 30f;

			return newTrackingOffsets;
		}
    }
}
#endif