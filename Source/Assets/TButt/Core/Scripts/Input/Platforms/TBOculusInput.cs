using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Input
{
    /// <summary>
    /// Button mappings for Oculus devices. Note that you should interface with hand controllers *individually* rather than as a combined set.
    /// </summary>
    #if TB_OCULUS
    public class TBInputOculus : TBSDKInputBase<OVRInput.RawButton>

    #else
    public class TBInputOculus : TBSDKInputBase<TBInput.Button>
    #endif
    {
        protected static TBInputOculus _instance;

        public static TBInputOculus instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBInputOculus();
                return _instance;
            }
        }

        #if TB_OCULUS
        private bool _leftRumble = false;
        private bool _rightRumble = false;

        #region CONTROLLER LOADING
        protected override void LoadHandControllers()
        {
            if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
            {
                switch (Settings.TBOculusSettings.GetOculusDeviceFamily())
                {
                    case Settings.TBOculusSettings.OculusDeviceFamily.Quest:
                        controller_LHand = TBController_Oculus_QuestLeft.instance;
                        controller_RHand = TBController_Oculus_QuestRight.instance;
                        break;
                    default:
                        Debug.LogWarning("Attempted to load hand controllers, but none are defined for " + Settings.TBOculusSettings.GetOculusDeviceFamily());
                        break;
                }
            }
            else
            {
                switch(TBCore.GetActiveHeadset())
                {
                    case VRHeadset.HTCVive:
                        controller_LHand = TBController_Oculus_ViveControllerLeft.instance;
                        controller_RHand = TBController_Oculus_ViveControllerRight.instance;
                        break;
                    default:
                        // Currently, the only supported hand controller on PC is Oculus Touch.
                        controller_LHand = TBController_Oculus_TouchLeft.instance;
                        controller_RHand = TBController_Oculus_TouchRight.instance;
                        break;
                }
            }
        }

        protected override void Load3DOFControllers()
        {
            if (!TBCore.UsingEditorMode() && (TBCore.GetActivePlatform() == VRPlatform.OculusMobile))    // Prevents 3DOF controller from initializing when on PC unless we're in the editor.
                controller_3DOF = TBController_Oculus_GearVRController.instance;
            else if (TBCore.UsingEditorMode() || (TBCore.GetActivePlatform() == VRPlatform.OculusPC))
            {
                if(TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
                    controller_3DOF = TBController_Oculus_TouchLeft.instance;
                else
                    controller_3DOF = TBController_Oculus_TouchRight.instance;

                TBLogging.LogMessage("Emulating 3DOF Controller with Oculus Touch controller with " + TBSettings.GetControlSettings().handedness3DOF + " handedness.");
            }
            else
                base.Load3DOFControllers();
        }

        protected override void LoadClickRemotes()
        {
            if (TBCore.UsingEditorMode() || (TBCore.GetActivePlatform() == VRPlatform.OculusPC))    // Oculus Remote is only supported click remote when in the editor or on PC builds.
            {
                controller_ClickRemote = TBController_Oculus_Remote.instance;
                #if UNITY_ANDROID
                TBLogging.LogMessage("Using Oculus Remote instead of Gear VR Touchpad when running Gear VR builds in the editor.");
                #endif
            }
            else if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)                         // Use Gear VR Touchpad when in Android builds
            { 
                switch(Settings.TBOculusSettings.GetOculusDeviceFamily())
                {
                    case Settings.TBOculusSettings.OculusDeviceFamily.GearVR:
                    case Settings.TBOculusSettings.OculusDeviceFamily.Unknown:
                        controller_ClickRemote = TBController_Oculus_GearVRTouchpad.instance;
                        break;
                }
            }
        }

        protected override void LoadGamepads()
        {
            controller_Gamepad = TBController_Oculus_Gamepad.instance;
        }
        #endregion
    
        public override void Update()
        {
            switch (TBInput.GetActiveControlType())
            {
                case TBInput.ControlType.HandControllers:
                    UpdateRumbleState(TBInput.Controller.LHandController);
                    UpdateRumbleState(TBInput.Controller.RHandController);
                    break;
            }
        }

        public override void FixedUpdate()
        {
           //  OVRInput.FixedUpdate();
        }

        /// <summary>
        /// Needed for generating a controller type compatible with Oculus SDK calls.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static OVRInput.Controller GetOculusControllerID(TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = TBInput.GetActiveController();

            switch (controller)
            {
                case TBInput.Controller.LHandController:
                        return OVRInput.Controller.LTouch;
                case TBInput.Controller.RHandController:
                        return OVRInput.Controller.RTouch;
                case TBInput.Controller.Mobile3DOFController:
                    if (TBCore.UsingEditorMode() || TBCore.GetActivePlatform() == VRPlatform.OculusPC)
                    {
                        if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
                            return OVRInput.Controller.LTouch;
                        else
                            return OVRInput.Controller.RTouch;
                    }
                    else
                    {
                        if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
                            return OVRInput.Controller.LTrackedRemote;
                        else
                            return OVRInput.Controller.RTrackedRemote;
                    }
                case TBInput.Controller.ClickRemote:
                    if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
                        return OVRInput.Controller.Touchpad;
                    else
                        return OVRInput.Controller.Remote;
                case TBInput.Controller.Gamepad:
                    return OVRInput.Controller.Gamepad;
            }

            TBLogging.LogError("Controller type " + controller + " has no match for Oculus.");
            return OVRInput.Controller.None;
        }

        public override TBInput.Mobile3DOFHandedness Get3DOFHandedness()
        {
            if (TBCore.UsingEditorMode() || TBCore.GetActivePlatform() == VRPlatform.OculusPC)
            {
                if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
                    return TBInput.Mobile3DOFHandedness.Left;
                else
                    return TBInput.Mobile3DOFHandedness.Right;
            }
            else
            {
                if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
                    return TBInput.Mobile3DOFHandedness.Left;
                else if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
                    return TBInput.Mobile3DOFHandedness.Right;
                else
                    return TBInput.Mobile3DOFHandedness.Center;
            }
        }

        protected override void StopRumbleInternal (TBInput.Controller controller)
        {
            switch(controller)
            {
                case TBInput.Controller.LHandController:
                    OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
                    break;
                case TBInput.Controller.RHandController:
                    OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
                    break;
            }
        }

        #region INPUT CHECKS
        public override bool ResolveButtonDown(OVRInput.RawButton button, TBInput.Controller controller)
        {
            OVRInput.Controller ctrl = GetOculusControllerID(controller);
            if (OVRInput.GetDown(button, ctrl))
                return true;
            return false;
        }

        public override bool ResolveButtonUp(OVRInput.RawButton button, TBInput.Controller controller)
        {
            OVRInput.Controller ctrl = GetOculusControllerID(controller);
            if (OVRInput.GetUp(button, ctrl))
                return true;
            return false;
        }

        public override bool ResolveButton(OVRInput.RawButton button, TBInput.Controller controller)
        {
            OVRInput.Controller ctrl = GetOculusControllerID(controller);
            if (OVRInput.Get(button, ctrl))
                return true;
            return false;
        }

        public override bool ResolveTouchDown(OVRInput.RawButton button, TBInput.Controller controller)
        {
            OVRInput.Controller ctrl = GetOculusControllerID(controller);
            if (OVRInput.GetDown(GetTouchEnum(button), ctrl))
                return true;
            return false;
        }

        public override bool ResolveTouchUp(OVRInput.RawButton button, TBInput.Controller controller)
        {
            OVRInput.Controller ctrl = GetOculusControllerID(controller);
            if (OVRInput.GetUp(GetTouchEnum(button), ctrl))
                return true;
            return false;
        }

        public override bool ResolveTouch(OVRInput.RawButton button, TBInput.Controller controller)
        {
            OVRInput.Controller ctrl = GetOculusControllerID(controller);
            if (OVRInput.Get(GetTouchEnum(button), ctrl))
                return true;
            return false;
        }

        public override float ResolveAxis1D(OVRInput.RawButton button, TBInput.Controller controller)
        {
            return OVRInput.Get(GetAxis1DEnum(button), GetOculusControllerID(controller));
        }

        public override Vector2 ResolveAxis2D(OVRInput.RawButton button, TBInput.Controller controller)
        {
            return OVRInput.Get(GetAxis2DEnum(button), GetOculusControllerID(controller));
        }

        public override Quaternion GetRawRotation(TBInput.Controller controller)
        {
            return OVRInput.GetLocalControllerRotation(GetOculusControllerID(controller));
        }

        public override Vector3 GetRawPosition(TBInput.Controller controller)
        {
            return OVRInput.GetLocalControllerPosition(GetOculusControllerID(controller));
        }

        public override Vector3 GetVelocity(TBInput.Controller controller)
        {
            return OVRInput.GetLocalControllerVelocity(GetOculusControllerID(controller));
        }

        public override Vector3 GetAngularVelocity(TBInput.Controller controller)
        {
            return OVRInput.GetLocalControllerAngularVelocity(GetOculusControllerID(controller));
        }

        public override Vector3 GetAcceleration(TBInput.Controller controller)
        {
            return OVRInput.GetLocalControllerAcceleration(GetOculusControllerID(controller));
        }

        protected override bool ResolveRumble(TBInput.Controller controller, float strength = 0.5F, float frequency = 0.5F)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    controller_LHand.SetRumbling(true);
                    OVRInput.SetControllerVibration(frequency, strength, OVRInput.Controller.LTouch);
                    return true;
                case TBInput.Controller.RHandController:
                    controller_RHand.SetRumbling(true);
                    OVRInput.SetControllerVibration(frequency, strength, OVRInput.Controller.RTouch);
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region MAP TRANSLATIONS
        // Specific to Oculus SDK - we need to convert enums into a different format to be read for values other than "button."

        public static OVRInput.RawTouch GetTouchEnum(OVRInput.RawButton button)
        {
            switch(button)
            {
                case OVRInput.RawButton.A:
                    return OVRInput.RawTouch.A;
                case OVRInput.RawButton.B:
                    return OVRInput.RawTouch.B;
                case OVRInput.RawButton.X:
                    return OVRInput.RawTouch.X;
                case OVRInput.RawButton.Y:
                    return OVRInput.RawTouch.Y;
                case OVRInput.RawButton.RThumbstick:
                    return OVRInput.RawTouch.RThumbstick;
                case OVRInput.RawButton.LThumbstick:
                    return OVRInput.RawTouch.LThumbstick;
                case OVRInput.RawButton.RIndexTrigger:
                    return OVRInput.RawTouch.RIndexTrigger;
                case OVRInput.RawButton.LIndexTrigger:
                    return OVRInput.RawTouch.LIndexTrigger;
                case OVRInput.RawButton.LTouchpad:
                    return OVRInput.RawTouch.LTouchpad;
                case OVRInput.RawButton.RTouchpad:
                    return OVRInput.RawTouch.RTouchpad;
                case OVRInput.RawButton.RShoulder:
                    return OVRInput.RawTouch.RThumbRest;
                case OVRInput.RawButton.LShoulder:
                    return OVRInput.RawTouch.LThumbRest;
            }
            return OVRInput.RawTouch.None;
        }

        public static OVRInput.RawAxis1D GetAxis1DEnum(OVRInput.RawButton button)
        {
            switch (button)
            {
                case OVRInput.RawButton.LHandTrigger:
                    return OVRInput.RawAxis1D.LHandTrigger;
                case OVRInput.RawButton.LIndexTrigger:
                    return OVRInput.RawAxis1D.LIndexTrigger;
                case OVRInput.RawButton.RIndexTrigger:
                    return OVRInput.RawAxis1D.RIndexTrigger;
                case OVRInput.RawButton.RHandTrigger:
                    return OVRInput.RawAxis1D.RHandTrigger;
            }
            return OVRInput.RawAxis1D.None;
        }

        public static OVRInput.RawAxis2D GetAxis2DEnum(OVRInput.RawButton button)
        {
            switch (button)
            {
                case OVRInput.RawButton.LThumbstick:
                    return OVRInput.RawAxis2D.LThumbstick;
                case OVRInput.RawButton.RThumbstick:
                    return OVRInput.RawAxis2D.RThumbstick;
                case OVRInput.RawButton.RTouchpad:
                    return OVRInput.RawAxis2D.RTouchpad;
                case OVRInput.RawButton.LTouchpad:
                    return OVRInput.RawAxis2D.LTouchpad;
            }
            return OVRInput.RawAxis2D.None;
        }
        #endregion
        #endif
    }
}

