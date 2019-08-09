// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{
  /// <summary>
  /// TThis class is responsible for building and applying penalty effects
  /// </summary>
  public class PenaltyEffects
  {
    public float baseReputationHit { get; private set; } = 0f;
    public float baseScienceHit { get; private set; } = 0f;
    public float baseFundsHit = 0f;

    public float currentFundsMultiplier { get; private set; } = 1f;
    public float currentScienceMultiplier { get; private set; } = 1f;
    public float currentReputationMultiplier { get; private set; } = 1f;

    public float FundsPenalty {
      get {return currentFundsMultiplier*baseFundsHit;}
    }
    public float SciencePenalty {
      get {return currentScienceMultiplier*baseScienceHit;}
    }
    public float ReputationPenalty {
      get {return currentReputationMultiplier*baseReputationHit;}
    }

    public PenaltyEffects()
    {

    }

    /// <summary>
    /// String representation
    /// </summary>
    public void ToString()
    {
      return String.Format("- {0} Science (x{1})\n - {2} Reputation (x{3})\n - {4} Funds (x{5})", baseScienceHit, currentScienceMultiplier, baseReputationHit, currentReputationMultiplier, baseFundsHit, currentFundsMultiplier );
    }

    /// <summary>
    /// Apply penalties to the player
    /// </summary>
    public void ApplyPenalties()
    {
      PenaltyHelpers.ApplyPenalties(ReputationPenalty, FundsPenalty, SciencePenalty);
    }

    /// <summary>
    /// Update locational multipliers
    /// </summary>
    /// <param name="v">The Vessel around which to base this penalty</param>
    public void UpdateMultipliers(Vessel v)
    {
      currentFundsMultiplier = PenaltyHelpers.CalculateReputationLoss(v);
      currentScienceMultiplier  = PenaltyHelpers.CalculateReputationLoss(v);
      currentReputationMultiplier = PenaltyHelpers.CalculateReputationLoss(v);
    }

    /// <summary>
    /// Add a reputation penalty
    /// </summary>
    /// <param name="amount">The amount to lose</param>
    public void AddReputationPenalty(float amount)
    {
      baseReputationHit += amount;
    }

    /// <summary>
    /// Add a science penalty
    /// </summary>
    /// <param name="amount">The amount to lose</param>
    public void AddSciencePenalty(float amount)
    {
      BaseScienceHit += amount;
    }

    /// <summary>
    /// Add a funds penalty
    /// </summary>
    /// <param name="amount">The amount to lose</param>
    public void AddFundsPenalty(float amount)
    {
      baseFundsHit += amount;
    }

    /// <summary>
    /// Add a generic penalty
    /// </summary>
    /// <param name="amount">The amount to lose</param>
    /// <param name="pType">The type of penalty</param>
    public void AddPenalty(PenaltyType pType, float amount)
    {
      if (amount > 0f)
      {
        switch pType:
          case PenaltyType.Science:
            baseScienceHit += amount;
            break;
          case PenaltyType.Funds:
            baseFundsHit += amount;
            break;
          case PenaltyType.Reputation:
            baseReputationHit += amount;
            break;
      }
    }
  }
}
