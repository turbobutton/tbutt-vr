using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TButt;

namespace TButt.Editor
{
    public class TBEditorSDKSettings : EditorWindow
    {
        static EditorWindow window;

        static bool hasCompiled;

        public int toolbar;
        string selectedSDKName;

        public TBEditorSDKSettingsBase _sdkSettings;

        static Texture[] toolbarImages;
        static Texture crossImage;
        static Texture checkImage;

        static bool showCameraSettings;
        static bool showDisplaySettings;

        static Dictionary<VRPlatform, TBSettings.TBDisplaySettings> displaySettingsDictionary;
        static TBSettings.TBCameraSettings camSettings;

        // SDK settings
        static SDKs editorSDKs;
        static Dictionary<string, TBEditorSDKSettingsBase> sdkSettings;

        [MenuItem("TButt/Core Settings...", false, 10000)]
        public static void ShowWindow()
        {
            TBEditorStyles.SetupSharedStyles();

            toolbarImages = new Texture[6];
            toolbarImages[0] = EditorGUIUtility.Load("TButt/Icons/global.png") as Texture2D;
            toolbarImages[1] = EditorGUIUtility.Load("TButt/Icons/oculus.png") as Texture2D;
            toolbarImages[2] = EditorGUIUtility.Load("TButt/Icons/steam.png") as Texture2D;
            toolbarImages[3] = EditorGUIUtility.Load("TButt/Icons/google.png") as Texture2D;
            toolbarImages[4] = EditorGUIUtility.Load("TButt/Icons/windows.png") as Texture2D;
            #if TB_HAS_UNITY_PS4
            toolbarImages[5] = EditorGUIUtility.Load("TButt/Icons/playstation.png") as Texture2D;
            #endif

            crossImage = EditorGUIUtility.Load("TButt/Icons/cross.png") as Texture2D;
            checkImage = EditorGUIUtility.Load("TButt/Icons/check.png") as Texture2D;

            displaySettingsDictionary = new Dictionary<VRPlatform, TBSettings.TBDisplaySettings>();
            RefreshSettings();

            // Grab the current build settings and parse them.
            window = EditorWindow.GetWindow(typeof(TBEditorSDKSettings), true, "Core Settings", true);
        }

        void OnGUI()
        {
            // Don't edit settings in play mode.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Cannot edit input settings while in play mode.", MessageType.Error);
                return;
            }

            // Waiting for it to recompile...
            if (EditorApplication.isCompiling || hasCompiled)
            {
                if (!hasCompiled)
                {
                    hasCompiled = true;
                }
                EditorGUILayout.HelpBox("Wait for compiling to finish...", MessageType.Warning);
                return;
            }

            if (window == null)
            {
                ShowWindow();
            }

            if (displaySettingsDictionary == null)
                RefreshSettings();

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Toolbar(10,
                new Texture[] {null,
                    editorSDKs.oculus ? checkImage : crossImage,
                    editorSDKs.steamVR ? checkImage : crossImage,
                    editorSDKs.googleVR ? checkImage : crossImage,
                    editorSDKs.windows ? checkImage : crossImage
                    #if TB_HAS_UNITY_PS4
                    ,editorSDKs.psvr ? checkImage : crossImage
                    #endif
                },
                    GUILayout.MinHeight(16), GUILayout.MaxHeight(16));
            EditorGUI.EndDisabledGroup();
            toolbar = GUILayout.Toolbar(toolbar, new Texture[] {
                toolbarImages[0],
                toolbarImages[1],
                toolbarImages[2],
                toolbarImages[3],
                toolbarImages[4]
                #if TB_HAS_UNITY_PS4
                ,toolbarImages[5]
                #endif
            }, GUILayout.MinHeight(32), GUILayout.MaxHeight(32));

            switch (toolbar)
            {
                case 0:
                    ShowGlobalSettings();
                    break;
                case 1:
                    ShowSDKSection(TBOculusEditorSDKSettings.instance.GetName(), ref editorSDKs.oculus);
                    break;
                case 2:
                    ShowSDKSection(TBSteamVREditorSDKSettings.instance.GetName(), ref editorSDKs.steamVR);              
                    break;
                case 3:
                    ShowSDKSection(TBGoogleEditorSDKSettings.instance.GetName(), ref editorSDKs.googleVR);
                    break;
                case 4:
                    ShowSDKSection(TBWindowsMREditorSDKSettings.instance.GetName(), ref editorSDKs.windows);
                    break;
                #if TB_HAS_UNITY_PS4
                case 5:
                    ShowSDKSection(TBPSVREditorSDKSettings.instance.GetName(), ref editorSDKs.psvr);
                    break;
                #endif
                default:
                    break;
            }

