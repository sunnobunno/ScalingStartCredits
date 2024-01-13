using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace ScalingStartCredits.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartingCreditsPatch
    {
        
        private static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(ScalingStartCreditsBase.modGUID);

        private static int groupCreditIncreaseIncrement;
        private static int playerThreshold;
        private static int playerCount;
        private static int days;

        private static HUDManager hudManager;

        [HarmonyPatch("StartGame")]
        [HarmonyPrefix]
        private static void StartMatchLeverPatch(ref StartOfRound ___playersManager)
        {
            groupCreditIncreaseIncrement = ScalingStartCreditsBase.Instance.configCreditIncrement.Value;
            playerThreshold = ScalingStartCreditsBase.Instance.configPlayerCountThreshold.Value;
            playerCount = ___playersManager.fullyLoadedPlayers.Count;
            days = ___playersManager.gameStats.daysSpent;
            bool conditionsMet = true;


            mls.LogInfo($"Days: {days}");
            mls.LogInfo($"Player Count: {playerCount}");

            if ( days != 0)
            {
                mls.LogInfo($"No longer first day. Aborting");
                conditionsMet = false;
            }

            if (playerCount <= playerThreshold && conditionsMet)
            {
                mls.LogInfo($"Player threshold not met. Aborting");
                conditionsMet = false;
            }

            if (conditionsMet)
            {
                hudManager = (HUDManager)UnityEngine.Object.FindObjectOfType(typeof(HUDManager));
                Terminal terminal = (Terminal)UnityEngine.Object.FindObjectOfType(typeof(Terminal));

                int additionalPlayers = playerCount - playerThreshold;
                int increaseInGroupCredits = additionalPlayers * groupCreditIncreaseIncrement;
                int newGroupCredits = terminal.groupCredits + increaseInGroupCredits;
                terminal.SyncGroupCreditsServerRpc(newGroupCredits, terminal.numberOfItemsInDropship);


                mls.LogInfo($"Credits Added for {additionalPlayers} players above threshold of {playerThreshold}: {increaseInGroupCredits}");
                mls.LogInfo($"Group Credits: {terminal.groupCredits}");

                hudManager.AddTextToChatOnServer($"Credits Added for {additionalPlayers} players above threshold of {playerThreshold}: {increaseInGroupCredits}");
                hudManager.AddTextToChatOnServer($"Group Credits: {terminal.groupCredits}");
            }
        }
    }
}
