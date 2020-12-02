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
                if (ScoreOverlayMod.ui.loaded)
                {
                    ScoreOverlayMod.ResetTracker();
                    ScoreOverlayMod.ui.FadeInOverlay();
                    ScoreOverlayMod.ui.UpdateUiInfo(SongDataHolder.I.songData);
                    
                    
                }
            }
        }

        [HarmonyPatch(typeof(InGameUI), "ReturnToSongList", new Type[0])]
        private static class ReturnToSongList
        {
            private static void Postfix(InGameUI __instance)
            {
                ScoreOverlayMod.ui.FadeOutOverlay();
            }


        }

        [HarmonyPatch(typeof(InGameUI), "Restart", new Type[0])]
        private static class Restart
        {
            private static void Postfix(InGameUI __instance)
            {
                ScoreOverlayMod.ui.FadeOutOverlay();

            }
        }


    }
}
