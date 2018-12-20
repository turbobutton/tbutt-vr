using System.Collections;
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
                    _instance.name = "Oculus Quest (Right)";
                    _instance.model = VRController.OculusQuestController;
                    _instance.fileName = "Maps_Oculus_QuestRight";
                    _instance.type = TBInput.Controller.RHandController;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
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

        public override OVRInput.RawButton[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.A, OVRInput.RawButton.RThumbstick };
                case TBInput.Finger.Index:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.RIndexTrigger };
                case TBInput.Finger.Grip:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.RHandTrigger };
                default:
                    return null;
            }
        }
    }
}
#endif