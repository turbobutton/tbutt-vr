using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace TButt
{
    public static class TBTracking
    {
        private static Dictionary<UnityEngine.XR.XRNode, Transform> _nodes;

        public delegate void TrackingEvent(UnityEngine.XR.XRNode node, Transform nodeTransform);
        public static TrackingEvent OnNodeConnected;

        public static void Initialize()
        {
            if (_nodes != null)
                _nodes.Clear();

            // Add tracked controller nodes under the camera rig if we need them.
            if (TBSettings.GetControlSettings().supportsHandControllers)
            {
                switch (TBCore.GetActivePlatform())
                {
                    case VRPlatform.OculusPC:
                    case VRPlatform.SteamVR:
                    case VRPlatform.PlayStationVR:
                    case VRPlatform.WindowsMR:
                        AddTrackedDeviceForNode(UnityEngine.XR.XRNode.LeftHand);
                        AddTrackedDeviceForNode(UnityEngine.XR.XRNode.RightHand);
                        break;
                    case VRPlatform.OculusMobile:
                        if (TBInput.GetControllerModel(TBInput.Controller.RHandController) != VRController.None)
                        {
                            AddTrackedDeviceForNode(UnityEngine.XR.XRNode.LeftHand);
                            AddTrackedDeviceForNode(UnityEngine.XR.XRNode.RightHand);
                        }
                        break;
                    case VRPlatform.Daydream:
                        if (TBInput.GetControllerModel(TBInput.Controller.RHandController) != VRController.None)
                        {
                            AddTrackedDeviceForNode(UnityEngine.XR.XRNode.LeftHand);
                            AddTrackedDeviceForNode(UnityEngine.XR.XRNode.RightHand);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (TBSettings.GetControlSettings().supports3DOFControllers)
            {
                 if (TBInput.GetControllerModel(TBInput.Controller.Mobile3DOFController) != VRController.None)
                 {
                    TBCameraRig.instance.GetTrackingVolume().gameObject.AddComponent<TB3DOFArmModel>().Initialize();
                    AddTrackedDeviceForNode(UnityEngine.XR.XRNode.GameController);
                 }
            }

            if (TBSettings.GetControlSettings().supportsGamepad)
            {
                switch (TBCore.GetActivePlatform())
                {
                    case VRPlatform.PlayStationVR:
                        AddTrackedDeviceForNode(UnityEngine.XR.XRNode.GameController);
                        break;
                    default:
                        break;
                }
            }

        }

        public static void AddTrackedDeviceForNode(UnityEngine.XR.XRNode node)
        {
            if (_nodes == null)
                _nodes = new Dictionary<UnityEngine.XR.XRNode, Transform>();

            GameObject newNode = new GameObject();

            _nodes.Add(node, newNode.transform);

            // Handle 3DOF nodes separately, since it relies on the arm model. Skip and assume gamepad if PSVR.
            if (node == UnityEngine.XR.XRNode.GameController)
            {
                switch (TBCore.GetActivePlatform())
                {
                    case VRPlatform.PlayStationVR:
                        break;
                    default:
                        newNode.AddComponent<TBTrackingNode3DOF>().TrackNode(node);
                        return;
                }
            }

            switch (TBCore.GetActivePlatform())
            {
                case VRPlatform.OculusMobile:
                case VRPlatform.OculusPC:
                    newNode.AddComponent<TBTrackingNodeOculus>().TrackNode(node);
                    break;
                case VRPlatform.SteamVR:
                case VRPlatform.WindowsMR:
                    newNode.AddComponent<TBTrackingNodeBase>().TrackNode(node);
                    break;
                case VRPlatform.Daydream:
                    newNode.AddComponent<TBTrackingNodeGoogle>().TrackNode(node);
                    break;
                #if TB_HAS_UNITY_PS4
                case VRPlatform.PlayStationVR:
                    newNode.AddComponent<TBTrackingNodePSVR>().TrackNode(node);
                    break;
                #endif
                default:
                    Debug.LogError("TButt doesn't support tracked device nodes on this platform yet!");
                    break;
            }
        }

        public static Transform GetTransformForNode(UnityEngine.XR.XRNode node)
        {
            Transform t;
            switch (node)
            {
                case UnityEngine.XR.XRNode.Head:
                case UnityEngine.XR.XRNode.CenterEye:
                   // if (TBCameraRig.instance == null)
                 //       return null;
                    return TBCameraRig.instance.GetCenter();
                case UnityEngine.XR.XRNode.TrackingReference:
                   // if (TBCameraRig.instance == null)
               //         return null;
                    return TBCameraRig.instance.GetTrackingVolume();
                case UnityEngine.XR.XRNode.GameController:
                    if (TB3DOFArmModel.instance != null)
                        return TB3DOFArmModel.instance.GetHandTransform();
                    else
                    {
                        if (_nodes.TryGetValue(node, out t))
                            return t;
                        else
                            return null;
                    }
                default:
                    if (_nodes == null)
                        _nodes = new Dictionary<UnityEngine.XR.XRNode, Transform>();

                    if (_nodes.TryGetValue(node, out t))
                        return t;
                    else
                    {
                        Debug.LogWarning("No node was found for " + node + ". Is the corresponding controller type enabled in TBInput's settings?");
                        return null;
                    }
            }
        }
    }

    public interface ITBTracking
    {
        void AddNodeForControlType(TBInput.ControlType controlType);

        bool Supports6DOFForHMD();

        bool Supports6DOFForControlType(TBInput.ControlType controlType);

        bool Supports3DOFForControlType(TBInput.ControlType controlType);

        int GetControllerCount(TBInput.ControlType controlType);

    }
}