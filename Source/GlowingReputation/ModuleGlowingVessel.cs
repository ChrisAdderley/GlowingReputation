using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{

  public class GlowingVessel: VesselModule
  {

    public override void OnStart()
    {
      GameEvents.onVesselDestroy.Add(new EventData<Vessel>.OnEvent(OnVesselDestroyed));

    }
    void OnDestroy()
      {
        GameEvents.onVesselDestroy.Remove(OnVesselDestroyed);

      }
    protected void OnVesselDestroyed()
    {
      if (vessel.loaded)
      {
        // Handled by the partModule
        Utils.Log("[GlowingVessel]: Vessel {0} was destroyed when loaded", vessel.vesselName);

      } else
      {
        Utils.Log("[GlowingVessel]: Vessel {0} was destroyed when not loaded", vessel.vesselName);
        DoReputationDestructionVessel();
      }
    }

    // Destroys the vessel
    protected void DoReputationDestructionVessel()
    {
      for (int i=0; i++; protoVessel.protoPartSnapshots.Count)
      {
        DoReputationDestruction(protoVessel.protoPartSnapshots[i]);
      }
    }

    protected void DoReputationDestruction(ProtoPartSnapshot protoPart)
    {
      ProtoPartModuleSnapshot protoSnapshot = protoPart.FindModule("ModuleReputationDestruction");
      if (protoSnapshot != null)
      {
        Utils.Log("[GlowingVessel]: {0} contains ReputationDestruction", protoPart.partName);

        bool safeUntilFirstActivation;
        bool hasBeenActivated;
        float baseReputationHit;
        protoSnapshot.moduleValues.TryGetValue("SafeUntilFirstActivation", ref safeUntilFirstActivation);
        protoSnapshot.moduleValues.TryGetValue("HasBeenActivated", ref hasBeenActivated);
        protoSnapshot.moduleValues.TryGetValue("BaseReputationHit", ref baseReputationHit);

        if (safeUntilFirstActivation && !hasBeenActivated)
        {
          Utils.Log("[GlowingVessel]: Unloaded Part Destroyed but was still safe!");
          return;
        } else
        {
          Utils.Log(String.Format("{0}, {1}", vessel.mainBody, ));
          float repScale = Utils.GetReputationScale(vessel.mainBody,
              Vector3.Distance(vessel.mainBody.position, vessel.transform.position - vessel.mainBody.Radius);

          float repLoss =  repScale * baseReputationHit;
          Utils.Log(String.Format("[GlowingVessel]: Part Destroyed resulted with a loss of {0} reputation, scaled from {1} by {2}%", repLoss, baseReputationHit, repScale * 100f));
        }
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
