using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TButt.Input;

namespace TButt
{
    /// <summary>
    /// Big 'ol input wrapper for all controller types used by major VR platforms.
    /// Set up button mappings using editor tools.
    /// </summary>
    public static class TBInput
    {
        public static ControlType connectedControlTypes;

        private static Transform _leftHand;
        private static Transform _rightHand;
        private static Transform _3DOFHand;

        private static float rumbleStrengthLow = 0.1f;
        private static float rumbleStrengthMed = 0.6f;
        private static float rumbleStrengthHi = 1f;

        private static float rumbleLengthShort = 0.05f;
        private static float rumbleLengthMed = 0.3f;
        private static float rumbleLengthLong = 0.6f;

        private static ControlType _activeControlType = ControlType.None;

        private static ITBSDKInput _activeSDK;
        private static bool _hasActiveSDK;

        #region VIRTUAL BUTTON AND CONTROLLER TYPES
        [Flags]
        public enum Button
        {
            None = 0,
            PrimaryTrigger = 0x000001,
            SecondaryTrigger = 0x000002,
            Start = 0x000010,
            Options = 0x000020,
            Action1 = 0x000100,
            Action2 = 0x000200,
            Action3 = 0x000400,
            Action4 = 0x000800,
            DpadUp = 0x001000,
            DpadDown = 0x002000,
            DpadLeft = 0x004000,
            DpadRight = 0x008000,
            Touchpad = 0x010000,
            Joystick = 0x020000,
            LeftStick = 0x040000,
            RightStick = 0x080000,
            LeftTrigger = 0x100000,
            RightTrigger = 0x200000,
            LeftBumper = 0x400000,
            RightBumper = 0x800000,
            Any = ~None
        }

        [Flags]
        public enum Controller
        {
            None,
            LHandController,            // Oculus Touch, HTC Vive, PlayStation Move
            RHandController,            // Oculus Touch, HTC Vive, PlayStation Move
            ClickRemote,                // Oculus Remote, Gear VR Touchpad, Mouse
            Mobile3DOFController,       // Daydream, Gear VR Controller
            Gamepad,
            Active
        }
        #endregion

        [Flags]
        public enum ControlType
        {
            None,
            Gamepad,
            HandControllers,
            Mobile3DOFController,
            ClickRemote
        }

        public enum Mobile3DOFHandedness
        {
            Left,
            Right,
            Center
        }

        public enum RumblePusleLength
        {
            Short,
            Medium,
            Long
        }

        public enum RumblePulseStrength
        {
            Low,
            Medium,
            High
        }

        public enum Finger
        {
            Thumb = 0,
            Index = 1,
            Grip = 10
        }

        /// <summary>
        /// Applies settings and adds required components for input to be detected based on the active platform.
        /// </summary>
        public static void Initialize(VRPlatform platform)
        {
            TBCore.OnUpdate += TBInput.Update;          

#if UNITY_EDITOR
            _activeControlType = TBSettings.GetControlSettings().defaultEditorControlType;
#endif

            _hasActiveSDK = true;

            switch (platform)
            {
                case VRPlatform.OculusPC:
                case VRPlatform.OculusMobile:
                    TBInputOculus.instance.Initialize();
                    TBCore.OnFixedUpdate += TBInputOculus.instance.FixedUpdate;
                    TBCore.OnUpdate += TBInputOculus.instance.Update;
                    _activeSDK = TBInputOculus.instance;
                    break;
                case VRPlatform.SteamVR:
                    TBInputSteamVR.instance.Initialize();
                    TBCore.OnFixedUpdate += TBInputSteamVR.instance.FixedUpdate;
                    TBCore.OnUpdate += TBInputSteamVR.instance.Update;
                    _activeSDK = TBInputSteamVR.instance;
                    break;
                case VRPlatform.Daydream:
                    TBInputGoogle.instance.Initialize();
                    TBCore.OnFixedUpdate += TBInputGoogle.instance.FixedUpdate;
                    TBCore.OnUpdate += TBInputGoogle.instance.Update;
                    _activeSDK = TBInputGoogle.instance;
                    break;
                case VRPlatform.PlayStationVR:
#if TB_HAS_UNITY_PS4
                    TBPSVRInput.instance.Initialize();
                    TBCore.OnFixedUpdate += TBPSVRInput.instance.FixedUpdate;
                    TBCore.OnUpdate += TBPSVRInput.instance.Update;
                    _activeSDK = TBPSVRInput.instance;
#else
                    UnityEngine.Debug.LogError("TBInput attempted to initialize for PSVR, but the PSVR module is not available. Is the module installed and set up with #TB_HAS_UNITY_PS4?");
#endif
                    break;
                case VRPlatform.WindowsMR:
                    TBWindowsMRInput.instance.Initialize();
                    TBCore.OnFixedUpdate += TBWindowsMRInput.instance.FixedUpdate;
                    TBCore.OnUpdate += TBWindowsMRInput.instance.Update;
                    _activeSDK = TBWindowsMRInput.instance;
                    break;
                default:
                    _hasActiveSDK = false;
                    UnityEngine.Debug.LogError("Attempted to initialize TBInput without an active SDK in TBCore. This shouldn't happen if TBCore exists in your scene.");
                    break;
            }

            TBLogging.LogMessage("Active Control Type: " + _activeControlType);
            if(_activeControlType == ControlType.None)
            {
                TBLogging.LogMessage("No active control type is assigned to TBInput. Input for type 'active' will be ignored until a control type is assigned.");
            }
        }

