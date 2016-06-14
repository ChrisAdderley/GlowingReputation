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

        [KSPField(isPersistant = false)]
        public string EngineID;

        private ModuleEnginesFX engineFX

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
              // Get engines
                foreach (ModuleEnginesFX fx in this.part.GetComponents<ModuleEnginesFX>)
                {
                  if (fx.EngineID == EngineID)
                    engineFX = fx;
                }
            }
        }


        public void FixedUpdate()
        {
            if (engineFX != null && !engineFX.Shutdown())
            {
                LoseReputation();
            }
        }



        protected void LoseReputation()
        {
            float repLoss = Utils.GetReputationScale(this.vessel.mainBody, this.vessel.altitude) * BaseReputationHit *
              (engineFX.requestedMassFlow/engineFX.maxFuelFlow);

            Reputation.Instance.AddReputation(repLoss, TransactionReasons.VesselLoss);
        }


    }
}
