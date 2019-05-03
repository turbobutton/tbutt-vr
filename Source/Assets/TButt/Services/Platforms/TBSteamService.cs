using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if TB_STEAM_SERVICE
using Steamworks;
#endif

namespace TButt.Services
{
    /// <summary>
    /// Script for interfacing with the Oculus Platform service.
    /// </summary>
    public class TBSteamService : TBServiceBase
    {
        public static readonly string serviceFilename = "ServiceSteam";
        public static int gameIDInt;

#if TB_STEAM_SERVICE
        protected Callback<UserStatsReceived_t> _userStatsReceivedCallback;
        protected Callback<UserStatsStored_t> _userStatsStoredCallback;
        protected Callback<UserAchievementStored_t> _achievementStoredCallback;

        private CGameID _gameID;

        protected bool _receivedStats;

        public override void Initialize()
        {
            gameIDInt = TBDataManager.DeserializeFromFile<int>(TBSettings.settingsFolder + TBSteamService.serviceFilename, TBDataManager.PathType.ResourcesFolder);
            TBCore.instance.gameObject.AddComponent<SteamManager>();
            StartCoroutine(WaitForSteamworksInitialization());
        }

        #region INITIALIZATION FUNCTIONS
        IEnumerator WaitForSteamworksInitialization()
        {
            float time = Time.realtimeSinceStartup;
            while (!SteamManager.Initialized)
            {
                time += Time.deltaTime;
                if(time > 10f)
                {
                    Debug.LogError("Steamworks initialization timed out!"); // This might happen if the user doesn't have Steam installed.
                    EvalEntitlement(false);
                    break;
                }
                yield return null;
            }

            _gameID = new CGameID(SteamUtils.GetAppID());

            // Setup callbacks for server communication events.
            _userStatsReceivedCallback = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            _achievementStoredCallback = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

            base.Initialize();  // Finish initializing only after Steamworks is ready.
        }

        protected override void PopulateUserData()
        {
            _username = SteamFriends.GetPersonaName();
            SetReady(true);
        }

        protected override void CheckEntitlement()
        {
            if (!Application.isEditor)
            {
                if (File.Exists("steam_appid.txt"))
                {
                    try
                    {
                        File.Delete("steam_appid.txt");
                    }
                    catch (System.Exception e) { Debug.Log(e.Message); }

                    if (File.Exists("steam_appid.txt"))
                    {
                        Debug.LogError("Found a steam_appid.txt file in the root folder.");
                        EvalEntitlement(false);
                    }
                }
            }
        
            if (SteamAPI.RestartAppIfNecessary(SteamUtils.GetAppID()))
            {
                EvalEntitlement(false);
            }
            else
            {
                EvalEntitlement(true);
            }
        }
        #endregion

        #region ACHIEVEMENTS
        public override void UnlockAchievement(string token)
        {
            base.UnlockAchievement(token);
            SteamUserStats.SetAchievement(token);
        }

        public override Dictionary<string, bool> GetAchievementDictionary()
        {
            if(!SteamManager.Initialized)
            {
                Debug.LogError("Couldn't create the achievement dictionary because Steam VR is not yet initialized!");
                return base.GetAchievementDictionary();
            }

            Dictionary<string, bool> achievements = new Dictionary<string, bool>();
            uint numAchievements = SteamUserStats.GetNumAchievements();

            for (uint i = 0; i < numAchievements; i++)
            {
                string name = SteamUserStats.GetAchievementName(i);
                bool unlocked;
                bool hasAchievement = SteamUserStats.GetAchievement(name, out unlocked);
                if (hasAchievement)
                    achievements.Add(name, unlocked);
                else
                    Debug.LogError("Steamworks failed to get achievement data for " + name);
            }

            return achievements;
        }

        private void OnUserStatsReceived(UserStatsReceived_t callback)
        {
            if ((ulong)_gameID == callback.m_nGameID)
            {
                if (EResult.k_EResultOK == callback.m_eResult)
                {
                    Debug.Log("Received stats and achievements from Steam\n");
                }
                else
                {
                    Debug.Log("Steamworks: RequestStats - failed, " + callback.m_eResult);
                }
            }
        }

        private void OnAchievementStored(UserAchievementStored_t callback)
        {
            if ((ulong)_gameID == callback.m_nGameID)
            {
                if (0 == callback.m_nMaxProgress)
                {
                    TBServiceManager.UpdateAchievementList(callback.m_rgchAchievementName, true);
                    Debug.Log("Achievement '" + callback.m_rgchAchievementName + "' unlocked!");
                }
                else
                {
                    Debug.Log("Achievement '" + callback.m_rgchAchievementName + "' progress callback, (" + callback.m_nCurProgress + "," + callback.m_nMaxProgress + ")");
                }
            }
        }
        #endregion
#endif
    }
}