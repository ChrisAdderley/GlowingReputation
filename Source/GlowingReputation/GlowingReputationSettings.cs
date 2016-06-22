using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace GlowingReputation
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class GlowingReputation : MonoBehaviour
    {
        public static GlowingReputation Instance { get; private set; }

        protected void Awake()
        {
            Instance = this;
        }
        protected void Start()
        {
            GlowingReputationSettings.Load();
        }
    }

    public static class GlowingReputationSettings
    {

        public static Dictionary<string, BodyReputationData> ReputationData;

        public static void Load()
        {
           ConfigNode settingsNode;
           ReputationData = new Dictionary<string, BodyReputationData>();

           Utils.Log("Settings: Started loading");
           if (GameDatabase.Instance.ExistsConfigNode("GlowingReputation/GLOWINGREPUTATIONSETTINGS"))
           {
               Utils.Log("Settings: Located settings file");

               settingsNode = GameDatabase.Instance.GetConfigNode("GlowingReputation/GLOWINGREPUTATIONSETTINGS");
               ConfigNode[] repNodes = settingsNode.GetNodes("REPUTATIONBODY");

               foreach (ConfigNode repNode in repNodes)
               {
                   BodyReputationData dat = new BodyReputationData(repNode.GetValue("name"));
                   dat.bodyRepCurve = Utils.GetValue(repNode, "reputationCurve", new FloatCurve());

                   ReputationData.Add(dat.bodyName, dat);
                   Utils.Log("Settings: Loaded data for " + dat.bodyName);
               }
           }
           else
           {
               Utils.Log("Settings: Couldn't find settings file, using defaults");
           }
            Utils.Log("Settings: Finished loading");
    }

        
  }
}
