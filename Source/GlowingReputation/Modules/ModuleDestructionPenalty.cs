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
      outStr += Localizer.Format("LOC_GlowingRepuation_ModulePenaltyDestruction_description_science", BaseFundsHit.ToString("F1"));
      if (BaseScienceHit > 0.0f)
        outStr += "\n ";
      outStr += Localizer.Format("LOC_GlowingRepuation_ModulePenaltyDestruction_description_funds", BaseScienceHit.ToString("F1"));

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

      if (SafeUntilFirstActivation && !HasBeenActivated)
      {
        Utils.Log(String.Format("[{0}]: PartDestroyed while loaded but was still safe", moduleName));
        return;
      }

      PenaltyEffects effects = new PenaltyEffects();

      effects.AddReputationPenalty(BaseReputationHit);
      effects.AddSciencePenalty(BaseScienceHit);
      effects.AddFundsPenalty(BaseFundsHit);

      effects.UpdateMultipliers(this.vessel);

      Utils.Log(String.Format("[ModuleDestructionPenalty]: Vessel destroyed, preparing to apply resulting penalties: {0}", penalty.ToString()));
      effects.ApplyPenalties();
      Utils.Log(String.Format("[ModuleDestructionPenalty]: Applied penalties: {0}", penalty.ToString()));
      string penaltyMsg = PenaltyHelpers.BuildPartDestroyedPenaltyMessage(this.part, effects);
      
      ScreenMessages.PostScreenMessage(
        new ScreenMessage(String.Format("[KEPA]: {0}", penaltyMsg)), 3.0f, ScreenMessageStyle.UPPER_CENTER));

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
          ScreenMessages.PostScreenMessage(
            new ScreenMessage(String.Format("[KEPA]: {0} has been activated and is now unsafe! Dispose of it carefully...", part.partInfo.title)), 3.0f, ScreenMessageStyle.UPPER_CENTER));
        }
      }

      // ModuleEnginesFX (ie nuclear engine)
      foreach (ModuleEnginesFX engine in engines)
      {
        if (engine.EngineIgnited)
        {
          HasBeenActivated = true;
          Utils.Log("[{0}]: {1} is now unsafe!", moduleName, part.partInfo.title);
          ScreenMessages.PostScreenMessage(
            new ScreenMessage(String.Format("[KEPA]: {0} has been activated and is now unsafe! Dispose of it carefully...", part.partInfo.title)), 3.0f, ScreenMessageStyle.UPPER_CENTER));
        }
      }
    }
  }
}
