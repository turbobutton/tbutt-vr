using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using TButt;
using System.Linq;

namespace TButt.Settings
{
    public class TBSettingsBase : MonoBehaviour
    {
        protected TBSettings.TBDisplaySettings _displaySettings;
        protected VRHeadset _headset = VRHeadset.None;

        public virtual void Initialize()
        {
            SetRenderscale(GetConfiguredRenderscale());
            PrintStartupResults();
        }

        public virtual void SetRenderscale(float newScale)
        {
            TBLogging.LogMessage("Setting renderscale (eyeTextureResolutionScale) to " + newScale + "...");
            if (UnityEngine.XR.XRSettings.eyeTextureResolutionScale == newScale)
            {
                TBLogging.LogMessage("Unity renderscale (eyeTextureResolutionScale) was already " + UnityEngine.XR.XRSettings.eyeTextureResolutionScale);
                return;
            }
            else
            {
                UnityEngine.XR.XRSettings.eyeTextureResolutionScale = newScale;
                TBLogging.LogMessage("Unity renderscale (eyeTextureResolutionScale) is now " + UnityEngine.XR.XRSettings.eyeTextureResolutionScale);
            }
        }

        protected virtual void SetRefreshRate()
        {
            TBLogging.LogMessage("Custom refresh rate assignment not available (or not implemented) for " + TBCore.GetActivePlatform() + ".");
        }

        public static TBSettings.TBDisplaySettings GetDefaultDisplaySettings(TBSettings.TBDisplaySettings defaultDisplaySettings)
        {
            defaultDisplaySettings.renderscale = 1.0f;
            defaultDisplaySettings.targetTimestep = TBSettings.TBTimestep.Locked;
            defaultDisplaySettings.maxTimestep = TBSettings.TBTimestep.Half;
            defaultDisplaySettings.qualityLevel = TBSettings.TBQualityLevel.High;
            defaultDisplaySettings.mirrorDisplay = true;
            defaultDisplaySettings.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.FrontToBack;
            defaultDisplaySettings.depthTextureMode = DepthTextureMode.None;
            defaultDisplaySettings.initialized = true;
            return defaultDisplaySettings;
        }

        public static TBSettings.TBQualitySettings GetDefaultQualitySettings(TBSettings.TBQualitySettings qualitySettings)
        {
            qualitySettings.textureQuality = TBSettings.TBTextureSize.FullRes;
            qualitySettings.antialiasingLevel = TBSettings.TBAntialiasingLevel._4x;
            qualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            qualitySettings.pixelLighCount = 10;
            qualitySettings.softParticles = true;
            qualitySettings.shadowQuality = ShadowQuality.All;
            qualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            qualitySettings.LODbias = 2;
            qualitySettings.maximumLODLevel = 0;
            qualitySettings.shadowProjection = ShadowProjection.CloseFit;
            qualitySettings.shadowDistance = 100;
            qualitySettings.blendWeights = BlendWeights.FourBones;
            return qualitySettings;
        }

        public TBSettings.TBDisplaySettings GetDisplaySettings()
        {
            return _displaySettings;
        }
       
        /// <summary>
        /// If the device supports multiple refresh rates, this will get whatever the TButt configured rate is. Otherwise it returns the Unity refresh rate.
        /// </summary>
        /// <returns></returns>
        public virtual float GetRefreshRate()
        {
            if (SupportsMultipleRefreshRates())
                return (int)_displaySettings.refreshRate;
            else
                return (int)UnityEngine.XR.XRDevice.refreshRate;
        }

        public virtual TBSettings.TBQualityLevel GetConfiguredQualityLevel()
        {
            return _displaySettings.qualityLevel;
        } 

        public virtual float GetConfiguredRenderscale()
        {
            return _displaySettings.renderscale;
        }

        public virtual VRHeadset GetActiveHeadset()
        {
            return _headset;
        }

        /// <summary>
        /// True if the device supports multiple refresh rates (Oculus Go, PSVR, etc)
        /// </summary>
        /// <returns></returns>
        public virtual bool SupportsMultipleRefreshRates()
        {
            return false;
        }

        [System.Diagnostics.Conditional("TB_ENABLE_LOGS")]
        protected virtual void PrintStartupResults()
        {
            TBLogging.LogMessage("TButt settings initialized. Diagnostics Report (click to view) \n" +
                "------------------------------------------------------------------------------------- \n" +
                "SystemInfo DeviceModel: " + SystemInfo.deviceModel + "\n" +
                "Device Name: " + SystemInfo.deviceName + "\n" +
                "Unity XR Device: " + UnityEngine.XR.XRSettings.loadedDeviceName + "\n" +
                "Unity XR Device Model: " + UnityEngine.XR.XRDevice.model + "\n" +
                "TButt Platform: " + TBCore.GetActivePlatform() + "\n" +
                "TButt HMD: " + TBCore.GetActiveHeadset() + "\n" + 
                "Configured Renderscale: " + _displaySettings.renderscale +  ", Actual Renderscale: " + UnityEngine.XR.XRSettings.eyeTextureResolutionScale + "\n" +
                "Configured Refresh Rate: " + _displaySettings.refreshRate + ", Actual Refresh Rate: " + UnityEngine.XR.XRDevice.refreshRate + "\n" +
                "Configured TBQuality Level: " + _displaySettings.qualityLevel);
        }
    }
}