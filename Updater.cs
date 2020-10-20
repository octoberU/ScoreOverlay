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
                    UI.FadeInOverlay();
                }
            }
        }
    }
}
