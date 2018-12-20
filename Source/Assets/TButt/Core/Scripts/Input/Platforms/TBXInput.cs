using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
#if (TB_STEAM_VR || TB_WINDOWS_MR) && TB_XINPUT
#if ENABLE_WINMD_SUPPORT
using Windows.Gaming.Input;
#else
using XInputDotNetPure;
#endif
#endif

namespace TButt.Input
{
    public class TBXInput : TBSDKInputBase<TBXInput.TBXInputButton>
    {
        protected static TBXInput _instance;

        public static TBXInput instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBXInput();
                return _instance;
            }
        }

#if (TB_STEAM_VR || TB_WINDOWS_MR) && TB_XINPUT

#if ENABLE_WINMD_SUPPORT
        // UWP XInput Workaround
        GamepadReading _prevReading = new GamepadReading();
        GamepadReading _reading = new GamepadReading();
        Gamepad gamepad;
#else
        // XInput
        bool _playerIndexSet = false;
        PlayerIndex playerIndex = PlayerIndex.One;
        GamePadState _state;
        GamePadState _prevState;
#endif

        protected override void LoadGamepads()
        {
            controller_Gamepad = TBController_XInput_Gamepad.instance;
        }

        public override void Update()
        {
#if ENABLE_WINMD_SUPPORT
            if (Gamepad.Gamepads == null)
                return;
            if (Gamepad.Gamepads.Count > 0)
            {
                gamepad = Gamepad.Gamepads[0];
                _prevReading = _reading;
                _reading = gamepad.GetCurrentReading();
            }
#else
            if (!_playerIndexSet || !_prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex = testPlayerIndex;
                        _playerIndexSet = true;
                    }
                }
            }

            if(TBInput.GetActiveControlType() == TBInput.ControlType.Gamepad)
                GamePad.SetVibration(0, 0, 0);

            _prevState = _state;
            _state = GamePad.GetState(playerIndex);
#endif
        }