        /// <summary>
        /// Updates input readouts if necessary.
        /// </summary>
        public static void Update()
        {
            // Input Events are only executed if enabled in Input Settings.
            if (TBSettings.GetControlSettings().useInputEvents)
                Events.UpdateEvents();
        }

        /// <summary>
        /// Gets the active input type. Primarily useful on platforms where there are lots of input schemes.
        /// </summary>
        /// <returns></returns>
        public static Controller GetActiveController()
        {
            if(_hasActiveSDK)
                return _activeSDK.GetActiveController();
            else
                return Controller.None;
        }

        public static ControlType GetActiveControlType()
        {
            return _activeControlType;
        }

        public static VRController GetControllerModel(Controller controller)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetControllerModel(controller);
            else
                return VRController.None;
        }

        public static void SetActiveControlType(TBInput.ControlType controlType)
        {
            if (_activeControlType == controlType)
                return;

            _activeControlType = controlType;
            Events.Internal.UpdateControlType(_activeControlType);

            TBLogging.LogMessage("Control type changed to " + controlType.ToString());
        }

        public static string GetControllerName(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetControllerName(controller);
            else
                return "";
        }

        public static bool ControllerHasRumbleSupport(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.ControllerHasRumbleSupport(controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the button is being held down.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool GetButton(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetButton(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true the frame a button was released.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool GetButtonUp(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetButtonUp(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true the frame a button was pressed.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool GetButtonDown(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetButtonDown(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the button is being held down.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool GetTouch(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetTouch(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true the frame a button was released.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool GetTouchUp(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetTouchUp(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true the frame a button was pressed.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool GetTouchDown(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetTouchDown(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns 2D axis as Vector2. Values range from -1 to 1. (such as for touchpad or joystick)
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Vector2 GetAxis2D(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetAxis2D(button, controller);
            else
                return Vector2.zero;
        }

        /// <summary>
        /// Returns 1D axis as float. Values range from 0 to 1. (such as for a trigger)
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static float GetAxis1D(Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetAxis1D(button, controller);
            else
                return 0;
        }

        /// <summary>
        /// Returns the position of the controller in world space as reported by the active SDK (for tracked controllers).
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Vector3 GetPosition(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetRawPosition(controller);
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Returns the rotation of the controller in world space, as reported by the active SDK.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Quaternion GetRotation(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetRawRotation(controller);
            else
                return Quaternion.identity;
        }

        /// <summary>
        /// Returns the velocity of the specified controller, as reported by the active SDK.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Vector3 GetVelocity(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetVelocity(controller);
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Returns the angular velocity of the specified controller, as reported by the active SDK.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Vector3 GetAngularVelocity(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetAngularVelocity(controller);
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Returns the acceleration of the specified controller, as reported by the active SDK.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Vector3 GetAcceleration(Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetAcceleration(controller);
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Returns the hand presence animation float value of a finger (or group of fingers) on a controller. Useful for animating hands on controllers.
        /// </summary>
        /// <param name="finger"></param>
        /// <param name="controller"></param>
        /// <returns>Returns value from 0 to 1 if finger is resting on a trigger that supports float input. 
        /// Returns 0 or 1 if finger is a digital button, or 0.1 if the button supports touch and the finger is resting on it.</returns>
        public static float GetFinger(Finger finger, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetFinger(finger, controller);
            else
                return 0f;
        }

        /// <summary>
        /// Returns true if the active controller has hand presence animation values for the requested finger.
        /// </summary>
        /// <param name="finger"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool SupportsFinger(Finger finger, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.SupportsFinger(finger, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the requested button map can provide physical button press readouts on the specified controller.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool SupportsButton(TBInput.Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.SupportsButton(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the requested button map can provide touch values on the specified controller.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool SupportsTouch(TBInput.Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.SupportsTouch(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the requested button map can be read as an Axis1D on the specified controller.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool SupportsAxis1D(TBInput.Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.SupportsAxis1D(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the requested button map can be read as an Axis2D on the specified controller.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool SupportsAxis2D(TBInput.Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.SupportsAxis2D(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns true if the given virtual button has any mapping set on the specified controller.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool SupportsVirtualButton(TBInput.Button button, Controller controller = Controller.Active)
        {
            if (_hasActiveSDK)
                return _activeSDK.SupportsInputType(button, controller);
            else
                return false;
        }

        /// <summary>
        /// Returns 3DOF controller handedness as reported by the active SDK (Left, Right, or Center).
        /// </summary>
        /// <returns></returns>
        public static Mobile3DOFHandedness Get3DOFHandedness()
        {
            // In the editor, override hardware settings with whatever is set in the TButt Input Settings tool.
            if (TBCore.UsingEditorMode())
            {
                if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
                    return Mobile3DOFHandedness.Left;
                else
                    return Mobile3DOFHandedness.Right;
            }

            if (_hasActiveSDK)
                return _activeSDK.Get3DOFHandedness();
            else
                return TBInput.Mobile3DOFHandedness.Center;
        }

        /// <summary>
        /// Returns the tracking offsets for hand controllers as stored in TButt's controller config files. These are Turbo Button's best guesses, not official values.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static HandTrackingOffsets GetControllerTrackingOffsets(Controller controller)
        {
            if (_hasActiveSDK)
                return _activeSDK.GetControllerTrackingOffsets(controller);
            else
                return new HandTrackingOffsets();
        }

        /// <summary>
        /// Set rumble for this frame.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="amount">Betweeen 0 and 1</param>
        public static void SetRumble(Controller controller = Controller.Active, float amount = 0)
        {
            // Rumble isn't wrapped on two layers like other functions, so make sure active controller is specified here.
            if (controller == TBInput.Controller.Active)   
                controller = GetActiveController();

            if (_hasActiveSDK)
                _activeSDK.SetRumble(controller, amount);
            else
                return;
        }

        /// <summary>
        /// Set controller rumbles for predefined time intervals.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="strength"></param>
        /// <param name="length"></param>
        public static void StartRumblePulse(Controller controller = Controller.Active, RumblePulseStrength strength = RumblePulseStrength.Low, RumblePusleLength length = RumblePusleLength.Short)
        {
            float l = 0;
            float s = 0;
            switch (strength)
            {
                case RumblePulseStrength.Low:
                    s = rumbleStrengthLow;
                    break;
                case RumblePulseStrength.Medium:
                    s = rumbleStrengthMed;
                    break;
                case RumblePulseStrength.High:
                    s = rumbleStrengthHi;
                    break;
            }

            switch (length)
            {
                case RumblePusleLength.Short:
                    l = rumbleLengthShort;
                    break;
                case RumblePusleLength.Medium:
                    l = rumbleLengthMed;
                    break;
                case RumblePusleLength.Long:
                    l = rumbleLengthLong;
                    break;
            }

            StartRumblePulse(controller, s, l);
        }

        /// <summary>
        /// Set controller rumble for a specific interval and strength.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="strength"></param>
        /// <param name="length"></param>
        public static void StartRumblePulse(Controller controller = Controller.Active, float strength = 0.5f, float length = 0.2f)
        {
            if (_hasActiveSDK)
                _activeSDK.StartRumblePulse(controller, strength, length);
            else
                return;
        }

#region EVENTS
        public static class Events
        {
            public delegate void ButtonEvent();

            public delegate void ControlTypeEvent(ControlType type);
            public delegate void HandednessEvent(Mobile3DOFHandedness handedness);

            public static event ControlTypeEvent OnControlTypeChanged;
            public static event ControlTypeEvent OnControllerConnected;
            public static event ControlTypeEvent OnControllerDisconnected;
            public static event HandednessEvent OnHandednessChanged;

            public static class Internal
            {
                public static void SendControllerConnectionEvent(ControlType controlType, bool connected)
                {
                    if (connected)
                    {
                        if (Events.OnControllerConnected != null)
                            Events.OnControllerConnected(controlType);
                    }
                    else
                    {
                        if (Events.OnControllerDisconnected != null)
                            Events.OnControllerDisconnected(controlType);
                    }
                }

                public static void RefreshInput(bool needed)
                {
                    if (!needed)
                        return;
                }

                public static void UpdateControlType(ControlType type)
                {
                    if (OnControlTypeChanged != null)
                        OnControlTypeChanged(type);
                }

                public static void UpdateHandedness(Mobile3DOFHandedness handedness)
                {
                    if (OnHandednessChanged != null)
                        OnHandednessChanged(handedness);
                }
            }

            /// <summary>
            /// Button events for presses and releases.
            /// </summary>
            public static class Button
            {
                public static event ButtonEvent OnPrimaryTriggerButtonDown;
                public static event ButtonEvent OnPrimaryTriggerButtonUp;
                public static event ButtonEvent OnSecondaryTriggerButtonDown;
                public static event ButtonEvent OnSecondaryTriggerButtonUp;
                public static event ButtonEvent OnStartButtonDown;
                public static event ButtonEvent OnStartButtonUp;
                public static event ButtonEvent OnOptionsButtonDown;
                public static event ButtonEvent OnOptionsButtonUp;
                public static event ButtonEvent OnAction1ButtonDown;
                public static event ButtonEvent OnAction1ButtonUp;
                public static event ButtonEvent OnAction2ButtonDown;
                public static event ButtonEvent OnAction2ButtonUp;
                public static event ButtonEvent OnAction3ButtonDown;
                public static event ButtonEvent OnAction3ButtonUp;
                public static event ButtonEvent OnAction4ButtonDown;
                public static event ButtonEvent OnAction4ButtonUp;

                /// <summary>
                /// Runs through events. Should only be called by TBInput.
                /// </summary>
                public static void UpdateButtonEvents(Controller controller)
                {
                    // Action Button Events
                    if (OnAction1ButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.Action1, controller))
                            OnAction1ButtonDown();
                    if (OnAction1ButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.Action1, controller))
                            OnAction1ButtonUp();
                    if (OnAction2ButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.Action2, controller))
                            OnAction2ButtonDown();
                    if (OnAction2ButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.Action2, controller))
                            OnAction2ButtonUp();
                    if (OnAction3ButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.Action3, controller))
                            OnAction3ButtonDown();
                    if (OnAction3ButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.Action3, controller))
                            OnAction3ButtonUp();
                    if (OnAction4ButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.Action4, controller))
                            OnAction4ButtonDown();
                    if (OnAction4ButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.Action4, controller))
                            OnAction4ButtonUp();

                    // Trigger Events
                    if (OnPrimaryTriggerButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.PrimaryTrigger, controller))
                            OnPrimaryTriggerButtonDown();
                    if (OnPrimaryTriggerButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.PrimaryTrigger, controller))
                            OnPrimaryTriggerButtonUp();
                    if (OnSecondaryTriggerButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.SecondaryTrigger, controller))
                            OnSecondaryTriggerButtonDown();
                    if (OnSecondaryTriggerButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.SecondaryTrigger, controller))
                            OnSecondaryTriggerButtonUp();

                    // Start / Options Events
                    if (OnStartButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.Start, controller))
                            OnStartButtonDown();
                    if (OnStartButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.Start, controller))
                            OnStartButtonUp();
                    if (OnOptionsButtonDown != null)
                        if (TBInput.GetButtonDown(TBInput.Button.Options, controller))
                            OnOptionsButtonDown();
                    if (OnOptionsButtonUp != null)
                        if (TBInput.GetButtonUp(TBInput.Button.Options, controller))
                            OnOptionsButtonUp();
                }
            }

            public static class Touch
            {
                public static event ButtonEvent OnPrimaryTriggerTouchDown;
                public static event ButtonEvent OnPrimaryTriggerTouchUp;
                public static event ButtonEvent OnSecondaryTriggerTouchDown;
                public static event ButtonEvent OnSecondaryTriggerTouchUp;
                public static event ButtonEvent OnStartTouchDown;
                public static event ButtonEvent OnStartTouchUp;
                public static event ButtonEvent OnOptionsTouchDown;
                public static event ButtonEvent OnOptionsTouchUp;
                public static event ButtonEvent OnAction1TouchDown;
                public static event ButtonEvent OnAction1TouchUp;
                public static event ButtonEvent OnAction2TouchDown;
                public static event ButtonEvent OnAction2TouchUp;
                public static event ButtonEvent OnAction3TouchDown;
                public static event ButtonEvent OnAction3TouchUp;
                public static event ButtonEvent OnAction4TouchDown;
                public static event ButtonEvent OnAction4TouchUp;

                /// <summary>
                /// Runs through events. Should only be called by TBInput.
                /// </summary>
                public static void UpdateButtonEvents(Controller controller)
                {
                    // Action Button Events
                    if (OnAction1TouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.Action1, controller))
                            OnAction1TouchDown();
                    if (OnAction1TouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.Action1, controller))
                            OnAction1TouchUp();
                    if (OnAction2TouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.Action2, controller))
                            OnAction2TouchDown();
                    if (OnAction2TouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.Action2, controller))
                            OnAction2TouchUp();
                    if (OnAction3TouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.Action3, controller))
                            OnAction3TouchDown();
                    if (OnAction3TouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.Action3, controller))
                            OnAction3TouchUp();
                    if (OnAction4TouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.Action4, controller))
                            OnAction4TouchDown();
                    if (OnAction4TouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.Action4, controller))
                            OnAction4TouchUp();

                    // Trigger Events
                    if (OnPrimaryTriggerTouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.PrimaryTrigger, controller))
                            OnPrimaryTriggerTouchDown();
                    if (OnPrimaryTriggerTouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.PrimaryTrigger, controller))
                            OnPrimaryTriggerTouchUp();
                    if (OnSecondaryTriggerTouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.SecondaryTrigger, controller))
                            OnSecondaryTriggerTouchDown();
                    if (OnSecondaryTriggerTouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.SecondaryTrigger, controller))
                            OnSecondaryTriggerTouchUp();

                    // Start / Options Events
                    if (OnStartTouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.Start, controller))
                            OnStartTouchDown();
                    if (OnStartTouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.Start, controller))
                            OnStartTouchUp();
                    if (OnOptionsTouchDown != null)
                        if (TBInput.GetTouchDown(TBInput.Button.Options, controller))
                            OnOptionsTouchDown();
                    if (OnOptionsTouchUp != null)
                        if (TBInput.GetTouchUp(TBInput.Button.Options, controller))
                            OnOptionsTouchUp();
                }
            }

            /// <summary>
            /// Runs through events. Should only be called by TBInput.
            /// </summary>
            public static void UpdateEvents()
            {
                switch (TBInput.GetActiveControlType())
                {
                    case ControlType.HandControllers:
                        Button.UpdateButtonEvents(Controller.LHandController);
                        Button.UpdateButtonEvents(Controller.RHandController);
                        Touch.UpdateButtonEvents(Controller.LHandController);
                        Touch.UpdateButtonEvents(Controller.RHandController);
                        break;
                    default:
                        Button.UpdateButtonEvents(Controller.Active);
                        Touch.UpdateButtonEvents(Controller.Active);
                        break;
                }
            }
        }
