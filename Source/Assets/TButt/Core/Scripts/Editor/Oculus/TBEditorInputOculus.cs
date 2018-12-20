using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Input;

namespace TButt.Editor
{
    public class TBEditorInputOculus
    {
        #if TB_OCULUS
        // Create a dictionary of controllers.
        public static Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<OVRInput.RawButton>>> controllers = new Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<OVRInput.RawButton>>>();
        #endif

        /// <summary>
        /// Displays the column of controllers for the Oculus SDK in TBEditorInputSettings.
        /// </summary>
        /// <param name="active"></param>
        /// <param name="control"></param>
        public static void ShowOculusControllerList(bool active, TBInput.ControlType control)
        {
            #if TB_OCULUS
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!active);

            // For each controller, we need to load the button defs before the window is opened.
            switch (control)
            {
                case TBInput.ControlType.HandControllers:
                    if (GUILayout.Button("Oculus Touch", TBEditorStyles.controllerSelectButton))
                    {
                        // Oculus Touch has two controllers (left and right) with different buttons, but we show them simulaneously, so we are overriding the normal behavior for buttons.
                        TBEditorInputSettings.LoadController<OVRInput.RawButton>(ref controllers, TBController_Oculus_TouchLeft.instance.GetName(), TBController_Oculus_TouchLeft.instance.GetDefaultDefs(), TBController_Oculus_TouchLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<OVRInput.RawButton>(ref controllers, TBController_Oculus_TouchRight.instance.GetName(), TBController_Oculus_TouchRight.instance.GetDefaultDefs(), TBController_Oculus_TouchRight.instance.GetFileName());
                        OculusTouchConfigWindow.ShowWindow("Oculus Touch");
                    }
                    if (GUILayout.Button("Oculus Quest", TBEditorStyles.controllerSelectButton))
                    {
                        // Oculus Quest / Santa Cruz has a prototype controller with a touchpad that differs from the final controller design.
                        TBEditorInputSettings.LoadController<OVRInput.RawButton>(ref controllers, TBController_Oculus_QuestLeft.instance.GetName(), TBController_Oculus_QuestLeft.instance.GetDefaultDefs(), TBController_Oculus_QuestLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<OVRInput.RawButton>(ref controllers, TBController_Oculus_QuestRight.instance.GetName(), TBController_Oculus_QuestRight.instance.GetDefaultDefs(), TBController_Oculus_QuestRight.instance.GetFileName());
                        OculusQuestConfigWindow.ShowWindow("Oculus Quest");
                    }
                    EditorGUI.BeginDisabledGroup(!Settings.TBOculusSettings.LoadOculusSettings(Settings.TBOculusSettings.OculusDeviceFamily.Rift).allowViveEmulation);
                    {
                        if (GUILayout.Button("Vive Controller", TBEditorStyles.controllerSelectButton))
                        {
                            // Oculus Touch has two controllers (left and right) with different buttons, but we show them simulaneously, so we are overriding the normal behavior for buttons.
                            TBEditorInputSettings.LoadController<OVRInput.RawButton>(ref controllers, TBController_Oculus_ViveControllerLeft.instance.GetName(), TBController_Oculus_ViveControllerLeft.instance.GetDefaultDefs(), TBController_Oculus_ViveControllerLeft.instance.GetFileName());
                            TBEditorInputSettings.LoadController<OVRInput.RawButton>(ref controllers, TBController_Oculus_ViveControllerRight.instance.GetName(), TBController_Oculus_ViveControllerRight.instance.GetDefaultDefs(), TBController_Oculus_ViveControllerRight.instance.GetFileName());
                            OculusViveControllerConfigWindow.ShowWindow("Vive Controller");
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                    break;
                case TBInput.ControlType.Mobile3DOFController:
                    TBEditorInputSettings.GetControllerButton<OVRInput.RawButton, OculusGearVRControllerConfigWindow>(TBController_Oculus_GearVRController.instance, ref controllers, active, "Oculus Go / Gear VR Controller");
                    break;
                case TBInput.ControlType.Gamepad:
                    TBEditorInputSettings.GetControllerButton<OVRInput.RawButton, OculusGamepadConfigWindow>(TBController_Oculus_Gamepad.instance, ref controllers, active);
                    break;
                case TBInput.ControlType.ClickRemote:
                    TBEditorInputSettings.GetControllerButton<OVRInput.RawButton, OculusGearVRTouchpadConfigWindow>(TBController_Oculus_GearVRTouchpad.instance, ref controllers, active);
                    TBEditorInputSettings.GetControllerButton<OVRInput.RawButton, OculusRemoteConfigWindow>(TBController_Oculus_Remote.instance, ref controllers, active);
                    break;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        #endif
        }
    }

    // Each controller gets its own window that extends TBButtonMapWindow. This allows multiple windows to be open at once.

    #if TB_OCULUS
    #region HAND CONTROLLERS
    public class OculusViveControllerConfigWindow : TBButtonMapWindow<OculusViveControllerConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.HelpBox(new GUIContent("Vive Controller support through Oculus Utiltiies is only used as a fallback if Steam VR plugin is not enabled. Note that you must disable this feature before making a build for Oculus Store, since Open VR DLLs are not permitted in builds for Oculus."));
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_ViveControllerLeft.instance.GetDefaultDefs(), TBController_Oculus_ViveControllerLeft.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_ViveControllerRight.instance.GetDefaultDefs(), TBController_Oculus_ViveControllerRight.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            EditorGUILayout.EndScrollView();
        }
    }

    public class OculusQuestConfigWindow : TBButtonMapWindow<OculusQuestConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_QuestLeft.instance.GetDefaultDefs(), TBController_Oculus_QuestLeft.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_QuestRight.instance.GetDefaultDefs(), TBController_Oculus_QuestRight.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            EditorGUILayout.EndScrollView();
        }
    }

    public class OculusTouchConfigWindow : TBButtonMapWindow<OculusTouchConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_TouchLeft.instance.GetDefaultDefs(), TBController_Oculus_TouchLeft.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_TouchRight.instance.GetDefaultDefs(), TBController_Oculus_TouchRight.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            EditorGUILayout.EndScrollView();
        }
    }
    #endregion

    #region 3DOF CONTROLLERS
    public class OculusGearVRControllerConfigWindow : TBButtonMapWindow<OculusGearVRControllerConfigWindow>
    {
        void OnGUI()
        {
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_GearVRController.instance.GetDefaultDefs(), "Oculus Go / Gear VR Controller", TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
        }
    }
    #endregion

    #region CLICK REMOTES
    public class OculusRemoteConfigWindow : TBButtonMapWindow<OculusRemoteConfigWindow>
    {
        void OnGUI()
        {
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_Remote.instance.GetDefaultDefs(), TBController_Oculus_Remote.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
        }
    }

    public class OculusGearVRTouchpadConfigWindow : TBButtonMapWindow<OculusGearVRTouchpadConfigWindow>
    {
        void OnGUI()
        {
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_GearVRTouchpad.instance.GetDefaultDefs(), TBController_Oculus_GearVRTouchpad.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
        }
    }
    #endregion
    #region GAMEPADS
    public class OculusGamepadConfigWindow : TBButtonMapWindow<OculusGamepadConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<OVRInput.RawButton>(ref TBEditorInputOculus.controllers, TBController_Oculus_Gamepad.instance.GetDefaultDefs(), TBController_Oculus_Gamepad.instance.GetName(), TBEditorStyles.sectionOverlayOculus, TBEditorStyles.solidOculus);
            EditorGUILayout.EndScrollView();
        }
    }
    #endregion
    #endif
}