using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using TButt;
using TButt.Services;
using System.Collections.Generic;

namespace TButt.Editor
{
    /// <summary>
    /// Allows you to define controller compatibility settings, prefabs for motion controllers, and button mappings for motion controllers and touchpad emulation.
    /// </summary>
    public class TBEditorServiceSettings : EditorWindow
    {
        // Toolbar stuff
        static EditorWindow window;

        static bool hasCompiled;

        public int toolbar;
        string selectedServiceName;

        static Texture[] toolbarImages;
        static Texture crossImage;
        static Texture checkImage;

        // Service Settings
        static TBEditorServiceDefines.Services selectedServices;
        public TBEditorServiceSettingsBase _selectedService;

        static Dictionary<string, TBEditorServiceSettingsBase> _serviceSettings;

        [MenuItem("TButt/Service Settings...", false, 901)]
        public static void ShowWindow()
        {
            TBEditorStyles.SetupSharedStyles();
            int platforms = 2;
            #if TB_HAS_XBOX_LIVE
            int xboxIndex = platforms;
            platforms++;
            #endif
            #if TB_HAS_UNITY_PS4
            int psnIndex = platforms;
            platforms++;
            #endif

            toolbarImages = new Texture[platforms];
            toolbarImages[0] = EditorGUIUtility.Load("TButt/Icons/oculus.png") as Texture2D;
            toolbarImages[1] = EditorGUIUtility.Load("TButt/Icons/steam.png") as Texture2D;
            #if TB_HAS_XBOX_LIVE
            toolbarImages[xboxIndex] = EditorGUIUtility.Load("TButt/Icons/windows.png") as Texture2D;
            #endif
            #if TB_HAS_UNITY_PS4
            toolbarImages[psnIndex] = EditorGUIUtility.Load("TButt/Icons/playstation.png") as Texture2D;
            #endif

            crossImage = EditorGUIUtility.Load("TButt/Icons/cross.png") as Texture2D;
            checkImage = EditorGUIUtility.Load("TButt/Icons/check.png") as Texture2D;

            RefreshSettings();

            window = EditorWindow.GetWindow(typeof(TBEditorServiceSettings), true, "Service Settings", true);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

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

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Toolbar(10,
                new Texture[] {
                    selectedServices.oculus ? checkImage : crossImage,
                    selectedServices.steam ? checkImage : crossImage
                    #if TB_HAS_XBOX_LIVE
                    ,selectedServices.xbox ? checkImage : crossImage
                    #endif
                    #if TB_HAS_UNITY_PS4
                    ,selectedServices.psn ? checkImage : crossImage
                    #endif
                },
                    GUILayout.MinHeight(16), GUILayout.MaxHeight(16));
            EditorGUI.EndDisabledGroup();

            toolbar = GUILayout.Toolbar(toolbar, toolbarImages, GUILayout.MinHeight(32), GUILayout.MaxHeight(32));
            EditorGUILayout.Separator();

            switch (toolbar)
            {
                case 0:
                    ShowSDKSection(TBOculusEditorServiceSettings.instance.GetName(), ref selectedServices.oculus);
                    break;
                case 1:
                    ShowSDKSection(TBSteamworksEditorServiceSettings.instance.GetName(), ref selectedServices.steam);
                    break;
                case 2:
                    #if TB_HAS_XBOX_LIVE
                    #endif
                    break;
                case 3:
                    #if TB_HAS_UNITY_PS4
                    #endif
                    break;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal(new GUILayoutOption[1] { GUILayout.Height(70) });
       
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save General Settings", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                SaveAllSettings();
                window.Close();
            }

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Close Without Saving", new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                window.Close();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        void ShowSDKSection(string sdk, ref bool enabled)
        {
            selectedServiceName = _serviceSettings[sdk].GetName();
            if (!_serviceSettings[sdk].HasSDK())
            {
                _serviceSettings[sdk].ShowSDKNotFoundMessage();
                _serviceSettings[sdk].ShowSDKDownloadButton();
            }
            else
            {
                bool isEnabled = enabled;
                _serviceSettings[sdk].ShowSDKToggleButton(ref enabled);
                _serviceSettings[sdk].ShowSettings();
                if(isEnabled != enabled)
                {
                    if(!enabled)
                    {
                        selectedServices = new TBEditorServiceDefines.Services();
                    }
                    else
                    {
                        selectedServices = TBEditorServiceDefines.GetServicesStruct(_serviceSettings[sdk].GetService());
                    }
                }
            }
        }

        static void RefreshSettings()
        {
            _serviceSettings = new Dictionary<string, TBEditorServiceSettingsBase>();
            _serviceSettings.Add(TBOculusEditorServiceSettings.instance.GetName(), TBOculusEditorServiceSettings.instance);
            _serviceSettings.Add(TBSteamworksEditorServiceSettings.instance.GetName(), TBSteamworksEditorServiceSettings.instance);

            #if TB_HAS_UNITY_PS4
            sdkSettings.Add(TBPSVREditorServiceSettings.instance.GetName(), TBPSVREditorServiceSettings.instance);
            #endif

           selectedServices = GetServices();
        }

        static void SaveAllSettings()
        {
            foreach (KeyValuePair<string, TBEditorServiceSettingsBase> entry in _serviceSettings)
            {
                SaveServiceSettings(entry.Value.GetService());
            }
            SaveEditorSettings();
            TBEditorDefines.SetScriptingDefines();
        }

        static void SaveServiceSettings(VRService service)
        {
            switch (service)
            {
                case VRService.Oculus:
                    TBOculusEditorServiceSettings.instance.SaveSettings();
                    break;
                case VRService.Steam:
                    TBSteamworksEditorServiceSettings.instance.SaveSettings();
                    break;
            #if TB_HAS_UNITY_PS4
            case VRPlatform.PlayStationVR:
                    TBPSVREditorSDKSettings.instance.SaveSettings();
                    break;
            #endif
            }
        }

        static TBEditorServiceDefines.Services GetServices()
        {
            FileInfo info = new FileInfo("TButtEditorServices.json");
            if (info.Exists)
                return TBDataManager.DeserializeFromFile<TBEditorServiceDefines.Services>("TButtEditorServices.json", TBDataManager.PathType.ProjectFolder);
            else
                return new TBEditorServiceDefines.Services();
        }

        static void SaveEditorSettings()
        {
            TBEditorHelper.CheckoutAndSaveJSONFile("TButtEditorServices.json", selectedServices, TBDataManager.PathType.ProjectFolder, true);

            if (selectedServices.oculus)
                TBEditorServiceDefines.SetTButtService(VRService.Oculus);
            else if (selectedServices.steam)
                TBEditorServiceDefines.SetTButtService(VRService.Steam);
            else if (selectedServices.psn)
                TBEditorServiceDefines.SetTButtService(VRService.PSN);
            else if (selectedServices.xbox)
                TBEditorServiceDefines.SetTButtService(VRService.XboxLive);
            else
                TBEditorServiceDefines.SetTButtService(VRService.None);
        }
    }
}