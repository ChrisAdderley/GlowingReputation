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

        private ModuleEnginesFX engineFX;

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
              // Get engines
                foreach (ModuleEnginesFX fx in this.part.GetComponents<ModuleEnginesFX>())
                {
                  if (fx.engineID == EngineID)
                    engineFX = fx;
                }
                if (engineFX == null)
                {
                    Utils.LogWarning(String.Format("Could not find ModuleEnginesFX with ID {0}", EngineID));
                }
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

            if (engineFX != null )
            {
               Debug.Log(Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude));
               //Debug.Log(this.vessel.mainBody.bodyName);
              if (engineFX.EngineIgnited && engineFX.requestedThrottle > 0f)
              {
                LoseReputation();
              } else
              {
                ReputationStatus = String.Format("Max {0:F3}/s",Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit);
              }




            }
          }
        }



        protected void LoseReputation()
        {
            float repLoss = Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit *
              (engineFX.requestedMassFlow/engineFX.maxFuelFlow);
            
            ReputationStatus = String.Format("{0:F3}/s", repLoss);
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                Reputation.Instance.AddReputation(-repLoss*TimeWarp.fixedDeltaTime, TransactionReasons.None);
        }


    }
}
