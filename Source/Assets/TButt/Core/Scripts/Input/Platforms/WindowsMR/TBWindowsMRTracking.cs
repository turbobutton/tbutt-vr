using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt.Settings;

namespace TButt.Input
{
    public class TBWindowsMRTracking : TBSDKTrackingBase
    {
        protected static TBWindowsMRTracking _instance;

        public static TBWindowsMRTracking instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TBWindowsMRTracking();
                return _instance;
            }
        }

        protected override TBTrackingNodeBase AddNode(TBNode node, GameObject go)
        {
            TBTrackingNodeOculus nodeComponent = go.AddComponent<TBTrackingNodeOculus>();
            return nodeComponent;
        }

        #region CONTROLLER TRACKING SUPPORT
        protected override bool SupportsHandControllers()
        {
            switch (TBOculusSettings.GetOculusDeviceFamily())
            {
                case TBOculusSettings.OculusDeviceFamily.Rift:
                case TBOculusSettings.OculusDeviceFamily.Quest:
                case TBOculusSettings.OculusDeviceFamily.Unknown:
                    return true;
                default:
                    return false;
            }
        }

        protected override bool Supports3DOFController()
        {
            switch (TBOculusSettings.GetOculusDeviceFamily())
            {
                case TBOculusSettings.OculusDeviceFamily.GearVR:
                case TBOculusSettings.OculusDeviceFamily.Go:
                    return true;
                case TBOculusSettings.OculusDeviceFamily.Quest:
                    return false;
                default:
                    if (TBCore.UsingEditorMode())
                    {
                        // Only allow 3DOF arm model emulation in the editor.
                        if (TBSettings.GetControlSettings().emulate3DOFArmModel)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
            }
        }

        public override bool SupportsHMDPositionalTracking()
        {
            switch (TBOculusSettings.GetOculusDeviceFamily())
            {
                case TBOculusSettings.OculusDeviceFamily.Go:
                case TBOculusSettings.OculusDeviceFamily.GearVR:
                    return false;
                default:
                    return true;
            }
        }

        #endregion
    }
}