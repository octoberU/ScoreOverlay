using System.IO;
using UnityEngine;

namespace ScoreOverlay
{
    public static class Utility
    {
        public static string modsPath = Application.dataPath + "/../Mods/";
        public static string overlayPath = modsPath + "scoreoverlay.overlay";

        public static bool RunSafetyChecks() // Returns true if all checks pass and the mod has been installed correctly.
        {
            bool overlayAssetExists = File.Exists(overlayPath);
            return overlayAssetExists;
        }
    }
}
