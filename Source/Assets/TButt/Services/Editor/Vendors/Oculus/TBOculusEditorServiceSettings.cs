using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt.Services;
using TButt;

namespace TButt.Editor
{
    public class TBOculusEditorServiceSettings : TBEditorServiceSettingsBase
    {
        protected OculusServiceIDs _oculusIDs;


        protected override string _pluginName
        {
            get
            {
                return "Oculus Platform";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "Oculus Platform";
            }
        }

        protected override string _version
        {
            get
            {
                return "1.30.0";
            }
        }

        protected override VRService _service
        {
            get
            {
                return VRService.Oculus;
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022";
            }
        }

        private static TBOculusEditorServiceSettings _instance;
        public static TBOculusEditorServiceSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBOculusEditorServiceSettings();
                }
                return _instance;
            }
        }

        public override bool HasSDK()
        {
            string[] assets = AssetDatabase.FindAssets("OculusPlatformSettingsEditor", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }

        public override void ShowSettings()
        {
            base.ShowSettings();

            StartSettingsSection();

            EditorGUILayout.LabelField("Application ID", TBEditorStyles.h3);
            _oculusIDs.oculusPC_ID = EditorGUILayout.TextField("Rift / PC", _oculusIDs.oculusPC_ID);
            _oculusIDs.oculusMobile_ID = EditorGUILayout.TextField("Go / Gear VR", _oculusIDs.oculusMobile_ID);
            _oculusIDs.oculusQuest_ID = EditorGUILayout.TextField("Quest", _oculusIDs.oculusQuest_ID);

            EditorGUILayout.Space();
            GUILayout.EndVertical();
        }

        protected override void LoadSettings()
        {
            _oculusIDs = TBDataManager.DeserializeFromFile<OculusServiceIDs>(TBSettings.settingsFolder + TBOculusService.serviceFilename, TBDataManager.PathType.ResourcesFolder);
            _loadedSettings = true;
        }

        public override void SaveSettings()
        {
            base.SaveSettingsFile(TBOculusService.serviceFilename, _oculusIDs);
        }
    }
}