            EditorGUILayout.BeginVertical();

            if (!AnySDKSelected())
                EditorGUILayout.HelpBox("You must select at least one SDK for TButt to function.", UnityEditor.MessageType.Error);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(new GUILayoutOption[1] { GUILayout.Height(45) });

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save All Settings", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                SaveAllSettings();
                window.Close();
            }

            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Save " + selectedSDKName +" Settings", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                SaveFocusedSettings();
                window.Close();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Close Without Saving", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                window.Close();
            }
            EditorGUILayout.EndHorizontal();          
        }

        void ShowSDKSection(string sdk, ref bool enabled)
        {
            selectedSDKName = sdkSettings[sdk].GetName();
            if (!sdkSettings[sdk].HasSDK())
            {
                sdkSettings[sdk].ShowSDKNotFoundMessage();
                sdkSettings[sdk].ShowSDKDownloadButton();
            }
            else
            {
                sdkSettings[sdk].ShowSDKToggleButton(ref enabled);
                sdkSettings[sdk].ShowSettings();
            }
        }

        void ShowGlobalSettings()
        {
            selectedSDKName = "TBCore Global";
            EditorGUILayout.BeginVertical(TBEditorStyles.sdkHeaderBox);
            EditorGUILayout.LabelField(selectedSDKName + " Settings", TBEditorStyles.h1, new GUILayoutOption[1] { GUILayout.Height(40) });
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("TBCameraRig Settings", TBEditorStyles.h2);
            camSettings.trackingOrigin = (TBSettings.TBTrackingOrigin)EditorGUILayout.EnumPopup("Tracking Origin", camSettings.trackingOrigin);
            EditorGUI.BeginDisabledGroup(camSettings.trackingOrigin == TBSettings.TBTrackingOrigin.Eye);            
            camSettings.uncalibratedFloorHeight = EditorGUILayout.FloatField("Uncalibrated Floor Height", camSettings.uncalibratedFloorHeight);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Debug Options", TBEditorStyles.h2);
            editorSDKs.logs = EditorGUILayout.Toggle("Enable TButt Editor Logs", editorSDKs.logs);
            editorSDKs.forceSync = EditorGUILayout.Toggle(new GUIContent("Force Sync", "Updates PlayerSettings' Scripting Define Symbols if they get out of sync with your TButt platform settings."), editorSDKs.forceSync);
            camSettings.calibrationHotkey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Calibration Hotkey", "Editor-only shortcut for re-centering the VR camera's tracking position."), camSettings.calibrationHotkey);
            EditorGUILayout.EndVertical();

            EditorGUILayout.HelpBox("v" + TBEditorDefines.versionNum + " |  www.tbutt.net", MessageType.None);
        }

        static bool AnySDKSelected()
        {
            return ((editorSDKs.googleVR || editorSDKs.oculus || editorSDKs.steamVR || editorSDKs.windows));
        }

#region SDK CHECKS
        static void RefreshSettings()
        {
            sdkSettings = new Dictionary<string, TBEditorSDKSettingsBase>();
            sdkSettings.Add(TBOculusEditorSDKSettings.instance.GetName(), TBOculusEditorSDKSettings.instance);
            sdkSettings.Add(TBSteamVREditorSDKSettings.instance.GetName(), TBSteamVREditorSDKSettings.instance);
            #if TB_HAS_UNITY_PS4
            sdkSettings.Add(TBPSVREditorSDKSettings.instance.GetName(), TBPSVREditorSDKSettings.instance);
            #endif
            sdkSettings.Add(TBGoogleEditorSDKSettings.instance.GetName(), TBGoogleEditorSDKSettings.instance);
            sdkSettings.Add(TBWindowsMREditorSDKSettings.instance.GetName(), TBWindowsMREditorSDKSettings.instance);
            ReadSettings();
        }
