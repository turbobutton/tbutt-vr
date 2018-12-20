using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TButt
{
    public class TBCameraRig : MonoBehaviour
    {
        public static TBCameraRig instance;

        /////////////////////////////////////////////////////////////////////////////////////
        // Camera settings. These get applied down to whatever cameras are used by the SDK.
        /////////////////////////////////////////////////////////////////////////////////////
        [HideInInspector]
        private CameraClearFlags _clearFlags;
        [HideInInspector]
        private LayerMask _cullingMask;
        [HideInInspector]
        private Color _backgroundColor;
        [HideInInspector]
        private float _nearClipPlane = 1;
        [HideInInspector]
        private float _farClipPlane = 250;
        [HideInInspector]
        private bool _useOcclusionCulling;
        [HideInInspector]
        private float _depth = -1;
        [HideInInspector]
        private float _stereoConvergence = 0;
        [HideInInspector]
        private float _stereoSeparation = 0;
        [HideInInspector]
        private OpaqueSortMode _sortMode = OpaqueSortMode.Default;

        private CameraMode _cameraMode;
        private float _startingScale;

        private Camera _primaryCamera;

        private Transform _centerEyeTransform;
        private Transform _leftEyeTransform;
        private Transform _rightEyeTransform;

        private TBCameraBase _baseCamera;

        private Transform _trackingVolume;
        private Transform _audioListenerTransform;	// Workaround for OVR Audio camera scale bug
        private AudioListener _audioListener;

        private bool _useBlackoutSphere = false;     // TODO: Put this in DisplaySettings. 
        private bool _initialized = false;
        private Transform _blackoutSphere;

        public virtual void Initialize()
        {
            if (_initialized)
                return;

            instance = this;
            _initialized = true;
            _startingScale = transform.localScale.x;
            TBLogging.LogMessage("Assigned new TBCameraRig instance...");
            ReadInitialCameraSettings();
            if (TBCore.GetActivePlatform() != VRPlatform.None)
            {
                _trackingVolume = new GameObject().transform;
                _trackingVolume.gameObject.name = "Tracking Volume";
                _trackingVolume.MakeZeroedChildOf(transform);
            }
            SetupCameraForPlatform(TBCore.GetActivePlatform());

            if (_useBlackoutSphere)
            {
                _blackoutSphere = (Instantiate(Resources.Load("BlackoutSphere")) as GameObject).transform;
                _blackoutSphere.gameObject.SetActive(false);
                _blackoutSphere.SetParent(_centerEyeTransform);
                _blackoutSphere.localScale = Vector3.one * 2;
                _blackoutSphere.localPosition = Vector3.zero;
            }
        }

        private void SetupCameraForPlatform(VRPlatform platform)
        {
            switch (platform)
            {
                default:
                    SetupNativeCamera(platform);
                    DestroyTempCamera();
                    break;
                case VRPlatform.None:
                    _primaryCamera = GetComponent<Camera>();
                    _centerEyeTransform = transform;
                    break;
            }
            _audioListenerTransform = new GameObject("AudioListener").transform;
            _audioListener = _audioListenerTransform.gameObject.AddComponent<AudioListener>();
            _audioListenerTransform.gameObject.AddComponent<TBAudioListener>();
        }

        #if UNITY_EDITOR
        protected void Update()
        {
            if (_baseCamera == null)
                return;
            if (UnityEngine.Input.GetKeyDown(TBSettings.GetCameraSettings().calibrationHotkey))
                _baseCamera.Recenter();
        }
        #endif

        private void ReadInitialCameraSettings()
        {
            Camera sceneCamera = GetComponent<Camera>();
            clearFlags = sceneCamera.clearFlags;
            cullingMask = sceneCamera.cullingMask;
            backgroundColor = sceneCamera.backgroundColor;
            nearClipPlane = sceneCamera.nearClipPlane;
            farClipPlane = sceneCamera.farClipPlane;
            useOcclusionCulling = sceneCamera.useOcclusionCulling;
            depth = sceneCamera.depth;
            stereoConvergence = sceneCamera.stereoConvergence;
            stereoSeparation = sceneCamera.stereoSeparation;
        }

        private void SetupNativeCamera(VRPlatform platform)
        {
            _centerEyeTransform = new GameObject().transform;
            _centerEyeTransform.MakeZeroedChildOf(_trackingVolume);
            _centerEyeTransform.gameObject.name = "Standard VR Camera";
            _centerEyeTransform.gameObject.tag = "MainCamera";
            _primaryCamera = _centerEyeTransform.gameObject.AddComponent<Camera>();
            switch (platform)
            {
                case VRPlatform.OculusMobile:
                case VRPlatform.OculusPC:
                    _cameraMode = CameraMode.Single;
                    _baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraOculus>();
                    break;
                case VRPlatform.SteamVR:
                    _cameraMode = CameraMode.Single;
                    _baseCamera = _primaryCamera.gameObject.AddComponent<TBSteamVRCamera>();
                    break;
                #if TB_HAS_UNITY_PS4
                case VRPlatform.PlayStationVR:
                    _cameraMode = CameraMode.Single;
                    _baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraPSVR>();
                    break;
                #endif
                case VRPlatform.Daydream:
                    _cameraMode = CameraMode.Single;
                    _baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraGoogle>();
                    break;
                default:
                    _cameraMode = CameraMode.Single;
                    _baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraBase>();
                    break;
            }
            _baseCamera.Initialize();

            if (TBTracking.OnNodeConnected != null)
            {
                TBTracking.OnNodeConnected(UnityEngine.XR.XRNode.CenterEye, _centerEyeTransform);
                TBTracking.OnNodeConnected(UnityEngine.XR.XRNode.Head, _centerEyeTransform);
                TBTracking.OnNodeConnected(UnityEngine.XR.XRNode.TrackingReference, _trackingVolume);
            }

            _trackingVolume.localScale = Vector3.one;
        }

        /// <summary>
        /// Removes the camera from the starting TBCameraRig game object.
        /// </summary>
        private void DestroyTempCamera()
        {
            if (gameObject.tag == "MainCamera")
                gameObject.tag = "Untagged";

            // We don't need FlareLayer or GUILayer, so we remove them if they're present because they prevent us from destroying the camera.
            var flare = GetComponent<FlareLayer>();
            if (flare != null)
                Destroy(flare);

            #if !UNITY_2019_1_OR_NEWER
            // GUILayer is obsolete, but needs to be removed if it's present from old projects getting upgraded.
            var ui = GetComponent<GUILayer>();
            if (ui != null)
                Destroy(ui);
            #endif

            var audioListener = GetComponent<AudioListener>();
            if (audioListener != null)
                Destroy(audioListener);

            Camera tempCamera = GetComponent<Camera>();
            tempCamera.stereoTargetEye = StereoTargetEyeMask.None;
            Destroy(tempCamera);
        }

        public virtual void ToggleBlackoutSphere(bool on)
        {
            if (!_useBlackoutSphere)
                return;

            if (on)
            {
                _blackoutSphere.SetParent(_centerEyeTransform);
                _blackoutSphere.localScale = 5f * Vector3.one;
                _blackoutSphere.position = _centerEyeTransform.position;
            }

            _blackoutSphere.gameObject.SetActive(on);
        }

        #region GETTERS AND SETTERS
        [HideInInspector]
        public CameraClearFlags clearFlags
        {
            get { return _clearFlags; }
            set
            {
                _clearFlags = value;
                if (Events.OnClearFlagsChanged != null)
                    Events.OnClearFlagsChanged();
            }
        }
        [HideInInspector]
        public bool useOcclusionCulling
        {
            get { return _useOcclusionCulling; }
            set
            {
                _useOcclusionCulling = value;
                if (Events.OnOcclusionCullingChanged != null)
                    Events.OnOcclusionCullingChanged();
            }
        }
        [HideInInspector]
        public LayerMask cullingMask
        {
            get { return _cullingMask; }
            set
            {
                _cullingMask = value;
                if (Events.OnCullingMaskChanged != null)
                    Events.OnCullingMaskChanged();
            }
        }
        [HideInInspector]
        public Color backgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                if (Events.OnBackgroundColorChanged != null)
                    Events.OnBackgroundColorChanged();
            }
        }
        [HideInInspector]
        public float nearClipPlane
        {
            get { return _nearClipPlane; }
            set
            {
                _nearClipPlane = value;
                if (Events.OnNearClipPlaneChanged != null)
                    Events.OnNearClipPlaneChanged();
            }
        }
        [HideInInspector]
        public float farClipPlane
        {
            get { return _farClipPlane; }
            set
            {
                _farClipPlane = value;
                if (Events.OnFarClipPlaneChanged != null)
                    Events.OnFarClipPlaneChanged();
            }
        }
        [HideInInspector]
        public OpaqueSortMode sortMode
        {
            get { return _sortMode; }
            set
            {
                _sortMode = value;
                if (Events.OnSortModeChanged != null)
                    Events.OnSortModeChanged();
            }
        }
        [HideInInspector]
        public float stereoConvergence
        {
            get { return _stereoConvergence; }
            set
            {
                _stereoConvergence = value;
                if (Events.OnStereoConvergenceChanged != null)
                    Events.OnStereoConvergenceChanged();
            }
        }
        [HideInInspector]
        public float stereoSeparation
        {
            get { return _stereoSeparation; }
            set
            {
                _stereoSeparation = value;
                if (Events.OnStereoSeparationChanged != null)
                    Events.OnStereoSeparationChanged();
            }
        }
        [HideInInspector]
        public float depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                if (Events.OnDepthChanged != null)
                    Events.OnDepthChanged();
            }
        }
        #endregion

        public CameraMode GetCameraMode()
        {
            return _cameraMode;
        }

        public enum CameraMode
        {
            Single,
            Dual
        }

        public Camera GetCenterEyeCamera()
        {
            if (_primaryCamera != null)
                return _primaryCamera;
            else
                return GetComponent<Camera>();
        }

        public Transform GetCenter()
        {
            return _centerEyeTransform;
        }

        public Transform GetTrackingVolume()
        {
            return _trackingVolume;
        }

        public AudioListener GetAudioListener()
        {
            return _audioListener;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void ResetScale()
        {
            SetScale(_startingScale);
        }

        public bool HasPositionalTracking()
        {
            return _baseCamera.HeadsetHasPositionTracking();
        }

        public static class Events
        {
            public delegate void CameraSettingChangeEvent();

            /// <summary>
            /// Fired when TBCameraRig's background color changes.
            /// </summary>
            public static CameraSettingChangeEvent OnBackgroundColorChanged;
            /// <summary>
            /// Fired when TBCameraRig's near clip changes.
            /// </summary>
            public static CameraSettingChangeEvent OnNearClipPlaneChanged;
            /// <summary>
            /// Fired when TBCameraRig's far clip changes.
            /// </summary>
            public static CameraSettingChangeEvent OnFarClipPlaneChanged;
            /// <summary>
            /// Fired when TBCameraRig's clear flags change.
            /// </summary>
            public static CameraSettingChangeEvent OnClearFlagsChanged;
            /// <summary>
            /// Fired when TBCameraRig's occlusion culling toggle changes.
            /// </summary>
            public static CameraSettingChangeEvent OnOcclusionCullingChanged;
            /// <summary>
            /// Fired when TBCameraRig's opaque sort mode changes.
            /// </summary>
            public static CameraSettingChangeEvent OnSortModeChanged;
            /// <summary>
            /// Fired when TBCameraRig's culling mask changes.
            /// </summary>
            public static CameraSettingChangeEvent OnCullingMaskChanged;
            /// <summary>
            /// Fired when TBCameraRig's culling mask changes.
            /// </summary>
            public static CameraSettingChangeEvent OnDepthChanged;
            /// <summary>
            /// Fired when TBCameraRig's stereo convergence changes.
            /// </summary>
            public static CameraSettingChangeEvent OnStereoConvergenceChanged;
            /// <summary>
            /// Fired when TBCameraRig's stereo separation changes.
            /// </summary>
            public static CameraSettingChangeEvent OnStereoSeparationChanged;
        }
    }
}