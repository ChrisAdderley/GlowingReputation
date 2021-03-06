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

    bool hasDestructionModule = false;
    float currentFundsMultiplier = 0f;
    float currentScienceMultiplier = 0f;
    float currentReputationMultiplier = 0f;

    public override void OnStart()
    {
      GameEvents.onVesselDestroy.Add(new EventData<Vessel>.OnEvent(OnVesselDestroyed));

      hasDestructionModule = CheckModules();
    }

    void OnDestroy()
    {
      GameEvents.onVesselDestroy.Remove(OnVesselDestroyed);
    }

    void FixedUpdate()
    {

    }

    /// <summary>
    /// Checks for the presence of a destruction penalty module on the ship
    /// </summary>
    protected bool CheckModules()
    {
      for (int i=0; i < protoPart.Modules.Count ; i++)
      {
        if (protoPart.Modules[i].moduleName == "ModuleDestructionPenalty")
        {
          Utils.Log(String.Format("[ModuleGlowingVessel]: Detected ModuleDestructionPenalty on {0}", vessel.vesselName));
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Fired when a vessel is destroyed
    /// </summary>
    protected void OnVesselDestroyed()
    {
      if (hasDestructionModule && vessel.loaded)
      {
        // Handled by the partModule
        Utils.Log(String.Format("[ModuleGlowingVessel]: Vessel {0} was destroyed when loaded", vessel.vesselName));
      } else if (hasDestructionModule && !vessel.loaded)
      {
        Utils.Log(String.Format("[ModuleGlowingVessel]: Vessel {0} was destroyed when not loaded", vessel.vesselName));
        VesselDestructionSequence();
      }
    }

    /// <summary>
    /// Does the sequence of destruction: generates penalties if needed, updates the locational multipliers, applies the penalties
    /// </summary>
    protected void VesselDestructionSequence()
    {
      PenaltyEffects effects = new PenaltyEffects();

      for (int i=0; i++; protoVessel.protoPartSnapshots.Count)
      {
        GeneratePartPenalties(protoVessel.protoPartSnapshots[i], effects);
      }
      effects.UpdateMultipliers(vessel);
      Utils.Log(String.Format("[ModuleGlowingVessel]: Unloaded vessel destroyed, preparing to apply resulting penalties: {0}", penalty.ToString()));
      effects.ApplyPenalties();
      Utils.Log(String.Format("[ModuleGlowingVessel]: Applied penalties: {0}", penalty.ToString()));

      PenaltyHelpers
    }

    /// <summary>
    /// Generates the penalties for all the parts and modules to be destroyed and adds them to the PenaltyEffects
    /// </summary>
    protected void GeneratePartPenalties(ProtoPartSnapshot protoPart, PenaltyEffects effects)
    {

      for (int i=0; i < protoPart.Modules.Count ; i++)
      {
        if (protoPart.Modules[i].moduleName == "ModuleDestructionPenalty")
        {
          GeneratePartModulePenalties(protoPart.Modules[i], effects);
        }
      }

    }
    /// <summary>
    /// Generates the penalties for a single module and adds them to the PenaltyEffects
    /// </summary>
    protected void GeneratePartModulePenalties(ProtoPartModuleSnapshot protoModule, PenaltyEffects effects)
    {
      bool safeUntilFirstActivation = false;
      bool hasBeenActivated = false;
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
  }
}
