﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if TB_GOOGLE
namespace TButt.Input
{
    public class TBController_Google_Daydream6DOFRight : TBControllerBase<GvrControllerButton>
    {
        protected static TBController_Google_Daydream6DOFRight _instance;

        public static TBController_Google_Daydream6DOFRight instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBController_Google_Daydream6DOFRight();
                    _instance.model = VRController.Daydream6DOF;
                    _instance.name = "Experimental Daydream 6DOF Controller (Right)";
                    _instance.fileName = "Maps_Google_Daydream6DOFRight";
                    _instance.type = TBInput.Controller.RHandController;
                    _instance.supportsRumble = false;
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        protected override void SetFingerPoseButtons()
        {
            _instance.thumbPoseButtons = new GvrControllerButton[] { GvrControllerButton.TouchPadButton };
            _instance.indexPoseButtons = new GvrControllerButton[] { GvrControllerButton.Trigger };
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
    }
}
#endif