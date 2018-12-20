using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace TButt
{
    public class TBTrackingNodeBase : MonoBehaviour
    {
        [HideInInspector]
        public new Transform transform;

        protected bool _subscribedToUpdate;
        protected UnityEngine.XR.XRNode _node = UnityEngine.XR.XRNode.CenterEye;

        protected bool _overridePosition = false;
        protected Transform _positionTarget;

        public virtual void TrackNode(UnityEngine.XR.XRNode node)
        {
            transform = GetComponent<Transform>();
            transform.MakeZeroedChildOf(TBCameraRig.instance.GetTrackingVolume());
			transform.localScale = Vector3.one;
            _node = node;

#if UNITY_EDITOR
            switch(node)
            {
                case UnityEngine.XR.XRNode.LeftHand:
                    gameObject.name = "LeftHand";
                    break;
                case UnityEngine.XR.XRNode.RightHand:
                    gameObject.name = "RightHand";
                    break;
                case UnityEngine.XR.XRNode.GameController:
                    gameObject.name = "GameController";
                    break;
            }
#endif

            if (TBTracking.OnNodeConnected != null)
                TBTracking.OnNodeConnected(node, transform);
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

        protected virtual void OnUpdatePoses()
        {
            if (!_overridePosition)
                transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(_node);
            else
                transform.position = _positionTarget.position;

            transform.localRotation = UnityEngine.XR.InputTracking.GetLocalRotation(_node);
        }

        protected void SubscribeToUpdate(bool on)
        {
            if (on == _subscribedToUpdate)
                return;

            if(on)
                TBCore.OnUpdate += OnUpdatePoses;
            else
                TBCore.OnUpdate -= OnUpdatePoses;

            _subscribedToUpdate = on;
        }
    }
}