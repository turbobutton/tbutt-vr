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

        public virtual List<TBInput.ButtonDef<T>> GetDefaultDefs()
        {
            return new List<TBInput.ButtonDef<T>>();
        }

        protected virtual void Initialize()
        {
            trackingOffsets = GetDefaultTrackingOffsets();
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

        public virtual T[] GetFingerButtons(TBInput.Finger finger)
        {
            return null;
        }

        protected virtual HandTrackingOffsets GetDefaultTrackingOffsets()
        {
            HandTrackingOffsets newTrackingOffsets = new HandTrackingOffsets();
            newTrackingOffsets.positionOffset = Vector3.zero;
            newTrackingOffsets.rotationOffset = Vector3.zero;

            return newTrackingOffsets;
        }
    }

    public struct HandTrackingOffsets
    {
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
    }
}