using UnityEditor;
using UnityEngine;

namespace TButt.Editor
{
    public class TBEditorInputDebugger : EditorWindow
    {
        Texture2D activeIcon;
        Texture2D inactiveIcon;

        GUILayoutOption[] rowSpacer;
        GUILayoutOption[] rowFirst;

        Vector2 scrollPos = Vector2.zero;

        bool initialized = false;

        int tableRowNum = 0;
        static EditorWindow window;

        float elementWidth;

        [MenuItem("TButt/Input Debugger", false, 101)]
        public static void ShowWindow()
        {
            window = EditorWindow.GetWindow(typeof(TBEditorInputDebugger),false, "TBInput Debugger", true);
        }

        private void Update()
        {
            Repaint();
        }


        void Initialize()
        {
            activeIcon = EditorGUIUtility.Load("TButt/Icons/active.png") as Texture2D;
            inactiveIcon = EditorGUIUtility.Load("TButt/Icons/inactive.png") as Texture2D;
            window = this;
            TBEditorStyles.SetupSharedStyles();
            initialized = true;
        }

        private void OnGUI()
        {
            if(!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter play mode to use the Input Debugger.", MessageType.Info);
                return;
            }

            if ((window == null) || !initialized)
            {
                Initialize();
                return;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            RefreshStyles();
            tableRowNum = 0;

            ShowButtonsForControlType(TBInput.GetActiveControlType());

            EditorGUILayout.EndScrollView();
        }

        void RefreshStyles()
        {
            elementWidth = this.position.width / 6f;
            rowFirst = new GUILayoutOption[] { GUILayout.Width(elementWidth * 2f), GUILayout.MinWidth(100) };
            rowSpacer = new GUILayoutOption[] { GUILayout.Width(elementWidth), GUILayout.MinWidth(50) };
        }

        void ShowHeaders()
        {
            GUILayout.BeginHorizontal(TBEditorStyles.tableHeaderRow);
            GUILayout.Label("Button/Action", rowFirst);
            GUILayout.Label("Press", rowSpacer);
            GUILayout.Label("Touch", rowSpacer);
            GUILayout.Label("Axis 1D", rowSpacer);
            GUILayout.Label("Axis 2D", rowSpacer);
            GUILayout.EndHorizontal();
        }

        void ShowFingerHeaders()
        {
            GUILayout.BeginHorizontal(TBEditorStyles.tableHeaderRow);
            GUILayout.Label("Finger", rowFirst);
            GUILayout.Label("Value", rowSpacer);
            GUILayout.EndHorizontal();
        }

        void ShowButtonsForControlType(TBInput.ControlType controlType)
        {
            EditorGUILayout.BeginVertical(TBEditorStyles.sectionBox);

            GUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow);
            GUILayout.Label("Active Control Type: ", rowFirst);
            GUILayout.Label(TBInput.GetActiveControlType().ToString(), rowFirst);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TBEditorStyles.tableOddRow);
            GUILayout.Label("Active Platform: ", rowFirst);
            GUILayout.Label(TBCore.GetActivePlatform().ToString(), rowFirst);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow);
            GUILayout.Label("Active Headset: ", rowFirst);
            GUILayout.Label(TBCore.GetActiveHeadset().ToString(), rowFirst);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUILayout.Space();

