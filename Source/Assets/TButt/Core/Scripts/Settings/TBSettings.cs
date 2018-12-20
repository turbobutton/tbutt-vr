using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using System.Diagnostics;
using TButt.Settings;

namespace TButt
{
    public static class TBSettings
    {
        public static readonly string settingsFolder = "TButtSettings/";
        public static readonly string controlSettingsFileName = "ControlSettings";
        public static readonly string coreSettingsFilename = "CoreSettings_";
        public static readonly string cameraSettingsFilename = "CameraSettings";
        public static readonly string displaySettingsFilename = "DisplaySetings";

        private static TBControlSettings _controlSettings;
        private static TBDisplaySettings _displaySettings;
        private static TBCameraSettings _cameraSettings;

        private static TBSettingsBase _settingsBase;

        private static bool _loadedSettings;

        /// <summary>
        /// Main initialization function to read TButt settings from JSON.
        /// </summary>
        public static void Initialize()
        {
            if (!_loadedSettings)
                LoadSettings(TBCore.GetActivePlatform());
        }

        /// <summary>
        /// Override function to force TBCore to initialize for a specific platform.
        /// Only necessary for devices that can start without a headset active.
        /// </summary>
        /// <param name="platform"></param>
        public static void Initialize(VRPlatform platform)
        {
            if (!_loadedSettings)
                LoadSettings(platform);
            else
                TBLogging.LogMessage("TBSettings.Initialize() was called, but settings were already initialized.");
        }

        private static void LoadSettings(VRPlatform platform)
        {
            if (_loadedSettings)
                return;

            // Get control and camera settings.
            _controlSettings = TBDataManager.DeserializeFromFile<TBControlSettings>(settingsFolder + controlSettingsFileName, TBDataManager.PathType.ResourcesFolder);
            if (!_controlSettings.supports3DOFControllers && !_controlSettings.supportsClickRemote && !_controlSettings.supportsGamepad && !_controlSettings.supportsHandControllers)
                UnityEngine.Debug.LogWarning("No controller types are enabled in TBInput's settings!");
            PrintControlSettings();
            _cameraSettings = TBDataManager.DeserializeFromFile<TBCameraSettings>(settingsFolder + cameraSettingsFilename, TBDataManager.PathType.ResourcesFolder);

            // Setup platform settings subclass.
            switch (platform)
            {
                case VRPlatform.OculusPC:
                case VRPlatform.OculusMobile:
                    _settingsBase = TBCore.instance.gameObject.AddComponent<TBOculusSettings>();
                    break;
                #if TB_HAS_UNITY_PS4
                case VRPlatform.PlayStationVR:
                    _settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsPSVR>();
                    break;
                #endif
                case VRPlatform.WindowsMR:
                    _settingsBase = TBCore.instance.gameObject.AddComponent<TBWindowsMRSettings>();
                    break;
                case VRPlatform.SteamVR:
                    _settingsBase = TBCore.instance.gameObject.AddComponent<TBSteamVRSettings>();
                    break;
                case VRPlatform.Daydream:
                    _settingsBase = TBCore.instance.gameObject.AddComponent<TBGoogleSettings>();
                    break;
                default:
                    _settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsBase>();
                    break;
            }
            _settingsBase.Initialize();

            // Apply quality settings overrides if needed.
            if (_settingsBase.GetDisplaySettings().qualitySettings.enableQualitySettingsOverride)
                ApplyQualitySettingsOverrides(_settingsBase.GetDisplaySettings().qualitySettings);

            // Set timestep.
            SetTimestep();

            _loadedSettings = true;
        }

        /// <summary>
        /// Sets the timestep overrides as specified in TBCore's settings.
        /// </summary>
        private static void SetTimestep()
        {
            switch(_settingsBase.GetDisplaySettings().targetTimestep)
            {
                case TBTimestep.Half:
                    Time.fixedDeltaTime = (1f / (GetRefreshRate()) * 2f);
                    break;
                case TBTimestep.Locked:
                    Time.fixedDeltaTime = 1f / GetRefreshRate();
                    break;
                case TBTimestep.KeepUnitySetting:
                    break;
            }

            switch(_settingsBase.GetDisplaySettings().maxTimestep)
            {
                case TBTimestep.Half:
                    Time.maximumDeltaTime = (1f / (GetRefreshRate()) * 2f);
                    break;
                case TBTimestep.Locked:
                    Time.maximumDeltaTime = 1f / GetRefreshRate();
                    break;
                case TBTimestep.KeepUnitySetting:
                    break;
            }

            TBLogging.LogMessage("Fixed Timestep: " + Time.fixedDeltaTime + ", Max Timestep: " + Time.maximumDeltaTime);
        }

