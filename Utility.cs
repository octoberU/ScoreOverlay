using Harmony;
using System.Collections.Generic;
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

        public static string RemoveFormatting(string input)
        {
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            return rx.Replace(input, "");
        }

        public static string GetModText()
        {
            var modifiers = GameplayModifiers.I;
            if (modifiers != null)
            {
                var mods = modifiers.GetCurrentModifiers();
                List<string> modStrings = new List<string>();
                for (int i = 0; i < mods.Length; i++)
                {
                    switch (mods[i])
                    {
                        case GameplayModifiers.Modifier.FastTargets:
                            modStrings.Add("FT");
                            break;
                        case GameplayModifiers.Modifier.TempoIncrement:
                            modStrings.Add("TI");
                            break;
                        case GameplayModifiers.Modifier.MirrorMode:
                            modStrings.Add("MM");
                            break;
                        case GameplayModifiers.Modifier.WrongHands:
                            modStrings.Add("WH");
                            break;
                        case GameplayModifiers.Modifier.ReducedAimAssist:
                            modStrings.Add("50% AA");
                            break;
                        case GameplayModifiers.Modifier.Psychedelia:
                            modStrings.Add("Rainbow");
                            break;
                        case GameplayModifiers.Modifier.MoreParticles:
                            modStrings.Add("EP");
                            break;
                        case GameplayModifiers.Modifier.InvisibleGuns:
                            modStrings.Add("IG");
                            break;
                        case GameplayModifiers.Modifier.NoTelegraphs:
                            modStrings.Add("NT");
                            break;
                        case GameplayModifiers.Modifier.SpeedWobble:
                            modStrings.Add("SW");
                            break;
                        case GameplayModifiers.Modifier.NoLook:
                            modStrings.Add("NL");
                            break;
                        case GameplayModifiers.Modifier.MaxMovement:
                            modStrings.Add("MaxMovement");
                            break;
                        case GameplayModifiers.Modifier.MinMovement:
                            modStrings.Add("MinMovement");
                            break;


                        default:
                            break;
                    }
                }
                return string.Join(" + ", modStrings);
            }
            else return "";
        }
    }
}
