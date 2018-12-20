using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt.Input;
#if TB_STEAM_VR
using Valve.VR;
#endif

namespace TButt.Editor
{
    public class TBEditorInputSteamVR
    {
        #if TB_STEAM_VR
        // Create a dictionary of controllers.
        public static Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<EVRButtonId>>> controllers = new Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<EVRButtonId>>>();
        #endif

        public static void ShowSteamVRControllerList(bool active, TBInput.ControlType control)
        {
            #if TB_STEAM_VR
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!active);
            switch (control)
            {
                case TBInput.ControlType.HandControllers:
                    if (GUILayout.Button("Vive Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<EVRButtonId>(ref controllers, TBController_SteamVR_ViveControllerLeft.instance.GetName(), TBController_SteamVR_ViveControllerLeft.instance.GetDefaultDefs(), TBController_SteamVR_ViveControllerLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<EVRButtonId>(ref controllers, TBController_SteamVR_ViveControllerRight.instance.GetName(), TBController_SteamVR_ViveControllerRight.instance.GetDefaultDefs(), TBController_SteamVR_ViveControllerRight.instance.GetFileName());
                        SteamVRViveConfigWindow.ShowWindow("Vive Controller");
                    }

                    if (GUILayout.Button("Oculus Touch", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<EVRButtonId>(ref controllers, TBController_SteamVR_OculusTouchLeft.instance.GetName(), TBController_SteamVR_OculusTouchLeft.instance.GetDefaultDefs(), TBController_SteamVR_OculusTouchLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<EVRButtonId>(ref controllers, TBController_SteamVR_OculusTouchRight.instance.GetName(), TBController_SteamVR_OculusTouchRight.instance.GetDefaultDefs(), TBController_SteamVR_OculusTouchRight.instance.GetFileName());
                        SteamVROculusTouchConfigWindow.ShowWindow("Oculus Touch");
                    }

                    if (GUILayout.Button("WMR Motion Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<EVRButtonId>(ref controllers, TBController_SteamVR_WindowsMixedRealityLeft.instance.GetName(), TBController_SteamVR_WindowsMixedRealityLeft.instance.GetDefaultDefs(), TBController_SteamVR_WindowsMixedRealityLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<EVRButtonId>(ref controllers, TBController_SteamVR_WindowsMixedRealityRight.instance.GetName(), TBController_SteamVR_WindowsMixedRealityRight.instance.GetDefaultDefs(), TBController_SteamVR_WindowsMixedRealityRight.instance.GetFileName());
                        SteamVRWindowsMixedRealityConfigWindow.ShowWindow("Windows Mixed Reality Motion Controller");
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

    #if TB_STEAM_VR
    public class SteamVRViveConfigWindow : TBButtonMapWindow<SteamVRViveConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<EVRButtonId>(ref TBEditorInputSteamVR.controllers, TBController_SteamVR_ViveControllerLeft.instance.GetDefaultDefs(), TBController_SteamVR_ViveControllerLeft.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<EVRButtonId>(ref TBEditorInputSteamVR.controllers, TBController_SteamVR_ViveControllerRight.instance.GetDefaultDefs(), TBController_SteamVR_ViveControllerRight.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }

    public class SteamVROculusTouchConfigWindow : TBButtonMapWindow<SteamVROculusTouchConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<EVRButtonId>(ref TBEditorInputSteamVR.controllers, TBController_SteamVR_OculusTouchLeft.instance.GetDefaultDefs(), TBController_SteamVR_OculusTouchLeft.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<EVRButtonId>(ref TBEditorInputSteamVR.controllers, TBController_SteamVR_OculusTouchRight.instance.GetDefaultDefs(), TBController_SteamVR_OculusTouchRight.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }

    public class SteamVRWindowsMixedRealityConfigWindow : TBButtonMapWindow<SteamVRWindowsMixedRealityConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<EVRButtonId>(ref TBEditorInputSteamVR.controllers, TBController_SteamVR_WindowsMixedRealityLeft.instance.GetDefaultDefs(), TBController_SteamVR_WindowsMixedRealityLeft.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<EVRButtonId>(ref TBEditorInputSteamVR.controllers, TBController_SteamVR_WindowsMixedRealityRight.instance.GetDefaultDefs(), TBController_SteamVR_WindowsMixedRealityRight.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }
#endif
}