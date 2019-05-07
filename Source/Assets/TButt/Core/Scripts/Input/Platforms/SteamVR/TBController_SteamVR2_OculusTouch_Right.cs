using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR_2

namespace TButt.Input
{
    public class TBController_SteamVR2_OculusTouch_Right: TBControllerBase<SteamVRHardwareButton>
    {
        protected static TBController_SteamVR2_OculusTouch_Right _instance;

        public static TBController_SteamVR2_OculusTouch_Right instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_SteamVR2_OculusTouch_Right();
                    _instance.name = "Oculus Touch (Right)";
                    _instance.fileName = "Maps_SteamVR_OculusTouchRight";
                    if(TBCore.GetActiveHeadset() == VRHeadset.OculusRift)
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
            _instance.thumbPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.Joystick, SteamVRHardwareButton.AX, SteamVRHardwareButton.BY };
            _instance.indexPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.PrimaryTrigger };
            _instance.gripPoseButtons = new SteamVRHardwareButton[] { SteamVRHardwareButton.Grip };
        }

        public override List<TBInput.ButtonDef<SteamVRHardwareButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<SteamVRHardwareButton>>
            {
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.AX,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "X Button",
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.BY,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2, TBInput.Button.Options },
                    name = "Y Button",
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Joystick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick, TBInput.Button.Touchpad },
                    name = "Joystick",
                    supportsTouch = true,
                    supportsAxis2D = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.PrimaryTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Index Trigger",
                    supportsAxis1D = true,
                    supportsTouch = true },
                new TBInput.ButtonDef<SteamVRHardwareButton>() {
                    rawButton = SteamVRHardwareButton.Grip,
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