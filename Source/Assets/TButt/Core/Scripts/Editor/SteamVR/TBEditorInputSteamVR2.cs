using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt.Input;
#if TB_STEAM_VR_2
using Valve.VR;
#endif

namespace TButt.Editor
{
    public class TBEditorInputSteamVR2
    {
        #if TB_STEAM_VR_2
        // Create a dictionary of controllers.
        public static Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<SteamVRHardwareButton>>> controllers = new Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<SteamVRHardwareButton>>>();
        #endif

        public static void ShowSteamVRControllerList(bool active, TBInput.ControlType control)
        {
            #if TB_STEAM_VR_2
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!active);
            switch (control)
            {
                case TBInput.ControlType.HandControllers:
                    if (GUILayout.Button("Vive Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_ViveController_Left.instance.GetName(), TBController_SteamVR2_ViveController_Left.instance.GetDefaultDefs(), TBController_SteamVR2_ViveController_Left.instance.GetFileName());
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_ViveController_Right.instance.GetName(), TBController_SteamVR2_ViveController_Right.instance.GetDefaultDefs(), TBController_SteamVR2_ViveController_Right.instance.GetFileName());
                        SteamVRViveConfigWindow.ShowWindow("Vive Controller");
                    }

                    if (GUILayout.Button("Index Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_IndexController_Left.instance.GetName(), TBController_SteamVR2_IndexController_Left.instance.GetDefaultDefs(), TBController_SteamVR2_IndexController_Left.instance.GetFileName());
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_IndexController_Right.instance.GetName(), TBController_SteamVR2_IndexController_Right.instance.GetDefaultDefs(), TBController_SteamVR2_IndexController_Right.instance.GetFileName());
                        SteamVRIndexConfigWindow.ShowWindow("Index Controller");
                    }

                    if (GUILayout.Button("Oculus Touch", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_OculusTouch_Left.instance.GetName(), TBController_SteamVR2_OculusTouch_Left.instance.GetDefaultDefs(), TBController_SteamVR2_OculusTouch_Left.instance.GetFileName());
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_OculusTouch_Right.instance.GetName(), TBController_SteamVR2_OculusTouch_Right.instance.GetDefaultDefs(), TBController_SteamVR2_OculusTouch_Right.instance.GetFileName());
                        SteamVROculusTouchConfigWindow.ShowWindow("Oculus Touch");
                    }

                    if (GUILayout.Button("WMR Motion Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_WMR_Left.instance.GetName(), TBController_SteamVR2_WMR_Left.instance.GetDefaultDefs(), TBController_SteamVR2_WMR_Left.instance.GetFileName());
                        TBEditorInputSettings.LoadController<SteamVRHardwareButton>(ref controllers, TBController_SteamVR2_WMR_Right.instance.GetName(), TBController_SteamVR2_WMR_Right.instance.GetDefaultDefs(), TBController_SteamVR2_WMR_Right.instance.GetFileName());
                        SteamVRWindowsMixedRealityConfigWindow.ShowWindow("Windows Mixed Reality Motion Controller");
                    }
                    break;
                case TBInput.ControlType.Mobile3DOFController:
                    break;
                case TBInput.ControlType.Gamepad:
                    break;
                case TBInput.ControlType.ClickRemote:
                    break;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            #endif
        }
    }

    #if TB_STEAM_VR_2
    public class SteamVRViveConfigWindow : TBButtonMapWindow<SteamVRViveConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_ViveController_Left.instance.GetDefaultDefs(), TBController_SteamVR2_ViveController_Left.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_ViveController_Right.instance.GetDefaultDefs(), TBController_SteamVR2_ViveController_Right.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }

    public class SteamVRIndexConfigWindow : TBButtonMapWindow<SteamVRIndexConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_IndexController_Left.instance.GetDefaultDefs(), TBController_SteamVR2_IndexController_Left.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_IndexController_Right.instance.GetDefaultDefs(), TBController_SteamVR2_IndexController_Right.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }

    public class SteamVROculusTouchConfigWindow : TBButtonMapWindow<SteamVROculusTouchConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_OculusTouch_Left.instance.GetDefaultDefs(), TBController_SteamVR2_OculusTouch_Left.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_OculusTouch_Right.instance.GetDefaultDefs(), TBController_SteamVR2_OculusTouch_Right.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }

    public class SteamVRWindowsMixedRealityConfigWindow : TBButtonMapWindow<SteamVRWindowsMixedRealityConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_WMR_Left.instance.GetDefaultDefs(), TBController_SteamVR2_WMR_Left.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            TBEditorInputSettings.ShowMaps<SteamVRHardwareButton>(ref TBEditorInputSteamVR2.controllers, TBController_SteamVR2_WMR_Right.instance.GetDefaultDefs(), TBController_SteamVR2_WMR_Right.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
            EditorGUILayout.EndScrollView();
        }
    }
#endif
}