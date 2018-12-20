using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Settings;

namespace TButt.Editor
{
    public class TBSteamVREditorSDKSettings : TBEditorSDKSettingsBase
    {
        protected override string _pluginName
        {
            get
            {
                return "Steam VR Plugin";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "Steam VR";
            }
        }

        protected override string _version
        {
            get
            {
                return "1.2.3";
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "https://github.com/ValveSoftware/steamvr_unity_plugin/tree/v4/";
            }
        }

        protected override VRPlatform _platform
        {
            get 
            {
                return VRPlatform.SteamVR;
            }
        }

        private static TBSteamVREditorSDKSettings _instance;
        public static TBSteamVREditorSDKSettings instance {
            get
            {
                if (_instance == null)
                    _instance = new TBSteamVREditorSDKSettings();
                return _instance;
            }
        }

        public override bool HasSDK()
        {
            string[] assets = AssetDatabase.FindAssets("SteamVR_Camera", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }

        protected override void LoadSettings()
        {
            _displaySettings = TBSteamVRSettings.LoadDisplaySettings(TBSteamVRSettings.settingsFilename);
            _displaySettings.initialized = true;
            _loadedSettings = true;
        }

        public override void SaveSettings()
        {
            base.SaveSettingsFile(TBSteamVRSettings.settingsFilename, _displaySettings);
        }

#if TB_STEAM_VR
        protected override string GetVersion()
        {
            return "";
        }
#endif
    }
}