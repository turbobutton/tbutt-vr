using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TButt.Editor
{
    public abstract class TBEditorSDKSettingsBase
    {
        abstract protected string _pluginName { get; }
        abstract protected string _sdkName { get; }
        abstract protected string _version { get; }
        abstract protected string _pluginURL { get; }
        abstract protected VRPlatform _platform { get; }
        
        protected TBSettings.TBDisplaySettings _displaySettings;
        protected bool _loadedSettings = false;

        protected Vector2 _scrollAmount = Vector2.zero;

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

        public virtual void ShowSettings()
        {
            if (!_loadedSettings)
                LoadSettings();

            ShowVersion();

            _scrollAmount = EditorGUILayout.BeginScrollView(_scrollAmount);
            ShowDisplaySettings(ref _displaySettings);
            ShowQualitySettings(ref _displaySettings.qualitySettings);
            EditorGUILayout.EndScrollView();
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

        protected virtual void LoadSettings()
        {
            throw new System.NotImplementedException();
        }

        protected void ShowDisplaySettings(ref TBSettings.TBDisplaySettings settings)
        {
            StartSettingsSection();
            EditorGUILayout.LabelField("Timestep Settings", TBEditorStyles.h2);
            settings.targetTimestep = (TBSettings.TBTimestep)EditorGUILayout.EnumPopup(new GUIContent("Target Timestep", "Timestep's relationship to the framerate. Leave at 'Locked' for smoothest physics movement."), settings.targetTimestep);
            settings.maxTimestep = (TBSettings.TBTimestep)EditorGUILayout.EnumPopup(new GUIContent("Max Timestep", "Timestep's relationship to the framerate. Leave at 'Half' to support low performance modes."), settings.maxTimestep);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Graphics Settings", TBEditorStyles.h2);
            settings.opaqueSortMode = (UnityEngine.Rendering.OpaqueSortMode)EditorGUILayout.EnumPopup(new GUIContent(
    "GPU Sorting Mode", "Generally, 'Front to Back' is better for GPU optimization, 'No Distance Sort' is better for mobile GPUs or CPU optimization. Leave on 'Default' if you aren't sure."), settings.opaqueSortMode);
            settings.depthTextureMode = (DepthTextureMode)EditorGUILayout.EnumPopup(new GUIContent("Camera Depth Texture", "Leave off for performance unless required by your shaders or other graphics settings."), settings.depthTextureMode);
            ShowRenderscaleSlider(ref settings);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Display Settings", TBEditorStyles.h2);
            settings.mirrorDisplay = EditorGUILayout.Toggle(new GUIContent("Mirror Display", "Show a mirror of the VR headset view on the main display, if available."), settings.mirrorDisplay);
            settings.qualityLevel = (TBSettings.TBQualityLevel)EditorGUILayout.EnumPopup(new GUIContent("TBQualityLevel", "Sets TButt's internal quality tier. NOT the same as Unity's 'Quality Settings'."), settings.qualityLevel);
            EndSettingsSection();
        }

        protected void ShowQualitySettings(ref TBSettings.TBQualitySettings settings)
        {
            StartSettingsSection();
            EditorGUILayout.LabelField("Quality Settings", TBEditorStyles.h2);
            settings.enableQualitySettingsOverride = EditorGUILayout.BeginToggleGroup("Override Unity Quality Settings", settings.enableQualitySettingsOverride);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Rendering");
            settings.pixelLighCount = (int)EditorGUILayout.IntField(new GUIContent("Pixel Light Count", "Number of pixel lights allowed."), settings.pixelLighCount);
            settings.textureQuality = (TBSettings.TBTextureSize)EditorGUILayout.EnumPopup(new GUIContent("Texture Quality", "Base texture level."), settings.textureQuality);
            settings.anisotropicFiltering = (AnisotropicFiltering)EditorGUILayout.EnumPopup(new GUIContent("Anisotropic Filtering", "Improves quality and removes stitching artifacts from mipmaps on surfaces at oblique viewing angles, at some GPU cost."), settings.anisotropicFiltering);
            settings.antialiasingLevel = (TBSettings.TBAntialiasingLevel)EditorGUILayout.EnumPopup(new GUIContent("Antialiasing", "MSAA level applied."), settings.antialiasingLevel);
            settings.softParticles = EditorGUILayout.Toggle(new GUIContent("Soft Particles", "Soft blending on particles. May be expensive because it requires the camera to render a depth texture."), settings.softParticles);
            settings.realtimeReflectionProbes = EditorGUILayout.Toggle(new GUIContent("Realtime Reflection Probes", "Allow realtime reflection probes?"), settings.realtimeReflectionProbes);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Shadows");
            settings.shadowQuality = (ShadowQuality)EditorGUILayout.EnumPopup(new GUIContent("Shadows Allowed", "Shadow quality."), settings.shadowQuality);
            EditorGUI.BeginDisabledGroup(settings.shadowQuality != ShadowQuality.Disable);
            settings.shadowResolution = (ShadowResolution)EditorGUILayout.EnumPopup(new GUIContent("Shadow Resolution", "Default resolution of shadow maps."), settings.shadowResolution);
            settings.shadowProjection = (ShadowProjection)EditorGUILayout.EnumPopup(new GUIContent("Shadow Projection", "Close = higher res, Stable = less wobbly."), settings.shadowProjection);
            settings.shadowDistance = (int)EditorGUILayout.IntField(new GUIContent("Shadow Distance", "Max distance shadows will be drawn."), settings.shadowDistance);
            settings.shadowNearPlaneOffset = (int)EditorGUILayout.IntField(new GUIContent("Shadow Near Plane Offset", "Offset to prevent shadow pancaking."), settings.shadowDistance);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Other");
            settings.blendWeights = (BlendWeights)EditorGUILayout.EnumPopup(new GUIContent("Blend Weights", "Bone count for mesh skinning."), settings.blendWeights);
            settings.LODbias = (float)EditorGUILayout.FloatField(new GUIContent("LOD Bias", "Less than 1 favors less detail, greater than 1 favors high detail."), settings.LODbias);
            settings.maximumLODLevel = (int)EditorGUILayout.IntField(new GUIContent("Maximum LOD Level", "Highest LOD to use (0 is highest quality)."), settings.maximumLODLevel);
            EditorGUILayout.EndToggleGroup();

            EndSettingsSection();
        }

        public virtual void ShowSDKNotFoundMessage()
        {
            EditorGUILayout.HelpBox(_pluginName + " not detected in this Unity project. TButt cannot support " + _sdkName + " until it is installed.", UnityEditor.MessageType.Error);
        }

        public virtual void ShowSDKDownloadButton()
        {
            if (GUILayout.Button("Download " + _pluginName + " " + _version, new GUILayoutOption[1] { GUILayout.Height(40) }))
            {
                Application.OpenURL(_pluginURL);
            }
        }

        protected virtual void ShowVersion()
        {
            if(GetVersion() == "???")
                GUILayout.Label("Enable and save to detect version. Known stable version: " + _version);
            else
                GUILayout.Label("Detected " + _pluginName + ": " + GetVersion() + ". Known stable version: " + _version);
        }

        public string GetName()
        {
            return _sdkName;
        }

        public VRPlatform GetPlatform()
        {
            return _platform;
        }

        public virtual bool HasSDK()
        {
            return false;
        }

        protected virtual void ShowRenderscaleSlider(ref TBSettings.TBDisplaySettings settings)
        {
            settings.renderscale = EditorGUILayout.Slider(new GUIContent("Renderscale", "Default Unity XR renderscale for this platform."), settings.renderscale, 0.1f, 1.9f);
        }

        protected virtual string GetVersion()
        {
            return "???";
        }
    }
}