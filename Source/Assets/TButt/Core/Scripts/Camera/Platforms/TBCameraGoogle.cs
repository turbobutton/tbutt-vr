using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt
{
    /// <summary>
    /// Class that manages camera for native integration. Uses events to stay in sync with settings given to TBCameraRig.
    /// </summary>
    public class TBCameraGoogle : TBCameraBase
    {
#if TB_GOOGLE
        public override bool HeadsetHasPositionTracking()
        {
            return GvrHeadset.SupportsPositionalTracking;
        }
#endif
    }
}