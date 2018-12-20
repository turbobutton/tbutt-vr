using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt.Settings
{
    public class TBGoogleSettings : TBSettingsBase
    {
        public static readonly string settingsFilename = "Settings_Google";

        public override void Initialize()
        {
            _displaySettings = LoadDisplaySettings(settingsFilename);
            _headset = VRHeadset.Daydream;

#if TB_GOOGLE
            if (GvrHeadset.SupportsPositionalTracking)
                _headset = VRHeadset.MirageSolo;
#endif
            base.Initialize();
        }

        private static TBSettings.TBDisplaySettings GetDefaultGoogleDisplaySettings(TBSettings.TBDisplaySettings settings)
        {
            settings = GetDefaultDisplaySettings(settings);
            settings.qualityLevel = TBSettings.TBQualityLevel.Low;
            settings.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.NoDistanceSort;
            return settings;
        }

        public static TBSettings.TBDisplaySettings LoadDisplaySettings(string filename)
        {
            TBSettings.TBDisplaySettings settings = TBDataManager.DeserializeFromFile<TBSettings.TBDisplaySettings>(TBSettings.settingsFolder + filename, TBDataManager.PathType.ResourcesFolder);

            if (settings.initialized)
                return settings;
            else
            {
                TBLogging.LogMessage("No settings were found for Google VR. Loading default settings.");
                return GetDefaultGoogleDisplaySettings(settings);
            }
        }
    }
}