
using System.Collections;
using Il2CppSystem.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Resources;
using System.Globalization;

namespace ScoreOverlay
{
    internal static class UI
    {
        public static GameObject overlay;
        private static GameplayStats gameplayStats;

        public static Canvas mainCanvas;
        public static CanvasScaler canvasScaler;
        public static CanvasGroup canvasGroup;
        public static Animator animator;

        public static TextMeshProUGUI scoreText;
        public static TextMeshProUGUI scoreLabel;
        public static TextMeshProUGUI comboText;

        public static TextMeshProUGUI songInfo;
        public static TextMeshProUGUI mapperInfo;
        public static TextMeshProUGUI difficultyLabel;

        public static TextMeshProUGUI averageTimingLabel;
        public static TextMeshProUGUI averageAimLabel;
        public static TextMeshProUGUI misfiresLabel;

        public static TextMeshProUGUI aimAssistLabel;
        public static TextMeshProUGUI ModifierText;

        public static Transform bottomLeft;
        public static Transform topRight;
        public static Transform bottomRight;
        public static Transform topLeft;

        public static bool loaded = false;

        public static void Initialize()
        {
            PrepareOverlay();
            GetReferences();
            canvasScaler.scaleFactor = ScoreOverlayConfig.OverlayScale;
            FadeOutOverlay();
            bottomRight.gameObject.SetActive(false);
            topLeft.gameObject.SetActive(false);
        }
        
        public static void PrepareOverlay()
        {
            var scoreOverlay = new Il2CppSystem.IO.MemoryStream(AudicaModding.Properties.Resources.scoreoverlay);
            var overlayBundle = Il2CppAssetBundleManager.LoadFromStream(scoreOverlay);
            //var overlayBundle = Il2CppAssetBundleManager.LoadFromFile(Utility.overlayPath);
            if (overlayBundle == null) MelonLogger.Log("Failed to load overlay asset, make sure you have installed the mod properly.");
            overlay = GameObject.Instantiate<GameObject>(overlayBundle.LoadAsset("Assets/ScoreOverlay/ScoreOverlay.prefab").Cast<GameObject>());
            overlay.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(overlay);
            overlay.GetComponent<Canvas>().worldCamera = Camera.current;
            overlay.SetActive(true);
        }

        public static void GetReferences()
        {
            mainCanvas = overlay.GetComponent<Canvas>();
            canvasGroup = overlay.GetComponent<CanvasGroup>();
            canvasScaler = overlay.GetComponent<CanvasScaler>();
            animator = overlay.GetComponent<Animator>();
            
            bottomLeft = overlay.transform.Find("BottomLeft");
            topRight = overlay.transform.Find("TopRight");
            bottomRight = overlay.transform.Find("BottomRight");
            topLeft = overlay.transform.Find("TopLeft");

            scoreText = bottomLeft.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            scoreLabel = bottomLeft.Find("ScoreLabel").GetComponent<TextMeshProUGUI>();
            comboText = bottomLeft.Find("ComboText").GetComponent<TextMeshProUGUI>();

            songInfo = topRight.Find("SongInfo").GetComponent<TextMeshProUGUI>();
            mapperInfo = topRight.Find("MapperInfo").GetComponent<TextMeshProUGUI>();
            difficultyLabel = topRight.Find("Difficulty").GetComponent<TextMeshProUGUI>();

            averageTimingLabel = bottomRight.Find("AverageTiming").GetComponent<TextMeshProUGUI>();
            averageAimLabel = bottomRight.Find("AverageAim").GetComponent<TextMeshProUGUI>();
            misfiresLabel = bottomRight.Find("Misfires").GetComponent<TextMeshProUGUI>();

            aimAssistLabel = topLeft.Find("AimAssist").GetComponent<TextMeshProUGUI>();
            ModifierText = topLeft.Find("ModText").GetComponent<TextMeshProUGUI>();
            loaded = true;
        }

        public static void FadeInOverlay()
        {
            animator.Play("FadeIn");
        }

        public static void FadeOutOverlay()
        {
            animator.Play("FadeOut");
        }

        public static void UpdateUiInfo(SongList.SongData data)
        {
            KataConfig kataConfig = KataConfig.I;

            songInfo.text = Utility.RemoveFormatting($"{data.artist} - {data.title}");
            mapperInfo.text = data.author;
            difficultyLabel.text = kataConfig.mDifficulty.ToString();

            var scoreKeeper = ScoreKeeper.I;
            scoreText.text = scoreKeeper.GetScore().ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            comboText.text = scoreKeeper.mStreak > 1 ? scoreKeeper.mStreak.ToString() + " Streak!" : "";

            int aimAssistPercent = (int)(PlayerPreferences.I.AimAssistAmount.mVal * 100f);
            aimAssistLabel.text = aimAssistPercent != 100 ? $"{aimAssistPercent}% AA" : "";
            ModifierText.text = Utility.GetModText();
        }


        public static void UpdateDisplay()
        {
            var scoreKeeper = ScoreKeeper.I;
            scoreText.text = scoreKeeper.mScore.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            comboText.text = scoreKeeper.mStreak > 1 ? scoreKeeper.mStreak.ToString() + " Streak!" : "";
            if (gameplayStats == null) GameObject.FindObjectOfType<GameplayStats>();
            if (gameplayStats != null)
            {
                misfiresLabel.text = gameplayStats.mMisfireCount.ToString(); 
            }
        }


        private static IEnumerator FadeOverlay(float aFrom, float aTo, float aTime)
        {
            for (float t = 0.05f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                canvasGroup.alpha = Mathf.Lerp(aFrom, aTo, t);
                MelonLogger.Log(canvasGroup.alpha);
                yield return null;
            }
        }
    }
}
