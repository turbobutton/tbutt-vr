using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_WINDOWS_MR

namespace TButt.Input
{
    public class TBController_WindowsMR_MotionControllerLeft : TBControllerBase<TBWindowsMRInput.Button>
    {
        protected static TBController_WindowsMR_MotionControllerLeft _instance;

        public static TBController_WindowsMR_MotionControllerLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_WindowsMR_MotionControllerLeft();
                    _instance.model = VRController.WMRController;
                    _instance.name = "Windows Mixed Reality Motion Controller (Left)";
                    _instance.fileName = "Maps_WindowsMR_MotionControllerLeft";
                    _instance.supportsRumble = true;
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public override List<TBInput.ButtonDef<TBWindowsMRInput.Button>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<TBWindowsMRInput.Button>>
            {
                new TBInput.ButtonDef<TBWindowsMRInput.Button>() {
                    rawButton = TBWindowsMRInput.Button.SelectTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Select Trigger",
                    supportsAxis1D = true },
                new TBInput.ButtonDef<TBWindowsMRInput.Button>() {
                    rawButton = TBWindowsMRInput.Button.Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip" },
                new TBInput.ButtonDef<TBWindowsMRInput.Button>() {
                    rawButton = TBWindowsMRInput.Button.Menu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Menu" },
                new TBInput.ButtonDef<TBWindowsMRInput.Button>() {
                    rawButton = TBWindowsMRInput.Button.Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true },
                new TBInput.ButtonDef<TBWindowsMRInput.Button>() {
                    rawButton = TBWindowsMRInput.Button.Thumbstick,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Joystick },
                    name = "Thumbstick",
                    supportsAxis2D = true }
            };
        }

		protected override HandTrackingOffsets GetDefaultTrackingOffsets ()
		{
			HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets ();
			newTrackingOffsets.positionOffset = new Vector3 (0f,  0f, -0.015f);
			newTrackingOffsets.rotationOffset = new Vector3(33,0,0);

			return newTrackingOffsets;
		}

        public override TBWindowsMRInput.Button[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new TBWindowsMRInput.Button[] { TBWindowsMRInput.Button.Touchpad };
                case TBInput.Finger.Index:
                    return new TBWindowsMRInput.Button[] { TBWindowsMRInput.Button.SelectTrigger };
                case TBInput.Finger.Grip:
                    return new TBWindowsMRInput.Button[] { TBWindowsMRInput.Button.Grip };
                default:
                    return null;
            }
        }
    }
}
#endif