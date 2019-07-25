using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{
  /// <summary>
  /// Stores reputation data for a vessel and tracks the destruction/creation of the vessel
  /// </summary>
  public class ModuleGlowingVessel: VesselModule
  {


    float currentFundsMultiplier = 0f;
    float currentScienceMultiplier = 0f;
    float currentReputationMultiplier = 0f;

    public override void OnStart()
    {
      GameEvents.onVesselDestroy.Add(new EventData<Vessel>.OnEvent(OnVesselDestroyed));
    }

    void OnDestroy()
    {
      GameEvents.onVesselDestroy.Remove(OnVesselDestroyed);
    }

    void FixedUpdate()
    {

    }
    /// <summary>
    /// Fired when a vessel is destroyed
    /// </summary>
    protected void OnVesselDestroyed()
    {
      if (vessel.loaded)
      {
        // Handled by the partModule
        Utils.Log("[ModuleGlowingVessel]: Vessel {0} was destroyed when loaded", vessel.vesselName);
      } else
      {
        Utils.Log("[ModuleGlowingVessel]: Vessel {0} was destroyed when not loaded", vessel.vesselName);
        VesselDestructionSequence();
      }
    }

    /// <summary>
    /// Gets the scaling factor for a penalty type given an altitude above the body
    /// </summary>
    protected void VesselDestructionSequence()
    {
      PenaltyEffects effects = new PenaltyEffects();

      for (int i=0; i++; protoVessel.protoPartSnapshots.Count)
      {
        GeneratePartPenalties(protoVessel.protoPartSnapshots[i], effects);
      }
      effects.UpdateMultipliers(vessel);
      Utils.Log(String.Format("[ModuleGlowingVessel]: Unloaded vessel destroyed, preparing to apply resulting penalties: {0}", penalty.ToString());
      effects.ApplyPenalties();
      Utils.Log(String.Format("[ModuleGlowingVessel]: Applied penalties: {0}", penalty.ToString());
    }

    protected void GeneratePartPenalties(ProtoPartSnapshot protoPart, PenaltyEffects effects)
    {

      for (int i=0 ;i < protoPart.Modules.Count ;i++)
      {
        if (protoPart.Modules[i].moduleName == "ModuleDestructionPenalty")
        {
          GeneratePartModulePenalties(protoPart.Modules[i], effects);
        }
      }

    }
    protected void GeneratePartModulePenalties(ProtoPartModuleSnapshot protoModule, PenaltyEffects effects)
    {
      bool safeUntilFirstActivation;
      bool hasBeenActivated;
      float baseReputationHit = -1f;
      float baseScienceHit = -1f;
      float baseFundsHit = -1f;

      protoModule.moduleValues.TryGetValue("SafeUntilFirstActivation", ref safeUntilFirstActivation);
      protoModule.moduleValues.TryGetValue("HasBeenActivated", ref hasBeenActivated);
      protoModule.moduleValues.TryGetValue("BaseReputationHit", ref baseReputationHit);
      protoModule.moduleValues.TryGetValue("BaseFundsHit", ref baseFundsHit);
      protoModule.moduleValues.TryGetValue("BaseScienceHit", ref baseScienceHit);

      if (safeUntilFirstActivation && !hasBeenActivated)
      {
        Utils.Log("[ModuleGlowingVessel]: Unloaded PartModule Destroyed but was still safe!");
        return;
      }
      else
      {
        effects.AddReputationPenalty(baseReputationHit);
        effects.AddSciencePenalty(baseScienceHit);
        effects.AddFundsPenalty(baseFundsHit);

        Utils.Log(String.Format("[ModuleGlowingVessel]: Unloaded PartModule was Destroyed");
      }
    }

    protected void SubtractReputation(float amt)
    {
      if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
      {
        Reputation.Instance.AddReputation(amt, TransactionReasons.VesselLoss);
      }
    }
  }
}
