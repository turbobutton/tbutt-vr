using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt.Settings;

namespace TButt.Input
{
    public class TBGoogleTracking : TBSDKTrackingBase
    {
        protected static TBGoogleTracking _instance;

        public static TBGoogleTracking instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBGoogleTracking();
                return _instance;
            }
        }

        protected override TBTrackingNodeBase AddNode(TBNode node, GameObject go)
        {
            TBTrackingNodeGoogle nodeComponent = go.AddComponent<TBTrackingNodeGoogle>();
            return nodeComponent;
        }

        #region CONTROLLER TRACKING SUPPORT
        protected override bool SupportsHandControllers()
        {
            #if TB_GOOGLE
            return GvrControllerInput.GetDevice(GvrControllerHand.Dominant).SupportsPositionalTracking;
            #else
            return false;
            #endif
        }

        protected override bool Supports3DOFController()
        {
            return true;
        }

        public override bool SupportsHMDPositionalTracking()
        {
            #if TB_GOOGLE
            return GvrHeadset.SupportsPositionalTracking;
            #else
            return false;
            #endif
        }

#endregion
    }
}