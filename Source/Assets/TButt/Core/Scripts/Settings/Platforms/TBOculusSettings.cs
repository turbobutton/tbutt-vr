using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt.Settings
{
    public class TBOculusSettings : TBSettingsBase
    {
        public static readonly string settingsFilename = "Settings_Oculus_";
        private static TBCoreSettingsOculus _oculusSettings;
        private static OculusDeviceFamily _devicefamily = OculusDeviceFamily.Unknown;

        private readonly static string _gearVRFilename = "GearVR";
        private readonly static string _goFilename = "Go";
        private readonly static string _riftFilename = "Rift";
        private readonly static string _questFilename = "Quest";

        #region OVERRIDES
        public override void Initialize()
        {
            #if !TB_OCULUS
            Debug.LogError("Detected Oculus device, but Oculus SDK is disabled in TBCore Settings!");
            #endif

            _devicefamily = GetOculusDeviceFamily();

            _oculusSettings = LoadOculusSettings(_devicefamily);
            _displaySettings = _oculusSettings.displaySettings;

            StartCoroutine(ApplySettingsToOVRManager());

            switch (GetOculusDeviceFamily())
            {
                case OculusDeviceFamily.Go:
                    _headset = VRHeadset.OculusGo;
                    TBLogging.LogMessage("Detected Oculus Go. Forcing 3DOF controller as input type and setting CPU/GPU levels to 3.");
                    TBInput.SetActiveControlType(TBInput.ControlType.Mobile3DOFController);
                    StartCoroutine(ApplySettingsToOVRManager());
                    break;
                case OculusDeviceFamily.Quest:
                    _headset = VRHeadset.OculusQuest;
                    TBLogging.LogMessage("Detected Oculus Quest. Enabling 6DOF controllers.");
                    TBInput.SetActiveControlType(TBInput.ControlType.HandControllers);
                    StartCoroutine(ApplySettingsToOVRManager());
                    break;
                case OculusDeviceFamily.Rift:
                    if (UnityEngine.XR.XRDevice.model.Contains("Vive"))
                    {
                        if (_oculusSettings.allowViveEmulation)
                        {
                            TBLogging.LogMessage("Oculus Utilities is now supporting HTC Vive with Open VR passthrough.");
                            _headset = VRHeadset.HTCVive;   // Fallback for Vive support with Oculus Utitlites.
                        }
                        else
                        {
                            TBLogging.LogMessage("Oculus Utilities detected a Vive headset. Enable Vive support in the Oculus Rift submenu of TButt's Core Settings to use Oculus Utilities for Vive, or enable the Steam VR plugin to use native Steam VR.");
                            _headset = VRHeadset.None;
                        }
                    }
                    else
                    {
                        _headset = VRHeadset.OculusRift;
                    }
                    break;
                case OculusDeviceFamily.GearVR:
                    _headset = VRHeadset.GearVR;
                    break;
            }

            base.Initialize();
        }

        public override void SetRenderscale(float newScale)
        {
            if (_oculusSettings.useDynamicResolution)
            {
                TBLogging.LogMessage("Renderscale assignment ignored because dynamic renderscale is enabled through Oculus SDK.");
                return;
            }
            else
            {
                base.SetRenderscale(newScale);
            }
        }

        public override float GetRefreshRate()
        {
            switch(TBCore.GetActivePlatform())
            {
                case VRPlatform.OculusPC:
                    if (UnityEngine.XR.XRDevice.model.Contains("DK2"))
                        return 75;
                    break;
            }
            return (int)_displaySettings.refreshRate;
        }

        protected override void PrintStartupResults()
        {
            base.PrintStartupResults();
            if (_devicefamily == OculusDeviceFamily.GearVR)
                TBLogging.LogMessage("Detected Gear VR version: " + GetGearVRDeviceName());
            #if TB_OCULUS
            if(SupportsFixedFoveatedRendering(_devicefamily))
                TBLogging.LogMessage("Configured Fixed Foveated Rendering Level: " + _oculusSettings.fixedFoveatedRenderingLevel + ", Actual Fixed Foveated Rendering Level: " + OVRManager.tiledMultiResLevel);
#           endif
        }
        #endregion

        public static TBSettings.TBDisplaySettings GetDefaultDisplaySettings(OculusDeviceFamily family)
        {
            TBSettings.TBDisplaySettings defaultDisplaySettings = GetDefaultDisplaySettings(new TBSettings.TBDisplaySettings());

            defaultDisplaySettings.renderscale = 1.0f;
            defaultDisplaySettings.targetTimestep = TBSettings.TBTimestep.Locked;
            defaultDisplaySettings.maxTimestep = TBSettings.TBTimestep.Half;
            defaultDisplaySettings.initialized = true;

            switch (family)
            {
                case OculusDeviceFamily.Rift:
                    defaultDisplaySettings.renderscale = 1.0f;
                    defaultDisplaySettings.mirrorDisplay = true;
                    defaultDisplaySettings.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.Default;
                    defaultDisplaySettings.depthTextureMode = DepthTextureMode.None;
                    defaultDisplaySettings.refreshRate = TBSettings.TBRefreshRate.FPS_90;
                    break;
                case OculusDeviceFamily.GearVR:
                    defaultDisplaySettings.renderscale = 1.0f;
                    defaultDisplaySettings.qualityLevel = TBSettings.TBQualityLevel.VeryLow;
                    defaultDisplaySettings.mirrorDisplay = false;
                    defaultDisplaySettings.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.NoDistanceSort;
                    defaultDisplaySettings.depthTextureMode = DepthTextureMode.None;
                    defaultDisplaySettings.refreshRate = TBSettings.TBRefreshRate.FPS_60;
                    break;
                case OculusDeviceFamily.Go:
                    defaultDisplaySettings.renderscale = 1.2f;
                    defaultDisplaySettings.qualityLevel = TBSettings.TBQualityLevel.Low;
                    defaultDisplaySettings.mirrorDisplay = false;
                    defaultDisplaySettings.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.NoDistanceSort;
                    defaultDisplaySettings.depthTextureMode = DepthTextureMode.None;
                    defaultDisplaySettings.refreshRate = TBSettings.TBRefreshRate.FPS_60;
                    break;
                case OculusDeviceFamily.Quest:
                    defaultDisplaySettings.renderscale = 1.2f;
                    defaultDisplaySettings.mirrorDisplay = false;
                    defaultDisplaySettings.qualityLevel = TBSettings.TBQualityLevel.Low;
                    defaultDisplaySettings.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.NoDistanceSort;
                    defaultDisplaySettings.depthTextureMode = DepthTextureMode.None;
                    defaultDisplaySettings.refreshRate = TBSettings.TBRefreshRate.FPS_72;
                    break;

            }
            return defaultDisplaySettings;
        }

        public static TBCoreSettingsOculus GetDefaultOculusSettings(OculusDeviceFamily family)
        {
            TBCoreSettingsOculus defaultSettings = new TBCoreSettingsOculus();

            defaultSettings.displaySettings = GetDefaultDisplaySettings(family);
            defaultSettings.displaySettings.qualitySettings = GetDefaultOculusQualitySettings(family);
            defaultSettings.fixedFoveatedRenderingLevel = FixedFoveatedRenderingLevel.None;

            switch (family)
            {
                case OculusDeviceFamily.Rift:
                    defaultSettings.useDynamicResolution = true;
                    defaultSettings.dynamicResolutionRange = new Vector2(0.7f, 1f);
                    break;
                case OculusDeviceFamily.GearVR:
                    break;
                case OculusDeviceFamily.Go:
                case OculusDeviceFamily.Quest:
                    defaultSettings.fixedFoveatedRenderingLevel = FixedFoveatedRenderingLevel.Medium;
                    break;
            }

            return defaultSettings;
        }

        public static TBSettings.TBQualitySettings GetDefaultOculusQualitySettings(OculusDeviceFamily family)
        {
            TBSettings.TBQualitySettings qualitySettings = new TBSettings.TBQualitySettings();
            qualitySettings = GetDefaultQualitySettings(qualitySettings);

            switch (family)
            {
                case OculusDeviceFamily.Rift:
                    break;
                case OculusDeviceFamily.GearVR:
                case OculusDeviceFamily.Go:
                    qualitySettings.antialiasingLevel = TBSettings.TBAntialiasingLevel._2x;
                    qualitySettings.pixelLighCount = 0;
                    qualitySettings.shadowQuality = ShadowQuality.Disable;
                    qualitySettings.blendWeights = BlendWeights.TwoBones;
                    break;
                case OculusDeviceFamily.Quest:
                    qualitySettings.antialiasingLevel = TBSettings.TBAntialiasingLevel._4x;
                    qualitySettings.pixelLighCount = 0;
                    qualitySettings.shadowQuality = ShadowQuality.Disable;
                    qualitySettings.blendWeights = BlendWeights.TwoBones;
                    break;
            }

            return qualitySettings;
        }

        #region OCULUS SDK INTERFACES
        IEnumerator ApplySettingsToOVRManager()
        {
            yield return TBYielders.EndOfFrame;
            yield return TBYielders.EndOfFrame;
            SetRefreshRate((TBOculusMobileRefreshRate)_oculusSettings.displaySettings.refreshRate);
            yield return TBYielders.EndOfFrame;
            Time.fixedDeltaTime = 1 / GetRefreshRate();
            SetFixedFoveatedRenderingLevel(_oculusSettings.fixedFoveatedRenderingLevel);
        }

        public static void SetFixedFoveatedRenderingLevel(FixedFoveatedRenderingLevel level)
        {
        #if TB_OCULUS
            if (SupportsFixedFoveatedRendering(GetOculusDeviceFamily()))
            {
                switch (level)
                {
                    case FixedFoveatedRenderingLevel.Low:
                        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSLow;
                        break;
                    case FixedFoveatedRenderingLevel.Medium:
                        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSMedium;
                        break;
                    case FixedFoveatedRenderingLevel.High:
                        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHigh;
                        break;
                    default:
                        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.Off;
                        break;
                }
                TBLogging.LogMessage("Fixed foveated rendering amount is now  " + OVRManager.tiledMultiResLevel);
            }
            else
            {
                // TBLogging.LogMessage("Fixed foveated rendering not supported on " + GetOculusDeviceFamily());
            }
        #endif
        }

        public static void SetRefreshRate(TBOculusMobileRefreshRate rate)
        {
        #if TB_OCULUS
            if (SupportsMultipleRefreshRates(GetOculusDeviceFamily()))
            {
                switch (rate)
                {
                    case TBOculusMobileRefreshRate.FPS_72:
                        OVRManager.display.displayFrequency = 72.0f;
                        break;
                    default:
                        OVRManager.display.displayFrequency = 60.0f;
                        break;
                }
                TBLogging.LogMessage("Refresh rate is now " + OVRManager.display.displayFrequency);
            }
            else
            {
               // TBLogging.LogMessage("Alternate refresh rates not supported on " + GetOculusDeviceFamily());
            }
        #endif
        }
#endregion

#region OCULUS DEVICE IDENTIFERS
        protected static OculusMobileVersion GetOculusMobileVersion()
        {
            switch (SystemInfo.deviceModel)
            {
                case "Oculus Pacific":
                case "Oculus Go":
                case "Mi v1o":                 // Xiaomi Mi VR uses same settings as Oculus Go.
                    return OculusMobileVersion.Go;
                case "Oculus Santa Cruz":
                case "Oculus Quest":
                case "Oculus Standalone HMD":
                    return OculusMobileVersion.Quest;
            }

            switch (GetGearVRDeviceName())
            {
                case "Galaxy Note 4":
                    return OculusMobileVersion.Galaxy2014;
                case "Galaxy S6":
                case "Galaxy S6 Edge":
                case "Galaxy S6 Edge+":
                case "Galaxy Note 5":
                    return OculusMobileVersion.Galaxy2015;
                case "Galaxy S7 Edge":
                case "Galaxy S7":
                case "Galaxy Note 7 (Exploding Edition)":
                case "Galaxy Note 7 (Fan Edition)":
                    return OculusMobileVersion.Galaxy2016;
                case "Galaxy S8":
                case "Galaxy S8+":
                case "Galaxy Note 8":
                    return OculusMobileVersion.Galaxy2017;
                case "Galaxy S9":
                case "Galaxy S9+":
                case "Galaxy Note 9":
                    return OculusMobileVersion.Galaxy2018;
                default:
                    TBLogging.LogMessage("Couldn't detect Oculus Mobile Version. SystemInfo DeviceModel is " + SystemInfo.deviceModel + ", Device Name: " + SystemInfo.deviceName);
                    return OculusMobileVersion.Galaxy2018;
            }
        }

        public static OculusDeviceFamily GetOculusDeviceFamily()
        {
            if (_devicefamily != OculusDeviceFamily.Unknown)
                return _devicefamily;

            if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
                return OculusDeviceFamily.Rift;
            else
            {
                switch (GetOculusMobileVersion())
                {
                    case OculusMobileVersion.Go:
                        return OculusDeviceFamily.Go;
                    case OculusMobileVersion.Quest:
                        return OculusDeviceFamily.Quest;
                    default:
                        return OculusDeviceFamily.GearVR;
                }
            }
        }

        protected static string GetGearVRDeviceName()
        {
            string deviceName = SystemInfo.deviceName;
            if (deviceName.Contains("SM-N910"))
                return "Galaxy Note 4";
            else if (deviceName.Contains("SM-G920"))
                return "Galaxy S6";
            else if (deviceName.Contains("SM-G925"))
                return "Galaxy S6 Edge";
            else if (deviceName.Contains("SM-G928"))
                return "Galaxy S6 Edge+";
            else if (deviceName.Contains("SM-N920"))
                return "Galaxy Note 5";
            else if (deviceName.Contains("SM-G935"))
                return "Galaxy S7 Edge";
            else if (deviceName.Contains("SM-G930"))
                return "Galaxy S7";
            else if (deviceName.Contains("SM-N930"))
                return "Galaxy Note 7 (Exploding Edition)";
            else if (deviceName.Contains("SM-N935"))
                return "Galaxy Note 7 (Fan Edition)";
            else if (deviceName.Contains("SM-G950"))
                return "Galaxy S8";
            else if (deviceName.Contains("SM-G955"))
                return "Galaxy S8+";
            else if (deviceName.Contains("SM-N950"))
                return "Galaxy Note 8";
            else if (deviceName.Contains("SM-G960"))
                return "Galaxy S9";
            else if (deviceName.Contains("SM-G965"))
                return "Galaxy S9+";
            else if (deviceName.Contains("SM-N960"))
                return "Galaxy Note 9";

            return "Other 2019+ Device";  // assume a newer device
        }
#endregion
       
#region STATIC GETTERS
        public static bool SupportsDynamicResolution(OculusDeviceFamily family)
        {
            switch (family)
            {
                case OculusDeviceFamily.Rift:
                    return true;
                default:
                    return false;
            }
        }

        public static TBCoreSettingsOculus LoadOculusSettings(OculusDeviceFamily family)
        {
            string filename = GetOculusSettingsFilename(family);

            TBCoreSettingsOculus settings = TBDataManager.DeserializeFromFile<TBCoreSettingsOculus>(TBSettings.settingsFolder + filename, TBDataManager.PathType.ResourcesFolder);

            if (settings.displaySettings.initialized)
                return settings;
            else
            {
                TBLogging.LogMessage("No settings were found for " + family +  ". Loading default settings.");
                return GetDefaultOculusSettings(family);
            }
        }

        public static string GetOculusSettingsFilename(OculusDeviceFamily family)
        {
            switch (family)
            {
                case OculusDeviceFamily.GearVR:
                    return settingsFilename + _gearVRFilename;
                case OculusDeviceFamily.Go:
                    return settingsFilename + _goFilename;
                case OculusDeviceFamily.Quest:
                    return settingsFilename + _questFilename;
                default:
                    return settingsFilename + _riftFilename;
            }
        }

        public static Vector2 GetResolutionScalingFactor(OculusDeviceFamily family)
        {
            switch (family)
            {
                case OculusDeviceFamily.Rift:
                    return new Vector2(1080, 1200);
                default:
                    return new Vector2(1024, 1024);
            }
        }

        public static bool SupportsFixedFoveatedRendering(OculusDeviceFamily family)
        {
            switch (family)
            {
                case OculusDeviceFamily.Go:
                case OculusDeviceFamily.Quest:
                    return true;
                default:
                    return false;
            }
        }

        public static bool SupportsMultipleRefreshRates(OculusDeviceFamily family)
        {
            switch (family)
            {
                case OculusDeviceFamily.Go:
                    return true;
                default:
                    return false;
            }
        }

        public static TBCoreSettingsOculus GetRuntimeOculusSettings()
        {
            return _oculusSettings;
        }
#endregion

#region ENUMS AND STRUCTS

        public enum OculusMobileVersion
        {
            Galaxy2014 = 0,
            Galaxy2015 = 1,
            Galaxy2016 = 2,
            Galaxy2017 = 3,
            Galaxy2018 = 4,
            Go = 100,
            Quest = 101,
            Default = 10000
        }

        public enum OculusDeviceFamily
        {
            Unknown =   -1,
            GearVR =    0,
            Go =        50,
            Quest = 100,
            Rift =      1000
        }

        public enum FixedFoveatedRenderingLevel
        {
            None = 0,
            Low = 1,
            Medium = 2,
            High = 3
        }

        public enum TBOculusMobileRefreshRate
        {
            FPS_60 = 60,
            FPS_72 = 72
        }

        [System.Serializable]
        public struct TBCoreSettingsOculus
        {
            public TBSettings.TBDisplaySettings displaySettings;
            public FixedFoveatedRenderingLevel fixedFoveatedRenderingLevel;
            public bool useDynamicResolution;
            public Vector2 dynamicResolutionRange;
            public bool allowViveEmulation;
        }
#endregion
    }
}