            switch (controlType)
            {
                case TBInput.ControlType.ClickRemote:
                case TBInput.ControlType.Gamepad:
                case TBInput.ControlType.Mobile3DOFController:
                    ShowButtonsForController(TBInput.GetActiveController());
                    break;
                case TBInput.ControlType.HandControllers:
                    ShowButtonsForController(TBInput.Controller.LHandController);
                    ShowButtonsForController(TBInput.Controller.RHandController);
                    break;
                default:
                    EditorGUILayout.HelpBox("Set an active control type to use the Input Debugger. See `TBInput.SetActiveControlType()`", MessageType.Info);
                    break;
            }
        }

        void ShowButtonsForController(TBInput.Controller controller)
        {
            EditorGUILayout.BeginVertical(TBEditorStyles.sectionBox);
            switch(controller)
            {
                case TBInput.Controller.LHandController:
                    GUILayout.Label("(L) " + TBInput.GetControllerModel(controller).ToString(), TBEditorStyles.h2);
                    break;
                case TBInput.Controller.RHandController:
                    GUILayout.Label("(R) " + TBInput.GetControllerModel(controller).ToString(), TBEditorStyles.h2);
                    break;
                default:
                    GUILayout.Label(TBInput.GetControllerModel(controller).ToString(), TBEditorStyles.h2);
                    break;
            }

            EditorGUILayout.Space();

            ShowHeaders();

            ShowRowForInputType(TBInput.Button.PrimaryTrigger, controller);
            ShowRowForInputType(TBInput.Button.SecondaryTrigger, controller);
            ShowRowForInputType(TBInput.Button.Joystick, controller);
            ShowRowForInputType(TBInput.Button.Touchpad, controller);
            ShowRowForInputType(TBInput.Button.Action1, controller);
            ShowRowForInputType(TBInput.Button.Action2, controller);
            ShowRowForInputType(TBInput.Button.Action3, controller);
            ShowRowForInputType(TBInput.Button.Action4, controller);
            ShowRowForInputType(TBInput.Button.Action5, controller);
            ShowRowForInputType(TBInput.Button.Action6, controller);
            ShowRowForInputType(TBInput.Button.Action7, controller);
            ShowRowForInputType(TBInput.Button.Action8, controller);
            ShowRowForInputType(TBInput.Button.LeftStick, controller);
            ShowRowForInputType(TBInput.Button.RightStick, controller);
            ShowRowForInputType(TBInput.Button.LeftTrigger, controller);
            ShowRowForInputType(TBInput.Button.RightTrigger, controller);
            ShowRowForInputType(TBInput.Button.DpadUp, controller);
            ShowRowForInputType(TBInput.Button.DpadDown, controller);
            ShowRowForInputType(TBInput.Button.DpadLeft, controller);
            ShowRowForInputType(TBInput.Button.DpadRight, controller);
            ShowRowForInputType(TBInput.Button.LeftBumper, controller);
            ShowRowForInputType(TBInput.Button.RightBumper, controller);
            ShowRowForInputType(TBInput.Button.Start, controller);
            ShowRowForInputType(TBInput.Button.Options, controller);

            tableRowNum = 0;

            ShowFingerHeaders();

            ShowRowForFinger(TBInput.Finger.Index, controller);
            ShowRowForFinger(TBInput.Finger.Grip, controller);
            ShowRowForFinger(TBInput.Finger.Thumb, controller);

            EditorGUILayout.EndVertical();
        }

        void ShowRowForFinger(TBInput.Finger finger, TBInput.Controller controller)
        {
            if (!TBInput.SupportsFinger(finger, controller))
                return;

            if ((tableRowNum % 2) == 0)
                GUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow);
            else
                GUILayout.BeginHorizontal(TBEditorStyles.tableOddRow);

            GUILayout.Label(finger.ToString(), rowFirst);
            GUILayout.Label(TBInput.GetFinger(finger, controller).ToString(), rowSpacer);

            GUILayout.EndHorizontal();
        }

        void ShowRowForInputType(TBInput.Button button, TBInput.Controller controller)
        {
            if (!TBInput.SupportsVirtualButton(button, controller))
                return;

            if((tableRowNum % 2) == 0)
                GUILayout.BeginHorizontal(TBEditorStyles.tableEvenRow);
            else
                GUILayout.BeginHorizontal(TBEditorStyles.tableOddRow);

            tableRowNum++;

            GUILayout.Label(button.ToString(), rowFirst);

            if (TBInput.SupportsButton(button, controller))
                GUILayout.Label(GetImage(TBInput.GetButton(button, controller)), rowSpacer);
            else
                GUILayout.Label("", rowSpacer);

            if (TBInput.SupportsTouch(button, controller))
                    GUILayout.Label(GetImage(TBInput.GetTouch(button, controller)), rowSpacer);
            else
                GUILayout.Label("", rowSpacer);

            if (TBInput.SupportsAxis1D(button, controller))
                GUILayout.Label(TBInput.GetAxis1D(button, controller).ToString(), rowSpacer);
            else
                GUILayout.Label("", rowSpacer);

            if (TBInput.SupportsAxis2D(button, controller))
                GUILayout.Label(TBInput.GetAxis2D(button, controller).ToString(), rowSpacer);
            else
                GUILayout.Label("", rowSpacer);

            GUILayout.EndHorizontal();
        }

        Texture2D GetImage(bool active)
        {
            if(active)
            {
                return activeIcon;
            }
            else
            {
                return inactiveIcon;
            }
        }
    }
}