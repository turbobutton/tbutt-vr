using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt.Input
{
    public interface ITBSDKTracking
    {
        // Functionality
        void Initialize();

        // Node Operations
        TBTrackingNodeBase CreateNode(TBNode node);

        bool SupportsNode(TBNode node);

        bool SupportsHMDPositionalTracking();

        bool HasPositionalTrackingForNode(TBNode node);

        void UpdateNodeState(TBTrackingNodeBase node);

        bool HasUserBoundarySystem();

        Vector2 GetPlayspaceDimensions();

        bool IsPointWithinPlayspace(Vector3 point);

        bool IsPointWithinUserBoundary(Vector3 point);
    }

    public class TBSDKTrackingBase : ITBSDKTracking
    {
        private bool _initialized = false;

        private List<XRNodeState> _nodeStates = new List<XRNodeState>();

        public virtual void Initialize()
        {
            if(!_initialized)
            {
                TBCore.OnUpdate += Update;
            }
        }

        public virtual TBTrackingNodeBase CreateNode(TBNode node)
        {
            if (!SupportsNode(node))
                return null;

            return AddNode(node, new GameObject());
        }

        public virtual bool SupportsNode(TBNode node)
        {
            switch(node)
            {
                case TBNode.LeftHand:
                case TBNode.RightHand:
                    return SupportsHandControllers();
                case TBNode.Head:
                    return true;
                case TBNode.Controller3DOF:
                    return Supports3DOFController();
                case TBNode.Gamepad:
                    return SupportsGamepad();
                case TBNode.None:
                    return false;
                case TBNode.HardwareTracker:
                    return SupportsTracker();
                case TBNode.TrackingVolume:
                    return true;
                default:
                    return false;
            }
        }

        public virtual bool HasPositionalTrackingForNode(TBNode node)
        {
            if (node == TBNode.Head)
                return SupportsHMDPositionalTracking();
            else
                return SupportsNode(node);
        }

        public virtual bool SupportsHMDPositionalTracking()
        {
            return true;
        }
 
        protected virtual void Update()
        {
            UpdateNodeStates();
        }

        protected virtual void UpdateNodeStates()
        {
            InputTracking.GetNodeStates(_nodeStates);
        }

        public virtual void UpdateNodeState(TBTrackingNodeBase node)
        {
            int count = _nodeStates.Count;
            XRNode xrNode = (XRNode)node.GetNodeType();

            for (int i = 0; i < count; i++)
            {
                if(_nodeStates[i].nodeType == xrNode)
                {
                    node.SetTracking(_nodeStates[i].tracked);
                    node.SetWithinBounds(true);
                    break;
                }
            }
        }

        public virtual Vector2 GetPlayspaceDimensions()
        {
            return Vector2.zero;
        }

        protected virtual TBTrackingNodeBase AddNode(TBNode node, GameObject go)
        {
            TBTrackingNodeBase nodeComponent = go.AddComponent<TBTrackingNodeBase>();
            return nodeComponent;
        }

        public virtual bool HasUserBoundarySystem()
        {
            return false;
        }

        public virtual bool IsPointWithinPlayspace(Vector3 point)
        {
            return true;
        }

        public virtual bool IsPointWithinUserBoundary(Vector3 point)
        {
            return true;
        }

        protected virtual bool SupportsHandControllers()
        {
            return false;
        }

        protected virtual bool Supports3DOFController()
        {
            return false;
        }

        protected virtual bool SupportsGamepad()
        {
            return false;
        }

        protected virtual bool SupportsTracker()
        {
            return false;
        }
    }
}