using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR || TB_STEAM_VR_2
using Valve.VR;
#endif

namespace TButt
{
    /// <summary>
    /// Steam VR camera extension class.
    /// </summary>
    public class TBSteamVRCamera : TBCameraBase
    {
        #if TB_STEAM_VR || TB_STEAM_VR_2
        protected override void SetTrackingOrigin(TBSettings.TBTrackingOrigin origin)
        {
            base.SetTrackingOrigin(origin);

            switch (origin)
            {
                case TBSettings.TBTrackingOrigin.Eye:
                    #if TB_STEAM_VR
                    SteamVR_Render.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
                    #elif TB_STEAM_VR_2
                    SteamVR_Action_Pose.SetTrackingUniverseOrigin(ETrackingUniverseOrigin.TrackingUniverseSeated);
                    #endif
                    break;
                case TBSettings.TBTrackingOrigin.Floor:
                    #if TB_STEAM_VR
                    SteamVR_Render.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
                    #elif TB_STEAM_VR_2
                    SteamVR_Action_Pose.SetTrackingUniverseOrigin(ETrackingUniverseOrigin.TrackingUniverseStanding);
                    #endif
                    break;
            }
        }
#endif
            }
}