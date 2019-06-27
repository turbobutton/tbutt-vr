using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt.Settings;
#if TB_STEAM_VR_2
using Valve.VR;
#endif

namespace TButt.Input
{
    public class TBSteamVR2Tracking : TBSDKTrackingBase
    {
        protected static TBSteamVR2Tracking _instance;

        public static TBSteamVR2Tracking instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBSteamVR2Tracking();
                return _instance;
            }
        }


        #if TB_STEAM_VR_2
        protected HmdQuad_t playAreaRect;

        public override void UpdateNodeState(TBTrackingNodeBase node)
        {
            ETrackingResult trackingResult;
            switch (node.GetNodeType())
            {
                case TBNode.LeftHand:
                    trackingResult = TBSteamVRActions.tButt_Pose.GetTrackingResult(SteamVR_Input_Sources.LeftHand);
                    break;
                case TBNode.RightHand:
                    trackingResult = TBSteamVRActions.tButt_Pose.GetTrackingResult(SteamVR_Input_Sources.RightHand);
                    break;
                case TBNode.Head:
                    node.SetTracking(TBSteamVRActions.tButt_Pose.GetPoseIsValid(SteamVR_Input_Sources.Head));
                    break;
                default:
                    base.UpdateNodeState(node);
                    break;
            }
        }

        #region BOUNDARY AND PLAYSPACE CHECKS
        public override bool HasUserBoundarySystem()
        {
            if (OpenVR.Chaperone == null)
                return false;
            else
                return (OpenVR.Chaperone.GetCalibrationState() == ChaperoneCalibrationState.OK);
        }

        public override Vector2 GetPlayspaceDimensions()
        {
            if (OpenVR.Chaperone != null)
            {
                if (OpenVR.Chaperone.GetPlayAreaRect(ref playAreaRect))
                {
                    return new Vector2(playAreaRect.vCorners0.v0 * 2, playAreaRect.vCorners0.v2 * 2);
                }
            }

            TBLogging.LogWarning("Failed to get playspace dimensions from Steam VR. Returning Vector2.zero.");
            return Vector2.zero;
        }

        public override bool IsPointWithinPlayspace(Vector3 point)
        {
            return OVRManager.boundary.TestPoint(point, OVRBoundary.BoundaryType.PlayArea).IsTriggering == false;
        }

        public override bool IsPointWithinUserBoundary(Vector3 point)
        {
            return OVRManager.boundary.TestPoint(point, OVRBoundary.BoundaryType.OuterBoundary).IsTriggering == false;
        }
        #endregion

        #region CONTROLLER TRACKING SUPPORT
        protected override bool SupportsHandControllers()
        {
            return true;
        }

        protected override bool SupportsTracker()
        {
            return true;
        }

        protected override bool Supports3DOFController()
        {
            if (TBCore.UsingEditorMode())
            {
                // Only allow 3DOF arm model emulation in the editor.
                if (TBSettings.GetControlSettings().emulate3DOFArmModel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public override bool SupportsHMDPositionalTracking()
        {
            return true;
        }
        #endregion
        #endif
    }
}