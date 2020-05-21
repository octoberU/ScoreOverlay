using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Threading;
using System.Globalization;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using NET_SDK;
using NET_SDK.Harmony;
using NET_SDK.Reflection;
using System.Runtime.InteropServices;
using UnityEngine.PostProcessing;
using System.Linq;
using Valve.VR;
using System.Collections.ObjectModel;
using TMPro;



namespace AudicaModding
{
    public static class BuildInfo
    {
        public const string Name = "ScoreOverlay"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "octo"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.4"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class ScoreOverlay : MelonMod
    {
        public static GameObject myCanvas;
        public static Patch LaunchPlay2;
        public static Patch Restart2;
        public static Patch goToSongs2;
        public static Patch PauseHandler_HMDMounted;
        private static bool scoreDisplayEnabled = false;
        public static ScoreKeeper scoreKeeper;
        public static GameplayStats gameplayStats;
        public static ScoreKeeperDisplay scoreKeeperDisplay;
        public static TextMeshProUGUI ScoreTextField;
        public static TextMeshProUGUI ComboLabelField;
        public static TextMeshProUGUI AverageTimingText;
        public static List<float> TimingError = new List<float>();

        public static CanvasGroup OverlayGroup;
        public static RectTransform OverlayTransform;
        public static float maxTimingBarLength = 250f;
        static ObservableCollection<GameObject> HitRectangles = new ObservableCollection<GameObject>();
        public static float timingBarSize = 1.8f;
        public float timingBarHeight = 1.5f;

        public override void OnApplicationStart()
        {
            MelonModLogger.Log("OnApplicationStart");

            Instance instance = Manager.CreateInstance("ScoreOverlay2");
            ScoreOverlay.LaunchPlay2 = instance.Patch(SDK.GetClass("LaunchPanel").GetMethod("Play"), typeof(ScoreOverlay).GetMethod("playsong2"));
            ScoreOverlay.Restart2 = instance.Patch(SDK.GetClass("InGameUI").GetMethod("Restart"), typeof(ScoreOverlay).GetMethod("RestartSong2"));
            ScoreOverlay.goToSongs2 = instance.Patch(SDK.GetClass("InGameUI").GetMethod("ReturnToSongList"), typeof(ScoreOverlay).GetMethod("ReturnToSongs"));
            PauseHandler_HMDMounted = instance.Patch(SDK.GetClass("GameplayStats").GetMethod("ReportTargetHit"), typeof(ScoreOverlay).GetMethod("ReportTargetHit"));
        }

        private static void MoveHitRect(float Timing, Color handColor)
        {
            HitRectangles.Move(HitRectangles.Count - 1, 0);
            HitRectangles[0].SetActive(true);
            HitRectangles[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(Timing, 0f);
            HitRectangles[0].GetComponent<Image>().color = handColor;
        }

        public unsafe static void ReportTargetHit(IntPtr @this, IntPtr cue, float tick, Vector2 targetHitPos)
        {
            PauseHandler_HMDMounted.InvokeOriginal(@this, new IntPtr[] {
                cue,
                new IntPtr((void*)(&tick)),
                new IntPtr((void*)(&targetHitPos))
            });
            if (!KataConfig.I.practiceMode)
            {
                SongCues.Cue targetCue = new SongCues.Cue(cue);
                float TargetError = (float)targetCue.tick - tick;
                //MelonModLogger.Log("Timing Error = " + TargetError.ToString());
                TimingError.Add(TargetError);
                if (TimingError.Count > 1)
                {
                    AverageTimingText.text = TimingError.Average().ToString("F") + "ms";
                }
                MoveHitRect(TargetError * -1, KataConfig.I.GetTargetColor(targetCue.handType));
            }

        }

        public static void playsong2(IntPtr @this)
        {
            ScoreOverlay.LaunchPlay2.InvokeOriginal(@this);
            if (!KataConfig.I.practiceMode)
            {
                MelonCoroutines.Start(StartScoreDisplay());
            }
            HitRectangles.Clear();
            TimingError.Clear();
        }
        public static void RestartSong2(IntPtr @this)
        {
            ScoreOverlay.Restart2.InvokeOriginal(@this);
            if (!KataConfig.I.practiceMode)
            {
                MelonCoroutines.Start(StartScoreDisplay());
            }
            if (myCanvas != null)
            {
                GameObject.Destroy(myCanvas);
                scoreDisplayEnabled = false;
            }
            HitRectangles.Clear();
            TimingError.Clear();
        }

        public static void ReturnToSongs(IntPtr @this)
        {
            ScoreOverlay.goToSongs2.InvokeOriginal(@this);
            if (myCanvas != null)
            {
                GameObject.Destroy(myCanvas);           
            }
            scoreDisplayEnabled = false;
            HitRectangles.Clear();
            TimingError.Clear();
        }

        static IEnumerator StartScoreDisplay()
        {
            if (!scoreDisplayEnabled)
            {
                yield return new WaitForSeconds(5);
                InitializeScoreDisplay();
            }
            else
            {
                GameObject.Destroy(myCanvas);
                scoreDisplayEnabled = false;
                yield return new WaitForSeconds(5);
                if (!scoreDisplayEnabled)
                {
                    InitializeScoreDisplay();
                }
            }
        }

        public static IEnumerator FadeOverlay(float aFrom, float aTo, float aTime)
        {
            for (float t = 0.05f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                OverlayGroup.alpha = Mathf.Lerp(aFrom, aTo, t);
                yield return null;
            }
        }

        public static IEnumerator MoveOverlay(float aTime)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Vector2 animatedposition = new Vector2(40, Mathf.Lerp(20f, 45f, t));
                OverlayTransform.anchoredPosition = animatedposition;
                yield return null;
            }
        }

