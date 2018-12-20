using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Input
{
    /// <summary>
    /// A layer on top of TBSDKInputBase for SDKs that need to fall back to XInput or Windows 10 Input for gamepads.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TBSDKInputWithXInputBase<T> : TBSDKInputBase<T>
    {
        private bool _useXInputWrapper = false;

        public override void Initialize()
        {
            base.Initialize();

            if (TBSettings.GetControlSettings().supportsGamepad)
            {
                TBXInput.instance.Initialize();
                TBCore.OnUpdate += TBXInput.instance.Update;
                TBCore.OnFixedUpdate += TBXInput.instance.FixedUpdate;
                _useXInputWrapper = true;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (TBSettings.GetControlSettings().supportsGamepad)
            {
                TBCore.OnUpdate -= TBXInput.instance.Update;
                TBCore.OnFixedUpdate -= TBXInput.instance.FixedUpdate;
            }
        }

        protected override void LoadGamepads()
        {
            TBLogging.LogMessage("Gamepads routing through TBXInput.");
        }

        protected bool ShouldRouteThroughXInput(TBInput.Controller controller)
        {
            if (_useXInputWrapper)
            {
                if (controller == TBInput.Controller.Active)
                    controller = GetActiveController();
                if (controller == TBInput.Controller.Gamepad)
                    return true;
            }
            return false;
        }

        #region INPUT CHECKS - GET FUNCTIONS
        public override bool GetButtonDown(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetButtonDown(button, controller);

            return base.GetButtonDown(button, controller);
        }

        public override bool GetButtonUp(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetButtonUp(button, controller);

            return base.GetButtonUp(button, controller);
        }

        public override bool GetButton(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetButton(button, controller);

            return base.GetButton(button, controller);
        }

        public override bool GetTouchDown(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetTouchDown(button, controller);

            return base.GetTouchDown(button, controller);
        }

        public override bool GetTouchUp(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetTouchUp(button, controller);

            return base.GetTouchUp(button, controller);
        }

        public override bool GetTouch(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetTouch(button, controller);

            return base.GetTouch(button, controller);
        }

        public override float GetAxis1D(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetAxis1D(button, controller);

            return base.GetAxis1D(button, controller);
        }

        public override Vector2 GetAxis2D(TBInput.Button button, TBInput.Controller controller)
        {
            if(ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetAxis2D(button, controller);

            return base.GetAxis2D(button, controller);
        }
        #endregion


        #region INPUT CHECKS - INFO FUNCTIONS
        public override bool ControllerHasRumbleSupport(TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.ControllerHasRumbleSupport(controller);

            return base.ControllerHasRumbleSupport(controller);
        }

        public override bool SupportsInputType(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.SupportsInputType(button, controller);

            return base.SupportsInputType(button, controller);
        }

        public override bool SupportsButton(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.SupportsButton(button, controller);

            return base.SupportsButton(button, controller);
        }

        public override bool SupportsTouch(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.SupportsTouch(button, controller);

            return base.SupportsTouch(button, controller);
        }

        public override bool SupportsAxis1D(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.SupportsAxis1D(button, controller);

            return base.SupportsAxis1D(button, controller);
        }

        public override bool SupportsAxis2D(TBInput.Button button, TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.SupportsAxis2D(button, controller);

            return base.SupportsAxis2D(button, controller);
        }

        public override string GetControllerName(TBInput.Controller controller)
        {
            if (ShouldRouteThroughXInput(controller))
                return TBXInput.instance.GetControllerName(controller);

            return base.GetControllerName(controller);
        }
        #endregion
    }
}