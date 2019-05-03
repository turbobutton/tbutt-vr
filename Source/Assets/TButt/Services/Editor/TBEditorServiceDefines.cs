using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TButt.Editor
{
    public static class TBEditorServiceDefines
    {
        // Platform Services
        public static readonly string oculusServiceDef = "TB_OCULUS_SERVICE";
        public static readonly string steamServiceDef = "TB_STEAM_SERVICE";
        public static readonly string psnServiceDef = "TB_PSN_SERVICE";
        public static readonly string xboxServiceDef = "TB_XBOX_SERVICE";

        static Services services;

        public static void SetTButtService(VRService service)
        {
            services = GetServicesStruct(service);

            TBEditorDefines.SetPlatformDefine(oculusServiceDef, services.oculus);
            TBEditorDefines.SetPlatformDefine(steamServiceDef, services.steam);
            TBEditorDefines.SetPlatformDefine(xboxServiceDef, services.xbox);
            #if TB_HAS_UNITY_PS4
            TBEditorDefines.SetPlatformDefine(psnServiceDef, services.psn);
            #endif
        }

        public static Services GetServicesStruct(VRService service)
        {
            services = new Services();

            switch (service)
            {
                case VRService.Oculus:
                    services.oculus = true;
                    break;
                case VRService.Steam:
                    services.steam = true;
                    break;
                case VRService.XboxLive:
                    services.xbox = true;
                    break;
                case VRService.PSN:
                    services.psn = true;
                    break;
            }

            return services;
        }

        [System.Serializable]
        public struct Services
        {
            public bool oculus;
            public bool steam;
            public bool psn;
            public bool xbox;
        }
    }
}