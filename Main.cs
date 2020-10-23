using MelonLoader;
using ScoreOverlay;


[assembly: MelonGame("Harmonix Music Systems, Inc.", "Audica")]
[assembly: MelonInfo(typeof(ScoreOverlayMod), "Score Overlay", "2.0.0", "octo", "https://github.com/octoberU/ScoreOverlay")]
[assembly: MelonOptionalDependencies("SongBrowser")]

namespace ScoreOverlay
{
    public class ScoreOverlayMod : MelonMod
    {
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
    }
}



