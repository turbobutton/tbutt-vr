using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt
{
    /// <summary>
    /// 2D camera extension class.
    /// </summary>
    public class TBCamera2D : TBCameraBase
    {
        protected override void Awake()
        {
            _camera = GetComponent<Camera>();
            _camera.stereoTargetEye = StereoTargetEyeMask.None;
            UpdateEverything();
            UpdateFOV();
        }

        protected override void OnEnable()
        {
            TBCameraRig.Events.OnFOVChanged += UpdateFOV;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            TBCameraRig.Events.OnFOVChanged -= UpdateFOV;
            base.OnDisable();
        }

        protected void UpdateFOV()
        {
            _camera.fieldOfView = TBCameraRig.instance.fieldOfView;
        }
    }
}