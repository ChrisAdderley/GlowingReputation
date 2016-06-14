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
          if (!GlowingReputationSettings.ReputationData.ContainsKey(body.name))
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

  //<NathanKell> take the bounding box of the dragcube
  //[13:54] <NathanKell> (the product of the final triplet in a DRAG_CUBE entry)
//[13:55] <NathanKell> Then multiply by the Y area divided by the X*Z dimensions
//[13:55] <NathanKell> (i.e. how much of the Y-facing area of the rectangle is in fact the part)
//[13:55] NathanKell> Then take the minimum of (the X and Z portions) , then add 2 * (x portion * zPortion), then divide by 3.
//13:56] <NathanKell> that is the final multiplier
//[13:56] <NathanKell> in effect we're figuring out, from projects from three axes, how much of the cube is hollow and how much is filled with th part
//[13:56] <NathanKell> the8
// [13:56] <NathanKell> the**
// size = Nertea: part.DragCubes.WeightedSize
//displacement = cube * xzPortion * yPortion
//where yPortion = yPortion = areas[2] / (size.x * size.z); and
//xPortion = areas[0] / (size.y * size.z)?
//zPortion = areas[1] / (size.y * size.x)?
//and xPortion and zPortion are calculated like yPortion
//[13:59] <NathanKell> (areas is a a float[6] from dragcubes.WeightedArea[] )z)
// xzPortion = (Math.Min(xPortion, zPortion) + 2d * (xPortion * zPortion)) * (1d / 3d);

}