        public override void OnUpdate()
        {
            if (scoreDisplayEnabled)
            {
                ScoreOverlay.ScoreTextField.text = ScoreOverlay.scoreKeeper.mScore.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
                if (ScoreOverlay.scoreKeeper.mStreak > 1)
                {
                    ScoreOverlay.ComboLabelField.text = ScoreOverlay.scoreKeeper.mStreak.ToString() + " Streak!";
                }
                else
                {
                    ScoreOverlay.ComboLabelField.text = null;
                }

            }
        }

        public static void InitializeScoreDisplay()
        {
            if (myCanvas != null)
            {
                GameObject.Destroy(myCanvas);
                scoreDisplayEnabled = false;
            }
            SpectatorCam specCam = UnityEngine.Object.FindObjectOfType<SpectatorCam>();
            ScoreOverlay.scoreKeeper = UnityEngine.Object.FindObjectOfType<ScoreKeeper>();
            ScoreOverlay.scoreKeeperDisplay = UnityEngine.Object.FindObjectOfType<ScoreKeeperDisplay>();
            ScoreOverlay.gameplayStats = UnityEngine.Object.FindObjectOfType<GameplayStats>();

            ScoreOverlay.myCanvas = new GameObject();
            Canvas canvasComp = myCanvas.AddComponent<Canvas>();
            OverlayGroup = myCanvas.AddComponent<CanvasGroup>();
            myCanvas.AddComponent<RectTransform>();


            GameObject ScoreText = new GameObject();
            ScoreText.name = "ScoreText";
            ScoreText.transform.SetParent(myCanvas.transform);

            OverlayTransform = ScoreText.AddComponent<RectTransform>();
            OverlayTransform.anchorMax = new Vector2(0, 0);
            OverlayTransform.anchorMin = new Vector2(0, 0);
            OverlayTransform.pivot = new Vector2(0, 0);
            OverlayTransform.anchoredPosition = new Vector2(60, 15); // 40


            ScoreOverlay.ScoreTextField = ScoreText.AddComponent<TextMeshProUGUI>();
            ScoreTextField.text = "0";
            ScoreTextField.font = specCam.headsetCameraModeDisplay.font;
            ScoreTextField.fontStyle = FontStyles.Superscript;
            ScoreTextField.alignment = TextAlignmentOptions.BottomLeft;
            OverlayTransform.sizeDelta = new Vector2(1000, 200);
            ScoreTextField.fontSize = 92;


            GameObject ScoreLabel = GameObject.Instantiate(ScoreText, ScoreText.transform);
            ScoreLabel.name = "ScoreLabel";

            RectTransform ScoreLabelRect = ScoreLabel.GetComponent<RectTransform>();
            TextMeshProUGUI ScoreLabelField = ScoreLabel.GetComponent<TextMeshProUGUI>();
            ScoreLabelField.text = "Score".ToUpper();
            ScoreLabelField.fontSize = 32;
            ScoreLabelRect.anchoredPosition = new Vector2(2, 90);

            GameObject ComboLabel = GameObject.Instantiate(ScoreLabel, ScoreText.transform);
            ComboLabel.name = "ComboLabel";

            RectTransform ComboLabelRect = ComboLabel.GetComponent<RectTransform>();
            ScoreOverlay.ComboLabelField = ComboLabel.GetComponent<TextMeshProUGUI>();
            ComboLabelField.text = "";
            ComboLabelField.fontSize = 32;
            ComboLabelField.color = new Color32(255, 232, 70, 255);
            ComboLabelRect.anchoredPosition = new Vector2(2, -12);


            GameObject TimingDisplay = new GameObject();
            TimingDisplay.name = "TimingDisplay";
            TimingDisplay.transform.SetParent(myCanvas.transform);
            RectTransform TimingDisplayTransform = TimingDisplay.AddComponent<RectTransform>();
            TimingDisplayTransform.anchorMax = new Vector2(0.5f, 0);
            TimingDisplayTransform.anchorMin = new Vector2(0.5f, 0);


            GameObject TimingBar = new GameObject();
            TimingBar.name = "TimingBar";
            TimingBar.transform.SetParent(TimingDisplay.transform);
            RectTransform TimingBarTransform = TimingBar.AddComponent<RectTransform>();
            TimingBarTransform.anchorMax = new Vector2(0.5f, 0);
            TimingBarTransform.anchorMin = new Vector2(0.5f, 0);

            GameObject OrangeBar = new GameObject();
            OrangeBar.name = "OrangeBar";
            OrangeBar.transform.SetParent(TimingBar.transform);
            Image OrangeBarImage = OrangeBar.AddComponent<Image>();
            RectTransform OrangeBarTransform = OrangeBar.GetComponent<RectTransform>();
            OrangeBarTransform.sizeDelta = new Vector2(maxTimingBarLength, 1.5f);
            OrangeBarTransform.anchoredPosition = new Vector2(0f, 0f);
            OrangeBarImage.color = new Color32(255, 255, 255, 255); // new Color32(255, 187, 0, 255);

            GameObject YellowBar = UnityEngine.Object.Instantiate(OrangeBar, TimingBar.transform);
            YellowBar.name = "YellowBar";
            YellowBar.GetComponent<Image>().color = new Color32(255, 255, 255, 255); // new Color32(255, 251, 37, 255);
            YellowBar.GetComponent<RectTransform>().sizeDelta = new Vector2(maxTimingBarLength * 0.7f, 1.5f);

            GameObject GreenBar = UnityEngine.Object.Instantiate(OrangeBar, TimingBar.transform);
            GreenBar.name = "GreenBar";
            GreenBar.GetComponent<Image>().color = new Color32(255, 255, 255, 255); // new Color32(142, 248, 67, 255);
            GreenBar.GetComponent<RectTransform>().sizeDelta = new Vector2(maxTimingBarLength * 0.25f, 1.5f);


            GameObject HitRect = UnityEngine.Object.Instantiate(OrangeBar, TimingBar.transform);
            HitRect.name = "HitRect";
            HitRect.GetComponent<Image>().color = new Color32(255, 255, 255, 125);
            HitRect.GetComponent<RectTransform>().sizeDelta = new Vector2(3.3f, 30.3f);
            HitRect.SetActive(false);

            GameObject AverageTiming = new GameObject();
            AverageTiming.transform.SetParent(TimingBar.transform);
            AverageTiming.name = "Average Timing";
            ScoreOverlay.AverageTimingText = AverageTiming.AddComponent<TextMeshProUGUI>();
            ScoreOverlay.AverageTimingText.text = "";
            ScoreOverlay.AverageTimingText.font = specCam.headsetCameraModeDisplay.font;
            ScoreOverlay.AverageTimingText.fontStyle = FontStyles.Superscript;
            ScoreOverlay.AverageTimingText.alignment = TextAlignmentOptions.Bottom;
            ScoreOverlay.AverageTimingText.fontSize = 14;
            AverageTiming.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 43.4f);

            TimingDisplayTransform.anchoredPosition = new Vector2(0f, 50f);
            TimingDisplayTransform.localScale = new Vector3(timingBarSize, timingBarSize, timingBarSize);

            for (int i = 0; i < 20; i++)
            {
                HitRectangles.Add(UnityEngine.Object.Instantiate(HitRect, TimingBar.transform));
            }

            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasComp.worldCamera = specCam.cam;


            myCanvas.SetActive(true);

            scoreDisplayEnabled = true;

            OverlayGroup.alpha = 0f;

            MelonCoroutines.Start(FadeOverlay(0f, 1f, 2f));
            MelonCoroutines.Start(MoveOverlay(2f));
        }
    }
}