using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.XR;

namespace TButt.Input
{
    /// <summary>
    /// Interface that lets TBInput talk to SDKs with generic types.
    /// </summary>
    public interface ITBSDKInput
    {
        // Functionality
        void Initialize();
        void Update();
        void FixedUpdate();
        void RefreshInput(bool needed);

        // Input checks
        bool GetButton(TBInput.Button button, TBInput.Controller controller);
        bool GetButtonDown(TBInput.Button button, TBInput.Controller controller);
        bool GetButtonUp(TBInput.Button button, TBInput.Controller controller);
        bool GetTouch(TBInput.Button button, TBInput.Controller controller);
        bool GetTouchDown(TBInput.Button button, TBInput.Controller controller);
        bool GetTouchUp(TBInput.Button button, TBInput.Controller controller);
        float GetAxis1D(TBInput.Button button, TBInput.Controller controller);
        Vector2 GetAxis2D(TBInput.Button button, TBInput.Controller controller);
        bool SetRumble(TBInput.Controller controller, float amount);
        void StartRumblePulse(TBInput.Controller controller, float strength, float length);

        // Tracking checks
        Vector3 GetRawPosition(TBInput.Controller controller);
        Quaternion GetRawRotation(TBInput.Controller controller);
        Vector3 GetVelocity(TBInput.Controller controller);
        Vector3 GetAngularVelocity(TBInput.Controller controller);
        Vector3 GetAcceleration(TBInput.Controller controller);

        float GetFinger(TBInput.Finger finger, TBInput.Controller controller);
        HandTrackingOffsets GetControllerTrackingOffsets(TBInput.Controller controller);

        // Info checks
        #if UNITY_2018_3_OR_NEWER
        InputDevice GetUnityXRInputDevice(TBInput.Controller controller);
        #endif
        TBInput.Controller GetActiveController();
        string GetControllerName(TBInput.Controller controller);
        VRController GetControllerModel(TBInput.Controller controller);
        bool ControllerHasRumbleSupport(TBInput.Controller controller);
        bool SupportsButton(TBInput.Button button, TBInput.Controller controller);
        bool SupportsTouch(TBInput.Button button, TBInput.Controller controller);
        bool SupportsAxis1D(TBInput.Button button, TBInput.Controller controller);
        bool SupportsAxis2D(TBInput.Button button, TBInput.Controller controller);
        bool SupportsInputType(TBInput.Button button, TBInput.Controller controller);
        bool SupportsFinger(TBInput.Finger finger, TBInput.Controller controller);
        TBInput.Mobile3DOFHandedness Get3DOFHandedness();
    }

    /// <summary>
    /// Base input class. All SDK-based classes extend from this.
    /// </summary>
    public abstract class TBSDKInputBase<T> : ITBSDKInput
    {
        protected TBInput.Controller _activeController;

        // This gets overridden when we want to use XInput for gamepad detection (such as on Steam)
        protected bool _usesXInputForGamepads = false;

        protected TBInput.ButtonLookupTable<T> emptyLookupTable;
        protected TBControllerBase<T> controller_LHand;
        protected TBControllerBase<T> controller_RHand;
        protected TBControllerBase<T> controller_3DOF;
        protected TBControllerBase<T> controller_Gamepad;
        protected TBControllerBase<T> controller_ClickRemote;
        protected T[] _fingerButtons;
        protected Coroutine leftRumbleRoutine;
        protected Coroutine rightRumbleRoutine;

        public virtual void Initialize()
        {
            emptyLookupTable = new TBInput.ButtonLookupTable<T>();
            _activeController = GetActiveController();

            // Load supported input types.
            if (TBSettings.GetControlSettings().supportsHandControllers)
                LoadHandControllers();
            if (TBSettings.GetControlSettings().supports3DOFControllers)
                Load3DOFControllers();
            if (TBSettings.GetControlSettings().supportsClickRemote)
                LoadClickRemotes();
            if (TBSettings.GetControlSettings().supportsGamepad)
                LoadGamepads();

            TBCore.Events.OnSystemMenu += RefreshInput;
        }