#endregion

        static void ReadSettings()
        {
            editorSDKs = GetEditorSDKs();

            if (File.Exists(TBEditorDefines.settingsPath + TBSettings.settingsFolder + TBSettings.cameraSettingsFilename + ".json"))
            {
                camSettings = TBDataManager.DeserializeFromFile<TBSettings.TBCameraSettings>(TBSettings.settingsFolder + TBSettings.cameraSettingsFilename, TBDataManager.PathType.ResourcesFolder);
            }
        }

        static void SaveAllSettings()
        {
            foreach (KeyValuePair<string, TBEditorSDKSettingsBase> entry in sdkSettings)
            {
                SavePlatformSettings(entry.Value.GetPlatform());
            }

            SaveEditorSettings();
            SaveGlobalSettings();
        }

        void SaveFocusedSettings()
        {
            SaveEditorSettings();
            if (selectedSDKName == "TBCore Global")
                SaveGlobalSettings();
            else
                SavePlatformSettings(sdkSettings[selectedSDKName].GetPlatform());
        }

        static void SaveEditorSettings()
        {
            TBEditorHelper.CheckoutAndSaveJSONFile("TButtEditorSDKs.json", editorSDKs, TBDataManager.PathType.ProjectFolder, true);
            SetScriptingDefines(editorSDKs);
        }

        public static void SetScriptingDefines(SDKs sdks)
        {
            TBEditorDefines.SetPlatformDefine(TBEditorDefines.logsDef, sdks.logs);
            TBEditorDefines.SetPlatformDefine(TBEditorDefines.oculusDef, sdks.oculus);
            TBEditorDefines.SetPlatformDefine(TBEditorDefines.steamVRDef, sdks.steamVR);
            TBEditorDefines.SetPlatformDefine(TBEditorDefines.googleDef, sdks.googleVR);
            #if TB_HAS_UNITY_PS4
            TBEditorDefines.SetPlatformDefine(TBEditorDefines.psvrDef, sdks.psvr);
            #endif
            TBEditorDefines.SetPlatformDefine(TBEditorDefines.windowsDef, sdks.windows);
            TBEditorDefines.SetScriptingDefines();
            TBEditorDefines.SetUnityVirtualRealitySDKs(sdks);
        }

        static void SaveGlobalSettings()
        {
            TBEditorHelper.CheckoutAndSaveJSONFile(TBEditorDefines.settingsPath + TBSettings.settingsFolder + TBSettings.cameraSettingsFilename + ".json", camSettings, TBDataManager.PathType.ResourcesFolder);
        }

        static void SavePlatformSettings(VRPlatform platform)
        {
            switch(platform)
            {
                case VRPlatform.OculusMobile:
                case VRPlatform.OculusPC:
                    TBOculusEditorSDKSettings.instance.SaveSettings();
                    break;
                case VRPlatform.SteamVR:
                    TBSteamVREditorSDKSettings.instance.SaveSettings();
                    break;
                case VRPlatform.Daydream:
                    TBGoogleEditorSDKSettings.instance.SaveSettings();
                    break;
            #if TB_HAS_UNITY_PS4
            case VRPlatform.PlayStationVR:
                    TBPSVREditorSDKSettings.instance.SaveSettings();
                    break;
            #endif
                case VRPlatform.WindowsMR:
                    TBWindowsMREditorSDKSettings.instance.SaveSettings();
                    break;
            }
        }

        public static SDKs GetActiveSDKs()
        {
            RefreshActiveSDKs();
            return editorSDKs;
        }

        public static SDKs GetEditorSDKs()
        {
            FileInfo info = new FileInfo("TButtEditorSDKs.json");
            if (info.Exists)
                return TBDataManager.DeserializeFromFile<SDKs>("TButtEditorSDKs.json", TBDataManager.PathType.ProjectFolder);
            else
                return new SDKs();
        }

        public static int GetNumActiveSDKs()
        {
            RefreshActiveSDKs();

            int i = 0;
            if (editorSDKs.oculus)
                i++;
            if (editorSDKs.googleVR)
                i++;
            if (editorSDKs.psvr)
                i++;
            if (editorSDKs.steamVR)
                i++;
            if (editorSDKs.windows)
                i++;

            return i;
        }

        private static void RefreshActiveSDKs()
        {
            if (window != null)
                return;

            editorSDKs.oculus = false;
            editorSDKs.googleVR = false;
            editorSDKs.steamVR = false;
            editorSDKs.psvr = false;
            editorSDKs.windows = false;

#if TB_OCULUS
            editorSDKs.oculus = true;
#endif
#if TB_STEAM_VR
            editorSDKs.steamVR = true;
#endif
#if TB_GOOGLE
            editorSDKs.googleVR = true;
#endif
#if TB_PSVR
            editorSDKs.psvr = true;
#endif
#if TB_WINDOWS_MR
            editorSDKs.windows = true;
#endif
        }

        [System.Serializable]
        public struct SDKs
        {
            public bool oculus;
            public bool steamVR;
            public bool googleVR;
            public bool psvr;
            public bool windows;
            public bool logs;
            public bool forceSync;
        }
    }
}
