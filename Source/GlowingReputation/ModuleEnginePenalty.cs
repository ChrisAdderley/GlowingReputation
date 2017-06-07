using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{
    public class ModuleReputationEngine:PartModule
    {

        // In rep/s at max thrust
        [KSPField(isPersistant = false)]
        public float BaseReputationHit = 1f;

        // Reputation Status string
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Rep. Loss")]
        public string ReputationStatus = "";

        [KSPField(isPersistant = false)]
        public string EngineID;

        protected bool useLegacyEngines = false;

        private ModuleEnginesFX engineFX;
        private ModuleEngines engineLegacy;

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
              SetupEngines();
            }
        }

        void SetupEngines()
        {
            ModuleEngines[] enginesLegacy = this.GetComponents<ModuleEngines>();
            ModuleEnginesFX[] engines = this.part.GetComponents<ModuleEnginesFX>();

            if (enginesLegacy.Length > 0)
            {

              Utils.Log("ReputationEngine: Using legacy engine module");
              useLegacyEngines = true;
              engineLegacy = enginesLegacy[0];
            } else
            {
              if (EngineID == "" || EngineID == String.Empty)
              {
                  Utils.LogWarning("ReputationEngine: EngineID field not specified, trying to use default engine");
                  if (engines.Length > 0)
                    engineLegacy = engines[0];
              }
              foreach (ModuleEnginesFX fx in engines)
              {
                if (fx.engineID == EngineID)
                {
                  engineFX = fx;
                }
              }
            }
            if (useLegacyEngines)
            {
              if (engineLegacy == null)
                Utils.LogError("ReputationEngine: Couldn't find a legacy engine module");
            } else
            {
              if (engineFX == null)
                Utils.LogError("ReputationEngine: Couldn't find a ModuleEnginesFX engine module");
            }
        }

        public string GetModuleTitle()
        {
            return "Reputation Damaging Engine";
        }
        public override string GetInfo()
        {
            string outStr = String.Format("Running in sensitive environments damages reputation. \n\n<b>Max Rate</b>: {0:F2} Rep/s", BaseReputationHit);
            return outStr;
        }

        public void FixedUpdate()
        {
           if (HighLogic.LoadedSceneIsFlight)
           {

             if (useLegacyEngines)
             {
               if (engineLegacy.EngineIgnited && engineLegacy.requestedThrottle > 0f)
               {
                 LoseReputation();
               }
               else
                {
                  ReputationStatus = String.Format("Max -{0:F2}/s", Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit);
                }
             } else
             {
               if (engineFX.EngineIgnited && engineFX.requestedThrottle > 0f)
               {
                 LoseReputation();
               }
               else
                {
                  ReputationStatus = String.Format("Max -{0:F2}/s", Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit);
                }
             }
          }
        }

        protected void LoseReputation()
        {
            float repLoss = Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit * GetEngineScale();

            ReputationStatus = String.Format("{0:F2}/s", repLoss);

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                Reputation.Instance.AddReputation(-repLoss*TimeWarp.fixedDeltaTime, TransactionReasons.None);
        }

        protected float GetEngineScale()
        {
          if (useLegacyEngines)
          {
            return (engineLegacy.requestedMassFlow/engineLegacy.maxFuelFlow);
          } else
          {
            return (engineFX.requestedMassFlow/engineFX.maxFuelFlow);
          }
        }

    }
}
