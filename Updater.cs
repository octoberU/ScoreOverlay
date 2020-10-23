using Harmony;
using MelonLoader;
using System;
using System.Globalization;

namespace ScoreOverlay
{
    internal static class Updater
    {
        [HarmonyPatch(typeof(AudioDriver), "StartPlaying", new Type[0])]
        private static class ScoreUpdater
        {
            private static void Postfix(AudioDriver __instance)
            {
                if (UI.loaded)
                {
                    UI.UpdateUiInfo(SongDataHolder.I.songData);
                    UI.FadeInOverlay();
                    
                }
            }
        }

        [HarmonyPatch(typeof(InGameUI), "ReturnToSongList", new Type[0])]
        private static class ReturnToSongList
        {
            private static void Postfix(InGameUI __instance)
            {
                UI.FadeOutOverlay();
            }
        }

        [HarmonyPatch(typeof(InGameUI), "Restart", new Type[0])]
        private static class Restart
        {
            private static void Postfix(InGameUI __instance)
            {
                UI.FadeOutOverlay();
            }
        }

        [HarmonyPatch(typeof(ScoreKeeperDisplay), "UpdateStreak", new Type[0])]
        private static class ScoreKeeperDisplayUpdate
        {
            private static void Postfix(ScoreKeeperDisplay __instance)
            {
                UI.UpdateDisplay();
            }
        }
    }
}
