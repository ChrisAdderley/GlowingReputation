// Utils
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GlowingReputation
{

  public static class Utils
  {
      public static float GetReputationScale(CelestialBody body, double altitude)
      {
          // if planet not in DB, no reputation penalty
          if (GlowingReputationSettings.ReputationData == null)
          {
              Utils.LogWarning("Could not locate the reputation data");
              return 0f;
          }
          else if(!GlowingReputationSettings.ReputationData.ContainsKey(body.name))
          {
              return 0f;
          }
          else
          {
              return GlowingReputationSettings.ReputationData[body.name].GetReputationScale(altitude);
          }
      }

    // Node loading
      // several variants for data types
    public static string GetValue(ConfigNode node, string nodeID, string defaultValue)
    {
        if (node.HasValue(nodeID))
        {
            return node.GetValue(nodeID);
        }
        return defaultValue;
    }
    public static int GetValue(ConfigNode node, string nodeID, int defaultValue)
    {
        if (node.HasValue(nodeID))
        {
            int val;
            if (int.TryParse(node.GetValue(nodeID), out val))
                return val;
        }
        return defaultValue;
    }
    public static float GetValue(ConfigNode node, string nodeID, float defaultValue)
    {
        if (node.HasValue(nodeID))
        {
            float val;
            if (float.TryParse(node.GetValue(nodeID), out val))
                return val;
        }
        return defaultValue;
    }
    public static double GetValue(ConfigNode node, string nodeID, double defaultValue)
    {
        if (node.HasValue(nodeID))
        {
            double val;
            if (double.TryParse(node.GetValue(nodeID), out val))
                return val;
        }
        return defaultValue;
    }
    public static bool GetValue(ConfigNode node, string nodeID, bool defaultValue)
    {
        if (node.HasValue(nodeID))
        {
            bool val;
            if ( bool.TryParse(node.GetValue(nodeID), out val))
                return val;
        }
        return defaultValue;
    }
      // Based on some Firespitter code by Snjo
    public static FloatCurve GetValue(ConfigNode node, string nodeID, FloatCurve defaultValue)
    {
        if (node.HasValue(nodeID))
        {
            FloatCurve theCurve = new FloatCurve();
            ConfigNode[] nodes = node.GetNodes(nodeID);
            for (int i = 0; i < nodes.Length; i++)
            {
                string[] valueArray = nodes[i].GetValues("key");
            
                for (int l = 0; l < valueArray.Length; l++)
                {
                    string[] splitString = valueArray[l].Split(' ');
                    Vector2 v2 = new Vector2(float.Parse(splitString[0]), float.Parse(splitString[1]));
                    theCurve.Add(v2.x, v2.y, 0, 0);
                }
            }
            return theCurve;
        }
        return defaultValue;
    }

    public static void Log(string str)
    {
        Debug.Log("GlowingReputation > " + str);
    }
    public static void LogError(string str)
    {
        Debug.LogError("GlowingReputationy > " + str);
    }
    public static void LogWarning(string str)
    {
        Debug.LogWarning("GlowingReputation > " + str);
    }


  }

}
