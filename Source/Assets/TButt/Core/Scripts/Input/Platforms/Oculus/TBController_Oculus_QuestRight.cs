﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_QuestRight : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_QuestRight _instance;

        public static TBController_Oculus_QuestRight instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_QuestRight();
                    _instance.name = "Oculus Touch V2/Quest (Right)";
                    _instance.model = VRController.OculusTouchV2;
                    _instance.fileName = "Maps_Oculus_QuestRight";
                    _instance.type = TBInput.Controller.RHandController;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
        }
        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.A, OVRInput.RawButton.B, OVRInput.RawButton.RThumbstick };
            _instance.indexPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.RIndexTrigger };
            _instance.gripPoseButtons = new OVRInput.RawButton[] { OVRInput.RawButton.RHandTrigger };
        }

        public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<OVRInput.RawButton>>
            {
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.A,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "A Button",
                    supportsTouch = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.B,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "B Button",
                    supportsTouch = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Joystick",
                    supportsTouch = true,
                    supportsAxis2D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Index Trigger",
                    supportsTouch = true,
                    supportsAxis1D = true},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RHandTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Hand Trigger",
                    supportsTouch = false,
                    supportsAxis1D = true }
            };
        }

		protected override HandTrackingOffsets GetDefaultTrackingOffsets ()
		{
			HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets ();
			newTrackingOffsets.positionOffset = new Vector3 (0f, -0.02f, -0.07f);
			newTrackingOffsets.rotationOffset = Vector3.zero;

			return newTrackingOffsets;
		}
    }
}
#endif