#endregion

#region GENERIC INPUT CHECKS
        /// <summary>
        /// Gets an array of raw (SDK-based) buttons for a given TBInput button type. Returns based on configured button maps.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="virtualButton"></param>
        /// <param name="lookupTable"></param>
        /// <returns></returns>
        public static T[] GetRawButtons<T>(Button virtualButton, ButtonLookupTable<T> lookupTable)
        {
            switch (virtualButton)
            {
                case Button.PrimaryTrigger:
                    return lookupTable.PrimaryTrigger;
                case Button.SecondaryTrigger:
                    return lookupTable.SecondaryTrigger;
                case Button.Start:
                    return lookupTable.Start;
                case Button.Options:
                    return lookupTable.Options;
                case Button.Action1:
                    return lookupTable.Action1;
                case Button.Action2:
                    return lookupTable.Action2;
                case Button.Action3:
                    return lookupTable.Action3;
                case Button.Action4:
                    return lookupTable.Action4;
                case Button.DpadUp:
                    return lookupTable.DpadUp;
                case Button.DpadDown:
                    return lookupTable.DpadDown;
                case Button.DpadLeft:
                    return lookupTable.DpadLeft;
                case Button.DpadRight:
                    return lookupTable.DpadRight;
                case Button.Touchpad:
                    return lookupTable.Touchpad;
                case Button.Joystick:
                    return lookupTable.Joystick;
                case Button.LeftTrigger:
                    return lookupTable.LeftTrigger;
                case Button.RightTrigger:
                    return lookupTable.RightTrigger;
                case Button.LeftBumper:
                    return lookupTable.LeftBumper;
                case Button.RightBumper:
                    return lookupTable.RightBumper;
                case Button.LeftStick:
                    return lookupTable.LeftStick;
                case Button.RightStick:
                    return lookupTable.RightStick;
                case Button.Any:
                    return lookupTable.Any;
            }

            UnityEngine.Debug.LogError("Button " + virtualButton + " isn't defined in the lookup table! This is a core TButt error, please report it.");
            return null;
        }
