using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if TB_GOOGLE
namespace TButt.Input
{
    public class TBController_Google_Daydream6DOFLeft : TBControllerBase<GvrControllerButton>
    {
        protected static TBController_Google_Daydream6DOFLeft _instance;

        public static TBController_Google_Daydream6DOFLeft instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Google_Daydream6DOFLeft();
                    _instance.model = VRController.Daydream6DOF;
                    _instance.name = "Experimental Daydream 6DOF Controller (Left)";
                    _instance.fileName = "Maps_Google_Daydream6DOFLeft";
                    _instance.type = TBInput.Controller.LHandController;
                    _instance.supportsRumble = false;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public override List<TBInput.ButtonDef<GvrControllerButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<GvrControllerButton>>()
            {
                 new TBInput.ButtonDef<GvrControllerButton>() {
                    rawButton = GvrControllerButton.TouchPadButton,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true},
                 new TBInput.ButtonDef<GvrControllerButton>() {
                    rawButton = GvrControllerButton.Trigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger Button",
                    supportsTouch = false,
                    supportsAxis2D = false},
                 new TBInput.ButtonDef<GvrControllerButton>() {
                    rawButton = GvrControllerButton.Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip button",
                    supportsTouch = false,
                    supportsAxis2D = false},
                 new TBInput.ButtonDef<GvrControllerButton>() {
                    rawButton = GvrControllerButton.App,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options, TBInput.Button.Action2 },
                    name = "App Button"}
            };
        }

        public override GvrControllerButton[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new GvrControllerButton[] { GvrControllerButton.TouchPadButton };
                case TBInput.Finger.Index:
                    return new GvrControllerButton[] { GvrControllerButton.Trigger };
                default:
                    return null;
            }
        }
    }
}
#endif