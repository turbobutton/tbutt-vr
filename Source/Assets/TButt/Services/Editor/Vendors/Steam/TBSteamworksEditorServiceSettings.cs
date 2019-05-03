using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Services;

namespace TButt.Editor
{
    public class TBSteamworksEditorServiceSettings : TBEditorServiceSettingsBase
    {
        protected override string _pluginName
        {
            get
            {
                return "Steamworks.NET";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "Steamworks";
            }
        }

        protected override string _version
        {
            get
            {
                return "1.42";
            }
        }

        protected override VRService _service
        {
            get
            {
                return VRService.Steam;
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "https://steamworks.github.io/installation/";
            }
        }

        private static TBSteamworksEditorServiceSettings _instance;
        private int _steamID;
        public static TBSteamworksEditorServiceSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBSteamworksEditorServiceSettings();
                }
                return _instance;
            }
        }

        public override void ShowSettings()
        {
            base.ShowSettings();

            StartSettingsSection();

            EditorGUILayout.LabelField("Application ID", TBEditorStyles.h3);
            _steamID = EditorGUILayout.IntField("Steam App ID", _steamID);

            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }

        protected override void LoadSettings()
        {
            _steamID = TBDataManager.DeserializeFromFile<int>(TBSettings.settingsFolder + TBSteamService.serviceFilename, TBDataManager.PathType.ResourcesFolder);
            _loadedSettings = true;
        }

        public override void SaveSettings()
        {
            base.SaveSettingsFile(TBSteamService.serviceFilename, _steamID);
        }

        public override bool HasSDK()
        {
            string[] assets = AssetDatabase.FindAssets("SteamManager", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }
    }
}