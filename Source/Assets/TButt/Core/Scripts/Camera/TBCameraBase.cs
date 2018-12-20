using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt
{
    /// <summary>
    /// Class that manages camera for native integration. Uses events to stay in sync with settings given to TBCameraRig.
    /// </summary>
    public class TBCameraBase : MonoBehaviour
    {
        protected Camera _camera;

        protected void Awake()
        {
            _camera = GetComponent<Camera>();
            UpdateEverything();
        }

        public virtual void Initialize()
        {
            SetTrackingOrigin(TBSettings.GetCameraSettings().trackingOrigin);
            TBCameraRig.instance.sortMode = TBSettings.GetDisplaySettings().opaqueSortMode;
            _camera.depthTextureMode = TBSettings.GetDisplaySettings().depthTextureMode;
            TBLogging.LogMessage("Opaque Sort Mode set to " + _camera.opaqueSortMode);
            _camera.allowHDR = false;
        }

        protected virtual void SetTrackingOrigin(TBSettings.TBTrackingOrigin origin)
        {
            // Use the predefined, uncalibrated height if headset doesn't support position tracking and tracking origin is floor.
            if(!HeadsetHasPositionTracking() && (origin == TBSettings.TBTrackingOrigin.Floor))
                TBCameraRig.instance.GetTrackingVolume().localPosition = new Vector3(0, TBSettings.GetCameraSettings().uncalibratedFloorHeight, 0);
        }

        public virtual bool HeadsetHasPositionTracking()
        {
            // this gets overridden per SDK
            return true;
        }

        public virtual void Recenter()
        {
            UnityEngine.XR.InputTracking.Recenter();
        }

        protected void OnEnable()
        {
            TBCameraRig.Events.OnClearFlagsChanged += UpdateClearFlags;
            TBCameraRig.Events.OnBackgroundColorChanged += UpdateBackgroundColor;
            TBCameraRig.Events.OnCullingMaskChanged += UpdateCullingMask;
            TBCameraRig.Events.OnOcclusionCullingChanged += UpdateOcclusionCulling;
            TBCameraRig.Events.OnFarClipPlaneChanged += UpdateFarClip;
            TBCameraRig.Events.OnNearClipPlaneChanged += UpdateNearClip;
            TBCameraRig.Events.OnSortModeChanged += UpdateSortMode;
            TBCameraRig.Events.OnStereoConvergenceChanged += UpdateStereoConvergence;
            TBCameraRig.Events.OnStereoSeparationChanged += UpdateStereoSeparation;
            TBCameraRig.Events.OnDepthChanged += UpdateDepth;
        }

        protected void OnDisable()
        {
            TBCameraRig.Events.OnClearFlagsChanged -= UpdateClearFlags;
            TBCameraRig.Events.OnBackgroundColorChanged -= UpdateBackgroundColor;
            TBCameraRig.Events.OnCullingMaskChanged -= UpdateCullingMask;
            TBCameraRig.Events.OnOcclusionCullingChanged -= UpdateOcclusionCulling;
            TBCameraRig.Events.OnFarClipPlaneChanged -= UpdateFarClip;
            TBCameraRig.Events.OnNearClipPlaneChanged -= UpdateNearClip;
            TBCameraRig.Events.OnSortModeChanged -= UpdateSortMode;
            TBCameraRig.Events.OnStereoConvergenceChanged -= UpdateStereoConvergence;
            TBCameraRig.Events.OnStereoSeparationChanged -= UpdateStereoSeparation;
            TBCameraRig.Events.OnDepthChanged -= UpdateDepth;
        }

        protected void UpdateEverything()
        {
            UpdateClearFlags();
            UpdateBackgroundColor();
            UpdateCullingMask();
            UpdateOcclusionCulling();
            UpdateFarClip();
            UpdateNearClip();
            UpdateSortMode();
            UpdateStereoConvergence();
            UpdateStereoSeparation();
            UpdateDepth();
        }

        #region EVENT RECEIVERS
        protected void UpdateClearFlags()
        {
            _camera.clearFlags = TBCameraRig.instance.clearFlags;
        }

        protected void UpdateBackgroundColor()
        {
            _camera.backgroundColor = TBCameraRig.instance.backgroundColor;
        }

        protected void UpdateCullingMask()
        {
            _camera.cullingMask = TBCameraRig.instance.cullingMask;
        }

        protected void UpdateOcclusionCulling()
        {
            _camera.useOcclusionCulling = TBCameraRig.instance.useOcclusionCulling;
        }

        protected void UpdateFarClip()
        {
            _camera.farClipPlane = TBCameraRig.instance.farClipPlane;
        }

        protected void UpdateNearClip()
        {
            _camera.nearClipPlane = TBCameraRig.instance.nearClipPlane;
        }

        protected void UpdateSortMode()
        {
            _camera.opaqueSortMode = TBCameraRig.instance.sortMode;
        }

        protected void UpdateStereoConvergence()
        {
            _camera.stereoConvergence = TBCameraRig.instance.stereoConvergence;
        }

        protected void UpdateStereoSeparation()
        {
            _camera.stereoSeparation = TBCameraRig.instance.stereoSeparation;
        }

        protected void UpdateDepth()
        {
            _camera.depth = TBCameraRig.instance.depth;
        }
        #endregion
    }
}