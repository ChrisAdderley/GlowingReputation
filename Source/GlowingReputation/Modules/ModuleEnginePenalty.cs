using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{
  public class ModuleEnginePenalty:PartModule
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

    [KSPField(isPersistant = false)]
    public string EngineID;

    private ModuleEngines engine;

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
      {
        if (EngineID == "" || EngineID == String.Empty)
        {
          Utils.LogWarning("[ModuleEnginePenalty]: EngineID field not specified, trying to use default engine");
          if (engines.Length > 0)
            engine = engines[0];
        }
        foreach (ModuleEnginesFX fx in engines)
        {
          if (fx.engineID == EngineID)
            engine = fx;
        }
      }

      if (engine == null)
        Utils.LogError("[ModuleEnginePenalty]: Couldn't find an engine module");
    }

    public string GetModuleTitle()
    {
      return "Dangerous Engine";
    }

    public override string GetInfo()
    {
      string outStr = String.Format("Running in sensitive environments has negative consequences. \n\n");
      if (BaseReputationLoss > 0f)
        outStr += String.Format("<b>Reputation Loss</b>: {0:F2} Rep/s\n", BaseReputationLoss);
      if (BaseFundsLoss > 0f)
        outStr += String.Format("<b>Funds Loss</b>: {0:F2} Rep/s\n", BaseFundsLoss);
      if (BaseScienceLoss > 0f)
        outStr += String.Format("<b>Reputation Loss</b>: {0:F2} Rep/s\n", BaseScienceLoss);
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
          Status = "";
          if (BaseReputationLoss > 0f)
            Status += String.Format("{0:F2} Rep/s\n", PenaltyHelpers.CalculateReputationLoss(this.vessel) * BaseReputationLoss);
          if (BaseFundsLoss > 0f)
            Status += String.Format("{0:F2} Funds/s\n", PenaltyHelpers.CalculateFundsLoss(this.vessel) * BaseFundsLoss);
          if (BaseScienceLoss > 0f)
            Status += String.Format("{0:F2} Science/s", PenaltyHelpers.CalculateScienceLoss(this.vessel) * BaseScienceLoss);

        }
      }
    }

    protected void LoseReputation()
    {
      float engineScale =  GetEngineScale();

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
