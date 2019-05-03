using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TB_OCULUS_SERVICE
using Oculus.Platform;
using Oculus.Platform.Models;
#endif

namespace TButt.Services
{
    /// <summary>
    /// Script for interfacing with the Oculus Platform service.
    /// </summary>
    public class TBOculusService : TBServiceBase
    {
        public static readonly string serviceFilename = "ServiceOculus";
        #if TB_OCULUS_SERVICE
        private static OculusServiceIDs _serviceIDs;

        public override void Initialize()
        {
            _serviceIDs = TBDataManager.DeserializeFromFile<OculusServiceIDs>(TBSettings.settingsFolder + TBOculusService.serviceFilename, TBDataManager.PathType.ResourcesFolder);

            switch (TBCore.GetActivePlatform())
            {
                case VRPlatform.OculusMobile:
                    switch(TButt.Settings.TBOculusSettings.GetOculusDeviceFamily())
                    {
                        case Settings.TBOculusSettings.OculusDeviceFamily.GearVR:
                        case Settings.TBOculusSettings.OculusDeviceFamily.Go:
                            Core.AsyncInitialize(_serviceIDs.oculusMobile_ID).OnComplete((Message msg) => ContinueSetup(msg));
                            break;
                        default:
                            Core.AsyncInitialize(_serviceIDs.oculusQuest_ID).OnComplete((Message msg) => ContinueSetup(msg)); ;
                            break;
                    }
                    break;
                case VRPlatform.OculusPC:
                    Core.AsyncInitialize(_serviceIDs.oculusPC_ID).OnComplete((Message msg) => ContinueSetup(msg));
                    break;
                default:
                    Debug.LogError("Attempted to initialize Oculus Platform service while running on a non-Oculus platform. This shouldn't happen and will probably cause you to be sad.");
                    break;
            }
        }

        protected void ContinueSetup(Message msg)
        {
            if (msg.IsError)
                Debug.LogError(msg.GetError());

            base.Initialize();
        }

        #region INITIALIZATION FUNCTIONS
        protected override void CheckEntitlement()
        {
            Entitlements.IsUserEntitledToApplication().OnComplete((Message msg) =>
            {
                if (msg.IsError)
                {
                    Debug.LogError(msg.GetError());
                    EvalEntitlement(false);
                }
                else
                {
                    EvalEntitlement(true);
                }
            });
        }

        protected override void PopulateUserData()
        {
            Users.GetLoggedInUser().OnComplete((Message msg) =>
            {
                if (!msg.IsError)
                    _username = msg.GetUser().OculusID;
                SetReady(true);
            });
        }
        #endregion

        #region ACHIEVEMENTS
        public override void UnlockAchievement(string token)
        {
            base.UnlockAchievement(token);
            Achievements.Unlock(token).OnComplete((Message<AchievementUpdate> msg) =>
            {
                if (msg.IsError)
                    Debug.LogError("Oculus Platform services encountered an error while trying to unlock the achievement!");
                else
                {
                    TBLogging.LogMessage("Achievement unlocked!");
                    TBServiceManager.UpdateAchievementList(token, msg.Data.JustUnlocked);
                }
            });
        }

        public override Dictionary<string, bool> GetAchievementDictionary()
        {
            Dictionary<string, bool> achievements = new Dictionary<string, bool>();
            Achievements.GetAllDefinitions().OnComplete((Message<AchievementDefinitionList> msg) =>
            {
                if (msg.IsError)
                    TBLogging.LogWarning("Failed to get Oculus achievement dictionary from server");
                else
                {
                    TBLogging.LogMessage("Received Oculus achievement dictionary from server with " + msg.Data.Count + " entries");
                }

                for (int i = 0; i < msg.Data.Count; i++)
                {
                    achievements.Add(msg.Data[i].Name, false);
                }

                Achievements.GetAllProgress().OnComplete((Message<AchievementProgressList> msg2) =>
                {
                    if (msg2.IsError)
                        TBLogging.LogWarning("Failed to get Oculus achievement progress from server");
                    else
                        TBLogging.LogMessage("Received Oculus achievement progress dictionary from server with " + msg.Data.Count + " entries");

                 
                        for (int j = 0; j < msg2.Data.Count; j++)
                        {
                            bool unlocked = false;
                            if(achievements.TryGetValue(msg2.Data[j].Name, out unlocked))
                            {
                                achievements[msg2.Data[j].Name] = unlocked;
                                TBLogging.LogMessage("Achievement " + msg2.Data[j].Name + " is unlocked.");
                            }
                        }
                    TBServiceManager.SetAchievementList(achievements);
                });
            });
            return achievements;
        }
        #endregion
#endif
    }


    [System.Serializable]
    public struct OculusServiceIDs
    {
        public string oculusPC_ID;
        public string oculusMobile_ID;
        public string oculusQuest_ID;
    }
}