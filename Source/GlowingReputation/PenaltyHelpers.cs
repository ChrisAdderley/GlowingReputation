// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{

  public static class PenaltyHelpers
  {
    /// <summary>
    /// Calculate the total penalty scaling factor for reputation loss
    /// </summary>
    /// <param name="vessel">The Vessel for which to calculate for</param>
    public static float CalculateReputationLoss(Vessel vessel)
    {
      float biomeScale = GetBiomeScale(vessel.mainBody, PenaltyType.Reputation, vessel.latitude, vessel.longitude);
      float bodyScale = GetBodyScale(vessel.mainBody, PenaltyType.Reputation, vessel.altitude);

      return biomeScale * bodyScale;
    }
    /// <summary>
    /// Calculate the total penalty scaling factor for funds loss
    /// </summary>
    /// <param name="vessel">The Vessel for which to calculate for</param>
    public static float CalculateFundsLoss(Vessel vessel)
    {
      float biomeScale = GetBiomeScale(vessel.mainBody, PenaltyType.Funds, vessel.latitude, vessel.longitude);
      float bodyScale = GetBodyScale(vessel.mainBody, PenaltyType.Funds, vessel.altitude);

      return biomeScale * bodyScale;
    }
    /// <summary>
    /// Calculate the total penalty scaling factor for science loss
    /// </summary>
    /// <param name="vessel">The Vessel for which to calculate for</param>
    public static float CalculateScienceLoss(Vessel vessel)
    {
      float biomeScale = GetBiomeScale(vessel.mainBody, PenaltyType.Science, vessel.latitude, vessel.longitude);
      float bodyScale = GetBodyScale(vessel.mainBody, PenaltyType.Science, vessel.altitude);

      return biomeScale * bodyScale;
    }

    /// <summary>
    /// Calculate the penalty scaling factor for the current biome
    /// </summary>
    /// <param name="body">The CelestialBody</param>
    /// <param name="penalty">The PenaltyType</param>
    /// <param name="lat">The current latitude</param>
    /// <param name="long">The current longitude</param>
    public static float GetBiomeScale(CelestialBody body, PenaltyType penalty, double lat, double long)
    {
      if (GlowingReputationData.BodyData == null)
      {
        // If we cannot find data, scaling is 1.
        Utils.LogWarning("Could not locate reputation data");
        return 1f;
      }
      else if(!GlowingReputationData.BodyData.ContainsKey(body.bodyName))
      {
        // If we cannot find biome data for the specific body, scaling is 1 (no effect).
        return 1f;
      }
      else
      {
        string biomeName = ScienceUtil.GetExperimentBiome(body, double lat, double long);
        return GlowingReputationData.BodyData[body.bodyName].GetBiomeMultiplier(penalty, biomeName);
      }
    }

    /// <summary>
    /// Calculate the penalty scaling factor for the current celestialbody
    /// </summary>
    /// <param name="body">The CelestialBody</param>
    /// <param name="penalty">The PenaltyType</param>
    /// <param name="altitude">The current altitude</param>
    public static float GetBodyScale(CelestialBody body, PenaltyType penalty, double altitude)
    {

      if (GlowingReputationData.BodyData == null)
      {
        // If we cannot data at all, scaling is 0 (no penalty)
        Utils.LogWarning("Could not locate penalty data");
        return 0f;
      }
      else if(!GlowingReputationData.BodyData.ContainsKey(body.bodyName))
      {
        // If we cannot find data for the specific body, scaling is 0 (no penalty)
        return 0f;
      }
      else
      {
        return GlowingReputationData.BodyData[body.bodyName].GetBodyMultiplier(penalty, altitude);
      }
    }

    /// <summary>
    /// Takes action when a relevant part is destroyed
    /// </summary>
    /// <param name="part">The part that was destroyed</param>
    /// <param name="rep">The amount of rep to remove</param>
    /// <param name="funds">The amount of rep to remove</param>
    /// <param name="science">The amount of science to remove</param>
    public static void DoPartDestroyed(Part part, float rep, float funds, float science)
    {
      string message = BuildPartDestroyedPenaltyMessage(part, rep, funds, science);
      ShowMessage(message);
      ApplyPenalties(rep, funds, science);
    }

    public static void ShowMessage(string msg)
    {

    }

    /// <summary>
    /// Applies the penalties
    /// </summary>
    /// <param name="rep">The amount of rep to remove</param>
    /// <param name="funds">The amount of rep to remove</param>
    /// <param name="science">The amount of science to remove</param>
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

    /// <summary>
    /// Builds a game message to describe what happened when something was lost
    /// </summary>
    /// <param name="partName">The part that was destroyed</param>
    /// <param name="effects">The penalty effects to apply</param>
    public static string BuildPartDestroyedPenaltyMessage(string partName, PenaltyEffects effects)
    {
      string msg = Localizer.Format("#LOC_GlowingRepuation_Message_PartDestroyed_Base", partName);
      if (rep > 0)
      {
        msg += Localizer.Format("#LOC_GlowingRepuation_Message_PartDestroyed_Rep", effects.ReputationPenalty.ToString("F1"));
      }
      if (funds > 0)
      {
        msg += Localizer.Format("#LOC_GlowingRepuation_Message_PartDestroyed_Funds", effects.FundsPenalty.ToString("F1"));
      }
      if (science > 0)
      {
        msg += Localizer.Format("#LOC_GlowingRepuation_Message_PartDestroyed_Science", effects.SciencePenalty.ToString("F1"));
      }
      return msg;
    }

    /// <summary>
    /// Builds a game message to describe what happened when something was lost
    /// </summary>
    /// <param name="part">The Part that was destroyed</param>
    /// <param name="effects">The penalty effects to apply</param>
    public static string BuildPartDestroyedPenaltyMessage(Part part, PenaltyEffects effects)
    {
      return PenaltyHelpers.BuildPartDestroyedPenaltyMessage(part.partInfo.title, effects);
    }

    /// <summary>
    /// Builds a game message to describe what happened when something was lost
    /// </summary>
    /// <param name="vessel">The vessel that was destroyed</param>
    /// <param name="effects">The penalty effects to apply</param>
    public static string BuildVesselDestroyedPenaltyMessage(Vessel vessel, PenaltyEffects effects)
    {
      string msg = Localizer.Format("#LOC_GlowingRepuation_Message_VesselDestroyed_Base", vessel.vesselName);
      if (rep > 0)
      {
        msg += Localizer.Format("#LOC_GlowingRepuation_Message_VesselDestroyed_Rep", effects.ReputationPenalty.ToString("F1"));
      }
      if (funds > 0)
      {
        msg += Localizer.Format("#LOC_GlowingRepuation_Message_VesselDestroyed_Funds", effects.FundsPenalty.ToString("F1"));
      }
      if (science > 0)
      {
        msg += Localizer.Format("#LOC_GlowingRepuation_Message_VesselDestroyed_Science", effects.SciencePenalty.ToString("F1"));
      }
      return msg;
    }

    /// <summary>
    /// Subtracts money
    /// </summary>
    /// <param name="amt">The amount to subtract</param>
    public static void SubtractFunds(float amt)
    {
      Funding.Instanace.AddFunds(amt, TransactionReasons.VesselLoss);
    }

    /// <summary>
    /// Subtracts science
    /// </summary>
    /// <param name="amt">The amount to subtract</param>
    public static void SubtractScience(float amt)
    {
      ResearchAndDevelopment.Instance.AddScience(amt, TransactionReasons.VesselLoss);
    }

    /// <summary>
    /// Subtracts rep
    /// </summary>
    /// <param name="amt">The amount to subtract</param>
    public static void SubtractReputation(float amt)
    {
      Reputation.Instance.AddReputation(amt, TransactionReasons.VesselLoss);
    }

  }

}
