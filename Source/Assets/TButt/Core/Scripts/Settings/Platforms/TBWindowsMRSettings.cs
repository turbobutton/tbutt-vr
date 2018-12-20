using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt.Settings
{
    public class TBWindowsMRSettings : TBSettingsBase
    {
        public static readonly string settingsFilename = "Settings_WindowsMR_";
        private readonly static string _baseFilename = "Base";
        private readonly static string _ultraFilename = "Ultra";

        public override void Initialize()
        {
            _displaySettings = LoadDisplaySettings(GetWindowsMRSettingsFilename(GetWindowsMixedRealityLevel()));
            _headset = VRHeadset.WindowsMR;
            base.Initialize();
        }

        public static TBSettings.TBDisplaySettings LoadDisplaySettings(string filename)
        {
            TBSettings.TBDisplaySettings settings = TBDataManager.DeserializeFromFile<TBSettings.TBDisplaySettings>(TBSettings.settingsFolder + filename, TBDataManager.PathType.ResourcesFolder);

            if (settings.initialized)
                return settings;
            else
            {
                TBLogging.LogMessage("No settings were found for Windows Mixed Reality. Loading default settings.");
                return GetDefaultDisplaySettings(settings);
            }
        }

        public static TBSettings.TBDisplaySettings GetDefaultDisplaySettings(TBWindowsMixedRealityLevel level)
        {
            TBSettings.TBDisplaySettings settings = GetDefaultDisplaySettings(new TBSettings.TBDisplaySettings());

            if (level == TBWindowsMixedRealityLevel.Base)
            {
                settings.qualityLevel = TBSettings.TBQualityLevel.Low;
            }

            return settings;
        }

        public static TBSettings.TBDisplaySettings LoadWindowsMRSettings(TBWindowsMixedRealityLevel level)
        {
            string filename = GetWindowsMRSettingsFilename(level);

            TBSettings.TBDisplaySettings settings = TBDataManager.DeserializeFromFile<TBSettings.TBDisplaySettings>(TBSettings.settingsFolder + filename, TBDataManager.PathType.ResourcesFolder);

            if (settings.initialized)
                return settings;
            else
            {
                TBLogging.LogMessage("No settings were found for " + level + ". Loading default settings.");
                return GetDefaultDisplaySettings(level);
            }
        }

        public static string GetWindowsMRSettingsFilename(TBWindowsMixedRealityLevel level)
        {
            switch (level)
            {
                case TBWindowsMixedRealityLevel.Ultra:
                    return settingsFilename + _ultraFilename;
                default:
                    return settingsFilename + _baseFilename;
            }
        }

        public TBWindowsMixedRealityLevel GetWindowsMixedRealityLevel()
        {
            if (GetRefreshRate() < 90)
            {
                return TBWindowsMixedRealityLevel.Base;
            }
            else
            {
                return TBWindowsMixedRealityLevel.Ultra;
            }
        }

        public enum TBWindowsMixedRealityLevel
        {
            Base    =   0,  // Intel Graphics 620 or better
            Ultra   =   1   // GeForce 1060 or better
        }
    }
}