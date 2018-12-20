using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt.Input;

namespace TButt.Editor
{
    public class TBEditorInputWindowsMR
    {
        #if TB_WINDOWS_MR
        // Create a dictionary of controllers.
        public static Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<TBWindowsMRInput.Button>>> controllers = new Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<TBWindowsMRInput.Button>>>();
        #endif

        public static void ShowWindowsMRControllerList(bool active, TBInput.ControlType control)
        {
            #if TB_WINDOWS_MR
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!active);
            switch (control)
            {
                case TBInput.ControlType.HandControllers:
                    if (GUILayout.Button("WMR Motion Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<TBWindowsMRInput.Button>(ref controllers, TBController_WindowsMR_MotionControllerLeft.instance.GetName(), TBController_WindowsMR_MotionControllerLeft.instance.GetDefaultDefs(), TBController_WindowsMR_MotionControllerLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<TBWindowsMRInput.Button>(ref controllers, TBController_WindowsMR_MotionControllerRight.instance.GetName(), TBController_WindowsMR_MotionControllerRight.instance.GetDefaultDefs(), TBController_WindowsMR_MotionControllerRight.instance.GetFileName());
                        WindowsMRMotionControllerConfigWindow.ShowWindow("Windows Mixed Reality Motion Controller");
                    }
                    break;
                case TBInput.ControlType.Mobile3DOFController:
                    break;
                case TBInput.ControlType.Gamepad:
                    TBEditorInputSettings.GetControllerButton<TBXInput.TBXInputButton, XInputGamepadConfigWindow>(TBController_XInput_Gamepad.instance, ref TBEditorInputXInput.controllers, active);
                    break;
                case TBInput.ControlType.ClickRemote:
                    break;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            #endif
        }
    }

    #if TB_WINDOWS_MR
    public class WindowsMRMotionControllerConfigWindow : TBButtonMapWindow<WindowsMRMotionControllerConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<TBWindowsMRInput.Button>(ref TBEditorInputWindowsMR.controllers, TBController_WindowsMR_MotionControllerLeft.instance.GetDefaultDefs(), TBController_WindowsMR_MotionControllerLeft.instance.GetName(), TBEditorStyles.sectionOverlayWindows, TBEditorStyles.solidWindows);
            TBEditorInputSettings.ShowMaps<TBWindowsMRInput.Button>(ref TBEditorInputWindowsMR.controllers, TBController_WindowsMR_MotionControllerRight.instance.GetDefaultDefs(), TBController_WindowsMR_MotionControllerRight.instance.GetName(), TBEditorStyles.sectionOverlayWindows, TBEditorStyles.solidWindows);
            EditorGUILayout.EndScrollView();
        }
    }

#endif
}
