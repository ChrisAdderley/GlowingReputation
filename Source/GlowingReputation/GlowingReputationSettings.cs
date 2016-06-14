using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace GlowingReputation
{

    public static class GlowingReputationSettings
    {

        public static Dictionary<string, BodyReputationData> ReputationData;

        public static void Load()
        {
            ConfigNode settingsNode;

           Utils.Log("Settings: Started loading");
           if (GameDatabase.Instance.ExistsConfigNode("GlowingReputation/GLOWINGREPUTATIONSETTINGS"))
           {
               Utils.Log("Settings: Located settings file");
               settingsNode = GameDatabase.Instance.GetConfigNode("GlowingReputation/GLOWINGREPUTATIONSETTINGS");


               ConfigNode[] repNodes = settingsNode.GetNodes("REPUTATIONBODY");

               foreach (ConfigNode repNode in repNodes)
               {
                   BodyReputationData dat = new BodyReputationData(repNode.GetValue("name"));
               }


               //raycastDistance = Utils.GetValue(settingsNode, "RaycastDistance", 2000f);
               //fluxCutoff = Utils.GetValue(settingsNode, "FluxCutoff", 0f);
               //defaultRaycastFluxStart = Utils.GetValue(settingsNode, "RaycastFluxStart", 1.0f);
               //maximumPositionDelta = Utils.GetValue(settingsNode, "RaycastPositionDelta", 0.5f);
               //maximumMassDelta = Utils.GetValue(settingsNode, "RaycastMassDelta", 0.05f);
               //defaultPartAttenuationCoefficient = Utils.GetValue(settingsNode, "DefaultMassAttenuationCoefficient", 1.5f);
               //defaultDensity = Utils.GetValue(settingsNode, "DefaultDensity", 0.5f);

               //overlayRayWidthMult = Utils.GetValue(settingsNode, "OverlayRayWidthMultiplier", 0.005f);
               //overlayRayWidthMin = Utils.GetValue(settingsNode, "OverlayRayMinimumWidth", 0.05f);
               //overlayRayWidthMax = Utils.GetValue(settingsNode, "OverlayRayMaximumWidth", 0.5f);
               //overlayRayLayer = Utils.GetValue(settingsNode, "OverlayRayLayer", 0);
               //overlayRayMaterial = Utils.GetValue(settingsNode, "OverlayRayMaterial", "GUI/Text Shader");

               //simulatePointRadiation = Utils.GetValue(settingsNode, "EnablePointRadiation", true);
               //simulateCosmicRadiation = Utils.GetValue(settingsNode, "EnableCosmicRadiation", false);
               //simulateSolarRadiation = Utils.GetValue(settingsNode, "EnableSolarRadiation", false);

               //enableKerbalEffects = Utils.GetValue(settingsNode, "EnableKerbalEffects", true);
               //enableScienceEffects = Utils.GetValue(settingsNode, "EnableScienceEffects", true);
               //enableProbeEffects = Utils.GetValue(settingsNode, "EnableProbeEffects", true);

               //enableKerbalDeath = Utils.GetValue(settingsNode, "EnableKerbalDeath", false);
               //kerbalSicknessThreshold = Utils.GetValue(settingsNode, "RadiationSicknessThreshold", 1f);
               //kerbalDeathThreshold = Utils.GetValue(settingsNode, "RadiationDeathThreshold", 10f);
               //kerbalHealRate = Utils.GetValue(settingsNode, "RadiationHealRate", 0.00001157407407);
               //kerbalHealRateKSC = Utils.GetValue(settingsNode, "RadiationHealRateKSC", 0.0001157407407);

               //debugUI = Utils.GetValue(settingsNode, "DebugUI", true);
               //debugOverlay = Utils.GetValue(settingsNode, "DebugOverlay", true);
               //debugNetwork = Utils.GetValue(settingsNode, "DebugNetwork", true);
               //debugRaycasting = Utils.GetValue(settingsNode, "DebugRaycasting", true);
               //debugSourceSinks = Utils.GetValue(settingsNode, "DebugSourcesAndSinks", true);
               //debugModules = Utils.GetValue(settingsNode, "DebugModules", true);
           }
           else
           {
               Utils.Log("Settings: Couldn't find settings file, using defaults");
           }
            Utils.Log("Settings: Finished loading");
    }

        
  }
}
