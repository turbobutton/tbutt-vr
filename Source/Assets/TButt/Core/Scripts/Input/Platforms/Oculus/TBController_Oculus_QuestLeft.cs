using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_QuestLeft : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_QuestLeft _instance;

        public static TBController_Oculus_QuestLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_QuestLeft();
                    _instance.name = "Oculus Quest (Left)";
                    _instance.model = VRController.OculusQuestController;
                    _instance.fileName = "Maps_Oculus_QuestLeft";
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.supportsRumble = true;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<OVRInput.RawButton>>()
            {
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.X,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action1 },
                    name = "X Button",
                    supportsTouch = true},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                     rawButton = OVRInput.RawButton.Y,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Action2 },
                    name = "Y Button",
                    supportsTouch = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LThumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Joystick",
                    supportsTouch = true,
                    supportsAxis2D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Index Trigger",
                    supportsTouch = true,
                    supportsAxis1D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.LHandTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Hand Trigger",
                    supportsTouch = false,
                    supportsAxis1D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Start,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Menu" }
            };
        }

        protected override HandTrackingOffsets GetDefaultTrackingOffsets()
        {
            HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets();
            newTrackingOffsets.positionOffset = new Vector3 (0f, -0.02f, -0.07f);
            newTrackingOffsets.rotationOffset = Vector3.zero;

            return newTrackingOffsets;
        }

        public override OVRInput.RawButton[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.X, OVRInput.RawButton.LThumbstick };
                case TBInput.Finger.Index:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.LIndexTrigger };
                case TBInput.Finger.Grip:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.LHandTrigger };
                default:
                    return null;
            }
        }
    }
}
#endif