        #region PUBLIC GET FUNCTIONS

        public static TBControlSettings GetControlSettings()
        {
            return _controlSettings;
        }

        public static TBCameraSettings GetCameraSettings()
        {
            return _cameraSettings;
        }

        public static TBDisplaySettings GetDisplaySettings()
        {
            if (_settingsBase == null)
                UnityEngine.Debug.LogError("Attempted to get TBDisplaySettings before they were loaded!");
            return _settingsBase.GetDisplaySettings();
        }

        /// <summary>
        /// Returns the TBQualityLevel configured in TBCore settings.
        /// </summary>
        /// <returns></returns>
        public static TBQualityLevel GetCurrentQualityLevel()
        {
            return _displaySettings.qualityLevel;
        }

        /// <summary>
        /// Gets the renderscale configured in TBCore (which may be different from the realtime, actual renderscale in Unity's XR Settings)
        /// </summary>
        /// <returns></returns>
        public static float GetConfiguredRenderscale()
        {
            return _settingsBase.GetConfiguredRenderscale();
        }

        /// <summary>
        /// Returns the refresh rate set in TBCore's settings. This may be different from the actual refresh rate reported by Unity's XRDevice settings.
        /// </summary>
        /// <returns></returns>
        public static float GetRefreshRate()
        {
            return _settingsBase.GetRefreshRate();
        }

        public static VRHeadset GetActiveHeadset()
        {
            if (_settingsBase != null)
            {
                return _settingsBase.GetActiveHeadset();
            }
            else
            {
                TBLogging.LogWarning("Attempted to read active headset before it was initialized.");
                return VRHeadset.None;
            }
        }

        #endregion

        [Conditional("TB_ENABLE_LOGS")]
        static void PrintControlSettings()
        {
            string inputList = "Supported inputs: ";
            if (_controlSettings.supportsHandControllers)
                inputList += " Hand Controllers. ";
            if (_controlSettings.supports3DOFControllers)
                inputList += " 3DOF Controllers. ";
            if (_controlSettings.supportsClickRemote)
                inputList += " Click Remotes. ";
            if (_controlSettings.supportsGamepad)
                inputList += " Gamepads. ";

            TBLogging.LogMessage(inputList);
        }

        static void ApplyQualitySettingsOverrides(TBQualitySettings settings)
        {
            TBLogging.LogMessage("Overriding Unity quality settings...");

            QualitySettings.masterTextureLimit = (int)settings.textureQuality;
            QualitySettings.antiAliasing = (int)settings.antialiasingLevel;
            QualitySettings.anisotropicFiltering = settings.anisotropicFiltering;
            QualitySettings.realtimeReflectionProbes = settings.realtimeReflectionProbes;
            QualitySettings.pixelLightCount = settings.pixelLighCount;
            QualitySettings.softParticles = settings.softParticles;
            QualitySettings.shadows = settings.shadowQuality;
            QualitySettings.shadowResolution = settings.shadowResolution;
            QualitySettings.shadowNearPlaneOffset = settings.shadowNearPlaneOffset;
            QualitySettings.shadowDistance = settings.shadowDistance;
            QualitySettings.lodBias = settings.LODbias;
            QualitySettings.maximumLODLevel = settings.maximumLODLevel;
            QualitySettings.blendWeights = settings.blendWeights;
        }


        #region SETTERS
        /// <summary>
        /// Sets the Unity XR renderscale.
        /// </summary>
        /// <param name="newScale"></param>
        public static void SetRenderscale(float newScale)
        {
            _settingsBase.SetRenderscale(newScale);
        }

        public static void SetQualityLevel(TBQualityLevel level)
        {
            _displaySettings.qualityLevel = level;
        }
        #endregion

        #region SETTINGS STRUCTS
        /// <summary>
        /// Defined in editor window or by platform defaults.
        /// </summary>
        [System.Serializable]
        public struct TBDisplaySettings
        {
            public TBTimestep targetTimestep;
            public TBTimestep maxTimestep;
            public float renderscale;
            public TBQualityLevel qualityLevel;
            public bool mirrorDisplay;
            public UnityEngine.Rendering.OpaqueSortMode opaqueSortMode;
            public DepthTextureMode depthTextureMode;
            public TBRefreshRate refreshRate;
            public TBQualitySettings qualitySettings;
            public bool initialized;
        }

