using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using TButt.Input;

namespace TButt
{
    public static class TBTracking
    {
        private static Dictionary<TBNode, TBTrackingNodeBase> _nodes;
        private static Dictionary<VRPlatform, ITBSDKTracking> _trackingSDKs;
        private static ITBSDKTracking _activeSDK;

        private static Bounds _activeBounds;

        [System.Obsolete("Use TrackingNodeEvent with TBNode instead of Unity XR nodes.")]
        public delegate void TrackingEvent(UnityEngine.XR.XRNode node, Transform nodeTransform);
        public delegate void NodeCreationEvent(TBNode node, Transform nodeTransform);
        public delegate void NodeTrackingEvent(TBNode node, bool isTracking);

        [System.Obsolete("Use OnTrackingNodeConnected with TBNode instead of Unity XR nodes.")]
        public static TrackingEvent OnNodeConnected;
        public static NodeCreationEvent OnNodeCreated;

        /// <summary>
        /// bool is false when tracking is lost, true when tracking is regained
        /// </summary>
        public static NodeTrackingEvent OnNodeTrackingChanged;
        public static NodeTrackingEvent OnNodeWithinBoundsChanged;

        [System.Obsolete]
        public static void Initialize()
        {
            Initialize(TBCore.GetActivePlatform());
        }

        public static void Initialize(VRPlatform platform)
        {
            if (_nodes != null)
                _nodes.Clear();

            if(_activeBounds != null)
                _activeBounds = new Bounds();

            _activeSDK = GetTrackingSDK(platform);

            // Add tracked controller nodes under the camera rig if we need them.
            if (TBSettings.GetControlSettings().supportsHandControllers)
            {
                AddTrackedDeviceForNode(TBNode.RightHand);
                AddTrackedDeviceForNode(TBNode.LeftHand);
            }

            if (TBSettings.GetControlSettings().supports3DOFControllers)
            {
                AddTrackedDeviceForNode(TBNode.Controller3DOF);

                if(_activeSDK.SupportsNode(TBNode.Controller3DOF))
                    TBCameraRig.instance.GetTrackingVolume().gameObject.AddComponent<TB3DOFArmModel>().Initialize();
            }

            if (TBSettings.GetControlSettings().supportsGamepad)
            {
                AddTrackedDeviceForNode(TBNode.Gamepad);
            }
        }

        public static void AddTrackedDeviceForNode(TBNode node)
        {
            if (_nodes == null)
                _nodes = new Dictionary<TBNode, TBTrackingNodeBase>();

            TBTrackingNodeBase newNode = _activeSDK.CreateNode(node);

            if(newNode != null)
            {
                _nodes.Add(node, newNode);
                TBLogging.LogMessage("Added node for " + node);
            }
            else
            {
                TBLogging.LogMessage("Attempted to add node for " + node + " but it is not supported on the current platform.");
            }
        }

        [System.Obsolete]
        public static void AddTrackedDeviceForNode(UnityEngine.XR.XRNode node)
        {
            switch(node)
            {
                case UnityEngine.XR.XRNode.LeftHand:
                    AddTrackedDeviceForNode(TBNode.LeftHand);
                    break;
                case UnityEngine.XR.XRNode.RightHand:
                    AddTrackedDeviceForNode(TBNode.RightHand);
                    break;
                case UnityEngine.XR.XRNode.GameController:
                    AddTrackedDeviceForNode(TBNode.Gamepad);
                    AddTrackedDeviceForNode(TBNode.Controller3DOF);
                    break;
            }
        }

        public static TBTrackingNodeBase GetNode(TBNode node)
        {
            TBTrackingNodeBase trackingNode;
            if (_nodes.TryGetValue(node, out trackingNode))
                return trackingNode;
            else
                return null;
        }

