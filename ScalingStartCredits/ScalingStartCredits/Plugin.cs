using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ScalingStartCredits
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ScalingStartCreditsBase : BaseUnityPlugin
    {
        public const string modGUID = "sunnobunno.scalingstartcredits";
        public const string modName = "Scaling Start Credits";
        public const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ScalingStartCreditsBase? Instance;

        internal ManualLogSource? mls;

        public ConfigEntry<int> configCreditIncrement;
        public ConfigEntry<int> configPlayerCountThreshold;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            BindConfiguration();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"{modGUID} is loading.");

            harmony.PatchAll();
            mls.LogInfo($"{modGUID} is loaded.");
        }

        private void BindConfiguration()
        {
            configCreditIncrement = Config.Bind("General",
                "Credit Increment",
                15,
                "The amount of credits per player added to the starting group credits");

            configPlayerCountThreshold = Config.Bind("General",
                "Player Threshold",
                4,
                "The number of players required in the lobby before credits are added per new player.");
        }
    }
}
