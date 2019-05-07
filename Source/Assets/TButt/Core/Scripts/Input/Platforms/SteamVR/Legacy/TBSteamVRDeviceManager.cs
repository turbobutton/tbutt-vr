using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR
using Valve.VR;
#endif

#if TB_STEAM_VR
namespace TButt
{
    public class TBSteamVRDeviceManager : MonoBehaviour
    {

        protected static TBSteamVRDeviceManager _instance;

        public static TBSteamVRDeviceManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<TBSteamVRDeviceManager>();
                    _instance.gameObject.AddComponent<SteamVR_ControllerManager>();
                }
                return _instance;
            }
        }

        public uint GetLeftControllerID()
        {
            return OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
        }
        
        public uint GetRightControllerID()
        {
            return OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
        }
    }
}
#endif