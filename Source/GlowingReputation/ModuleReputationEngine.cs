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
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Rep. Status")]
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
            }
        }

        public string GetModuleTitle()
        {
            return "Reputation Damaging Engine";
        }
        public override string GetInfo()
        {
            string outStr = String.Format("Running in sensitive environments damages reputation at a rate of {0}/s at maximum thrust", BaseReputationHit);

            return outStr;
        }

        public void FixedUpdate()
        {
           if (HighLogic.LoadedSceneIsFlight)
           {

            if (engineFX != null )
            {
              if (engineFX.EngineIgnited && engineFX.reqestedThrottle > 0f)
              {
                LoseReputation();
              } else
              {
                ReputationStatus = String.Format("Max Loss {0:F3}/s",Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude * BaseReputationHit));
              }




            }
          }
        }



        protected void LoseReputation()
        {
            float repLoss = Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit *
              (engineFX.requestedMassFlow/engineFX.maxFuelFlow);
              ReputationStatus = String.Format("Losing {0:F3}/s", repLoss);
            Reputation.Instance.AddReputation(repLoss*TimeWarp.fixedDeltaTime, TransactionReasons.VesselLoss);
        }


    }
}