        protected virtual void OnDestroy()
        {
            TBCore.Events.OnSystemMenu -= RefreshInput;
        }

        protected virtual void LoadHandControllers()
        {
            TBLogging.LogMessage("Hand Controllers are not defined for " + TBCore.GetActivePlatform() + ". Skipping...");
        }

        protected virtual void Load3DOFControllers()
        {
            TBLogging.LogMessage("3DOF Controllers are not defined for " + TBCore.GetActivePlatform() + ". Skipping...");
        }

        protected virtual void LoadClickRemotes()
        {
            TBLogging.LogMessage("Click Remotes are not defined for " + TBCore.GetActivePlatform() + ". Skipping...");
        }

        protected virtual void LoadGamepads()
        {
            TBLogging.LogMessage("Gamepads are not defined for " + TBCore.GetActivePlatform() + ". Skipping...");
        }
   
        public virtual void Update()
        {
            return;
        }

        public virtual void FixedUpdate()
        {
            return;
        }

        public virtual void RefreshInput(bool needed)
        {
            if (!needed)
                return;
            else
                TBCore.instance.StartCoroutine(RefreshInputRoutine());
        }

        protected virtual IEnumerator RefreshInputRoutine()
        {
            switch (TBInput.GetActiveControlType())
            {
                case TBInput.ControlType.Mobile3DOFController:
                    while (Get3DOFHandedness() == TBInput.Mobile3DOFHandedness.Center)
                        yield return null;
                    TBInput.Events.Internal.UpdateHandedness(Get3DOFHandedness());
                    break;
            }
        }

        public virtual TBInput.Controller GetActiveController()
        {
            switch (TBInput.GetActiveControlType())
            {
                case TBInput.ControlType.ClickRemote:
                    return TBInput.Controller.ClickRemote;
                case TBInput.ControlType.Gamepad:
                    return TBInput.Controller.Gamepad;
                case TBInput.ControlType.HandControllers:
                    return TBInput.Controller.RHandController;
                case TBInput.ControlType.Mobile3DOFController:
                    return TBInput.Controller.Mobile3DOFController;
                default:
                    return TBInput.Controller.None;
            }
        }

        #if UNITY_2018_3_OR_NEWER
        public virtual InputDevice GetUnityXRInputDevice(TBInput.Controller controller)
        {
            if(controller == TBInput.Controller.Active)
                controller = GetActiveController();

            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    return InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                case TBInput.Controller.RHandController:
                    return InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                default:
                    return InputDevices.GetDeviceAtXRNode(XRNode.GameController);
            }
        }
        #endif

        public virtual string GetControllerName(TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = GetActiveController();

            if (GetControllerForType(controller) != null)
                return GetControllerForType(controller).GetName();
            TBLogging.LogWarning("Attempted to read controller name, but controller is undefined!");
            return "Undefined";
        }

        public virtual VRController GetControllerModel(TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = GetActiveController();

            if (GetControllerForType(controller) != null)
                return GetControllerForType(controller).GetModel();
            TBLogging.LogWarning("Attempted to get controller model for " + controller + " but controller is undefined!");
            return VRController.None;
        }

        protected static List<TBInput.ButtonDef<T>> LoadMappingsForController(TBControllerBase<T> controller)
        {
            return TBInput.LoadButtonDefs<T>(controller.GetDefaultDefs(), controller.GetFileName());
        }

