
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
    public class UI
    {
        public GameObject overlay;
        private GameplayStats gameplayStats;

        private KataConfig kataConfig;

        public Canvas mainCanvas;
        public CanvasScaler canvasScaler;
        public CanvasGroup canvasGroup;
        public Animator animator;

        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI scoreLabel;
        public TextMeshProUGUI comboText;
        public TextMeshProUGUI highscoreLabel;

        public TextMeshProUGUI songInfo;
        public TextMeshProUGUI mapperInfo;
        public TextMeshProUGUI difficultyLabel;

        public TextMeshProUGUI averageTimingLabel;
        public TextMeshProUGUI averageAimLabel;
        public TextMeshProUGUI misfiresLabel;

        public TextMeshProUGUI aimAssistLabel;
        public TextMeshProUGUI ModifierText;

        public Transform bottomLeft;
        public Transform topRight;
        public Transform bottomRight;
        public Transform topLeft;

        public bool loaded = false;

        public void Initialize()
        {
            PrepareOverlay();
            GetReferences();
            FadeOutOverlay();
            overlay.SetActive(false);
            bottomRight.gameObject.SetActive(false);
            topLeft.gameObject.SetActive(Config.ShowModifiers);
            kataConfig = KataConfig.I;
            OnModSettingsApplied();
        }
        
        public void PrepareOverlay()
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

        public void GetReferences()
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
            highscoreLabel = bottomLeft.Find("Highscore").GetComponent<TextMeshProUGUI>();

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

        public void FadeInOverlay()
        {
            if(!overlay.activeSelf) overlay.SetActive(true);
            animator.Play("FadeIn");
        }

        public void FadeOutOverlay()
        {
            animator.Play("FadeOut");
        }

        public void UpdateUiInfo(SongList.SongData data)
        {
            var currentDifficulty = kataConfig.mDifficulty;
            string difficultyColor = ColorUtility.ToHtmlStringRGB(kataConfig.GetDifficultyColor(currentDifficulty));

            songInfo.text = Utility.RemoveFormatting($"{data.artist} - {data.title}");
            mapperInfo.text = data.author;
            difficultyLabel.text = $"<color=#{difficultyColor}>{currentDifficulty}</color>";

            var scoreKeeper = ScoreKeeper.I;
            scoreText.text = scoreKeeper.GetScore().ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            comboText.text = scoreKeeper.mStreak > 1 ? scoreKeeper.mStreak.ToString() + " Streak!" : "";
            highscoreLabel.text = "";

            int aimAssistPercent = (int)(PlayerPreferences.I.AimAssistAmount.mVal * 100f);
            aimAssistLabel.text = aimAssistPercent != 100 ? $"{aimAssistPercent}% AA" : "";
            ModifierText.text = Utility.GetModText();
        }

        public void OnModSettingsApplied()
        {
            canvasScaler.scaleFactor = Config.OverlayScale;
            topRight.gameObject.SetActive(Config.ShowSongInfo);
            topLeft.gameObject.SetActive(Config.ShowModifiers);
            bottomLeft.gameObject.SetActive(Config.ShowScore);
        }


        public void UpdateDisplay(int score, int streak)
        {
            scoreText.text = score.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            comboText.text = streak > 1 ? streak.ToString() + " Streak!" : "";
            if (gameplayStats == null) GameObject.FindObjectOfType<GameplayStats>();
            if (gameplayStats != null)
            {
                misfiresLabel.text = gameplayStats.mMisfireCount.ToString(); 
            }
        }

        public void UpdateHighscore(int score, int highscore)
        {
            if (highscore == 0 || highscore > score || !Config.ShowHighscoreDifference)
            {
                if (highscoreLabel.gameObject.activeSelf) highscoreLabel.gameObject.SetActive(false);
            }
            else
            {
                if (!highscoreLabel.gameObject.activeSelf) highscoreLabel.gameObject.SetActive(true);
                highscoreLabel.text = "New highscore! +" + (score - highscore).ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

    }
}
