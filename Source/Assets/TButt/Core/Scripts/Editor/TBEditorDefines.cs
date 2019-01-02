using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TButt.Editor
{
    public static class TBEditorDefines
    {
        public static readonly string settingsPath = "Assets/Resources/";   // Location of the TButtSettings folder.

        public static readonly string versionNum = "1.0.1";

        // Misc
        public static string logsDef = "TB_ENABLE_LOGS";

        // SDKs
        public static string steamVRDef = "TB_STEAM_VR";
        public static string oculusDef = "TB_OCULUS";
        public static string googleDef = "TB_GOOGLE";
        public static string psvrDef = "TB_PSVR";
        public static string windowsDef = "TB_WINDOWS_MR";

        // Platform Services
        public static readonly string oculusServiceDef = "TB_OCULUS_SERVICE";
        public static readonly string steamServiceDef = "TB_STEAM_SERVICE";
        public static readonly string psnServiceDef = "TB_PSN_SERVICE";
        public static readonly string xboxServiceDef = "TB_XBOX_SERVICE";

        static string buildDefString = "";

        public static void SetScriptingDefines()
        {
            if (buildDefString.StartsWith(";"))
                buildDefString = buildDefString.Remove(0, 1);

            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, buildDefString);
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.PS4, buildDefString);
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, buildDefString);
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WSA, buildDefString);
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, buildDefString);
        }

        public static void SetPlatformDefine(string platform, bool on)
        {
            if(string.IsNullOrEmpty(buildDefString))
                buildDefString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);

            if (on)
            {
                if (!buildDefString.Contains(platform))
                {
                    TBLogging.LogMessage("Adding " + platform);
                    buildDefString += ";" + platform;
                }
            }
            else
            {
                if (buildDefString.Contains(platform))
                {
                    TBLogging.LogMessage("Removing " + platform);
                    buildDefString = buildDefString.Remove(buildDefString.IndexOf(platform), platform.Length);
                }
            }
        }

        public static void SetUnityVirtualRealitySDKs(TBEditorSDKSettings.SDKs sdks)
        {
            #region PC / STANDALONE CHECKS
            string[] currentTargets = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone);
            string[] wantedTargets = new string[] { "None" };
            // Kind of ugly, but the build platforms have to be set in a particular order for things to work properly.
            // Assume that if Oculus and Steam are both enabled, Oculus should be first in the list.
            if (sdks.oculus && sdks.steamVR)
                wantedTargets = new string[] { TBSettings.VRDeviceNames.Oculus, TBSettings.VRDeviceNames.SteamVR };
            else if (sdks.oculus)
            {
                if(Settings.TBOculusSettings.LoadOculusSettings(Settings.TBOculusSettings.OculusDeviceFamily.Rift).allowViveEmulation)
                    wantedTargets = new string[] { TBSettings.VRDeviceNames.Oculus, TBSettings.VRDeviceNames.SteamVR };
                else
                    wantedTargets = new string[] { TBSettings.VRDeviceNames.Oculus };
            }
            else if (sdks.steamVR)
                wantedTargets = new string[] { TBSettings.VRDeviceNames.SteamVR };
            SetPlayerSettingsSDKs(BuildTargetGroup.Standalone, wantedTargets, currentTargets);

            currentTargets = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.WSA);
            wantedTargets = new string[] { "None" };
            if (sdks.windows)
                wantedTargets = new string[] { TBSettings.VRDeviceNames.WindowsMR };
            SetPlayerSettingsSDKs(BuildTargetGroup.WSA, wantedTargets, currentTargets);
            #endregion

            #region ANDROID CHECKS
            currentTargets = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Android);
            wantedTargets = new string[] { "None" };
            if (sdks.oculus && sdks.googleVR)
                wantedTargets = new string[2] { TBSettings.VRDeviceNames.Oculus, TBSettings.VRDeviceNames.Daydream };
            else if (sdks.oculus)
                wantedTargets = new string[1] { TBSettings.VRDeviceNames.Oculus };
            else if (sdks.googleVR)
                wantedTargets = new string[1] { TBSettings.VRDeviceNames.Daydream };
            SetPlayerSettingsSDKs(BuildTargetGroup.Android, wantedTargets, currentTargets);
            #endregion
        }

        public static void SetTButtSDKForPlatform(TButt.VRPlatform platform)
        {
            switch(platform)
            {
                case VRPlatform.OculusPC:
                case VRPlatform.OculusMobile:
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, new string[] { TBSettings.VRDeviceNames.Oculus });
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new string[] { TBSettings.VRDeviceNames.Oculus });
                    TBEditorDefines.SetPlatformDefine(steamVRDef, false);
                    TBEditorDefines.SetPlatformDefine(oculusDef, true);
                    break;
                case VRPlatform.SteamVR:
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, new string[] { TBSettings.VRDeviceNames.SteamVR });
                    TBEditorDefines.SetPlatformDefine(steamVRDef, true);
                    TBEditorDefines.SetPlatformDefine(oculusDef, false);
                    break;
            }
        }

        static void SetPlayerSettingsSDKs(BuildTargetGroup group, string[] wantedSDKs, string[] targetSDKs)
        {
            bool needRefresh = false;

            if (wantedSDKs.Length != targetSDKs.Length)
            {
                needRefresh = true;
            }
            else
            {
                for (int i = 0; i < wantedSDKs.Length; i++)
                {
                    if (wantedSDKs[i] != targetSDKs[i])
                    {
                        needRefresh = true;
                        break;
                    }
                }
            }

            if (needRefresh)
            {
                string buildMessage = "TButt updated Unity XR Settings to match TButt settings...\n - Platform: " + group + "\n - Old SDKs:";

                for (int i = 0; i < targetSDKs.Length; i++)
                {
                    buildMessage += " " + targetSDKs[i];
                }
                buildMessage += "\n - New SDKs";
                for (int i = 0; i < wantedSDKs.Length; i++)
                {
                    buildMessage += " " + wantedSDKs[i];
                }

                Debug.Log(buildMessage);
                PlayerSettings.SetVirtualRealitySDKs(group, wantedSDKs);
            }
        }
    }
}