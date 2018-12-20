using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_OCULUS
namespace TButt.Input
{
    public class TBController_Oculus_GearVRController : TBControllerBase<OVRInput.RawButton>
    {
        protected static TBController_Oculus_GearVRController _instance;

        public static TBController_Oculus_GearVRController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Oculus_GearVRController();
                    _instance.model = VRController.GearVRController;
                    _instance.name = "Gear VR Controller";
                    _instance.fileName = "Maps_Oculus_GearVRController";
                    _instance.type = TBInput.Controller.Mobile3DOFController;
                    _instance.supportsRumble = false;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Override to set up inputs for the left and right controllers in a single def on 3DOF.
        /// </summary>
        protected override void Initialize()
        {
            loadedButtonDefs = TBInput.LoadButtonDefs(GetDefaultDefs(), fileName);

            loadedButtonDefs.Add(new TBInput.ButtonDef<OVRInput.RawButton>()
            {
                rawButton = OVRInput.RawButton.LTouchpad,
                virtualButtons = loadedButtonDefs[0].virtualButtons,
                name = "Touchpad Left",
                supportsTouch = true,
                supportsAxis2D = true
            });
            loadedButtonDefs.Add(new TBInput.ButtonDef<OVRInput.RawButton>()
            {
                rawButton = OVRInput.RawButton.LIndexTrigger,
                virtualButtons = loadedButtonDefs[1].virtualButtons,
                name = "Trigger Left",
                supportsTouch = false,
                supportsAxis2D = false
            });

            lookupTable = TBInput.NewLookupTableFromDefs(loadedButtonDefs);
            loaded = true;
        }

        public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<OVRInput.RawButton>>
            {
                new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RTouchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad, TBInput.Button.Action1 },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true },
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.RIndexTrigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger"},
                 new TBInput.ButtonDef<OVRInput.RawButton>() {
                    rawButton = OVRInput.RawButton.Back,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Back"}
            };
        }

        public override OVRInput.RawButton[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.LTouchpad, OVRInput.RawButton.RTouchpad };
                case TBInput.Finger.Index:
                    return new OVRInput.RawButton[] { OVRInput.RawButton.LIndexTrigger, OVRInput.RawButton.RIndexTrigger };
                default:
                    return null;
            }
        }

        public override VRController GetModel()
        {
            switch (TBCore.GetActiveHeadset())
            {
                case VRHeadset.OculusGo:
                case VRHeadset.MiVRStandalone:
                    return VRController.OculusGoController;
                default:
                    return VRController.GearVRController;
            }
        }
    }
}
#endif