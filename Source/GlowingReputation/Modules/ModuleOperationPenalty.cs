using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{
  public class ModuleOperationPenalty: PartModule
  {
    // In rep/s at max thrust
    [KSPField(isPersistant = false)]
    public float BaseReputationLoss = 1f;

    [KSPField(isPersistant = false)]
    public float BaseFundsLoss = 1f;

    [KSPField(isPersistant = false)]
    public float BaseScienceLoss = 1f;

    // Reputation Status string
    [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Rep. Loss")]
    public string Status = "";

    private List<ModuleEngines> validEngines;

    public void Start()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        SetupEngines();
      }
    }

    void SetupEngines()
    {
      ModuleEngines[] engines = this.GetComponents<ModuleEngines>();
      if (engines.Length > 0)
        validEngines = engine.ToList();
      else
        Utils.LogError("[ModuleEnginePenalty]: Couldn't find an engine module");
    }

    public string GetModuleTitle()
    {
      return Localizer.Format("#LOC_GlowingRepuation_ModulePenaltyOperation_title");
    }

    public override string GetInfo()
    {
      string outStr = Localizer.Format("#LOC_GlowingRepuation_ModulePenaltyOperation_description");
      if (BaseReputationHit > 0.0f)
        outStr += Localizer.Format("#LOC_GlowingRepuation_ModulePenaltyOperation_description_rep", BaseReputationHit.ToString("F2"));
      if (BaseFundsHit > 0.0f)
        outStr += Localizer.Format("#LOC_GlowingRepuation_ModulePenaltyOperation_description_science", BaseFundsHit.ToString("F2"));
      if (BaseScienceHit > 0.0f)
        outStr += Localizer.Format("#LOC_GlowingRepuation_ModulePenaltyOperation_description_funds", BaseScienceHit.ToString("F2"));
      return outStr;
    }

    public void FixedUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        if (engine.EngineIgnited && engine.requestedThrottle > 0f)
        {
          LoseReputation();
        }
        else
        {
        }
      }
    }

    protected void LoseReputation()
    {
      float engineScale = GetEngineScale();

      float sciLoss = PenaltyHelpers.CalculateScienceLoss(this.vessel) * BaseScienceLoss;
      float fundsLoss = PenaltyHelpers.CalculateScienceLoss(this.vessel) * BaseFundsLoss;
      float repLoss = PenaltyHelpers.CalculateScienceLoss(this.vessel) * BaseReputationLoss;

      Status = "";
      if (BaseReputationLoss > 0f)
        Status += String.Format("{0:F2} Rep/s\n", PenaltyHelpers.CalculateReputationLoss(this.vessel) * BaseReputationLoss);
      if (BaseFundsLoss > 0f)
        Status += String.Format("{0:F2} Funds/s\n", PenaltyHelpers.CalculateFundsLoss(this.vessel) * BaseFundsLoss);
      if (BaseScienceLoss > 0f)
        Status += String.Format("{0:F2} Science/s", PenaltyHelpers.CalculateScienceLoss(this.vessel) * BaseScienceLoss);


      if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
        Reputation.Instance.AddReputation(-repLoss*TimeWarp.fixedDeltaTime, TransactionReasons.None);
    }

    protected float GetEngineScale()
    {
      return (engine.requestedMassFlow/engineFX.maxFuelFlow);
    }
  }
}
