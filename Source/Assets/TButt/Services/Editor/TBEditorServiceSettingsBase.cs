using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TButt.Editor
{
    public abstract class TBEditorServiceSettingsBase
    {
        abstract protected string _pluginName { get; }
        abstract protected string _sdkName { get; }
        abstract protected string _version { get; }
        abstract protected string _pluginURL { get; }
        abstract protected VRService _service { get; }

        protected bool _loadedSettings = false;
        protected Vector2 _scrollAmount = Vector2.zero;


        public virtual void ShowSDKDownloadButton()
        {
            if (GUILayout.Button("Download " + _pluginName + " " + _version, new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                Application.OpenURL(_pluginURL);
            }
        }

        protected virtual void ShowVersion()
        {
            if (GetVersion() == "???")
                GUILayout.Label("Enable and save to detect version. Known stable version: " + _version);
            else
                GUILayout.Label("Detected " + _pluginName + ": " + GetVersion() + ". Known stable version: " + _version);
        }

        public virtual void ShowSDKToggleButton(ref bool sdk)
        {
            EditorGUILayout.BeginVertical(TBEditorStyles.sdkHeaderBox);
            GUI.backgroundColor = (sdk ? Color.green : Color.red);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_sdkName + " Settings", TBEditorStyles.h1, new GUILayoutOption[1] { GUILayout.Height(40) });
            sdk = GUILayout.Toggle(sdk, sdk ? "Enabled" : "Disabled", "Button", new GUILayoutOption[1] { GUILayout.Height(40) });
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }

        public virtual void ShowSettings()
        {
            if (!_loadedSettings)
                LoadSettings();
        }

        public string GetName()
        {
            return _sdkName;
        }

        public VRService GetService()
        {
            return _service;
        }

        public virtual void ShowSDKNotFoundMessage()
        {
            EditorGUILayout.HelpBox(_pluginName + " not detected in this Unity project. TButt cannot support " + _sdkName + " until it is installed.", UnityEditor.MessageType.Error);
        }
    
        protected virtual string GetVersion()
        {
            return "???";
        }

        public virtual bool HasSDK()
        {
            return false;
        }

        protected virtual void LoadSettings()
        {
            throw new System.NotImplementedException();
        }

        public virtual void SaveSettings()
        {
            if (HasSDK())
            {
                throw new System.NotImplementedException();
            }
            else
            {
                return;
            }
        }

        public virtual void SaveSettingsFile(string filename, object obj)
        {
            TBLogging.LogMessage("Saving " + filename + "...");
            TBEditorHelper.CheckoutAndSaveJSONFile(TBEditorDefines.settingsPath + TBSettings.settingsFolder + filename + ".json", obj, TBDataManager.PathType.ResourcesFolder);
        }

        protected virtual void StartSettingsSection()
        {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Separator();
        }

        protected virtual void EndSettingsSection()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
        }
    }
}