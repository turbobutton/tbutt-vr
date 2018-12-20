using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TButt;
using TButt.Settings;

namespace TButt.Editor
{
    public class TBPSVREditorSDKSettingsPlaceholder : TBEditorSDKSettingsBase
    {
        protected override string _pluginName
        {
            get
            {
                return "Unity PS4";
            }
        }

        protected override string _sdkName
        {
            get
            {
                return "PSVR";
            }
        }

        protected override string _version
        {
            get
            {
                return "1.28.0";
            }
        }

        protected override string _pluginURL
        {
            get
            {
                return "http://www.tbutt.net";
            }
        }

        protected override VRPlatform _platform
        {
            get
            {
                return VRPlatform.PlayStationVR;
            }
        }


        public override void ShowSDKNotFoundMessage()
        {
            EditorGUILayout.HelpBox(_pluginName + " is not supported in this version of TButt.", UnityEditor.MessageType.Error);
        }

        public override void ShowSDKDownloadButton()
        {
            return;
        }


        private static TBPSVREditorSDKSettingsPlaceholder _instance;
        public static TBPSVREditorSDKSettingsPlaceholder instance {
            get
            {
                if (_instance == null)
                    _instance = new TBPSVREditorSDKSettingsPlaceholder();
                return _instance;
            }
        }
    }
}