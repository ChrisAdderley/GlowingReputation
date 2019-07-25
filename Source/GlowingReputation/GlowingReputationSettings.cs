using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace GlowingReputation
{

  /// <summary>
  /// KSPAddon to load settings and data for the mod
  /// </summary>

  [KSPAddon(KSPAddon.Startup.MainMenu, false)]
  public class GlowingReputation : MonoBehaviour
  {

    public bool FirstLoad = true;

    public static GlowingReputation Instance { get; private set; }

    protected void Awake()
    {
      Instance = this;
    }
    protected void Start()
    {
      GlowingReputationSettings.Load();
      GlowingReputationData.Load();
    }
  }

  /// <summary>
  /// Class to load and hold configurable settings
  /// </summary>
  public static class Settings
  {
    public bool DebugUIMode = true;
    public bool DebugMode = true;

    public float MultiplierWarningLevel = 0.0f;
    public float DangerWarningLevel = 1.0f;

    public static void Load()
    {
      ConfigNode settingsNode;
      Utils.Log("[Settings]: Started loading");
      if (GameDatabase.Instance.ExistsConfigNode("GlowingReputation/GlowingReputationSettings"))
      {
        Utils.Log("[Settings]: Located settings file");
        settingsNode = GameDatabase.Instance.GetConfigNode("GlowingReputation/GLOWINGREPUTATIONSETTINGS");
      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults");
      }
      Utils.Log("[Settings]: Finished loading");
    }
  }
}
