using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt.Settings;

namespace TButt.Input
{
    public class TBOculusTracking : TBSDKTrackingBase
    {
        protected static TBOculusTracking _instance;

        public static TBOculusTracking instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBOculusTracking();
                return _instance;
            }
        }

        #if TB_OCULUS
        public override void UpdateNodeState(TBTrackingNodeBase node)
        {
            switch(node.GetNodeType())
            {
                case TBNode.LeftHand:
                    node.SetTracking(OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch));
                    node.SetWithinBounds(OVRManager.boundary.TestNode(OVRBoundary.Node.HandLeft, OVRBoundary.BoundaryType.OuterBoundary).IsTriggering);
                    break;
                case TBNode.RightHand:
                    node.SetTracking(OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch));
                    node.SetWithinBounds(OVRManager.boundary.TestNode(OVRBoundary.Node.HandRight, OVRBoundary.BoundaryType.OuterBoundary).IsTriggering);
                    break;
                case TBNode.Head:
                    node.SetTracking(OVRManager.tracker.isPositionTracked);
                    node.SetWithinBounds(OVRManager.boundary.TestNode(OVRBoundary.Node.Head, OVRBoundary.BoundaryType.OuterBoundary).IsTriggering);
                    break;
                default:
                    base.UpdateNodeState(node);
                    break;
            }
        }

        #region BOUNDARY AND PLAYSPACE CHECKS
        public override bool HasUserBoundarySystem()
        {
            return OVRManager.boundary.GetConfigured();
        }

        public override Vector2 GetPlayspaceDimensions()
        {
            Vector3 dimensions = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
            return new Vector2(dimensions.x, dimensions.z);
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

        protected override TBTrackingNodeBase AddNode(TBNode node, GameObject go)
        {
            TBTrackingNodeOculus nodeComponent = go.AddComponent<TBTrackingNodeOculus>();
            nodeComponent.TrackNode(node);
            return nodeComponent;
        }

        #region CONTROLLER TRACKING SUPPORT
        protected override bool SupportsHandControllers()
        {
            switch(TBOculusSettings.GetOculusDeviceFamily())
            {
                case TBOculusSettings.OculusDeviceFamily.Rift:
                case TBOculusSettings.OculusDeviceFamily.Quest:
                case TBOculusSettings.OculusDeviceFamily.Unknown:
                    return true;
                default:
                    return false;
            }
        }

        protected override bool Supports3DOFController()
        {
            switch (TBOculusSettings.GetOculusDeviceFamily())
            {
                case TBOculusSettings.OculusDeviceFamily.GearVR:
                case TBOculusSettings.OculusDeviceFamily.Go:
                    return true;
                case TBOculusSettings.OculusDeviceFamily.Quest:
                    return false;
                default:
                    if(TBCore.UsingEditorMode())
                    {
                        // Only allow 3DOF arm model emulation in the editor.
                        if(TBSettings.GetControlSettings().emulate3DOFArmModel)
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
        }

        public override bool SupportsHMDPositionalTracking()
        {
            switch (TBOculusSettings.GetOculusDeviceFamily())
            {
                case TBOculusSettings.OculusDeviceFamily.Go:
                case TBOculusSettings.OculusDeviceFamily.GearVR:
                    return false;
                default:
                    return true;
            }
        }

        #endregion
        #endif
    }
}