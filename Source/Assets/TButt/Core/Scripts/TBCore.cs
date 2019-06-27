using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace TButt
{
    public class TBCore : MonoBehaviour
    {
        public static TBCore instance;
        private static VRPlatform _activePlatform = VRPlatform.None;
        private static bool _initialized = false;

        public delegate void TBCoreEvent();
        public static TBCoreEvent OnNewScene;
        public static TBCoreEvent OnUpdate;
        public static TBCoreEvent OnFixedUpdate;
		public static TBCoreEvent OnLateUpdate;
        public static TBCoreEvent OnBeforeRender;

        void Awake()
        {
            if (instance != null)
            {
                TBLogging.LogMessage("An instance of TBCore already exists. Deleting this instance...");
                Destroy(gameObject);
                if (OnNewScene != null)
                    OnNewScene();
                InitializePerScene();
                return;
            }
            else
            {
                // First TBCore instance.
                instance = this;
                if(!Internal.IsValidSetup())
                {
                    Debug.LogError("No platforms are enabled in TBCore. At least one platform must be enabled for TButt to work. See the 'Core Settings' menu.");
                    return;
                }

                #if TB_PSVR && UNITY_PS4
                TBCore.instance.gameObject.AddComponent<TBPSVRSystemEvents>();
                #endif
                SetActivePlatform();
                Internal.InitializeStartup();
            }
        }

        protected void OnEnable()
        {
            Application.onBeforeRender += BeforeRender;
        }
        protected void OnDisable()
        {
            Application.onBeforeRender -= BeforeRender;
        }

        void InitializePerScene()
        {
            if (GetActivePlatform() == VRPlatform.None)
                return;

            InitializeCameraAndTracking(FindObjectOfType<TBCameraRig>());
        }

        void Update()
        {
            if (OnUpdate != null)
                OnUpdate();
        }

		void LateUpdate ()
		{
			if (OnLateUpdate != null)
				OnLateUpdate ();
		}

        void FixedUpdate()
        {
            if (OnFixedUpdate != null)
                OnFixedUpdate();
        }

        void BeforeRender()
        {
            if (OnBeforeRender != null)
                OnBeforeRender();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                Internal.SetSystemSuspend();
            else
                Internal.SetSystemResume();
        }

        private static void SetActivePlatform()
        {
            if (!UnityEngine.XR.XRSettings.enabled)
            {
                TBLogging.LogMessage("VR is disabled in PlayerSettings (or Unity's XRSettings). Setting VRPlatform to None.");
                _activePlatform = VRPlatform.None;
                return;
            }

            switch (UnityEngine.XR.XRSettings.loadedDeviceName)
            {
                case TBSettings.VRDeviceNames.Oculus:
                    if (Application.isMobilePlatform)
                        _activePlatform = VRPlatform.OculusMobile;
                    else
                        _activePlatform = VRPlatform.OculusPC;
					break;
                case TBSettings.VRDeviceNames.SteamVR:
                    #if !TB_STEAM_VR && !TB_STEAM_VR_2
                    _activePlatform = VRPlatform.OculusPC;  // Allows Oculus Utilities to be used as a fallback if Steam VR plugin is not present.
                    #else
                    _activePlatform = VRPlatform.SteamVR;
                    #endif
                    break;
                case TBSettings.VRDeviceNames.Daydream:
                    _activePlatform = VRPlatform.Daydream;
                    break;
                case TBSettings.VRDeviceNames.PlayStationVR:
                    _activePlatform = VRPlatform.PlayStationVR;
                    break;
                case TBSettings.VRDeviceNames.WindowsMR:
                    _activePlatform = VRPlatform.WindowsMR;
                    break;
                default:
                    if (string.IsNullOrEmpty(UnityEngine.XR.XRSettings.loadedDeviceName))
                    {
                        TBLogging.LogMessage("No VR device is currently loaded in Unity's XRSettings, even though VR was enabled. That may be a sign of an error.");
                    }
                    else
                    {
                        Debug.LogError("Detected " + UnityEngine.XR.XRSettings.loadedDeviceName);
                        Debug.LogError("The current HMD / SDK pairing doesn't match any known TButt preset! TCore initialization failed.");
                    }
                    _activePlatform = VRPlatform.None;
                    UnityEngine.XR.XRSettings.enabled = false;
                    return;
            }
            TBLogging.LogMessage("TBCore's VRPlatform is now " + _activePlatform + ". Configured for " + UnityEngine.XR.XRDevice.model + " running through " + UnityEngine.XR.XRSettings.loadedDeviceName + " SDK");
        }

        private void ToggleVRMode(bool on) // TODO
        {
            if (on)
            {
                if (!UsingVRMode())
                {
                    if (!_initialized)
                    {
                      
                    }

                    if (Events.OnVRModeEnabled != null)
                        Events.OnVRModeEnabled(true);
                }
                else
                {
                    TBLogging.LogWarning("Tried to enter VR mode, but we were already using VR Mode.", gameObject);
                }
            }
            else
            {
                if (UsingVRMode())
                {
                    TBLogging.LogWarning("Tried to exit VR mode, but we were already out of VR Mode.", gameObject);
                }
                else
                {
                    if (Events.OnVRModeEnabled != null)
                        Events.OnVRModeEnabled(false);
                }
            }
        }

        private void StartCheckingForHMD()
        {
            StartCoroutine(WaitForHMDConnection());
        }

        private void InitializeCameraAndTracking(TBCameraRig rig)
        {
            if (rig == null)
            {
                TBLogging.LogMessage("TBCameraRig wasn't found at startup. Waiting for TBCameraRig instance.", gameObject);
                StartCoroutine(WaitForCameraRig());
            }
            else
            {
                rig.Initialize();
                TBTracking.Initialize(GetActivePlatform());
            }
        }

        private IEnumerator WaitForHMDConnection()
        {
            FindObjectOfType<TBCameraRig>().Initialize();
            #if UNITY_PS4 && !UNITY_EDITOR
            TBSettings.Initialize(VRPlatform.PlayStationVR);
            #endif

            while ((UnityEngine.XR.XRSettings.loadedDeviceName == "None") || string.IsNullOrEmpty(UnityEngine.XR.XRSettings.loadedDeviceName))
                yield return null;

            SetActivePlatform();

            TBCore.Internal.InitializeStartup();
        }

        public void ForceInit()
        {
            _initialized = true;
        }

        /// <summary>
        /// Returns the active platform. "Platform" refers to the ecosystem / store / SDK your game is running on, rather than the specific headset.
        /// </summary>
        /// <returns></returns>
        public static VRPlatform GetActivePlatform()
        {
            return _activePlatform;
        }        

        /// <summary>
        /// Returns the active headset, if one has been initialized.
        /// </summary>
        /// <returns></returns>
        public static VRHeadset GetActiveHeadset()
        {
            return TBSettings.GetActiveHeadset();
        }

        /// <summary>
        /// Returns the family of the active headset, if one has been initialized.
        /// </summary>
        /// <returns></returns>
        public static VRFamily GetActiveHeadsetFamily()
        {
            return TBSettings.GetHeadsetFamily();
        }

        /// <summary>
        /// A bool used to prevent things from initializing in builds when they aren't needed.
        /// </summary>
        /// <returns></returns>
        public static bool UsingEditorMode()
        {
            #if UNITY_EDITOR
            return true;
            #else
            return false;
            #endif
        }

        public static bool UsingVRMode()
        {
            return UnityEngine.XR.XRSettings.enabled;
        }

        public static bool IsNativeIntegration()
        {
            switch(GetActivePlatform())
            {
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Fires the "on system menu" event for when we're going to or coming from a system-level menu during gameplay.
        /// </summary>
        /// <param name="focus"></param>
        private IEnumerator OnApplicationFocus(bool focus)
        {
            if (focus)
                TBLogging.LogMessage("Application regained focus!", gameObject);
            else
                TBLogging.LogMessage("Application lost focus!", gameObject);

            yield return null;
            Internal.SetSystemMenu(focus);
        }

        private IEnumerator WaitForCameraRig()
        {
            yield return null;
            TBCameraRig rig;
            while (true)
            {
                rig = FindObjectOfType<TBCameraRig>();
                if (rig != null)
                {
                    TBLogging.LogMessage("TBCameraRig found!", gameObject);
                    InitializeCameraAndTracking(rig);
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }

        public static class Events
        {
            public delegate void TBCoreSystemEvent(bool on);

            public static TBCoreSystemEvent OnHMDConnected;
            public static TBCoreSystemEvent OnSystemMenu;
            public static TBCoreSystemEvent OnHMDMounted;
            public static TBCoreSystemEvent OnVRModeEnabled;
            public static TBCoreEvent OnTrackingReset;
            public static TBCoreEvent OnSystemSuspend;
            public static TBCoreEvent OnSystemResume; 
        }

        public static class Internal
        {
            public static void InitializeStartup()
            {
                // Initialize all required TButt components.
                switch(TBCore.GetActivePlatform())
                {
                    case VRPlatform.None:
                        // On platforms where we have to first initialize the HMD, wait for that to finish before initializing TButt.
                        TBCore.instance.StartCheckingForHMD();
                        break;
                    default:
                        TBLogging.LogMessage("TBCore is starting up...", TBCore.instance.gameObject);
                        TBSettings.Initialize();
                        TBInput.Initialize(GetActivePlatform());
                        instance.InitializePerScene();
                        DontDestroyOnLoad(instance.gameObject);
                        TBLogging.LogMessage("TBCore has finished starting up.", TBCore.instance.gameObject);
                        _initialized = true;
                        break;
                }
            }

            public static void ResetTracking()
            {
                switch(TBCore.GetActivePlatform())
                {
                    case VRPlatform.None:
                        TBLogging.LogWarning("Cannot handle recenter event when no HMD is loaded.", TBCore.instance.gameObject);
                        break;
                    default:
                        UnityEngine.XR.InputTracking.Recenter();
                        break;
                }
                TBLogging.LogMessage("Reset tracking at Core level.", TBCore.instance.gameObject);
                if (Events.OnTrackingReset != null)
                    Events.OnTrackingReset();
            }

            public static void SetHMDConnected(bool on)
            {
                if (Events.OnHMDConnected != null)
                    Events.OnHMDConnected(on);
            }

            public static void SetSystemMenu(bool on)
            {
                if (Events.OnSystemMenu != null)
                    Events.OnSystemMenu(on);
            }

            public static void SetHMDMounted(bool on)
            {
                if (Events.OnHMDMounted != null)
                    Events.OnHMDMounted(on);
            }

            public static void SetSystemSuspend()
            {
                if (Events.OnSystemSuspend != null)
                    Events.OnSystemSuspend();
            }

            public static void SetSystemResume()
            {
                if (Events.OnSystemResume != null)
                    Events.OnSystemResume();
            }

            public static bool IsValidSetup()
            {
#if TB_OCULUS || TB_STEAM_VR || TB_PSVR || TB_GOOGLE || TB_WINDOWS_MR || TB_STEAM_VR_2
                return true;
#else
                return false;
#endif
            }
        }
    }


    public enum VRPlatform
    {
        None = 0,
        OculusPC = 1,
        SteamVR = 2,
        OculusMobile = 3,
        PlayStationVR = 4,
        Daydream = 5,
        // Cardboard = 6,  // no longer supported
        WindowsMR = 7
    }
}
