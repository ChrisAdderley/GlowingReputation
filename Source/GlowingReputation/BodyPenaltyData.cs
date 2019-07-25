using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlowingReputation
{
  /// <summary>
  /// This class stores penalty data for a body
  /// </summary>
  public class BodyPenaltyData
  {
    public string BodyName;

    public FloatCurve ReputationPenaltyCurve  = new FloatCurve();
    public FloatCurve FundsPenaltyCurve  = new FloatCurve();
    public FloatCurve SciencePenaltyCurve = new FloatCurve();

    public Dictionary<string, float> ReputationBiomeScalars = new Dictionary<string, float>();
    public Dictionary<string, float> ScienceBiomeScalars = new Dictionary<string, float>();
    public Dictionary<string, float> FundsBiomeScalars = new Dictionary<string, float>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nm">The name of the CelestialBody that is considered </param>
    public BodyPenaltyData(string nm)
    {
      bodyName = nm;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">The ConfigNode to build the class from</param>
    public BodyPenaltyData(ConfigNode node)
    {
      Load(node);
    }

    /// <summary>
    /// Load the class from a ConfigNode
    /// </summary>
    /// <param name="node">The ConfigNode to build the class from</param>
    public void Load(ConfigNode node)
    {
      node.TryParse("name", ref bodyName);

      ReputationPenaltyCurve.Load(node.GetValue("reputationCurve"));
      FundsPenaltyCurve.Load(node.GetValue("fundsCurve"));
      SciencePenaltyCurve.Load(node.GetValue("scienceCurve"));

      ReputationBiomeScalars = new Dictionary<string, float>();
      ScienceBiomeScalars = new Dictionary<string, float>();
      FundsBiomeScalars = new Dictionary<string, float>();

      biomeNodes = node.GetNodes("BIOME")
      foreach (ConfigNode biomeNode in biomeNodes)
      {
        string name;
        float repuationScalar;
        float fundsScalar;
        float scienceScalar;

        if (biomeNode.TryParse("name", ref name) )
        {
          if (biomeNode.TryParse("reputationMultiplier", ref reputationScalar))
            ReputationBiomeScalars.Add(name, reputationScalar);
          f (biomeNode.TryParse("fundsMultiplier", ref fundsScalar))
            FundsBiomeScalars.Add(name, fundsScalar);
          if (biomeNode.TryParse("scienceMultiplier", ref scienceScalar))
            ScienceBiomeScalars.Add(name, scienceScalar);
        }
      }
    }

    /// <summary>
    /// Gets a scaling factor for a penalty type given a specific biome
    /// </summary>
    /// <param name="penalty">The PenaltyType to retrieve scaling factor</param>
    /// <param name="biomeName">The name of the biome</param>
    public float GetBiomeMultiplier(PenaltyType penalty, string biomeName)
    {
      if (biomeName != "")
      {
        float scale = 1f;

        switch (penalty)
        {
          case PenaltyType.Reputation:
            if (ReputationBiomeScalars.TryGetValue("biomeName", out scale)
              return scale;
            break;
          case PenaltyType.Funds:
            if (FundsBiomeScalars.TryGetValue("biomeName", out scale)
              return scale;
            break;
          case PenaltyType.Science:
            if (ScienceBiomeScalars.TryGetValue("biomeName", out scale)
              return scale;
            break;
        }
      }
      return 1f;
    }

    /// <summary>
    /// Gets the scaling factor for a penalty type given an altitude above the body
    /// </summary>
    /// <param name="penalty">The PenaltyType to retrieve scaling factor</param>
    /// <param name="biomeName">The name of the biome</param>
    public float GetBodyMultiplier(PenaltyType penalty, double altitude)
    {
      switch (penalty)
      {
        case PenaltyType.Reputation:
          return ReputationPenaltyCurve.Evaluate((float)altitude);
          break;
        case PenaltyType.Funds:
          return FundsPenaltyCurve.Evaluate((float)altitude);
          break;
        case PenaltyType.Science:
          return SciencePenaltyCurve.Evaluate((float)altitude);
          break;
      }
      return 0f;
    }
  }
}