        [System.Serializable]
        public struct TBCameraSettings
        {
            public TBTrackingOrigin trackingOrigin;
            public KeyCode calibrationHotkey;
            public float uncalibratedFloorHeight;
            public bool disablePositionTracking;
        }

        [System.Serializable]
        public struct TBControlSettings
        {
            public bool supportsHandControllers;
            public bool supports3DOFControllers;
            public bool emulate3DOFArmModel;
            public TBHardwareHandedness handedness3DOF;
            public bool supportsClickRemote;
            public bool supportsGamepad;
            public bool useInputEvents;
            public TBInput.ControlType defaultEditorControlType;
        }

        [System.Serializable]
        public struct TBQualitySettings
        {
            public TBTextureSize textureQuality;
            public TBAntialiasingLevel antialiasingLevel;
            public AnisotropicFiltering anisotropicFiltering;
            public bool realtimeReflectionProbes;
            public int pixelLighCount;
            public bool softParticles;
            public ShadowQuality shadowQuality;
            public ShadowResolution shadowResolution;
            public ShadowProjection shadowProjection;
            public int shadowDistance;
            public int shadowNearPlaneOffset;
            public float LODbias;
            public int maximumLODLevel;
            public BlendWeights blendWeights;
            public bool enableQualitySettingsOverride;
        }

        [System.Serializable]
        public enum TBQualityLevel
        {
            VeryLow,
            Low,
            Medium,
            High,
            VeryHigh,
            Ultra
        }

        [System.Serializable]
        public enum TBTimestep
        {
            Locked,
            Half,
            KeepUnitySetting
        }

        [System.Serializable]
        public enum TBTrackingOrigin
        {
            Eye,
            Floor
        }

        [System.Serializable]
        public enum TBRefreshRate
        {
            FPS_60  =   60,
            FPS_72 =    72,
            FPS_75 =    75,
            FPS_90  =   90,
            FPS_120 =   120
        }

        [System.Serializable]
        public enum TBGoogleRefreshRate
        {
            FPS_60  =   60,
            FPS_90 =    90
        }

        [System.Serializable]
        public enum TBPSVRRefreshRate
        {
            FPS_60  =   60,
            FPS_90  =   90,
            FPS_120 =   120
        }

        [System.Serializable]
        public enum TBHardwareHandedness
        {
            Left,
            Right
        }

        public enum TBAntialiasingLevel
        {
            Off      = 0,
            _2x      = 2,
            _4x      = 4,
            _8x      = 8
        }

        public enum TBTextureSize
        {
            FullRes     = 0,
            HalfRes     = 1,
            QuaterRes   = 2,
            EighthRes   = 3
        }

        /// <summary>
        /// Helper class for Unity's VR device names that should've been an enum in the first place (I'm not mad)
        /// </summary>
        public class VRDeviceNames
        {
            public const string Oculus = "Oculus";
            public const string SteamVR = "OpenVR";
            public const string Daydream = "daydream";
            public const string Cardboard = "cardboard";
            public const string Unknown = "Unknown";
            public const string PlayStationVR = "PlayStationVR";
            public const string WindowsMR = "WindowsMR";
            public const string None = "";
        }
        #endregion
    }

    public enum VRHeadset
    {
        None = 0,

        // Oculus PC
        OculusRift = 1,

        // Oculus Mobile
        GearVR = 200,
        OculusGo = 201,
        OculusQuest = 202,
        MiVRStandalone = 203,

        // Steam VR
        HTCVive = 400,

        // Windows Mixed Reality
        WindowsMR = 600,

        // Daydream
        Daydream = 800,
        MirageSolo = 801,

        // PlayStation
        PSVR = 1000
    }

    public enum VRController
    {
        None = 0,

        // Oculus
        OculusRemote              = 1,
        OculusTouch               = 2,
        XboxOneGamepad            = 3,
        GearVRController          = 4,
        OculusGoController        = 5,
        OculusQuestController     = 6,
        GearVRTouchpad            = 7,

        // Steam VR
        ViveController = 200,
        XInputGamepad       = 201,
        
        // Windows Mixed Reality
        WMRController       = 400,

        // Daydream
        Daydream3DOF        = 600,
        Daydream6DOF        = 601,

        // PlayStation
        DualShock4          = 800,
        PlayStationMove     = 801,
        PlayStationAim      = 802
    }
}