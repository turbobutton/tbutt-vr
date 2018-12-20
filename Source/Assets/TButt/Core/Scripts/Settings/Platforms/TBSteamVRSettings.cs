using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt.Settings
{
    public class TBSteamVRSettings : TBSettingsBase
    {
        public static readonly string settingsFilename = "Settings_SteamVR";

        public override void Initialize()
        {
            _displaySettings = LoadDisplaySettings(settingsFilename);

#if TB_STEAM_VR
            string trackingSystem = SteamVR.instance.hmd_TrackingSystemName.ToLower();

            if (trackingSystem.Contains("holographic"))
                _headset = VRHeadset.WindowsMR;
            else if (trackingSystem.Contains("oculus"))
            {
                 _headset = VRHeadset.OculusRift;
            }
            else
            {
                _headset = VRHeadset.HTCVive;
            }
#else
            _headset = VRHeadset.HTCVive;
#endif

            base.Initialize();
        }

        public static TBSettings.TBDisplaySettings LoadDisplaySettings(string filename)
        {
            TBSettings.TBDisplaySettings settings = TBDataManager.DeserializeFromFile<TBSettings.TBDisplaySettings>(TBSettings.settingsFolder + filename, TBDataManager.PathType.ResourcesFolder);

            if (settings.initialized)
            {
                TBLogging.LogMessage("Found settings for Steam VR");
                return settings;
            }
            else
            {
                TBLogging.LogMessage("No settings were found for Steam VR. Loading default settings.");
                return GetDefaultDisplaySettings(settings);
            }
        }
    }
}