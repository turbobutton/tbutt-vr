using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt.Input;

namespace TButt.Editor
{
    public class TBEditorInputGoogle
    {
        #if TB_GOOGLE
        // Create a dictionary of controllers.
        public static Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<GvrControllerButton>>> controllers = new Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<GvrControllerButton>>>();
        #endif

        public static void ShowGoogleControllerList(bool active, TBInput.ControlType control)
        {
            #if TB_GOOGLE
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(!active);
            switch (control)
            {
                case TBInput.ControlType.HandControllers:
                    if (GUILayout.Button("Daydream 6DOF Controller", TBEditorStyles.controllerSelectButton))
                    {
                        TBEditorInputSettings.LoadController<GvrControllerButton>(ref controllers, TBController_Google_Daydream6DOFLeft.instance.GetName(), TBController_Google_Daydream6DOFLeft.instance.GetDefaultDefs(), TBController_Google_Daydream6DOFLeft.instance.GetFileName());
                        TBEditorInputSettings.LoadController<GvrControllerButton>(ref controllers, TBController_Google_Daydream6DOFRight.instance.GetName(), TBController_Google_Daydream6DOFRight.instance.GetDefaultDefs(), TBController_Google_Daydream6DOFRight.instance.GetFileName());
                        GoogleDaydream6DOFControllerConfigWindow.ShowWindow("Daydream 6DOF Controller (Experimental)");
                    }
                    break;
                case TBInput.ControlType.Mobile3DOFController:
                    TBEditorInputSettings.GetControllerButton<GvrControllerButton, GoogleDaydreamControllerConfigWindow>(TBController_Google_Daydream.instance, ref controllers, active);
                    break;
                case TBInput.ControlType.Gamepad:
                    break;
                case TBInput.ControlType.ClickRemote:
                    break;
            }
            #endif
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }

    #if TB_GOOGLE
    public class GoogleDaydreamControllerConfigWindow : TBButtonMapWindow<GoogleDaydreamControllerConfigWindow>
    {
        void OnGUI()
        {
            TBEditorInputSettings.ShowMaps<GvrControllerButton>(ref TBEditorInputGoogle.controllers, TBController_Google_Daydream.instance.GetDefaultDefs(), TBController_Google_Daydream.instance.GetName(), TBEditorStyles.sectionOverlayGoogle, TBEditorStyles.solidGoogle);
        }
    }

    public class GoogleDaydream6DOFControllerConfigWindow : TBButtonMapWindow<GoogleDaydream6DOFControllerConfigWindow>
    {
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            TBEditorInputSettings.ShowMaps<GvrControllerButton>(ref TBEditorInputGoogle.controllers, TBController_Google_Daydream6DOFLeft.instance.GetDefaultDefs(), TBController_Google_Daydream6DOFLeft.instance.GetName(), TBEditorStyles.sectionOverlayGoogle, TBEditorStyles.solidGoogle);
            TBEditorInputSettings.ShowMaps<GvrControllerButton>(ref TBEditorInputGoogle.controllers, TBController_Google_Daydream6DOFRight.instance.GetDefaultDefs(), TBController_Google_Daydream6DOFRight.instance.GetName(), TBEditorStyles.sectionOverlayGoogle, TBEditorStyles.solidGoogle);
            EditorGUILayout.EndScrollView();
        }
    }
#endif
}