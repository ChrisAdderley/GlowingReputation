using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlowingReputation
{
    public enum PenaltyType
    {
      Reputation,
      Funds,
      Science
    }

    public class BodyPenaltyData
    {
        public string BodyName;
        public FloatCurve ReputationPenaltyCurve;
        public FloatCurve FundsPenaltyCurve;
        public FloatCurve SciencePenaltyCurve = new FloatCurve();

        public Dictionary<string, float> ReputationBiomeScalars = new Dictionary<string, float>();
        public Dictionary<string, float> ScienceBiomeScalars = new Dictionary<string, float>();
        public Dictionary<string, float> FundsBiomeScalars = new Dictionary<string, float>();

        // Basic constructor
        public BodyPenaltyData(string nm)
        {
            bodyName = nm;
        }
        // Constructor given a configNode
        public BodyPenaltyData(ConfigNode node)
        {
            Load(node);
        }

        // Loads from a configNode
        public void Load(ConfigNode node)
        {
          node.TryParse("name", ref bodyName);

          ReputationPenaltyCurve = Utils.GetValue(node, "reputationCurve", new FloatCurve());
          FundsPenaltyCurve = Utils.GetValue(node, "fundsCurve", new FloatCurve());
          SciencePenaltyCurve = Utils.GetValue(node, "scienceCurve", new FloatCurve());

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

        // Gets the biome scaling factor for the biome if it exists given the penalty type
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

        // Gets the altitude scaling factor for the penalty type
        public float GetBodyMultiplier(PenaltyType penalty, double altitude)
        {
            switch (penalty)
            {
              case PenaltyType.Reputation:
                return ReputationPenaltyCurve.Evaluate((float)altitude);
              case PenaltyType.Funds:
                return FundsPenaltyCurve.Evaluate((float)altitude);
              case PenaltyType.Science:
                return SciencePenaltyCurve.Evaluate((float)altitude);
            }
            return 0f;
        }
    }
}
