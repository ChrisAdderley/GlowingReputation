using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

namespace GlowingReputation
{
    public class ModuleDestructionPenalty:PartModule
    {
        [KSPField(isPersistant = false)]
        public bool SafeUntilFirstActivation = false;

        [KSPField(isPersistant = false)]
        public float BaseReputationHit = 15f;

        [KSPField(isPersistant = false)]
        public float BaseFundsHit = 0f;

        [KSPField(isPersistant = false)]
        public float BaseScienceHit = 0f;

        // Reputation Status string
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Rep. Status")]
        public string ReputationStatus = "Safe";

        [KSPField(isPersistant = true)]
        public bool HasBeenActivated = false;

        ModuleResourceConverter converter;
        ModuleEnginesFX[] engines;

        public void Start()
        {
            //Fields["ReputationStatus"].guiName = Localizer.Format();

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!SafeUntilFirstActivation)
                {
                  HasBeenActivated = true;
                }
                converter = this.GetComponent<ModuleResourceConverter>();
                engines = this.GetComponents<ModuleEnginesFX>();

                GameEvents.onPartExplode.Add(new EventData<GameEvents.ExplosionReaction>.OnEvent(OnPartDestroyed));
                if (SafeUntilFirstActivation && !HasBeenActivated)
                  ReputationStatus = "Safe";
            }
        }

        public string GetModuleTitle()
        {
            return Localizer.Format("#LOC_GlowingRepuation_ModulePenaltyDestruction_title");
        }
        public override string GetInfo()
        {
            string outStr = Localizer.Format("LOC_GlowingRepuation_ModulePenaltyDestruction_description");
            if (BaseReputationHit > 0.0f)
              outStr += "\n ";
              outStr += Localizer.Format("LOC_GlowingRepuation_ModulePenaltyDestruction_description_rep", BaseReputationHit.ToString("F1"));
            if (BaseFundsHit > 0.0f)
              outStr += "\n ";
              outStr += Localizer.Format("LOC_GlowingRepuation_ModulePenaltyDestruction_description_science", BaseFundsHit.ToString("F1");
            if (BaseScienceHit > 0.0f)
              outStr += "\n ";
              outStr += Localizer.Format("LOC_GlowingRepuation_ModulePenaltyDestruction_description_funds", BaseScienceHit.ToString("F1");

            return outStr;
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {

                if (SafeUntilFirstActivation && !HasBeenActivated)
                {
                    EvaluateSafety();
                }
                if (!SafeUntilFirstActivation || (SafeUntilFirstActivation && HasBeenActivated))
                {

                    //ReputationStatus = String.Format("-{0:F1} when lost", repScale * BaseReputationHit);
                }

            }
        }

        public void OnDestroy()
        {
            GameEvents.onPartExplode.Remove(new EventData<GameEvents.ExplosionReaction>.OnEvent(OnPartDestroyed));
        }

        protected void OnPartDestroyed(GameEvents.ExplosionReaction p)
        {
            Utils.Log(SafeUntilFirstActivation.ToString());


            if (SafeUntilFirstActivation && !HasBeenActivated)
            {
              Utils.Log(String.Format("[{0}]: PartDestroyed but was still safe", moduleName));
              return;
            }

            float funds = 0f;
            float rep = 0f;
            float science = 0f;

            if (BaseReputationHit > 0f)
              rep = GlowingReputation.CalculateReputationLoss(vessel);
            if (BaseFundsHit > 0f)
              funds = GlowingReputation.CalculateFundsLoss(vessel);
            if (BaseScienceHit > 0f)
              science = GlowingReputation.CalculateScienceLoss(vessel);

            GlowingReputation.ApplyPenalties(rep * BaseReputationHit,
                    funds * BaseFundsHit, science * BaseScienceHit);

            Utils.Log(String.Format("[{0}]: Part Destroyed resulted in a loss of:
               \n* {1} reputation, scaled from {2} by {3}%
               \n* {4} funds, scaled from {5} by {6}%
               \n* {7} science, scaled from {8} by {9}%",
               moduleName,
               rep*BaseReputationHit, BaseReputationHit, rep*100f,
               funds*BaseFundsHit, BaseFundsHit, funds*100f,
               science*BaseScienceHit, BaseScienceHit, science*100f));
        }



        protected void EvaluateSafety()
        {
            // ModuleResourceConverter (ie fission reactor)
            if (converter != null)
            {
              if (converter.ModuleIsActive())
              {
                HasBeenActivated = true;
                Utils.Log("[{0}]: {1} is now unsafe!", moduleName, part.partInfo.title);
              }
            }

            // ModuleEnginesFX (ie nuclear engine)
            foreach (ModuleEnginesFX engine in engines)
            {
              if (engine.EngineIgnited)
              {
                HasBeenActivated = true;
                Utils.Log("[{0}]: {1} is now unsafe!", moduleName, part.partInfo.title);
              }
            }
        }
    }
}
