using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Settings;

namespace TButt.Editor
{
    public class TBOculusEditorSDKSettings : TBEditorSDKSettingsBase
    {

        #region REQUIREMENTS
        protected override string _pluginName
        {
            get
            {
                return "Oculus Utilities";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "Oculus";
            }
        }

        protected override string _version
        {
            get
            {
                return "1.30.0";
            }
        }

        protected override VRPlatform _platform
        {
            get
            {
                return VRPlatform.OculusPC;
            }
        }


        protected override string _pluginURL
        {
            get
            {
                return "https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022";
            }
        }

        private static TBOculusEditorSDKSettings _instance;
        public static TBOculusEditorSDKSettings instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new TBOculusEditorSDKSettings();
                }
                return _instance;
            }
        }

        #endregion
        
        protected int _oculusToolbar;
        protected TBOculusSettings.TBCoreSettingsOculus _gearVRSettings;
        protected TBOculusSettings.TBCoreSettingsOculus _questSettings;
        protected TBOculusSettings.TBCoreSettingsOculus _goSettings;
        protected TBOculusSettings.TBCoreSettingsOculus _riftSettings;
        protected TBOculusSettings.OculusDeviceFamily _selectedDeviceFamily;

        public override void ShowSettings()
        {
            if (!_loadedSettings)
                LoadSettings();

            _oculusToolbar = GUILayout.Toolbar(_oculusToolbar, new string[] { "Rift", "Gear VR", "Go", "Quest" }, GUILayout.MinHeight(24), GUILayout.MaxHeight(24));

            ShowVersion();

            _scrollAmount = EditorGUILayout.BeginScrollView(_scrollAmount);
            switch (_oculusToolbar)
            {
                case 0:
                    _selectedDeviceFamily = TBOculusSettings.OculusDeviceFamily.Rift;
                    ShowSettingsForSubplatform(ref _riftSettings);
                    break;
                case 1:
                    _selectedDeviceFamily = TBOculusSettings.OculusDeviceFamily.GearVR;
                    ShowSettingsForSubplatform(ref _gearVRSettings);
                    break;
                case 2:
                    _selectedDeviceFamily = TBOculusSettings.OculusDeviceFamily.Go;
                    ShowSettingsForSubplatform(ref _goSettings);
                    break;
                case 3:
                    _selectedDeviceFamily = TBOculusSettings.OculusDeviceFamily.Quest;
                    ShowSettingsForSubplatform(ref _questSettings);
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        public override void SaveSettings()
        {
            base.SaveSettingsFile(TBOculusSettings.GetOculusSettingsFilename(TBOculusSettings.OculusDeviceFamily.Rift), _riftSettings);
            base.SaveSettingsFile(TBOculusSettings.GetOculusSettingsFilename(TBOculusSettings.OculusDeviceFamily.GearVR), _gearVRSettings);
            base.SaveSettingsFile(TBOculusSettings.GetOculusSettingsFilename(TBOculusSettings.OculusDeviceFamily.Go), _goSettings);
            base.SaveSettingsFile(TBOculusSettings.GetOculusSettingsFilename(TBOculusSettings.OculusDeviceFamily.Quest), _questSettings);
        }

        protected void ShowSettingsForSubplatform(ref TBOculusSettings.TBCoreSettingsOculus subplatform)
        {
            ShowDisplaySettings(ref subplatform.displaySettings);

            if (_selectedDeviceFamily == TBOculusSettings.OculusDeviceFamily.Rift)
            {
                StartSettingsSection();
                subplatform.allowViveEmulation = GUILayout.Toggle(subplatform.allowViveEmulation, new GUIContent("Support Vive through Oculus Utilities", "Use Oculus Utilities for Vive support if Steam VR plugin is not installed."));
                EndSettingsSection();
            }

            StartSettingsSection();
            EditorGUILayout.LabelField("Oculus Rendering Options", TBEditorStyles.h2);

            ShowFixedFoveatedRenderingOptions(ref subplatform);
            ShowRefreshRateOptions(ref subplatform);
            ShowDynamicResolutionOptions(ref subplatform);

            EndSettingsSection();

            ShowQualitySettings(ref subplatform.displaySettings.qualitySettings);

        }

        protected void ShowFixedFoveatedRenderingOptions(ref TBOculusSettings.TBCoreSettingsOculus subplatform)
        {
            EditorGUI.BeginDisabledGroup(!TBOculusSettings.SupportsFixedFoveatedRendering(_selectedDeviceFamily));
            if (TBOculusSettings.SupportsFixedFoveatedRendering(_selectedDeviceFamily))
                subplatform.fixedFoveatedRenderingLevel = (TBOculusSettings.FixedFoveatedRenderingLevel)EditorGUILayout.EnumPopup(new GUIContent("Fixed Foveated Rendering", "FFR amount (Go / Santa Cruz only)."), subplatform.fixedFoveatedRenderingLevel);
            else
                EditorGUILayout.LabelField("Fixed Foveated Rendering", "Not Supported");
            EditorGUI.EndDisabledGroup();
        }

        protected void ShowRefreshRateOptions(ref TBOculusSettings.TBCoreSettingsOculus subplatform)
        {
            EditorGUI.BeginDisabledGroup(!TBOculusSettings.SupportsMultipleRefreshRates(_selectedDeviceFamily));
            TBOculusSettings.TBOculusMobileRefreshRate refreshRate = (TBOculusSettings.TBOculusMobileRefreshRate)subplatform.displaySettings.refreshRate;
            if(TBOculusSettings.SupportsMultipleRefreshRates(_selectedDeviceFamily))
            {
                refreshRate = (TBOculusSettings.TBOculusMobileRefreshRate)EditorGUILayout.EnumPopup(new GUIContent("Refresh Rate", "Choose target refresh rate (Go only)."), refreshRate);
                subplatform.displaySettings.refreshRate = (TBSettings.TBRefreshRate)refreshRate;
            }
            else
            {
                EditorGUILayout.LabelField("Refresh Rate", subplatform.displaySettings.refreshRate.ToString());
            }
            EditorGUI.EndDisabledGroup();
        }

        protected void ShowDynamicResolutionOptions(ref TBOculusSettings.TBCoreSettingsOculus subplatform)
        {
          
            EditorGUI.BeginDisabledGroup(!TBOculusSettings.SupportsDynamicResolution(_selectedDeviceFamily));
            if (TBOculusSettings.SupportsDynamicResolution(_selectedDeviceFamily))
            {
                subplatform.useDynamicResolution = EditorGUILayout.BeginToggleGroup(new GUIContent("Enable Adaptive Resolution", "Adjust resolution to maintain performance (Rift / PC only)."), subplatform.useDynamicResolution);
            }
            else
                EditorGUILayout.LabelField("Enable Adaptive Resolution", "Not Supported");

            subplatform.dynamicResolutionRange.x = EditorGUILayout.Slider(new GUIContent("Minimum Renderscale", "Lowest renderscale allowed by dynamic resolution."), subplatform.dynamicResolutionRange.x, 0.1f, 1.9f);
            subplatform.dynamicResolutionRange.y = EditorGUILayout.Slider(new GUIContent("Maximum Renderscale", "Highest renderscale allowed by dynamic resolution."), subplatform.dynamicResolutionRange.y, 0.1f, 1.9f);

            if (TBOculusSettings.SupportsDynamicResolution(_selectedDeviceFamily))
                    EditorGUILayout.EndToggleGroup();

            EditorGUI.EndDisabledGroup();
        }

        protected override void ShowRenderscaleSlider(ref TBSettings.TBDisplaySettings settings)
        {
            base.ShowRenderscaleSlider(ref settings);
            EditorGUILayout.LabelField("Eye Texture Resolution: " + TBOculusSettings.GetResolutionScalingFactor(_selectedDeviceFamily) * settings.renderscale);
        }

        public override bool HasSDK()
        {
            string[] assets = AssetDatabase.FindAssets("OVRCameraRig", new string[] { "Assets" });
            if (assets.Length == 0)
                return false;
            else
                return true;
        }

        protected override void LoadSettings()
        {
            _gearVRSettings = GetSettingsForDeviceFamily(TBOculusSettings.OculusDeviceFamily.GearVR);
            _questSettings = GetSettingsForDeviceFamily(TBOculusSettings.OculusDeviceFamily.Quest);
            _goSettings = GetSettingsForDeviceFamily(TBOculusSettings.OculusDeviceFamily.Go);
            _riftSettings = GetSettingsForDeviceFamily(TBOculusSettings.OculusDeviceFamily.Rift);
            _loadedSettings = true;
        }

        protected TBOculusSettings.TBCoreSettingsOculus GetSettingsForDeviceFamily(TBOculusSettings.OculusDeviceFamily family)
        {
            return TBOculusSettings.LoadOculusSettings(family);
        }

#if TB_OCULUS
        protected override string GetVersion()
        {
            return OVRPlugin.wrapperVersion.ToString();
        }
#endif
    }
}