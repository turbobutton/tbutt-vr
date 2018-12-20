using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Settings;

namespace TButt.Editor
{
    public class TBGoogleEditorSDKSettings : TBEditorSDKSettingsBase
    {
        protected override string _pluginName
        {
            get
            {
                return "Google VR SDK";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "Google VR";
            }
        }

        protected override string _version
        {
            get
            {
                return "1.180.0";
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "https://github.com/googlevr/gvr-unity-sdk";
            }
        }

        protected override VRPlatform _platform
        {
            get
            {
                return VRPlatform.Daydream;
            }
        }

        private static TBGoogleEditorSDKSettings _instance;
        public static TBGoogleEditorSDKSettings instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBGoogleEditorSDKSettings();
                return _instance;
            }
        }

        public override bool HasSDK()
        {
            string[] assets = AssetDatabase.FindAssets("GvrSettings", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }

        protected override void LoadSettings()
        {
            _displaySettings = TBGoogleSettings.LoadDisplaySettings(TBGoogleSettings.settingsFilename);
            _displaySettings.initialized = true;
            _loadedSettings = true;
        }

        public override void SaveSettings()
        {
            base.SaveSettingsFile(TBGoogleSettings.settingsFilename, _displaySettings);
        }



#if TB_GOOGLE
        protected override string GetVersion()
        {
            return GvrUnitySdkVersion.GVR_SDK_VERSION;
        }
#endif
    }
}