using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Settings;

namespace TButt.Editor
{
    public class TBWindowsMREditorSDKSettings : TBEditorSDKSettingsBase
    {
        #region REQUIREMENTS
        protected override string _pluginName
        {
            get
            {
                return "Windows Native SDK";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "Windows Mixed Reality";
            }
        }

        protected override string _version
        {
            get
            {
                return "N/A";
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "N/A";
            }
        }

        protected override VRPlatform _platform
        {
            get
            {
                return VRPlatform.WindowsMR;
            }
        }

        private static TBWindowsMREditorSDKSettings _instance;
        public static TBWindowsMREditorSDKSettings instance {
            get
            {
                if (_instance == null)
                    _instance = new TBWindowsMREditorSDKSettings();
                return _instance;
            }
        }

        #endregion

        protected int _windowsMRToolbar;
        protected TBSettings.TBDisplaySettings _baseSettings;
        protected TBSettings.TBDisplaySettings _ultraSettings;
        protected TBWindowsMRSettings.TBWindowsMixedRealityLevel _selectedLevel;

        public override void ShowSettings()
        {
            if (!_loadedSettings)
                LoadSettings();

            _windowsMRToolbar = GUILayout.Toolbar(_windowsMRToolbar, new string[] { "Base", "Ultra" }, GUILayout.MinHeight(24), GUILayout.MaxHeight(24));

            _scrollAmount = EditorGUILayout.BeginScrollView(_scrollAmount);

            switch (_windowsMRToolbar)
            {
                case 0:
                    _selectedLevel = TBWindowsMRSettings.TBWindowsMixedRealityLevel.Base;
                    EditorGUILayout.HelpBox("'Base' assumes Intel Graphics 620 or better, running at 60 FPS.", UnityEditor.MessageType.Info);
                    ShowSettingsForSubplatform(ref _baseSettings);
                    break;
                case 1:
                    _selectedLevel = TBWindowsMRSettings.TBWindowsMixedRealityLevel.Ultra;
                    EditorGUILayout.HelpBox("'Ultra' assumes NVIDIA GeForce 1060 or better, running at 90 FPS.", UnityEditor.MessageType.Info);
                    ShowSettingsForSubplatform(ref _ultraSettings);
                    break;
            }

            EditorGUILayout.EndScrollView();
        }


        protected void ShowSettingsForSubplatform(ref TBSettings.TBDisplaySettings subplatform)
        {
            ShowDisplaySettings(ref subplatform);
            ShowQualitySettings(ref subplatform.qualitySettings);
        }

        public override void SaveSettings()
        {
            base.SaveSettingsFile(TBWindowsMRSettings.GetWindowsMRSettingsFilename(TBWindowsMRSettings.TBWindowsMixedRealityLevel.Base), _baseSettings);
            base.SaveSettingsFile(TBWindowsMRSettings.GetWindowsMRSettingsFilename(TBWindowsMRSettings.TBWindowsMixedRealityLevel.Ultra), _ultraSettings);
        }

        protected override void LoadSettings()
        {
            _baseSettings = TBWindowsMRSettings.LoadWindowsMRSettings(TBWindowsMRSettings.TBWindowsMixedRealityLevel.Base);
            _ultraSettings = TBWindowsMRSettings.LoadWindowsMRSettings(TBWindowsMRSettings.TBWindowsMixedRealityLevel.Ultra);
            _loadedSettings = true;
        }

        protected override void ShowVersion()
        {
            return;
        }

        public override bool HasSDK()
        {
            return true;    // Windows Mixed Reality SDK is built into Unity 2017+
        }
    }
}