        protected TBControllerBase<T> GetControllerForType(TBInput.Controller controller)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    return controller_LHand;
                case TBInput.Controller.RHandController:
                    return controller_RHand;
                case TBInput.Controller.Mobile3DOFController:
                    return controller_3DOF;
                case TBInput.Controller.ClickRemote:
                    return controller_ClickRemote;
                case TBInput.Controller.Gamepad:
                    return controller_Gamepad;
            }
            return null;
        }

        protected TBInput.ButtonLookupTable<T> GetButtonLookupTableForController(TBInput.Controller controller)
        {
            if (GetControllerForType(controller) != null)
                return GetControllerForType(controller).GetLookupTable();
            else
                return emptyLookupTable;
        }

        protected virtual T[] GetButtonArray(TBInput.Button button, ref TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = TBInput.GetActiveController();

            return TBInput.GetRawButtons<T>(button, GetButtonLookupTableForController(controller));
        }

               #region INPUT CHECKS - GET FUNCTIONS
        // The "Get" functions rely on the generic type T, which gets filled in by the classes that extend this one for each SDK.
        public virtual bool GetButtonDown(TBInput.Button button, TBInput.Controller controller)
        {
            bool resolution = false;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ResolveButtonDown(buttons[i], controller))
                    resolution = true;
            }
            return resolution;
        }

        public virtual bool GetButtonUp(TBInput.Button button, TBInput.Controller controller)
        {
            bool resolution = false;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ResolveButtonUp(buttons[i], controller))
                    resolution = true;
            }
            return resolution;
        }

        public virtual bool GetButton(TBInput.Button button, TBInput.Controller controller)
        {
            bool resolution = false;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ResolveButton(buttons[i], controller))
                    resolution = true;
            }
            return resolution;
        }

        public virtual bool GetTouchDown(TBInput.Button button, TBInput.Controller controller)
        {
            bool resolution = false;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ResolveTouchDown(buttons[i], controller))
                    resolution = true;
            }
            return resolution;
        }

        public virtual bool GetTouchUp(TBInput.Button button, TBInput.Controller controller)
        {
            bool resolution = false;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ResolveTouchUp(buttons[i], controller))
                    resolution = true;
            }
            return resolution;
        }

        public virtual bool GetTouch(TBInput.Button button, TBInput.Controller controller)
        {
            bool resolution = false;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ResolveTouch(buttons[i], controller))
                    resolution = true;
            }
            return resolution;
        }

        public virtual float GetAxis1D(TBInput.Button button, TBInput.Controller controller)
        {
            float resolution = 0;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return resolution;
            for (int i = 0; i < buttons.Length; i++)
            {
                float axis = ResolveAxis1D(buttons[i], controller);
                if (axis > resolution)
                    resolution = axis;
            }
            return resolution;
        }

        public virtual Vector2 GetAxis2D(TBInput.Button button, TBInput.Controller controller)
        {
            Vector2 resolution = Vector2.zero;
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return resolution;
            for (int i = 0; i < buttons.Length; i++)
            {
                Vector2 axis = ResolveAxis2D(buttons[i], controller);
                if (axis == Vector2.zero)
                    continue;
                if (axis.sqrMagnitude > resolution.sqrMagnitude)
                    resolution = axis;
            }
            return resolution;
        }

        public virtual Vector3 GetRawPosition(TBInput.Controller controller)
        {
            Vector3 resolution = Vector3.zero;
            return resolution;
        }

        public virtual Quaternion GetRawRotation(TBInput.Controller controller)
        {
            Quaternion resolution = Quaternion.identity;
            return resolution;
        }

        public virtual Vector3 GetVelocity(TBInput.Controller controller)
        {
            Debug.Log("Velocity check is not yet implemented for this platform.");
            return Vector3.zero;
        }

        public virtual Vector3 GetAngularVelocity(TBInput.Controller controller)
        {
            Debug.Log("Angular velocity check is not yet implemented for this platform.");
            return Vector3.zero;
        }

        public virtual Vector3 GetAcceleration(TBInput.Controller controller)
        {
            Debug.Log("Acceleration check is not yet implemented for this platform.");
            return Vector3.zero;
        }

        public virtual bool SetRumble(TBInput.Controller controller, float strength)
        {
            TBLogging.LogMessage("Rumble is not implemented for this platform.");
            return false;
        }

        public virtual void StartRumblePulse(TBInput.Controller controller, float strength, float length)
        {
            if (controller == TBInput.Controller.Active)
                controller = GetActiveController();

            if (!SetRumble(controller, strength))
                return;

            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    if (leftRumbleRoutine != null)
                        TBCore.instance.StopCoroutine(leftRumbleRoutine);
                    leftRumbleRoutine = TBCore.instance.StartCoroutine(RumblePulseRoutine(controller, strength, length));
                    break;
                case TBInput.Controller.RHandController:
                    if (rightRumbleRoutine != null)
                        TBCore.instance.StopCoroutine(rightRumbleRoutine);
                    rightRumbleRoutine = TBCore.instance.StartCoroutine(RumblePulseRoutine(controller, strength, length));
                    break;
                case TBInput.Controller.Mobile3DOFController:
                case TBInput.Controller.Gamepad:
                case TBInput.Controller.ClickRemote:
                    if (leftRumbleRoutine != null)
                        TBCore.instance.StopCoroutine(leftRumbleRoutine);
                    leftRumbleRoutine = TBCore.instance.StartCoroutine(RumblePulseRoutine(controller, strength, length));
                    break;
            }
        }

        protected virtual IEnumerator RumblePulseRoutine(TBInput.Controller controller, float strength, float length)
        {
            yield return null;
            float t = 0;
            while (t < length)
            {
                SetRumble(controller, strength);
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        public virtual float GetFinger(TBInput.Finger finger, TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = GetActiveController();

            _fingerButtons = GetControllerForType(controller).GetFingerButtons(finger);
            if (_fingerButtons == null)
                return 0;

            float biggestAmount = 0;

            for(int i = 0; i < _fingerButtons.Length; i++)
            {
                float amount = ResolveAxis1D(_fingerButtons[i], controller);
                if (amount < 0.1f)
                {
                    if (ResolveButton(_fingerButtons[i], controller))
                        amount = 1;
                    else if (ResolveTouch(_fingerButtons[i], controller))
                        amount = 0.1f;
                    else
                        amount = 0;
                }

                if (amount > biggestAmount)
                    biggestAmount = amount;
            }

            return biggestAmount;
        }
        #endregion

        #region INPUT CHECKS - RESOLVE FUNCTIONS
        // The "Resolve..." functions should be overridden by the classes that extend from this one for each SDK.

        public virtual bool ResolveButtonDown(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetButtonDown");
            return false;
        }

        public virtual bool ResolveButtonUp(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetButtonUp");
            return false;
        }

        public virtual bool ResolveButton(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetButton");
            return false;
        }

        public virtual bool ResolveTouchDown(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetTouchDown");
            return false;
        }

        public virtual bool ResolveTouchUp(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetTouchUp");
            return false;
        }

        public virtual bool ResolveTouch(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetTouch");
            return false;
        }

        public virtual float ResolveAxis1D(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetAxis1D");
            return 0;
        }

        public virtual Vector2 ResolveAxis2D(T button, TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetAxis2D");
            return Vector2.zero;
        }

        public virtual Vector3 ResolveRawPosition(TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetRawPosition");
            return Vector3.zero;
        }

        public virtual Quaternion ResolveRawRotation(TBInput.Controller controller)
        {
            TBLogging.LogMessage(TBCore.GetActivePlatform() + " does not have any inputs that support GetRawRotation");
            return Quaternion.identity;
        }

        protected virtual void SetActiveController(TBInput.Controller controller)
        {
            if (_activeController == controller)
                return;

            _activeController = controller;
            // TODO: fire an event when controller is changed
        }
        #endregion

        #region INPUT CHECKS - INFO FUNCTIONS
        public virtual bool ControllerHasRumbleSupport(TBInput.Controller controller)
        {
            if (GetControllerForType(controller) != null)
                return GetControllerForType(controller).HasRumbleSupport();
            return false;
        }

        public virtual bool SupportsInputType(TBInput.Button button, TBInput.Controller controller)
        {
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;
            else
                return true;
        }

        public virtual bool SupportsButton(TBInput.Button button, TBInput.Controller controller)
        {
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;

            bool supported = false;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (SupportsRawButton(buttons[i], controller))
                    supported = true;
            }
            return supported;
        }

        public virtual bool SupportsRawButton(T button, TBInput.Controller controller)
        {
            TBControllerBase<T> loadedController = GetControllerForType(controller);
            for (int i = 0; i < loadedController.GetLoadedButtonDefs().Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(button, loadedController.GetLoadedButtonDefs()[i].rawButton))
                    return (loadedController.GetLoadedButtonDefs()[i].supportsButton);
            }
            return false;
        }

        public virtual bool SupportsTouch(TBInput.Button button, TBInput.Controller controller)
        {
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;

            bool supported = false;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (SupportsRawTouch(buttons[i], controller))
                    supported = true;
            }
            return supported;
        }

        public virtual bool SupportsRawTouch(T button, TBInput.Controller controller)
        {
            TBControllerBase<T> loadedController = GetControllerForType(controller);
            for (int i = 0; i < loadedController.GetLoadedButtonDefs().Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(button, loadedController.GetLoadedButtonDefs()[i].rawButton))
                    return (loadedController.GetLoadedButtonDefs()[i].supportsTouch);
            }
            return false;
        }

        public virtual bool SupportsAxis1D(TBInput.Button button, TBInput.Controller controller)
        {
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;

            bool supported = false;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (SupportsRawAxis1D(buttons[i], controller))
                    supported = true;
            }
            return supported;
        }

        public virtual bool SupportsRawAxis1D(T button, TBInput.Controller controller)
        {
            TBControllerBase<T> loadedController = GetControllerForType(controller);
            for (int i = 0; i < loadedController.GetLoadedButtonDefs().Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(button, loadedController.GetLoadedButtonDefs()[i].rawButton))
                    return (loadedController.GetLoadedButtonDefs()[i].supportsAxis1D);
            }
            return false;
        }

        public virtual bool SupportsAxis2D(TBInput.Button button, TBInput.Controller controller)
        {
            T[] buttons = GetButtonArray(button, ref controller);
            if (buttons == null)
                return false;

            bool supported = false;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (SupportsRawAxis2D(buttons[i], controller))
                    supported = true;
            }
            return supported;
        }

        public virtual bool SupportsRawAxis2D(T button, TBInput.Controller controller)
        {
            TBControllerBase<T> loadedController = GetControllerForType(controller);
            for (int i = 0; i < loadedController.GetLoadedButtonDefs().Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(button, loadedController.GetLoadedButtonDefs()[i].rawButton))
                    return (loadedController.GetLoadedButtonDefs()[i].supportsAxis2D);
            }
            return false;
        }

        public virtual bool SupportsFinger(TBInput.Finger finger, TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = GetActiveController();

            if (GetControllerForType(controller).GetFingerButtons(finger) == null)
                return false;
            else
                return true;
        }

        public virtual TBInput.Mobile3DOFHandedness Get3DOFHandedness()
        {
            return TBInput.Mobile3DOFHandedness.Center;
        }

        public virtual HandTrackingOffsets GetControllerTrackingOffsets(TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = GetActiveController();

            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    return controller_LHand.GetTrackingOffsets();
                case TBInput.Controller.RHandController:
                    return controller_RHand.GetTrackingOffsets();
                case TBInput.Controller.Mobile3DOFController:
                    return controller_3DOF.GetTrackingOffsets();
                case TBInput.Controller.ClickRemote:
                    return controller_ClickRemote.GetTrackingOffsets();
                case TBInput.Controller.Gamepad:
                    return controller_Gamepad.GetTrackingOffsets();
            }

            return new HandTrackingOffsets();
        }
        #endregion
    }
}