#endregion

#region BUTTON DEF STRUCTURES
        [Serializable]
        public class ButtonDef<T>
        {
            [SerializeField]
            public T rawButton;
            [SerializeField]
            public TBInput.Button[] virtualButtons;
            [SerializeField]
            public string name;
            [SerializeField]
            public bool supportsTouch = false;
            [SerializeField]
            public bool supportsAxis1D = false;
            [SerializeField]
            public bool supportsAxis2D = false;
            [SerializeField]
            public bool supportsButton = true;
        }

        [Serializable]
        public class SerializedButtonDef
        {
            [SerializeField]
            public TBInput.Button[] virtualButtons;
        }

        /// <summary>
        /// Loads a button set from JSON by its filename.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buttons"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<ButtonDef<T>> LoadButtonDefs<T>(List<ButtonDef<T>> buttons, string fileName)
        {
            if (Application.isPlaying)
                TBLogging.LogMessage("Loading button maps from " + TBSettings.settingsFolder + fileName + "...");

            SerializedButtonDef[] serializedButtonDefs = TBDataManager.FromJsonWrapper<SerializedButtonDef>(TBDataManager.DeserializeFromFile<TBDataManager.Wrapper<TBInput.SerializedButtonDef>>(TBSettings.settingsFolder + fileName, TBDataManager.PathType.ResourcesFolder));

            if (serializedButtonDefs == null)
            {
                if (Application.isPlaying)
                    TBLogging.LogWarning("Serialized button maps for " + fileName + " were invalid (empty file). Using defaults.");
                return buttons;
            }

            if (serializedButtonDefs.Length != buttons.Count)
                TBLogging.LogError("Serialized button maps for " + fileName + " exist, but are invalid! (Input count mismatch). You need to clear the JSON file or manually fix it.");

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].virtualButtons = serializedButtonDefs[i].virtualButtons;
            }

            if (Application.isPlaying)
                TBLogging.LogMessage("Done! Found maps for " + buttons.Count + " buttons in " + TBSettings.settingsFolder + fileName + ".");

            return buttons;
        }
