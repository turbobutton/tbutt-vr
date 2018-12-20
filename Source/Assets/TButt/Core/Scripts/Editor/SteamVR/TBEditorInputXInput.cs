using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt.Input;

namespace TButt.Editor
{
    public class TBEditorInputXInput
    {
        #if TB_STEAM_VR || TB_WINDOWS_MR
        // Create a dictionary of controllers.
        public static Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<TBXInput.TBXInputButton>>> controllers = new Dictionary<string, TBEditorInputSettings.ButtonMapGroup<TBInput.ButtonDef<TBXInput.TBXInputButton>>>();
        #endif
    }

    #if TB_STEAM_VR || TB_WINDOWS_MR
    public class XInputGamepadConfigWindow : TBButtonMapWindow<TBEditorInputXInput>
    {
        void OnGUI()
        {
            TBEditorInputSettings.ShowMaps<TBXInput.TBXInputButton>(ref TBEditorInputXInput.controllers, TBController_XInput_Gamepad.instance.GetDefaultDefs(), TBController_XInput_Gamepad.instance.GetName(), TBEditorStyles.sectionOverlaySteam, TBEditorStyles.solidSteam);
        }
    }
#endif
}