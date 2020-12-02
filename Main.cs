using MelonLoader;
using ScoreOverlay;
using TMPro;
using UnityEngine;

[assembly: MelonGame("Harmonix Music Systems, Inc.", "Audica")]
[assembly: MelonInfo(typeof(ScoreOverlayMod), "Score Overlay", "2.0.3", "octo", "https://github.com/octoberU/ScoreOverlay")]
[assembly: MelonOptionalDependencies("SongBrowser")]

namespace ScoreOverlay
{
    public class ScoreOverlayMod : MelonMod
    {
        public ScoreKeeper scoreKeeper;
        public static UI ui;

        static int score = 0;
        static int streak = 0;

        public override void OnApplicationStart()
        {
            ui = new UI();
            Config.RegisterConfig();

        }

        public override void OnLevelWasInitialized(int level)
        {
            if (level == 1) ui.Initialize();
        }

        public override void OnModSettingsApplied()
        {
            Config.OnModSettingsApplied();
            ui.OnModSettingsApplied();
        }

        public override void OnUpdate()
        {
            if (scoreKeeper != null)
            {
                if (scoreKeeper.mScore != score || scoreKeeper.mStreak != streak)
                {
                    score = scoreKeeper.mScore;
                    streak = scoreKeeper.mStreak;
                    ui.UpdateDisplay(score, streak);
                    ui.UpdateHighscore(score, scoreKeeper.mHighScore);
                }
            }
            else scoreKeeper = ScoreKeeper.I;
        }

        public static void ResetTracker()
        {
            score = streak = 0;
        }

    }
}



