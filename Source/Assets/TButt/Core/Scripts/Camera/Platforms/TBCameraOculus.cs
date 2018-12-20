using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt
{
    /// <summary>
    /// Oculus camera extension class.
    /// </summary>
    public class TBCameraOculus : TBCameraBase
    {
        #if TB_OCULUS
        public override void Initialize()
        {
            gameObject.AddComponent<OVRManager>();
            OVRManager.instance.enableAdaptiveResolution = Settings.TBOculusSettings.GetRuntimeOculusSettings().useDynamicResolution;
            if (Settings.TBOculusSettings.GetRuntimeOculusSettings().useDynamicResolution)
            {
                OVRManager.instance.minRenderScale = Settings.TBOculusSettings.GetRuntimeOculusSettings().dynamicResolutionRange.x;
                OVRManager.instance.maxRenderScale = Settings.TBOculusSettings.GetRuntimeOculusSettings().dynamicResolutionRange.y;
            }
            OVRManager.instance.useRecommendedMSAALevel = false;
            base.Initialize();
        }

        protected override void SetTrackingOrigin(TBSettings.TBTrackingOrigin origin)
        {
            base.SetTrackingOrigin(origin);

            switch (origin)
            {
                case TBSettings.TBTrackingOrigin.Eye:
                    OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
                    break;
                case TBSettings.TBTrackingOrigin.Floor:
                    OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
                    break;
            }
        }

        public override bool HeadsetHasPositionTracking()
        {
            switch(Settings.TBOculusSettings.GetOculusDeviceFamily())
            {
                case Settings.TBOculusSettings.OculusDeviceFamily.GearVR:
                case Settings.TBOculusSettings.OculusDeviceFamily.Go:
                    return false;
                default:
                    return true;
            }
        }
        #endif
    }
}