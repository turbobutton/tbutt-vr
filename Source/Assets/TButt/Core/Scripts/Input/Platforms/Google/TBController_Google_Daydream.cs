using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if TB_GOOGLE
namespace TButt.Input
{
    public class TBController_Google_Daydream : TBControllerBase<GvrControllerButton>
    {
        protected static TBController_Google_Daydream _instance;

        public static TBController_Google_Daydream instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Google_Daydream();
                    _instance.model = VRController.Daydream3DOF;
                    _instance.name = "Daydream Controller";
                    _instance.fileName = "Maps_Google_Daydream";
                    _instance.type = TBInput.Controller.Mobile3DOFController;
                    _instance.supportsRumble = false;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new GvrControllerButton[] { GvrControllerButton.TouchPadButton };
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
                    rawButton = GvrControllerButton.App,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options, TBInput.Button.Action2 },
                    name = "App Button"}
            };
        }
    }
}
#endif