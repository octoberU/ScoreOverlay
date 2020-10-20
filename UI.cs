
using System.Collections;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreOverlay
{
    internal static class UI
    {
        public static GameObject overlay;

        public static Canvas mainCanvas;
        public static CanvasScaler canvasScaler;
        public static CanvasGroup canvasGroup;

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
            canvasScaler.scaleFactor = 1.5f;
            canvasGroup.alpha = 0f;
        }
        
        public static void PrepareOverlay()
        {
            var overlayBundle = Il2CppAssetBundleManager.LoadFromFile(Utility.overlayPath);
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
            canvasGroup = overlay.AddComponent<CanvasGroup>();
            canvasScaler = overlay.GetComponent<CanvasScaler>();
            
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
            MelonCoroutines.Start(FadeOverlay(0f, 1f, Config.fadeTime));
        }

        public static void FadeOutOverlay()
        {
            MelonCoroutines.Start(FadeOverlay(1f, 0f, Config.fadeTime));
        }

        public static IEnumerator FadeOverlay(float aFrom, float aTo, float aTime)
        {
            for (float t = 0.05f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                canvasGroup.alpha = Mathf.Lerp(aFrom, aTo, t);
                yield return null;
            }
        }
    }
}