        public static Transform GetTransformForNode(TBNode node)
        {
            TBTrackingNodeBase trackingNode;
            switch (node)
            {
                case TBNode.Head:
                    return TBCameraRig.instance.GetCenter();
                case TBNode.TrackingVolume:
                    return TBCameraRig.instance.GetTrackingVolume();
                case TBNode.Controller3DOF:
                    if (TB3DOFArmModel.instance != null)
                        return TB3DOFArmModel.instance.GetHandTransform();
                    else
                    {
                        if (_nodes.TryGetValue(node, out trackingNode))
                            return trackingNode.transform;
                        else
                            return null;
                    }
                default:
                    if (_nodes == null)
                        _nodes = new Dictionary<TBNode, TBTrackingNodeBase>();

                    if (_nodes.TryGetValue(node, out trackingNode))
                        return trackingNode.transform;
                    else
                    {
                        Debug.LogWarning("No node was found for " + node + ". Is the corresponding controller type enabled in TBInput's settings?");
                        return null;
                    }
            }
        }

        public static void UpdateNodeState(TBTrackingNodeBase node)
        {
            _activeSDK.UpdateNodeState(node);
        }

        [System.Obsolete]
        public static Transform GetTransformForNode(UnityEngine.XR.XRNode node)
        {
            return GetTransformForNode((TBNode)node);
        }

        public static void SendNodeConnectedEvents(TBNode node, Transform t)
        {
            if (OnNodeCreated != null)
                OnNodeCreated(node, t);

            #pragma warning disable 618
            if (OnNodeConnected != null)
                OnNodeConnected((UnityEngine.XR.XRNode)node, t);
            #pragma warning restore 618
        }

        public static bool HasPositionalTrackingForNode(TBNode node)
        {
            if (_activeSDK == null)
                return GetTrackingSDK(TBCore.GetActivePlatform()).HasPositionalTrackingForNode(node);
            else
                return _activeSDK.HasPositionalTrackingForNode(node);
        }

        public static bool HasPlayspaceBounds()
        {
            return _activeSDK.GetPlayspaceDimensions() != Vector2.zero;
        }

        public static bool IsPointWithinPlayspaceBounds(Vector3 point)
        {
            _activeBounds.center = GetTransformForNode(TBNode.TrackingVolume).position;
            point.y = _activeBounds.center.y;
            return _activeBounds.Contains(point);
        }

        static void RefreshBounds()
        {
            Vector2 startingDimensions = _activeSDK.GetPlayspaceDimensions();
            _activeBounds.size = new Vector3(startingDimensions.x, 1f, startingDimensions.y);
            _activeBounds.center = GetTransformForNode(TBNode.TrackingVolume).position;
        }

        static ITBSDKTracking GetTrackingSDK(VRPlatform platform)
        {
            switch (platform)
            {
                case VRPlatform.OculusPC:
                case VRPlatform.OculusMobile:
                    return TBOculusTracking.instance;
                case VRPlatform.SteamVR:
                    #if TB_STEAM_VR_2
                    return TBSteamVR2Tracking.instance;
                    #else
                    return TBSteamVRTracking.instance;
                    #endif
                case VRPlatform.Daydream:
                    return TBGoogleTracking.instance;
                case VRPlatform.PlayStationVR:
                    #if TB_HAS_UNITY_PS4
                    return TBPSVRTracking.instance;
                    #else
                    UnityEngine.Debug.LogError("TBInput attempted to initialize for PSVR, but the PSVR module is not available. Is the module installed and set up with #TB_HAS_UNITY_PS4?");
                    #endif
                case VRPlatform.WindowsMR:
                    return TBWindowsMRTracking.instance;
                default:
                    UnityEngine.Debug.LogError("Attempted to initialize TBInput without an active SDK in TBCore. This shouldn't happen if TBCore exists in your scene.");
                    return null;
            }
        }
    }

    [System.Serializable]
    public enum TBNode
    {
        None            = 0,
        Head            = 3,
        HardwareTracker = 8,
        TrackingVolume  = 7,
        Controller3DOF  = 6,
        Gamepad         = 10,
        LeftHand        = 4,
        RightHand       = 5
    }
}