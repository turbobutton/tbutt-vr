using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR_2

namespace TButt.Input
{
    public class TBController_SteamVR2_ViveController_Left : TBControllerBase<SteamVRHardwareButton>
    {
        protected static TBController_SteamVR2_ViveController_Left _instance;

        public static TBController_SteamVR2_ViveController_Left instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR2_ViveController_Left();
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
            _instance.thumbPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.Touchpad, SteamVRHardwareButton.Menu };
            _instance.indexPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.PrimaryTrigger };
        }


        public override List<TBInput.ButtonDef<SteamVRHardwareButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<SteamVRHardwareButton>>
            {
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.PrimaryTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger",
                    supportsAxis1D = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip" },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Menu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2, TBInput.Button.Options },
                    name = "Menu" },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1, TBInput.Button.Touchpad },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true }
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