#if !ENABLE_WINMD_SUPPORT
        #region XINPUT CLASSIC
        public ButtonState GetState(TBXInputButton button, GamePadState gamepadState)
        {
            switch (button)
            {
                case TBXInputButton.ButtonA:
                    return gamepadState.Buttons.A;
                case TBXInputButton.ButtonB:
                    return gamepadState.Buttons.B;
                case TBXInputButton.ButtonX:
                    return gamepadState.Buttons.X;
                case TBXInputButton.ButtonY:
                    return gamepadState.Buttons.Y;
                case TBXInputButton.LeftStick:
                    return gamepadState.Buttons.LeftStick;
                case TBXInputButton.RightStick:
                    return gamepadState.Buttons.RightStick;
                case TBXInputButton.DpadUp:
                    return gamepadState.DPad.Up;
                case TBXInputButton.DpadRight:
                    return gamepadState.DPad.Right;
                case TBXInputButton.DpadDown:
                    return gamepadState.DPad.Down;
                case TBXInputButton.DpadLeft:
                    return gamepadState.DPad.Left;
                case TBXInputButton.Back:
                    return gamepadState.Buttons.Back;
                case TBXInputButton.Guide:
                    return gamepadState.Buttons.Guide;
                case TBXInputButton.Start:
                    return gamepadState.Buttons.Start;
                case TBXInputButton.RightBumper:
                    return gamepadState.Buttons.RightShoulder;
                case TBXInputButton.LeftBumper:
                    return gamepadState.Buttons.LeftShoulder;
                case TBXInputButton.LeftTrigger:
                    if (gamepadState.Triggers.Left > 0)
                        return ButtonState.Pressed;
                    else
                        return ButtonState.Released;
                case TBXInputButton.RightTrigger:
                    if (gamepadState.Triggers.Right > 0)
                        return ButtonState.Pressed;
                    else
                        return ButtonState.Released;
                default:
                    TBLogging.LogWarning("Could not find requested XInput type.");
                    return ButtonState.Released;
            }
        }

        public override bool SetRumble(TBInput.Controller controller, float strength)
        {
            //GamePad.SetVibration(playerIndex, strength, strength);
            return false;
        }

        #region INPUT CHECKS
        public override bool ResolveButtonDown(TBXInputButton button, TBInput.Controller controller)
        {
            if ((GetState(button, _state) == ButtonState.Pressed) && (GetState(button, _prevState) == ButtonState.Released))
                return true;
            else
                return false;
        }

        public override bool ResolveButtonUp(TBXInputButton button, TBInput.Controller controller)
        {
            if ((GetState(button, _prevState) == ButtonState.Pressed) && (GetState(button, _state) == ButtonState.Released))
                return true;
            else
                return false;
        }

        public override bool ResolveButton(TBXInputButton button, TBInput.Controller controller)
        {
            if (GetState(button, _state) == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public override float ResolveAxis1D(TBXInputButton button, TBInput.Controller controller)
        {
            switch(button)
            {
                case TBXInputButton.LeftTrigger:
                    return _state.Triggers.Left;
                case TBXInputButton.RightTrigger:
                    return _state.Triggers.Right;
                default:
                    return 0;
            }
        }

        public override Vector2 ResolveAxis2D(TBXInputButton button, TBInput.Controller controller)
        {
            switch (button)
            {
                case TBXInputButton.LeftStick:
                    return new Vector2(_state.ThumbSticks.Left.X, _state.ThumbSticks.Left.Y);
                case TBXInputButton.RightStick:
                    return new Vector2(_state.ThumbSticks.Right.X, _state.ThumbSticks.Right.Y);
                default:
                    return Vector2.zero;
            }
        }
        #endregion
        #endregion
#elif ENABLE_WINMD_SUPPORT
        #region XINPUT FOR WINDOWS 10
        public bool GetState(TBXInputButton button, GamepadReading gamepadState)
        {
            switch (button)
            {
                case TBXInputButton.ButtonA:
                    return (GamepadButtons.A == (gamepadState.Buttons & GamepadButtons.A));
                case TBXInputButton.ButtonB:
                    return (GamepadButtons.B == (gamepadState.Buttons & GamepadButtons.B));
                case TBXInputButton.ButtonX:
                    return (GamepadButtons.X == (gamepadState.Buttons & GamepadButtons.X));
                case TBXInputButton.ButtonY:
                    return (GamepadButtons.Y == (gamepadState.Buttons & GamepadButtons.Y));
                case TBXInputButton.LeftStick:
                    return (GamepadButtons.LeftThumbstick == (gamepadState.Buttons & GamepadButtons.LeftThumbstick));
                case TBXInputButton.RightStick:
                    return (GamepadButtons.RightThumbstick == (gamepadState.Buttons & GamepadButtons.RightThumbstick));
                case TBXInputButton.DpadUp:
                    return (GamepadButtons.DPadUp == (gamepadState.Buttons & GamepadButtons.DPadUp));
                case TBXInputButton.DpadRight:
                    return (GamepadButtons.DPadRight == (gamepadState.Buttons & GamepadButtons.DPadRight));
                case TBXInputButton.DpadDown:
                    return (GamepadButtons.DPadDown == (gamepadState.Buttons & GamepadButtons.DPadDown));
                case TBXInputButton.DpadLeft:
                    return (GamepadButtons.DPadLeft == (gamepadState.Buttons & GamepadButtons.DPadLeft));
                case TBXInputButton.Back:
                    return (GamepadButtons.View == (gamepadState.Buttons & GamepadButtons.View));
                case TBXInputButton.Start:
                    return (GamepadButtons.Menu == (gamepadState.Buttons & GamepadButtons.Menu));
                case TBXInputButton.RightBumper:
                    return (GamepadButtons.RightShoulder == (gamepadState.Buttons & GamepadButtons.RightShoulder));
                case TBXInputButton.LeftBumper:
                    return (GamepadButtons.LeftShoulder == (gamepadState.Buttons & GamepadButtons.LeftShoulder));
                case TBXInputButton.LeftTrigger:
                    return (gamepadState.LeftTrigger > 0);
                case TBXInputButton.RightTrigger:
                    return (gamepadState.RightTrigger > 0);
                default:
                    TBLogging.LogWarning("Could not find requested Windows Input type.");
                    return false;
            }
        }

        public override bool SetRumble(TBInput.Controller controller, float strength)
        {
            //GamePad.SetVibration(playerIndex, strength, strength);
            return false;
        }

        #region INPUT CHECKS
        public override bool ResolveButtonDown(TBXInputButton button, TBInput.Controller controller)
        {
            if (GetState(button, _reading) && !GetState(button, _prevReading))
                return true;
            else
                return false;
        }

        public override bool ResolveButtonUp(TBXInputButton button, TBInput.Controller controller)
        {
            if (GetState(button, _prevReading) && !GetState(button, _reading))
                return true;
            else
                return false;
        }

        public override bool ResolveButton(TBXInputButton button, TBInput.Controller controller)
        {
            if (GetState(button, _reading))
                return true;
            else
                return false;
        }

        public override float ResolveAxis1D(TBXInputButton button, TBInput.Controller controller)
        {
            switch(button)
            {
                case TBXInputButton.LeftTrigger:
                    return (float)_reading.LeftTrigger;
                case TBXInputButton.RightTrigger:
                    return (float)_reading.RightTrigger;
                default:
                    return 0;
            }
        }

        public override Vector2 ResolveAxis2D(TBXInputButton button, TBInput.Controller controller)
        {
            switch (button)
            {
                case TBXInputButton.LeftStick:
                    return new Vector2((float)_reading.LeftThumbstickX, (float)_reading.LeftThumbstickY);
                case TBXInputButton.RightStick:
                    return new Vector2((float)_reading.RightThumbstickX, (float)_reading.RightThumbstickY);
                default:
                    return Vector2.zero;
            }
        }
        #endregion
        #endregion
#endif
#endif


        public enum TBXInputButton
        {
            ButtonA,
            ButtonB,
            ButtonX,
            ButtonY,
            LeftStick,
            RightStick,
            DpadUp,
            DpadRight,
            DpadDown,
            DpadLeft,
            Back,
            Guide,
            RightBumper,
            LeftBumper,
            LeftTrigger,
            RightTrigger,
            Start
        }
    }
}
