using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Input
{
#if TB_GOOGLE
    public class TBInputGoogle : TBSDKInputBase<GvrControllerButton>
#else
    public class TBInputGoogle : TBSDKInputBase<TBInput.Button>
#endif
    {
        protected static TBInputGoogle _instance;
        public static TBInputGoogle instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBInputGoogle();
                return _instance;
            }
        }

#if TB_GOOGLE && UNITY_HAS_GOOGLEVR
        public override void Initialize()
        {
            if (TBCore.GetActivePlatform() == VRPlatform.Daydream)
            {
                if (TBCore.instance.gameObject.GetComponent<GvrControllerInput>() == null)
                    TBCore.instance.gameObject.AddComponent<GvrControllerInput>();
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (GvrControllerInput.Recentered)
                TBCore.Internal.ResetTracking();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        public override TBInput.Controller GetActiveController()
        {
            if (TBInput.GetActiveControlType() == TBInput.ControlType.HandControllers)
            {
                if (GvrControllerInput.GetDevice(GvrControllerHand.Dominant).SupportsPositionalTracking)
                    return TBInput.Controller.RHandController;
                else
                    return TBInput.Controller.Mobile3DOFController;
            }
            else
                return TBInput.Controller.Mobile3DOFController;
        }       

        public override TBInput.Mobile3DOFHandedness Get3DOFHandedness()
        {
            if (GvrSettings.Handedness == GvrSettings.UserPrefsHandedness.Left)
                return TBInput.Mobile3DOFHandedness.Left;
            else if (GvrSettings.Handedness == GvrSettings.UserPrefsHandedness.Right)
                return TBInput.Mobile3DOFHandedness.Right;
            else
                return TBInput.Mobile3DOFHandedness.Center;
        }

        protected GvrControllerInputDevice GetGoogleControllerID(TBInput.Controller controller)
        {
            if (controller == TBInput.Controller.Active)
                controller = TBInput.GetActiveController();

            switch (controller)
            {
                case TBInput.Controller.LHandController:
                    return GvrControllerInput.GetDevice(GvrControllerHand.Left);
                case TBInput.Controller.RHandController:
                    return GvrControllerInput.GetDevice(GvrControllerHand.Right);
                case TBInput.Controller.Mobile3DOFController:
                    return GvrControllerInput.GetDevice(GvrControllerHand.Dominant);
                default:
                    TBLogging.LogError("No controller for " + controller + " exists in TButt's Google VR implementation. Falling back to dominant hand controller.");
                    return GvrControllerInput.GetDevice(GvrControllerHand.Dominant);
            }
        }

        #region CONTROLLER LOADING
        protected override void Load3DOFControllers()
        {
            if (TBCore.GetActivePlatform() == VRPlatform.Daydream)
                controller_3DOF = TBController_Google_Daydream.instance;
            else
                base.Load3DOFControllers();
        }

        protected override void LoadHandControllers()
        {
            controller_LHand = TBController_Google_Daydream6DOFLeft.instance;
            controller_RHand = TBController_Google_Daydream6DOFRight.instance;
        }
        #endregion

        public override bool ResolveButton(GvrControllerButton button, TBInput.Controller controller)
        {
            return GetGoogleControllerID(controller).GetButton(button);
        }

        public override bool ResolveButtonDown(GvrControllerButton button, TBInput.Controller controller)
        {
            return GetGoogleControllerID(controller).GetButtonDown(button);
        }

        public override bool ResolveButtonUp(GvrControllerButton button, TBInput.Controller controller)
        {
            return GetGoogleControllerID(controller).GetButtonUp(button);
        }

        public override bool ResolveTouch(GvrControllerButton button, TBInput.Controller controller)
        {
            if (button == GvrControllerButton.TouchPadButton)
                return GetGoogleControllerID(controller).GetButton(GvrControllerButton.TouchPadTouch);
            return false;
        }

        public override bool ResolveTouchDown(GvrControllerButton button, TBInput.Controller controller)
        {
            if (button == GvrControllerButton.TouchPadButton)
                return GetGoogleControllerID(controller).GetButtonDown(GvrControllerButton.TouchPadTouch);
            return false;
        }

        public override bool ResolveTouchUp(GvrControllerButton button, TBInput.Controller controller)
        {
            if (button == GvrControllerButton.TouchPadButton)
                return GetGoogleControllerID(controller).GetButtonUp(GvrControllerButton.TouchPadTouch);
            return false;
        }

        public override Vector2 ResolveAxis2D(GvrControllerButton button, TBInput.Controller controller)
        {
            if (button == GvrControllerButton.TouchPadButton)
                if(GetGoogleControllerID(controller).GetButton(GvrControllerButton.TouchPadTouch))
                    return GetGoogleControllerID(controller).TouchPos;
            return Vector2.zero;
        }

        public override Vector3 GetAngularVelocity(TBInput.Controller controller)
        {
            return GetGoogleControllerID(controller).Gyro;
        }

        public override Vector3 GetAcceleration(TBInput.Controller controller)
        {
            return GetGoogleControllerID(controller).Accel;
        }

        public override Quaternion GetRawRotation(TBInput.Controller controller)
        {
            return GetGoogleControllerID(controller).Orientation;
        }
    #endif
    }
}
