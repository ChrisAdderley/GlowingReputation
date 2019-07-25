using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace GlowingReputation
{
  /// <summary>
  /// Class to load and hold the penalty data for the mod
  /// </summary>
  public static class GlowingReputationData
  {
    public static Dictionary<string, BodyPenaltyData> BodyData;

    public static void Load()
    {
      BodyData = new Dictionary<string, BodyPenaltyData>();
      Utils.Log("[Data]: Started loading");
      ConfigNode[] bodyDataNodes = GameDatabase.GetConfigNodes("GlowingReputationBodyData");

      foreach (ConfigNode bodyDataNode in bodyDataNodes)
      {
        BodyData dat = new BodyPenaltyData(repNode);
        BodyData.Add(dat.bodyName, dat);
        Utils.Log("[Data]: Loaded penalty data for " + dat.bodyName);
      }
      Utils.Log("[Data]: Finished loading");
    }
  }
}
