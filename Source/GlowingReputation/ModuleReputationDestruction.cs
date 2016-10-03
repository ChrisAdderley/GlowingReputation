using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GlowingReputation
{
    public class ModuleReputationDestruction:PartModule
    {
        [KSPField(isPersistant = false)]
        public bool SafeUntilFirstActivation = false;

        [KSPField(isPersistant = false)]
        public float BaseReputationHit = 15f;

        // Reputation Status string
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Rep. Status")]
        public string ReputationStatus = "Safe";

        [KSPField(isPersistant = true)]
        public bool Unsafe = true;

        ModuleResourceConverter converter;
        ModuleEnginesFX[] engines;

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!Unsafe && SafeUntilFirstActivation)
                    Unsafe = false;
                else
                  Unsafe = true;

                converter = this.GetComponent<ModuleResourceConverter>();
                engines = this.GetComponents<ModuleEnginesFX>();
                GameEvents.onPartExplode.Add(new EventData<GameEvents.ExplosionReaction>.OnEvent(OnPartDestroyed));
                if (!Unsafe)
                  ReputationStatus = "Safe";
            }
        }

        public string GetModuleTitle()
        {
            return "Reputation Damaging Part";
        }
        public override string GetInfo()
        {
            string outStr = String.Format("Destroying this part damages reputation. \n\n<b>Max loss</b>: {0:F1} Rep", BaseReputationHit);

            return outStr;
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (SafeUntilFirstActivation && !Unsafe)
                {
                    EvaluateSafety();
                }
                if (!SafeUntilFirstActivation || (SafeUntilFirstActivation && Unsafe))
                {
                    float repScale = Utils.GetReputationScale(this.part.vessel.mainBody,
                        Vector3.Distance(this.part.vessel.mainBody.position, this.part.partTransform.position) - this.part.vessel.mainBody.Radius);
                    ReputationStatus = String.Format("-{0:F1} when lost", repScale * BaseReputationHit);
                }
            }
        }

        public void OnDestroy()
        {
            GameEvents.onPartExplode.Remove(new EventData<GameEvents.ExplosionReaction>.OnEvent(OnPartDestroyed));
        }

        protected void OnPartDestroyed(GameEvents.ExplosionReaction p)
        {
            Debug.Log(SafeUntilFirstActivation);
            Debug.Log(Unsafe);

            if (SafeUntilFirstActivation && !Unsafe)
              return;

            float repScale = Utils.GetReputationScale(this.part.vessel.mainBody,
                Vector3.Distance(this.part.vessel.mainBody.position, this.part.partTransform.position) - this.part.vessel.mainBody.Radius);
            float repLoss =  repScale * BaseReputationHit;

            Utils.Log(String.Format("Destruction resulted in a loss of {0} reputation, scaled from {1} by {2}%", repLoss, BaseReputationHit, repScale*100f));

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                Reputation.Instance.AddReputation(repLoss, TransactionReasons.VesselLoss);
        }

        protected void EvaluateSafety()
        {
            // ModuleResourceConverter (ie fission reactor)
            if (converter != null)
            {
              if (converter.ModuleIsActive())
              {
                Unsafe = true;
                Utils.Log("This part is now unsafe!");

              }
            }

            // ModuleEnginesFX (ie nuclear engine)
            foreach (ModuleEnginesFX engine in engines)
            {
              if (engine.EngineIgnited)
              {
                Unsafe = true;
                Utils.Log("This part is now unsafe!");
              }
            }
        }
    }
}
