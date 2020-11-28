using MelonLoader;
using System.Reflection;

namespace ScoreOverlay
{
    public static class Config
    {
        public const string Category = "ScoreOverlay";

        public static bool ShowModifiers;
        public static bool ShowScore;
        public static bool ShowSongInfo;
        public static bool ShowHighscoreDifference;

        public static float OverlayScale;

        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(ShowModifiers), true, "Display active modifiers during gameplay.");
            MelonPrefs.RegisterBool(Category, nameof(ShowScore), true, "Display score & streak during gameplay.");
            MelonPrefs.RegisterBool(Category, nameof(ShowSongInfo), true, "Display song info during gameplay.");
            MelonPrefs.RegisterBool(Category, nameof(ShowHighscoreDifference), true, "Display score difference from your highscore");

            MelonPrefs.RegisterFloat(Category, nameof(OverlayScale), 1.4f, "Changes the scale of the overlay [0,5,0.2,1.4]");

            OnModSettingsApplied();
        }

        public static void OnModSettingsApplied()
        {
            foreach (var fieldInfo in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.FieldType == typeof(int))
                    fieldInfo.SetValue(null, MelonPrefs.GetInt(Category, fieldInfo.Name));

                if (fieldInfo.FieldType == typeof(bool))
                    fieldInfo.SetValue(null, MelonPrefs.GetBool(Category, fieldInfo.Name));
                
                if (fieldInfo.FieldType == typeof(float))
                    fieldInfo.SetValue(null, MelonPrefs.GetFloat(Category, fieldInfo.Name));
            }
        }
    }
}
