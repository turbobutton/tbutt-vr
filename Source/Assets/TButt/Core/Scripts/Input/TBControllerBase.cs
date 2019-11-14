using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Input
{
    [ExecuteInEditMode]
    public abstract class TBControllerBase<T>
    {
        protected string name;
        protected string fileName;
        protected TBInput.Controller type = TBInput.Controller.None;
        protected VRController model = VRController.None;
        protected bool supportsRumble = false;
        protected bool loaded = false;
        protected List<TBInput.ButtonDef<T>> loadedButtonDefs;
        protected TBInput.ButtonLookupTable<T> lookupTable;
        protected HandTrackingOffsets trackingOffsets;
        protected bool _isRumbling;

        protected T[] thumbPoseButtons;
        protected T[] indexPoseButtons;
        protected T[] middlePoseButtons;
        protected T[] ringPoseButtons;
        protected T[] pinkyPoseButtons;
        protected T[] gripPoseButtons;

        public virtual List<TBInput.ButtonDef<T>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<T>>();
        }

        protected virtual void Initialize()
        {
            trackingOffsets = GetDefaultTrackingOffsets();
            SetFingerPoseButtons();

            // Fallback for animating non-dominant fingers.
            if (gripPoseButtons != null)
            {
                if (ringPoseButtons == null)
                    ringPoseButtons = gripPoseButtons;
                if (middlePoseButtons == null)
                    middlePoseButtons = gripPoseButtons;
                if (pinkyPoseButtons == null)
                    pinkyPoseButtons = gripPoseButtons;
            }

            loadedButtonDefs = TBInput.LoadButtonDefs<T>(GetDefaultDefs(), fileName);
            lookupTable = TBInput.NewLookupTableFromDefs<T>(loadedButtonDefs);
            loaded = true;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual VRController GetModel()
        {
            return model;
        }

        public virtual string GetFileName()
        {
            return fileName;
        }

        public virtual TBInput.Controller GetControllerType()
        {
            return type;
        }

        public virtual bool HasRumbleSupport()
        {
            return supportsRumble;
        }

        public virtual bool IsLoaded()
        {
            return loaded;
        }

        public virtual List<TBInput.ButtonDef<T>> GetLoadedButtonDefs()
        {
            return loadedButtonDefs;
        }

        public virtual TBInput.ButtonLookupTable<T> GetLookupTable()
        {
            return lookupTable;
        }

        public virtual HandTrackingOffsets GetTrackingOffsets()
        {
            return trackingOffsets;
        }

        protected virtual void SetFingerPoseButtons()
        {
            return;
        }

        public virtual T[] GetFingerButtons(TBInput.Finger finger)
        {
            switch (finger)
            {
                case TBInput.Finger.Thumb:
                    return thumbPoseButtons;
                case TBInput.Finger.Index:
                    return indexPoseButtons;
                case TBInput.Finger.Middle:
                    return middlePoseButtons;
                case TBInput.Finger.Ring:
                    return ringPoseButtons;
                case TBInput.Finger.Pinky:
                    return pinkyPoseButtons;
                case TBInput.Finger.Grip:
                    return gripPoseButtons;
                default:
                    return null;
            }
        }

        protected virtual HandTrackingOffsets GetDefaultTrackingOffsets()
        {
            HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets();
            newTrackingOffsets.positionOffset = Vector3.zero;
            newTrackingOffsets.rotationOffset = Vector3.zero;

            return newTrackingOffsets;
        }

        public virtual bool IsRumbling
        {
            get { return _isRumbling; }
        }

        public virtual void SetRumbling(bool on)
        {
            _isRumbling = on;
        }
    }

    public struct HandTrackingOffsets
    {
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
    }
}