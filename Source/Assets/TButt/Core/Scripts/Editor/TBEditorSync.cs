using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Settings;

namespace TButt.Editor
{
    /// <summary>
    /// Forces PlayerSettings to stay in sync with TButt's Core settings. Off by default, can be toggled on the Core Settings menu.
    /// </summary>
    [InitializeOnLoad]
    class TBEditorSync
    {
        static TBEditorSDKSettings.SDKs _sdks;

        static TBEditorSync()
        {
            SyncSDKs();
        }

        public static void SyncSDKs()
        {
            if (TBEditorSDKSettings.GetNumActiveSDKs() == 0)
            {
                Debug.LogError("No platforms are currently enabled in TButt Core Settings. Please enable at least one platform in the Core Settings menu.");
            }
            else
            {
                _sdks = TBEditorSDKSettings.GetEditorSDKs();

                if (!_sdks.forceSync)
                    return;
                else
                    TBEditorSDKSettings.SetScriptingDefines(_sdks);

                if(!PlayerSettings.virtualRealitySupported)
                    PlayerSettings.virtualRealitySupported = true;

                TBEditorDefines.SetUnityVirtualRealitySDKs(_sdks);
            }
        }
    }
}