using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Services
{
    public class TBServiceBase : MonoBehaviour
    {
        protected string _username = "null";

        protected bool _ready;

        /// <summary>
        /// Establishes connections, checks entitlements, and populates user IDs with the service.
        /// </summary>
        public virtual void Initialize()
        {
            CheckEntitlement();
            PopulateUserData();
        }

        protected virtual void CheckEntitlement()
        {
            throw new System.NotImplementedException();
        }

        protected virtual void PopulateUserData()
        {
            throw new System.NotImplementedException();
        }

        protected void SetReady(bool on)
        {
            _ready = on;
            if(on)
                TBLogging.LogMessage("Service is ready! Local instances populated.");
        }

        protected virtual void EvalEntitlement(bool passed)
        {
            if (passed)
            {
                Debug.Log("Verified entitlement check for " + TBServiceManager.GetActiveService());
                TBServiceManager.OnEntitlementCheckComplete();
            }
            else
            {
                Debug.LogError("Failed entitlement check for " + TBServiceManager.GetActiveService());

                if (TBServiceManager.Events.OnEntitlementCheckComplete != null)
                    TBServiceManager.Events.OnEntitlementCheckComplete(false);
                else
                    Application.Quit();
            }
        }

        public virtual string GetUsername()
        {
            return _username;
        }

        public virtual bool IsReady()
        {
            return _ready;
        }

        /// <summary>
        /// Tells a service to unlock an achievement.
        /// </summary>
        /// <param name="token"></param>
        public virtual void UnlockAchievement(string token)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the full dictionary of achievement names and their unlock status from the service.
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, bool> GetAchievementDictionary()
        {
            throw new System.NotImplementedException();
        }
    }
}