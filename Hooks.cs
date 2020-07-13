using Harmony;
using MelonLoader;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

namespace AudicaModding
{
    internal static class Hooks
    {
        public static void ApplyHooks(HarmonyInstance instance)
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(GameplayStats), "ReportTargetHit", new Type[] { typeof(SongCues.Cue), typeof(float), typeof(Vector2), })]
        private static class ReportTarget
        {
            private static void Postfix(GameplayStats __instance, SongCues.Cue cue, float tick, Vector2 targetHitPos)
            {
                if (!KataConfig.I.practiceMode && ScoreOverlay.showTimingBar)
                {
                    float TargetError = (float)cue.tick - tick;
                    ScoreOverlay.TimingError.Add(TargetError);
                    if (ScoreOverlay.TimingError.Count > 1)
                    {
                        ScoreOverlay.AverageTimingText.text = ScoreOverlay.TimingError.Average().ToString("F") + "ms";
                    }
                    ScoreOverlay.MoveHitRect(TargetError * -1, KataConfig.I.GetTargetColor(cue.handType));
                }
            }
        }

        [HarmonyPatch(typeof(InGameUI), "Restart", new Type[0])]
        private static class Restart
        {
            private static void Postfix(InGameUI __instance)
            {
                if (!KataConfig.I.practiceMode)
                {
                    MelonCoroutines.Start(ScoreOverlay.StartScoreDisplay());
                }
                if (ScoreOverlay.myCanvas != null)
                {
                    GameObject.Destroy(ScoreOverlay.myCanvas);
                    ScoreOverlay.scoreDisplayEnabled = false;
                }
                ScoreOverlay.HitRectangles.Clear();
                ScoreOverlay.TimingError.Clear();
            }
        }

        [HarmonyPatch(typeof(InGameUI), "ReturnToSongList", new Type[0])]
        private static class ReturnToSongList
        {
            private static void Postfix(InGameUI __instance)
            {
                if (ScoreOverlay.myCanvas != null)
                {
                    GameObject.Destroy(ScoreOverlay.myCanvas);
                }
                ScoreOverlay.scoreDisplayEnabled = false;
                ScoreOverlay.HitRectangles.Clear();
                ScoreOverlay.TimingError.Clear();
            }
        }

        [HarmonyPatch(typeof(AudioDriver), "StartPlaying", new Type[0])]
        private static class StartPlaying
        {
            private static void Postfix(AudioDriver __instance)
            {
                if (!KataConfig.I.practiceMode)
                {
                    if (!KataConfig.I.practiceMode)
                    {
                        MelonCoroutines.Start(ScoreOverlay.StartScoreDisplay());
                    }
                    ScoreOverlay.HitRectangles.Clear();
                    ScoreOverlay.TimingError.Clear(); 
                }
            }
        }

    }
}
