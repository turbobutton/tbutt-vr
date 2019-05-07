using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
#if TB_STEAM_VR_2
using Valve.VR;
#endif

namespace TButt.Input
{
    #if TB_STEAM_VR_2
    public class TBInputSteamVR2 : TBSDKInputBase<SteamVRHardwareButton>
    #else
    public class TBInputSteamVR2 : TBSDKInputWithXInputBase<TBInput.Button>
    #endif
    { 
        protected static TBInputSteamVR2 _instance;

        protected Vector3 lastLeftControllerVelocity = Vector3.one;
        protected Vector3 lastRightControllerVelocity = Vector3.one;

        public static TBInputSteamVR2 instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBInputSteamVR2();
                return _instance;
            }
        }

        #if TB_STEAM_VR_2
        protected SteamVR_Action_Boolean targetButton;
        protected SteamVR_Action_Boolean targetTouch;
        protected SteamVR_Action_Single targetVector1D;
        protected SteamVR_Action_Vector2 targetVector2D;
        protected SteamVR_Action_Pose targetPose;

        public override void Initialize()
        {
            SteamVR_Input.Initialize();
            TBSteamVRActions.Refresh();
            SteamVR_Input.UpdateNonVisualActions();
            Debug.Log(TBSteamVRActions.tButt_MainTrigger_V1.renderModelComponentName);
            TBCore.OnBeforeRender += BeforeRender;
            SteamVR_Events.DeviceConnected.AddListener(DeviceConnectedEvent);
            base.Initialize();
        }

        protected void DeviceConnectedEvent(int arg0, bool arg1)
        {
            TBLogging.LogMessage("A new Steam VR controller was connected.");
            if(arg1)
                RefreshBindings();
        }

        protected void RefreshBindings()
        {
            TBSteamVRActions.Refresh();
            LoadControllers();
        }

        public override void Update()
        {
            SteamVR_Input.Update();
        }

        public override void FixedUpdate()
        {
            SteamVR_Input.FixedUpdate();
        }

        public void BeforeRender()
        {
            SteamVR_Input.OnPreCull();
        }

        protected override void LoadHandControllers()
        {
            switch(TBCore.GetActiveHeadsetFamily())
            {
                case VRFamily.Oculus:   
                    // Touch is the only available controller for Oculus devices.
                    controller_LHand = TBController_SteamVR2_OculusTouch_Left.instance;
                    controller_RHand = TBController_SteamVR2_OculusTouch_Right.instance;
                    break;
                case VRFamily.Windows:
                    // WMR is the only available controller for WMR devices.
                    controller_LHand = TBController_SteamVR2_WMR_Left.instance;
                    controller_RHand = TBController_SteamVR2_WMR_Right.instance;
                    break;
                default:
                    // Valve and HTC headsets can use the same controllers.
                    if (HasKnucklesController())
                    {
                        controller_LHand = TBController_SteamVR2_IndexController_Left.instance;
                        controller_RHand = TBController_SteamVR2_IndexController_Right.instance;
                    }
                    else
                    {
                        controller_LHand = TBController_SteamVR2_ViveController_Left.instance;
                        controller_RHand = TBController_SteamVR2_ViveController_Right.instance;
                    }
                    break;
            }
        }

        protected virtual bool HasKnucklesController()
        {
            uint handle = OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            if (handle > 9000)
                handle = OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);

            string deviceName = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, handle);

            deviceName = deviceName.ToLower();

            bool isKnuckles = false;

            if (deviceName.Contains("index"))
                isKnuckles = true;
            else if (deviceName.Contains("knuckles"))
                isKnuckles = true;

            return isKnuckles;
        }

        protected override void Load3DOFControllers()
        {
            // 3DOF Controller emulation in the editor.
            if (TBCore.UsingEditorMode() && TBSettings.GetControlSettings().emulate3DOFArmModel)
            {
                if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
                {
                    switch (TBCore.GetActiveHeadsetFamily())
                    {
                        case VRFamily.Oculus:
                            controller_3DOF = TBController_SteamVR2_OculusTouch_Left.instance;
                            break;
                        case VRFamily.Windows:
                            controller_3DOF = TBController_SteamVR2_WMR_Left.instance;
                            break;
                        default:
                            if (HasKnucklesController())
                                controller_3DOF = TBController_SteamVR2_IndexController_Left.instance;
                            else
                                controller_3DOF = TBController_SteamVR2_ViveController_Left.instance;
                            break;
                    }
                }
                else
                {
                    switch (TBCore.GetActiveHeadsetFamily())
                    {
                        case VRFamily.Oculus:
                            controller_3DOF = TBController_SteamVR2_OculusTouch_Right.instance;
                            break;
                        case VRFamily.Windows:
                            controller_3DOF = TBController_SteamVR2_WMR_Right.instance;
                            break;
                        default:
                            if (HasKnucklesController())
                                controller_3DOF = TBController_SteamVR2_IndexController_Right.instance;
                            else
                                controller_3DOF = TBController_SteamVR2_ViveController_Right.instance;
                            break;
                    }
                }
            }
        }

        public SteamVR_Input_Sources GetSteamVRInputSource(TBInput.Controller controller)
        {
            switch(controller)
            {
                case TBInput.Controller.RHandController:
                    return SteamVR_Input_Sources.RightHand;
                case TBInput.Controller.LHandController:
                    return SteamVR_Input_Sources.LeftHand;
                case TBInput.Controller.Gamepad:
                    return SteamVR_Input_Sources.Gamepad;
            }
            TBLogging.LogWarning("The controller type " + controller + " is not supported on Steam VR");
            return SteamVR_Input_Sources.Head;
        }

        public override bool SetRumble(TBInput.Controller controller, float strength)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                case TBInput.Controller.RHandController:
                    TBSteamVRActions.tButt_Haptic.Execute(0f, Time.unscaledDeltaTime, 100, strength, GetSteamVRInputSource(controller));
                    return true;
            }

            return false;
        }

        #region INPUT CHECKS
        public override bool ResolveButtonDown(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetButton = GetSteamVRButton(button);
            if (targetButton != null)
            {
                return targetButton.GetStateDown(GetSteamVRInputSource(controller));
            }
            else
            {
                return false;
            }
        }

        public override bool ResolveButtonUp(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetButton = GetSteamVRButton(button);
            if (targetButton != null)
            {
                return targetButton.GetStateUp(GetSteamVRInputSource(controller));
            }
            else
            {
                return false;
            }
        }

        public override bool ResolveButton(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetButton = GetSteamVRButton(button);
            if (targetButton != null)
            {
                return targetButton.GetState(GetSteamVRInputSource(controller));
            }
            else
            {
                return false;
            }
        }

        public override bool ResolveTouchDown(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetTouch = GetSteamVRTouch(button);
            if (targetTouch != null)
            {
                return targetTouch.GetStateDown(GetSteamVRInputSource(controller));
            }
            else
            {
                return false;
            }
        }

        public override bool ResolveTouchUp(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetTouch = GetSteamVRTouch(button);
            if (targetTouch != null)
            {
                return targetTouch.GetStateUp(GetSteamVRInputSource(controller));
            }
            else
            {
                return false;
            }
        }

        public override bool ResolveTouch(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetTouch = GetSteamVRTouch(button);
            if (targetTouch != null)
            {
                return targetTouch.GetState(GetSteamVRInputSource(controller));
            }
            else
            {
                return false;
            }
        }

        public override float ResolveAxis1D(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetVector1D = GetSteamVRVector1D(button);
            if (targetVector1D != null)
            {
                return targetVector1D.GetAxis(GetSteamVRInputSource(controller));
            }
            else
            {
                return 0;
            }
        }

        public override Vector2 ResolveAxis2D(SteamVRHardwareButton button, TBInput.Controller controller)
        {
            targetVector2D = GetSteamVRVector2D(button);
            if (targetVector2D != null)
            {
                return targetVector2D.GetAxis(GetSteamVRInputSource(controller));
            }
            else
            {
                return Vector2.zero;
            }
        }

        public override Quaternion GetRawRotation(TBInput.Controller controller)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                case TBInput.Controller.RHandController:
                    return TBSteamVRActions.tButt_Pose.GetLocalRotation(GetSteamVRInputSource(controller));
            }

            return Quaternion.identity;
        }

        public override Vector3 GetRawPosition(TBInput.Controller controller)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                case TBInput.Controller.RHandController:
                    return TBSteamVRActions.tButt_Pose.GetLocalPosition(GetSteamVRInputSource(controller));
            }
            return Vector3.zero;
        }

        public override Vector3 GetVelocity(TBInput.Controller controller)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                case TBInput.Controller.RHandController:
                    return TBSteamVRActions.tButt_Pose.GetVelocity(GetSteamVRInputSource(controller));
            }
            return Vector3.zero;
        }

        public override Vector3 GetAngularVelocity(TBInput.Controller controller)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                case TBInput.Controller.RHandController:
                    return TBSteamVRActions.tButt_Pose.GetAngularVelocity(GetSteamVRInputSource(controller));
            }
            return Vector3.zero;
        }

        /// <summary>
        /// On Steam, we only calculate acceleration when it's called. So the first frame in a series of calls will not be useful.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public override Vector3 GetAcceleration(TBInput.Controller controller)
        {
            Vector3 accel = Vector3.zero;
            Vector3 velocity;

            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    velocity = TBSteamVRActions.tButt_Pose.GetVelocity(GetSteamVRInputSource(controller));
                    accel = (velocity - lastLeftControllerVelocity) / Time.unscaledDeltaTime;
                    lastLeftControllerVelocity = velocity;
                    break;
                case TBInput.Controller.RHandController:
                    velocity = TBSteamVRActions.tButt_Pose.GetVelocity(GetSteamVRInputSource(controller));
                    accel = (velocity - lastRightControllerVelocity) / Time.unscaledDeltaTime;
                    lastRightControllerVelocity = velocity;
                    break;
            }

            return accel;
        }

        #region STEAM VR ACTION MAPPINGS
        protected SteamVR_Action_Boolean GetSteamVRButton(SteamVRHardwareButton button)
        {
            switch(button)
            {
                case SteamVRHardwareButton.PrimaryTrigger:
                    return TBSteamVRActions.tButt_MainTrigger_Button;
                case SteamVRHardwareButton.Grip:
                    return TBSteamVRActions.tButt_Grip_Button;
                case SteamVRHardwareButton.AX:
                    return TBSteamVRActions.tButt_AX_Button;
                case SteamVRHardwareButton.BY:
                    return TBSteamVRActions.tButt_BY_Button;
                case SteamVRHardwareButton.Touchpad:
                    return TBSteamVRActions.tButt_Joystick1_Button;
                case SteamVRHardwareButton.Joystick:
                    return TBSteamVRActions.tButt_Joystick2_Button;
                case SteamVRHardwareButton.Menu:
                    return TBSteamVRActions.tButt_Menu_Button;
                case SteamVRHardwareButton.Squeeze:
                    return TBSteamVRActions.tButt_Squeeze_Button;
                default:
                    return null;
            }
        }

        protected SteamVR_Action_Boolean GetSteamVRTouch(SteamVRHardwareButton button)
        {
            switch (button)
            {
                case SteamVRHardwareButton.PrimaryTrigger:
                    return TBSteamVRActions.tButt_MainTrigger_Touch;
                case SteamVRHardwareButton.Grip:
                    return TBSteamVRActions.tButt_Grip_Touch;
                case SteamVRHardwareButton.AX:
                    return TBSteamVRActions.tButt_AX_Touch;
                case SteamVRHardwareButton.BY:
                    return TBSteamVRActions.tButt_BY_Touch;
                case SteamVRHardwareButton.Touchpad:
                    return TBSteamVRActions.tButt_Joystick1_Touch;
                case SteamVRHardwareButton.Joystick:
                    return TBSteamVRActions.tButt_Joystick2_Touch;
                case SteamVRHardwareButton.Menu:
                    return TBSteamVRActions.tButt_Menu_Touch;
                default:
                    return null;
            }
        }

        protected SteamVR_Action_Single GetSteamVRVector1D(SteamVRHardwareButton button)
        {
            switch (button)
            {
                case SteamVRHardwareButton.PrimaryTrigger:
                    return TBSteamVRActions.tButt_MainTrigger_V1;
                case SteamVRHardwareButton.Grip:
                    return TBSteamVRActions.tButt_Grip_V1;
                case SteamVRHardwareButton.Squeeze:
                    return TBSteamVRActions.tButt_Squeeze_V1;
                default:
                    return null;
            }
        }

        protected SteamVR_Action_Vector2 GetSteamVRVector2D(SteamVRHardwareButton button)
        {
            switch (button)
            {
                case SteamVRHardwareButton.Touchpad:
                    return TBSteamVRActions.tButt_Joystick1_V2;
                case SteamVRHardwareButton.Joystick:
                    return TBSteamVRActions.tButt_Joystick2_V2;
                default:
                    return null;
            }
        }

        protected SteamVR_Action_Pose GetSteamVRPose()
        {
            return TBSteamVRActions.tButt_Pose;
        }
        #endregion
        #endregion
    #endif
            }
    [System.Serializable]
    public enum SteamVRHardwareButton
    {
        PrimaryTrigger = 0x000001,
        Grip = 0x000002,
        Menu = 0x000010,
        AX = 0x000100,
        BY = 0x000200,
        Squeeze = 0x001000,
        Touchpad = 0x010000,
        Joystick = 0x020000
    }
}
