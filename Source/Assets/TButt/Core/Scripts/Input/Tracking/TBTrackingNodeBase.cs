using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt
{
    public class TBTrackingNodeBase : MonoBehaviour
    {
        [HideInInspector]
        public new Transform transform;

        protected bool _subscribedToUpdate;
        protected TBNode _node = TBNode.None;
        protected XRNode _xrNode = XRNode.CenterEye;

        protected bool _overridePosition = false;
        protected Transform _positionTarget;

        protected bool _isTracking;
        protected bool _isWithinBounds;

        [System.Obsolete]
        public virtual void TrackNode(XRNode node)
        {
            TrackNode((TBNode)node);
        }

        public virtual void TrackNode(TBNode node)
        {
            transform = GetComponent<Transform>();
            transform.MakeZeroedChildOf(TBCameraRig.instance.GetTrackingVolume());
            transform.localScale = Vector3.one;
            _node = node;
            _xrNode = (XRNode)node;

            #if UNITY_EDITOR
            switch (node)
            {
                case TBNode.LeftHand:
                    gameObject.name = "LeftHand";
                    break;
                case TBNode.RightHand:
                    gameObject.name = "RightHand";
                    break;
                case TBNode.Gamepad:
                    gameObject.name = "Gamepad";
                    break;
                case TBNode.Controller3DOF:
                    gameObject.name = "3DOFController";
                    break;
                case TBNode.HardwareTracker:
                    gameObject.name = "Tracker";
                    break;
            }
            #endif

            TBTracking.SendNodeConnectedEvents(node, transform);
        }

        public virtual void TogglePositionOverride(bool on, Transform targetTransform = null)
        {
            _overridePosition = on;

            if (on)
            {
                if (targetTransform == null)
                    targetTransform = TBCameraRig.instance.transform;
                _positionTarget = targetTransform;
            }
        }

        protected virtual void OnEnable()
        {
            SubscribeToUpdate(true);
        }

        protected virtual void OnDisable()
        {
            SubscribeToUpdate(false);
        }

        protected virtual void OnUpdate()
        {
            OnUpdatePoses();
            TBTracking.UpdateNodeState(this);
        }

        protected virtual void OnUpdatePoses()
        {
            if (!_overridePosition)
                transform.localPosition = InputTracking.GetLocalPosition(_xrNode);
            else
                transform.position = _positionTarget.position;

            transform.localRotation = InputTracking.GetLocalRotation(_xrNode);
        }

        protected void SubscribeToUpdate(bool on)
        {
            if (on == _subscribedToUpdate)
                return;

            if(on)
                TBCore.OnUpdate += OnUpdate;
            else
                TBCore.OnUpdate -= OnUpdate;

            _subscribedToUpdate = on;
        }

        public TBNode GetNodeType()
        {
            return _node;
        }

        #region TRACKING STATUS

        public bool IsTracking
        {
            get { return tracking; }
        }

        public bool IsWithinBounds
        {
            get { return withinBounds; }
        }

        public void SetWithinBounds(bool isWithinBounds)
        {
            if (withinBounds != isWithinBounds)
            {
                withinBounds = isWithinBounds;

                if(TBTracking.OnNodeWithinBoundsChanged != null)
                {
                    TBTracking.OnNodeWithinBoundsChanged(_node, withinBounds);
                }
            }
        }

        public void SetTracking(bool isTracking)
        {
            if (tracking != isTracking)
            {
                tracking = isTracking;

                if (TBTracking.OnNodeTrackingChanged != null)
                {
                    TBTracking.OnNodeTrackingChanged(_node, tracking);
                }
            }
        }

        private bool tracking;
        private bool withinBounds;

        #endregion
    }
}