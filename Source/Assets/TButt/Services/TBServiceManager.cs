using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt.Services;

namespace TButt
{
    public class TBServiceManager : MonoBehaviour
    {
        private static Dictionary<string, bool> _achievementList;
        private static TBServiceBase _activeService;
        private static VRService _service = VRService.None;
        private static bool _initialized = false;

        private void Start()
        {
            if(!_initialized)
                Initialize();
        }

        public static void Initialize()
        {
#if TB_OCULUS_SERVICE
            _service = VRService.Oculus;
            _activeService = TBCore.instance.gameObject.AddComponent<TBOculusService>();
#elif TB_STEAM_SERVICE
            _service = VRService.Steam;
            _activeService = TBCore.instance.gameObject.AddComponent<TBSteamService>();
#else
             _service = VRService.None;
             _activeService = TBCore.instance.gameObject.AddComponent<TBServiceBase>();
#endif
            _activeService.Initialize();
            _initialized = true;
            TBLogging.LogMessage("Initialized TBServices!");
        }

        public static void OnEntitlementCheckComplete()
        {
            if (Events.OnEntitlementCheckComplete != null)
                Events.OnEntitlementCheckComplete(true);

            if (_service != VRService.None)
                _achievementList = _activeService.GetAchievementDictionary();
        }

        /// <summary>
        /// True once the service has finished populating local data
        /// </summary>
        /// <returns></returns>
        public static bool IsReady()
        {
            if (_service != VRService.None)
                return _activeService.IsReady();
            else
                return false;
        }

        /// <summary>
        /// Gets the public-facing username for the logged in player. Check "IsReady" before calling
        /// </summary>
        /// <returns></returns>
        public static string GetUsername()
        {
            if (_service != VRService.None)
                return "NoUsernameFound";

            if (_activeService.IsReady())
                return _activeService.GetUsername();
            else
                return "NotPopulated";
        }

        /// <summary>
        /// Checks if an achievement has been unlocked.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsAchievementUnlocked(string token)
        {
            if (_service == VRService.None)
            {
                TBLogging.LogWarning("Tried to check status of achievement " + token + " but no service is initialized.", null, "via TButt.TBServiceManager");
            }

            if (_achievementList == null)
            {
                TBLogging.LogWarning("Tried to check unlock status of achievement " + token + " but the dictionary wasn't initialized!", null, "via TButt.TBServiceManager");
                return true;
            }
            bool unlocked;
            if (_achievementList.TryGetValue(token, out unlocked))
                return unlocked;
            else
            {
                TBLogging.LogWarning("Tried to check unlock status of achievement " + token + " but it was not found in the achievement dictionary!", null, "via TButt.TBServiceManager");
                return true;    // Tell the game that the achievement is already unlocked so that it gets skipped from the SDK calls.
            }
        }

        /// <summary>
        /// Unlocks an achievement if it has not already been unlocked.
        /// </summary>
        /// <param name="token"></param>
        public static void UnlockAchievement(string token)
        {
            if(_service == VRService.None)
            {
                TBLogging.LogWarning("Tried to unlock achievement " + token + " but no service is initialized.", null, "via TButt.TBServiceManager");
                return;
            }

            if (!IsAchievementUnlocked(token))
            {
                _activeService.UnlockAchievement(token);

                if (Events.OnAchievementUnlocked != null)
                    Events.OnAchievementUnlocked(token);
            }
        }

        /// <summary>
        /// Syncs achievement data with the service. Gets called by service extension classes in their callbacks when new achievements are unlocked.
        /// </summary>
        public static void UpdateAchievementList(string token, bool value)
        {
            bool um;
            if (_achievementList.TryGetValue(token, out um))
                _achievementList[token] = value;
            else
                Debug.LogError("Attempted to update unlock status for the token " + token + " but it doesn't exist in the achievement dictionary!");
        }

        public static int GetNumAchievements()
        {
            if (_service == VRService.None)
            {
                TBLogging.LogWarning("Tried to get achievement list, but no service is initialized.", null, "via TButt.TBServiceManager");
                return 0;
            }

            if (_achievementList == null)
                return 0;

            return _achievementList.Count;
        }

        public static int GetNumAchievements(bool unlocked)
        {
            if (_service == VRService.None)
            {
                TBLogging.LogWarning("Tried to get achievement list, but no service is initialized.", null, "via TButt.TBServiceManager");
                return 0;
            }

            if (_achievementList == null)
                return 0;

            int num = 0;
            foreach (KeyValuePair<string, bool> entry in _achievementList)
            {
                if (entry.Value == unlocked)
                    num++;
            }
            return num;
        }

        public static void SetAchievementList(Dictionary<string, bool> list)
        {
            _achievementList = list;
            TBLogging.LogMessage("Achievement list popualted with " + list.Count + " entries.", null, "via TButt.TBServiceManager");
        }

        public static VRService GetActiveService()
        {
            return _service;
        }

        public static class Events
        {
            public delegate void TBAchievementEvent(string token);
            public delegate void TBEntitlementCheckEvent (bool pass);
            public static TBEntitlementCheckEvent OnEntitlementCheckComplete;
            public static TBAchievementEvent OnAchievementUnlocked;
        }

    }
    public enum VRService
    {
        None        = 0,
        Steam       = 1,
        Oculus      = 2,
        PSN         = 3,
        XboxLive    = 4
    }
}