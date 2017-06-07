// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{

  public static class GlowingReputation
  {
    public static float CalculateReputationLoss(Vessel vessel)
    {
      float biomeScale = GetBiomeScale(vessel.mainBody, PenaltyType.Reputation, vessel.latitude, vessel.longitude);
      float bodyScale = GetBodyScale(vessel.mainBody, PenaltyType.Reputation, vessel.altitude);

      return biomeScale * bodyScale;
    }
    public static float CalculateFundsLoss(Vessel vessel)
    {
      float biomeScale = GetBiomeScale(vessel.mainBody, PenaltyType.Funds, vessel.latitude, vessel.longitude);
      float bodyScale = GetBodyScale(vessel.mainBody, PenaltyType.Funds, vessel.altitude);

      return biomeScale * bodyScale;
    }
    public static float CalculateScienceLoss(Vessel vessel)
    {
      float biomeScale = GetBiomeScale(vessel.mainBody, PenaltyType.Science, vessel.latitude, vessel.longitude);
      float bodyScale = GetBodyScale(vessel.mainBody, PenaltyType.Science, vessel.altitude);

      return biomeScale * bodyScale;
    }

    public static float GetBiomeScale(CelestialBody body, PenaltyType penalty, double lat, double long)
    {
      if (GlowingReputationSettings.PenaltyData == null)
      {
          Utils.LogWarning("Could not locate reputation data");
          return 1f;
      }
      else if(!GlowingReputationSettings.PenaltyData.ContainsKey(body.bodyName))
      {
          return 1f;
      } else
      {
        string biomeName = ScienceUtil.GetExperimentBiome(body, double lat, double long);
        return GlowingReputationSettings.PenaltyData[body.bodyName].GetBiomeMultiplier(penalty, biomeName);
      }
    }

    public static float GetBodyScale(CelestialBody body, PenaltyType penalty, double altitude)
    {
        // if planet not in DB, no reputation penalty
        if (GlowingReputationSettings.PenaltyData == null)
        {
            Utils.LogWarning("Could not locate penalty data");
            return 0f;
        }
        else if(!GlowingReputationSettings.PenaltyData.ContainsKey(body.bodyName))
        {
            return 0f;
        }
        else
        {
            return GlowingReputationSettings.PenaltyData[body.bodyName].GetBodyMultiplier(penalty, altitude);
        }
    }

    public static void DoPartDestroyed(Part part, float rep, float funds, float science)
    {
      BuildPartDestroyedPenaltyMessage(part, rep, funds, science);
      ApplyPenalties(rep, funds, science);
    }

    public static void ApplyPenalties(float rep, float funds, float science)
    {
      if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
      {
        // Do all penalties
          SubtractReputation(rep);
          SubtractFunds(funds);
          SubtractScience(science);


      } else if (HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX)
      {
        // Only do Science penalty
        SubtractScience(science);
      }
    }

    public static void BuildPartDestroyedPenaltyMessage(Part part, float rep, float funds, float science)
    {
      string msg = Localizer.Format("", part.partInfo.title);
    }

    public static void ScienceSandboxPenaltyMessage()
    {

    }

    public static void SubtractFunds(float amt)
    {
      Funding.Instanace.AddFunds(amt, TransactionReasons.VesselLoss);
    }

    public static void SubtractScience(float amt)
    {
      ResearchAndDevelopment.Instance.AddScience(amt, TransactionReasons.VesselLoss);
    }

    public static void SubtractReputation(float amt)
    {
      Reputation.Instance.AddReputation(amt, TransactionReasons.VesselLoss);
    }

  }
}
