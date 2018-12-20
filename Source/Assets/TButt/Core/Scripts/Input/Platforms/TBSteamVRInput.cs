using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
#if TB_STEAM_VR
using Valve.VR;
#endif

namespace TButt.Input
{
    #if TB_STEAM_VR
    public class TBInputSteamVR : TBSDKInputWithXInputBase<EVRButtonId>
    #else
    public class TBInputSteamVR : TBSDKInputWithXInputBase<TBInput.Button>
    #endif
    { 
        protected static TBInputSteamVR _instance;

        protected Vector3 lastLeftControllerVelocity = Vector3.one;
        protected Vector3 lastRightControllerVelocity = Vector3.one;

        public static TBInputSteamVR instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBInputSteamVR();
                return _instance;
            }
        }

        #if TB_STEAM_VR
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadHandControllers()
        {
            switch(TBCore.GetActiveHeadset())
            {
                case VRHeadset.OculusRift:
                    controller_LHand = TBController_SteamVR_OculusTouchLeft.instance;
                    controller_RHand = TBController_SteamVR_OculusTouchRight.instance;
                    break;
                case VRHeadset.WindowsMR:
                    controller_LHand = TBController_SteamVR_WindowsMixedRealityLeft.instance;
                    controller_RHand = TBController_SteamVR_WindowsMixedRealityRight.instance;
                    break;
                default:
                    controller_LHand = TBController_SteamVR_ViveControllerLeft.instance;
                    controller_RHand = TBController_SteamVR_ViveControllerRight.instance;
                    break;
            }
        }

        protected override void Load3DOFControllers()
        {
            // 3DOF Controller emulation in the editor.
            if (TBCore.UsingEditorMode() && TBSettings.GetControlSettings().emulate3DOFArmModel)
            {
                if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
                {
                    switch (TBCore.GetActiveHeadset())
                    {
                        case VRHeadset.OculusRift:
                            controller_3DOF = TBController_SteamVR_OculusTouchLeft.instance;
                            break;
                        case VRHeadset.WindowsMR:
                            controller_3DOF = TBController_SteamVR_WindowsMixedRealityLeft.instance;
                            break;
                        default:
                            controller_3DOF = TBController_SteamVR_ViveControllerLeft.instance;
                            break;
                    }
                }
                else
                {
                    switch (TBCore.GetActiveHeadset())
                    {
                        case VRHeadset.OculusRift:
                            controller_3DOF = TBController_SteamVR_OculusTouchRight.instance;
                            break;
                        case VRHeadset.WindowsMR:
                            controller_3DOF = TBController_SteamVR_WindowsMixedRealityRight.instance;
                            break;
                        default:
                            controller_3DOF = TBController_SteamVR_ViveControllerRight.instance;
                            break;
                    }
                }
            }
        }

        public uint GetSteamVRControllerID(TBInput.Controller controller)
        {
            switch(controller)
            {
                case TBInput.Controller.RHandController:
                    return TBSteamVRDeviceManager.instance.GetRightControllerID();
                case TBInput.Controller.LHandController:
                    return TBSteamVRDeviceManager.instance.GetLeftControllerID();
            }
            TBLogging.LogWarning("Requested device index for controller type " + controller + " that is not supported by Steam VR. Assuming device ID is 0");
            return 0;
        }

        public override bool SetRumble(TBInput.Controller controller, float strength)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                SteamVR_Controller.Input((int)id).TriggerHapticPulse((ushort)(3999 * strength), EVRButtonId.k_EButton_SteamVR_Touchpad);
            }
            return true;
        }

                #region INPUT CHECKS
        public override bool ResolveButtonDown(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                if (SteamVR_Controller.Input((int)id).GetPressDown(button))
                    return true;
            }
            return false;
        }

        public override bool ResolveButtonUp(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                if (SteamVR_Controller.Input((int)GetSteamVRControllerID(controller)).GetPressUp(button))
                    return true;
            }
            return false;
        }

        public override bool ResolveButton(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                if (SteamVR_Controller.Input((int)id).GetPress(button))
                    return true;
            }
            return false;
        }

        public override bool ResolveTouchDown(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                if (SteamVR_Controller.Input((int)id).GetTouchDown(button))
                    return true;
            }
            return false;
        }

        public override bool ResolveTouchUp(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                if (SteamVR_Controller.Input((int)id).GetTouchUp(button))
                    return true;
            }
            return false;
        }

        public override bool ResolveTouch(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                if (SteamVR_Controller.Input((int)id).GetTouch(button))
                    return true;
            }
            return false;
        }

        public override float ResolveAxis1D(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                return SteamVR_Controller.Input((int)id).GetAxis(button).x;
            }
            return 0;
        }

        public override Vector2 ResolveAxis2D(EVRButtonId button, TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);
            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                return SteamVR_Controller.Input((int)id).GetAxis(button);
            }
            return Vector2.zero;
        }

        public override Quaternion GetRawRotation(TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);

            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
               return SteamVR_Controller.Input((int)id).transform.rot;
            }

            return Quaternion.identity;
        }

        public override Vector3 GetRawPosition(TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);

            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
               return SteamVR_Controller.Input((int)id).transform.pos;
            }

            return Vector3.zero;
        }

        public override Vector3 GetVelocity(TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);

            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                return SteamVR_Controller.Input((int)id).velocity;
            }

            return Vector3.zero;
        }

        public override Vector3 GetAngularVelocity(TBInput.Controller controller)
        {
            uint id = GetSteamVRControllerID(controller);

            if (OpenVR.System.IsTrackedDeviceConnected(id))
            {
                return SteamVR_Controller.Input((int)id).angularVelocity;
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
            uint id = GetSteamVRControllerID(controller);
            Vector3 accel = Vector3.zero;
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    accel = (SteamVR_Controller.Input((int)id).velocity - lastLeftControllerVelocity) / Time.unscaledDeltaTime;
                    lastLeftControllerVelocity = SteamVR_Controller.Input((int)id).velocity;
                    break;
                case TBInput.Controller.RHandController:
                    accel = (SteamVR_Controller.Input((int)id).velocity - lastRightControllerVelocity) / Time.unscaledDeltaTime;
                    lastRightControllerVelocity = SteamVR_Controller.Input((int)id).velocity;
                    break;
            }
            return accel;
        }
                #endregion

                #region DEFAULT BUTTON DEFS
        public static List<TBInput.ButtonDef<EVRButtonId>> GetDefaultViveDefs()
        {
            return new List<TBInput.ButtonDef<EVRButtonId>>
            {
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Trigger,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.PrimaryTrigger },
                    name = "Trigger",
                    supportsAxis1D = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_Grip,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.SecondaryTrigger },
                    name = "Grip" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_ApplicationMenu,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Options },
                    name = "Menu" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_SteamVR_Touchpad,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.Touchpad },
                    name = "Touchpad",
                    supportsTouch = true,
                    supportsAxis2D = true },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_DPad_Left,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadLeft },
                    name = "Touchpad Left" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_DPad_Right,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadRight },
                    name = "Touchpad Right" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_DPad_Up,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadUp },
                    name = "Touchpad Up" },
                new TBInput.ButtonDef<EVRButtonId>() {
                    rawButton = EVRButtonId.k_EButton_DPad_Down,
                    virtualButtons = new TBInput.Button[] { TBInput.Button.DpadDown },
                    name = "Touchpad Down" }
            };
        }
                #endregion
#endif
            }
        }