#endregion

#region RUNTIME LOOKUP TABLE CREATION
        public struct ButtonLookupTable<T>
        {
            public T[] PrimaryTrigger;
            public T[] SecondaryTrigger;
            public T[] Start;
            public T[] Options;
            public T[] Action1;
            public T[] Action2;
            public T[] Action3;
            public T[] Action4;
            public T[] DpadUp;
            public T[] DpadDown;
            public T[] DpadLeft;
            public T[] DpadRight;
            public T[] Touchpad;
            public T[] Joystick;
            public T[] LeftStick;
            public T[] RightStick;
            public T[] LeftTrigger;
            public T[] RightTrigger;
            public T[] LeftBumper;
            public T[] RightBumper;
            public T[] Any;
        }

        public static ButtonLookupTable<T> NewLookupTableFromDefs<T>(List<ButtonDef<T>> defs)
        {
            ButtonLookupTable<T> lookupTable = new ButtonLookupTable<T>();

            for (int i = 0; i < defs.Count; i++)
            {
                SetupVirtualMap(ref lookupTable, defs[i].rawButton, Button.Any);

                for (int j = 0; j < defs[i].virtualButtons.Length; j++)
                {
                    if (defs[i].virtualButtons[j] != (Button.None | Button.Any))
                    {
                        SetupVirtualMap(ref lookupTable, defs[i].rawButton, defs[i].virtualButtons[j]);
                    }
                }
            }
            return lookupTable;
        }

        public static void SetupVirtualMap<T>(ref ButtonLookupTable<T> table, T rawButton, TBInput.Button virtualButton)
        {
            switch (virtualButton)
            {
                case Button.PrimaryTrigger:
                    BindVirtualMapToButton<T>(ref table.PrimaryTrigger, rawButton);
                    break;
                case Button.SecondaryTrigger:
                    BindVirtualMapToButton<T>(ref table.SecondaryTrigger, rawButton);
                    break;
                case Button.Start:
                    BindVirtualMapToButton<T>(ref table.Start, rawButton);
                    break;
                case Button.Options:
                    BindVirtualMapToButton<T>(ref table.Options, rawButton);
                    break;
                case Button.Action1:
                    BindVirtualMapToButton<T>(ref table.Action1, rawButton);
                    break;
                case Button.Action2:
                    BindVirtualMapToButton<T>(ref table.Action2, rawButton);
                    break;
                case Button.Action3:
                    BindVirtualMapToButton<T>(ref table.Action3, rawButton);
                    break;
                case Button.Action4:
                    BindVirtualMapToButton<T>(ref table.Action4, rawButton);
                    break;
                case Button.DpadUp:
                    BindVirtualMapToButton<T>(ref table.DpadUp, rawButton);
                    break;
                case Button.DpadDown:
                    BindVirtualMapToButton<T>(ref table.DpadDown, rawButton);
                    break;
                case Button.DpadLeft:
                    BindVirtualMapToButton<T>(ref table.DpadLeft, rawButton);
                    break;
                case Button.DpadRight:
                    BindVirtualMapToButton<T>(ref table.DpadRight, rawButton);
                    break;
                case Button.Touchpad:
                    BindVirtualMapToButton<T>(ref table.Touchpad, rawButton);
                    break;
                case Button.Joystick:
                    BindVirtualMapToButton<T>(ref table.Joystick, rawButton);
                    break;
                case Button.LeftTrigger:
                    BindVirtualMapToButton<T>(ref table.LeftTrigger, rawButton);
                    break;
                case Button.RightTrigger:
                    BindVirtualMapToButton<T>(ref table.RightTrigger, rawButton);
                    break;
                case Button.LeftBumper:
                    BindVirtualMapToButton<T>(ref table.LeftBumper, rawButton);
                    break;
                case Button.RightBumper:
                    BindVirtualMapToButton<T>(ref table.RightBumper, rawButton);
                    break;
                case Button.LeftStick:
                    BindVirtualMapToButton<T>(ref table.LeftStick, rawButton);
                    break;
                case Button.RightStick:
                    BindVirtualMapToButton<T>(ref table.RightStick, rawButton);
                    break;
                case Button.Any:
                    BindVirtualMapToButton<T>(ref table.Any, rawButton);
                    break;
            }
        }

        public static void BindVirtualMapToButton<T>(ref T[] virtualButtonGroup, T rawButton)
        {
            T[] newVirtualButton;
            if (virtualButtonGroup != null)
            {
                newVirtualButton = new T[virtualButtonGroup.Length + 1];
                for (int i = 0; i < virtualButtonGroup.Length; i++)
                {
                    newVirtualButton[i] = virtualButtonGroup[i];
                }
                newVirtualButton[virtualButtonGroup.Length] = rawButton;
            }
            else
            {
                newVirtualButton = new T[1] { rawButton };
            }
            virtualButtonGroup = newVirtualButton;
        }
#endregion
    }
}