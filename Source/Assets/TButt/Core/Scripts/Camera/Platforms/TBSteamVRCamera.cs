using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_STEAM_VR
using Valve.VR;
#endif

namespace TButt
{
    /// <summary>
    /// Steam VR camera extension class.
    /// </summary>
    public class TBSteamVRCamera : TBCameraBase
    {
    #if TB_STEAM_VR
        protected override void SetTrackingOrigin(TBSettings.TBTrackingOrigin origin)
        {
            base.SetTrackingOrigin(origin);

            switch (origin)
            {
                case TBSettings.TBTrackingOrigin.Eye:
                    SteamVR_Render.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
                    break;
                case TBSettings.TBTrackingOrigin.Floor:
                    SteamVR_Render.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
                    break;
            }
        }
    #endif
    }
}