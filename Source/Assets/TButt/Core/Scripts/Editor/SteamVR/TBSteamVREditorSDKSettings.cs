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

        protected string _version2
        {
            get
            {
                return "2.2.0";
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "https://github.com/ValveSoftware/steamvr_unity_plugin/tree/v4/";
            }
        }

        protected string _pluginURL2
        {
            get
            {
                return "https://github.com/ValveSoftware/steamvr_unity_plugin/";
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

        public override void ShowSettings()
        {
            if (HasSDK2())
            {
                if(!HasTButtSteamVRActions())
                {
                    EditorGUILayout.HelpBox("Plugin detected, but cannot be enabled until TButt Steam VR Actions are installed.", UnityEditor.MessageType.Error);
                }
                EditorGUI.BeginDisabledGroup(!HasTButtSteamVRActions());
            }

            base.ShowSettings();

            if (HasSDK2())
            {
                EditorGUI.EndDisabledGroup();
            }
        }

        public override void ShowSDKToggleButton(ref bool sdk)
        {
            if (HasSDK1())
            {
                base.ShowSDKToggleButton(ref TBEditorSDKSettings.editorSDKs.steamVR);
                TBEditorSDKSettings.editorSDKs.steamVR2 = false;
            }
            else
            {
                base.ShowSDKToggleButton(ref TBEditorSDKSettings.editorSDKs.steamVR2);
                TBEditorSDKSettings.editorSDKs.steamVR = false;
            }
        }

        public override bool HasSDK()
        {
            if (HasSDK1() || HasSDK2())
                return true;
            else
                return false;
        }

        public override void ShowSDKDownloadButton()
        {
            if (GUILayout.Button("Download " + _pluginName + " " + _version2, new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                Application.OpenURL(_pluginURL2);
            }

            if (GUILayout.Button("Download " + _pluginName + " " + _version + " (Legacy)", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                Application.OpenURL(_pluginURL);
            }
        }

        public bool HasSDK1()
        {
            string[] assets = AssetDatabase.FindAssets("SteamVR_Controller", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }

        public bool HasSDK2()
        {
            string[] assets = AssetDatabase.FindAssets("SteamVR_ActionSet_Manager", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }

        protected bool HasTButtSteamVRActions()
        {
            string[] assets = AssetDatabase.FindAssets("SteamVR_Input_ActionSet_TButt", new string[] { "Assets" });
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

        protected override void ShowVersion()
        {
            if(HasSDK1())
            {
                    GUILayout.Label("Known stable version: " + _version);
            }
            else
            {
                    GUILayout.Label("Known stable version: " + _version2);
            }
        }

#if TB_STEAM_VR_2
        protected override string GetVersion()
        {
            return "";
        }
#endif
    }
}