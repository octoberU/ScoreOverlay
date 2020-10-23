using MelonLoader;
using ScoreOverlay;


[assembly: MelonGame("Harmonix Music Systems, Inc.", "Audica")]
[assembly: MelonInfo(typeof(ScoreOverlayMod), "Score Overlay", "2.0.0", "octo", "https://github.com/octoberU/ScoreOverlay")]
[assembly: MelonOptionalDependencies("SongBrowser")]

namespace ScoreOverlay
{
    public class ScoreOverlayMod : MelonMod
    {
        public ScoreKeeper scoreKeeper;

        static int score = 0;
        static int streak = 0;

        public override void OnApplicationStart()
        {
            ScoreOverlayConfig.RegisterConfig();
            
            Utility.RunSafetyChecks();
        }

        public override void OnLevelWasInitialized(int level)
        {
            if (level == 1) UI.Initialize();
        }

        public override void OnModSettingsApplied()
        {
            ScoreOverlayConfig.OnModSettingsApplied();
        }

        public override void OnUpdate()
        {
            if (scoreKeeper != null)
            {
                ScoreKeeper scoreKeeper = ScoreKeeper.I;
                if (scoreKeeper.mScore != score || scoreKeeper.mStreak != streak)
                {
                    score = scoreKeeper.mScore;
                    streak = scoreKeeper.mStreak;
                    UI.UpdateDisplay(score, streak);
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



