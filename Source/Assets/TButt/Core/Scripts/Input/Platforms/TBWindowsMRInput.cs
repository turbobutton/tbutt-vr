using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
#if TB_WINDOWS_MR
using UnityEngine.XR.WSA.Input;
#endif

namespace TButt.Input
{
    #if TB_WINDOWS_MR
    public class TBWindowsMRInput : TBSDKInputWithXInputBase<TBWindowsMRInput.Button>
    #else 
    public class TBWindowsMRInput : TBSDKInputWithXInputBase<TBInput.Button>
    #endif
    { 
        protected static TBWindowsMRInput _instance;
      
        protected Vector3 lastLeftControllerVelocity = Vector3.one;
        protected Vector3 lastRightControllerVelocity = Vector3.one;

        public static TBWindowsMRInput instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBWindowsMRInput();
                return _instance;
            }
        }

#if TB_WINDOWS_MR
        InteractionSourceState[] _prevStates;
        InteractionSourceState[] _states;

        int _leftID = -1;
        int _rightID = -1;

        public override void Initialize()
        {
            base.Initialize();
            InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_SourceLost;
        }

        void OnDestroy()
        {
            InteractionManager.InteractionSourceDetected -= InteractionManager_SourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_SourceLost;
        }

        protected override void LoadHandControllers()
        {
            controller_LHand = TBController_WindowsMR_MotionControllerLeft.instance;
            controller_RHand = TBController_WindowsMR_MotionControllerRight.instance;
        }

        public override void Update()
        {
            if(TBSettings.GetControlSettings().supportsHandControllers)
                UpdateHandControllers();
        }

        protected void UpdateHandControllers()
        {
            _leftID = -1;
            _rightID = -1;
            _prevStates = _states;
            _states = InteractionManager.GetCurrentReading();

            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i].source.handedness == InteractionSourceHandedness.Left)
                    _leftID = i;
                else if (_states[i].source.handedness == InteractionSourceHandedness.Right)
                    _rightID = i;
            }
        }

        public int GetControllerID(TBInput.Controller controller)
        {
            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    return _leftID;
                case TBInput.Controller.RHandController:
                    return _rightID;
                default:
                    return -1;
            }
        }

        public bool GetState(Button button, InteractionSourceState[] state, TBInput.Controller controller)
        {
            int id = GetControllerID(controller);

            if (id == -1)
                return false;

            switch (button)
            {
                case Button.SelectTrigger:
                    return state[id].selectPressed;
                case Button.Grip:
                    return state[id].grasped;
                case Button.Menu:
                    return state[id].menuPressed;
                case Button.Thumbstick:
                    return state[id].thumbstickPressed;
                case Button.Touchpad:
                    return state[id].touchpadPressed;
                default:
                    // TBLogging.LogWarning("Could not find requested Windows MR button type for " + button);
                    return false;
            }
        }

        public bool GetTouchState(Button button, InteractionSourceState[] state, TBInput.Controller controller)
        {
            int id = GetControllerID(controller);

            if (id == -1)
                return false;

            switch (button)
            {
                case Button.Touchpad:
                    return state[id].touchpadTouched;
                default:
                    // TBLogging.LogWarning("Could not find requested Windows MR touch type for " + button);
                    return false;
            }
        }
        public override bool SetRumble(TBInput.Controller controller, float strength)
        {
            #if UNITY_2018_3_OR_NEWER
            return false;
            #else
            return false;
            #endif
        }

            #region INPUT CHECKS
        public override bool ResolveButtonDown(Button button, TBInput.Controller controller)
        {
            return ((GetState(button, _states, controller) && (!GetState(button, _prevStates, controller))));
        }

        public override bool ResolveButtonUp(Button button, TBInput.Controller controller)
        {
            return ((GetState(button, _prevStates, controller) && (!GetState(button, _states, controller))));
        }

        public override bool ResolveButton(Button button, TBInput.Controller controller)
        {
            return (GetState(button, _states, controller));
        }

        public override bool ResolveTouchDown(Button button, TBInput.Controller controller)
        {
            return ((GetTouchState(button, _states, controller) && (!GetTouchState(button, _prevStates, controller))));
        }

        public override bool ResolveTouchUp(Button button, TBInput.Controller controller)
        {
            return ((GetTouchState(button, _prevStates, controller) && (!GetTouchState(button, _states, controller))));
        }

        public override bool ResolveTouch(Button button, TBInput.Controller controller)
        {
            return (GetTouchState(button, _states, controller));
        }

        public override float ResolveAxis1D(Button button, TBInput.Controller controller)
        {
            int id = GetControllerID(controller);

            if (id == -1)
                return 0;

            switch (button)
            {
                case Button.SelectTrigger:
                    return _states[id].selectPressedAmount;
                default:
                    return 0;
            }
        }

        public override Vector2 ResolveAxis2D(Button button, TBInput.Controller controller)
        {
            int id = GetControllerID(controller);

            if (id == -1)
                return Vector2.zero;

            switch (button)
            {
                case Button.Thumbstick:
                    return _states[id].thumbstickPosition;
                case Button.Touchpad:
                    return _states[id].touchpadPosition;
                default:
                    return Vector2.zero;
            }
        }
            #endregion

            #region CONTROLLER CONNECTION EVENTS
        void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs state)
        {
            TBLogging.LogMessage("Motion controller connected");
            RefreshSource(state.state);
        }

        void InteractionManager_SourceLost(InteractionSourceLostEventArgs state)
        {
            TBLogging.LogWarning("Motion controller disconnected");
            RefreshSource(state.state);
        }

        void RefreshSource(InteractionSourceState state)
        {
            
        }

#if UNITY_WSA

#endif

            #endregion
#endif

        public enum Button
        {
            SelectTrigger,
            Grip,
            Menu,
            Thumbstick,
            Touchpad
        }

    }
}
