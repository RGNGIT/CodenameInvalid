using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WispExtensions;
using System.Text;
using System;

public class WispChessClock : MonoBehaviour
{
    enum Turn {none, right, left, GameIsPaused, TimeIsOver}
    
    [SerializeField] private WispButton rightButton = null;
    [SerializeField] private WispButton leftButton = null;
    [SerializeField] private WispTextMeshPro rightCounterText = null;
    [SerializeField] private WispTextMeshPro leftCounterText = null;
    [SerializeField] private WispGuiStyle whiteStyle = null;
    [SerializeField] private WispGuiStyle blackStyle = null;
    [SerializeField] private WispButtonPanel btnPanel = null;
    [SerializeField] private AudioClip clickSound = null;
    [SerializeField] private AudioClip pauseSound = null;
    [SerializeField] private AudioClip unpauseSound = null;
    [SerializeField] private AudioClip startSound = null;
    [SerializeField] private AudioClip endSound = null;

    private WispPanel rightPanel;
    private WispPanel leftPanel;
    private Turn currentTurn = Turn.none;
    private float rightCounter = 0;
    private float leftCounter = 0;
    private float maxTime = 600; // 10 minutes by default
    private Color rightColor = Color.clear;
    private Color leftColor = Color.clear;
    private StringBuilder stringBuilder = new StringBuilder();
    private WispInputResult inputResult;
    private AudioSource audioSource;
    private bool isGamePaused = false;

    void Awake () 
    {
        #if !UNITY_EDITOR
            QualitySettings.vSyncCount = 0;  // VSync must be disabled
            Application.targetFrameRate = 30;
        #endif
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rightButton.IconPlacement = WispButton.WispButtonIconPlacement.Full;
        rightButton.SetIcon(WispIconLibrary.Default.Hourglass);
        rightButton.AddOnClickAction(rightBtnOnClick);
        
        rightPanel = rightButton.GetComponentInParent<WispPanel>();
        rightButton.SetParent(rightPanel, true);

        // --------------------------------------------------------------------

        leftButton.IconPlacement = WispButton.WispButtonIconPlacement.Full;        
        leftButton.SetIcon(WispIconLibrary.Default.Hourglass);
        leftButton.AddOnClickAction(leftBtnOnClick);

        leftPanel = leftButton.GetComponentInParent<WispPanel>();
        leftButton.SetParent(leftPanel, true);

        // --------------------------------------------------------------------

        ApplyPreGameStyle();
        UpdateTimers();
        GeneratePreGameButtons();

        // --------------------------------------------------------------------

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // --------------------------------------------------------------------

        Screen.autorotateToLandscapeRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGamePaused)
            return;
        
        if (currentTurn == Turn.right)
        {
            rightCounter += Time.deltaTime;

            if (rightCounter >= maxTime)
            {
                currentTurn = Turn.TimeIsOver;
                PlayEndSound();
                rightCounterText.SetValue("00:00:00");
                WispMessageBox.OpenOneButtonDialog("Time is over for " + ColorName(rightColor), "Ok", WispWindow.CloseParentWindow());
                GeneratePostGameButtons();
                return;
            }

            UpdateTimers();
        }
        else if (currentTurn == Turn.left)
        {
            leftCounter += Time.deltaTime;

            if (leftCounter >= maxTime)
            {
                currentTurn = Turn.TimeIsOver;
                PlayEndSound();
                leftCounterText.SetValue("00:00:00");
                GeneratePostGameButtons();
                WispMessageBox.OpenOneButtonDialog("Time is over for " + ColorName(leftColor), "Ok", WispWindow.CloseParentWindow());
                return;
            }

            UpdateTimers();
        }
    }

    private void rightBtnOnClick()
    {
        if (isGamePaused || currentTurn == Turn.left)
            return;

        if (currentTurn == Turn.TimeIsOver)
        {
            return;
        }
        
        if (currentTurn == Turn.none)
        {
            // Right started so right is white.
            rightColor = Color.white;
            leftColor = Color.black;
            // rightPanel.Style = whiteStyle;
            // leftPanel.Style = blackStyle;

            currentTurn = Turn.right;
            ApplyInGameStyle(currentTurn);

            GenerateInGameButtons();
            PlayStartSound();

            return;
        }
        
        currentTurn = Turn.left;

        PlayButtonClickSound();
    }

    private void leftBtnOnClick()
    {
        if (isGamePaused  || currentTurn == Turn.right)
            return;

        if (currentTurn == Turn.TimeIsOver)
        {
            return;
        }
        
        if (currentTurn == Turn.none)
        {
            // Left started so left is white.
            rightColor = Color.black;
            leftColor = Color.white;
            // leftPanel.Style = whiteStyle;
            // rightPanel.Style = blackStyle;
            
            currentTurn = Turn.left;
            ApplyInGameStyle(currentTurn);

            GenerateInGameButtons();
            PlayStartSound();

            return;
        }
        
        currentTurn = Turn.right;
        PlayButtonClickSound();
    }

    private string GetTripleSegmentTime(float ParamTime)
    {
        string str;
        
        if (ParamTime == 0)
            str = maxTime.ToString();
        else
            str  = (maxTime - ParamTime).ToString();

        if (!str.Contains("."))
        {
            str += ".00";
        }

        string[] array = str.Split('.');

        string decPart = array[1];
        
        if (decPart.Length == 1)
        decPart += "0";

        // string fracPart = array[1].Substring(0,2);
        decPart = decPart.Substring(0,2);

        string intPart = array[0];
        string s = (intPart.ToInt() % 60).ToString().PadLeft(2,'0');
        string m = (intPart.ToInt() / 60).ToString().PadLeft(2,'0');

        stringBuilder.Clear();
        stringBuilder.Append(m);
        stringBuilder.Append(":");
        stringBuilder.Append(s);
        stringBuilder.Append(":");
        stringBuilder.Append(decPart);

        return stringBuilder.ToString();
    }

    private string ColorName(Color ParamColor)
    {
        if (ParamColor == Color.white)
            return "White";
        else if (ParamColor == Color.black)
            return "Black";

        return "Unknown";
    }

    private void UpdateTimers()
    {
        rightCounterText.SetValue(GetTripleSegmentTime(rightCounter));
        leftCounterText.SetValue(GetTripleSegmentTime(leftCounter));
    }

    private void OpenAdjustTimerWindow()
    {
        inputResult = WispInputBox.OpenInputDialog("Entire player allowed time in minutes : ", UpdateMaximumTime);
    }

    private void UpdateMaximumTime()
    {
        maxTime = inputResult.Result.ToFloat() * 60;

        rightCounter = 0;
        leftCounter = 0;

        UpdateTimers();
    }

    private void PlayButtonClickSound()
    {
        audioSource.clip = clickSound;
        audioSource.time = 0.05f;
        audioSource.Play();
    }

    private void PlayPauseSound()
    {
        audioSource.clip = pauseSound;
        audioSource.time = 0f;
        audioSource.Play();
    }

    private void PlayUnpauseSound()
    {
        audioSource.clip = unpauseSound;
        audioSource.time = 0f;
        audioSource.Play();
    }

    private void PlayStartSound()
    {
        audioSource.clip = startSound;
        audioSource.time = 0f;
        audioSource.Play();
    }

    private void PlayEndSound()
    {
        audioSource.clip = endSound;
        audioSource.time = 0f;
        audioSource.Play();
    }

    private void GeneratePreGameButtons()
    {
        btnPanel.Clear();
        btnPanel.AddButton("time", "Adjust Timer", OpenAdjustTimerWindow, null);
    }

    private void GenerateInGameButtons()
    {
        btnPanel.Clear();
        btnPanel.AddButton("pause", "Pause game", PauseGameOnClick, null);
        btnPanel.AddButton("abort", "Abort game", AbortGameOnClick, null);
    }

    private void GeneratePostGameButtons()
    {
        btnPanel.Clear();
        btnPanel.AddButton("time", "Adjust Timer", OpenAdjustTimerWindow, null);
        btnPanel.AddButton("rematch", "Rematch", Rematch, null);
    }

    private void PauseGameOnClick()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            btnPanel.GetButton("pause").SetValue("Continue");
            WispAnimationGlow.AttachToGameObject(btnPanel.GetButton("pause").gameObject, 2f, Color.blue);
            rightPanel.Opacity = 0.5f;
            leftPanel.Opacity = 0.5f;
            PlayPauseSound();
        }
        else
        {
            isGamePaused = false;
            btnPanel.GetButton("pause").SetValue("Pause");

            WispAnimationGlow anim = btnPanel.GetButton("pause").GetComponent<WispAnimationGlow>();

            if (anim != null)
            {
                Destroy(anim);
            }

            rightPanel.Opacity = 1f;
            leftPanel.Opacity = 1f;

            PlayUnpauseSound();
        }
    }

    private void Rematch()
    {
        currentTurn = Turn.none;
        rightCounter = 0;
        leftCounter = 0;

        UpdateTimers();
        ApplyPreGameStyle();
    }

    private void AbortGameOnClick()
    {
        WispMessageBox.OpenTwoButtonsDialog("Are you sure you want to abort this game ?", "Yes", AbortGame, "No", WispWindow.CloseParentWindow());
    }

    private void AbortGame()
    {
        WispMessageBox.CloseCurrentlyOpenedVisualComponent();
        currentTurn = Turn.none;
        ApplyPreGameStyle();
        GeneratePreGameButtons();
        PlayEndSound();
    }

    private void ApplyPreGameStyle()
    {
        rightPanel.Style = whiteStyle;
        leftPanel.Style = whiteStyle;
        rightPanel.Opacity = 0.5f;
        leftPanel.Opacity = 0.5f;
    }

    private void ApplyInGameStyle(Turn ParamFirstPlayer)
    {
        if (ParamFirstPlayer == Turn.right)
        {
            rightPanel.Style = whiteStyle;
            leftPanel.Style = blackStyle;
        }
        else if (ParamFirstPlayer == Turn.left)
        {
            rightPanel.Style = blackStyle;
            leftPanel.Style = whiteStyle;
        }
        else
        {
            WispVisualComponent.LogError("Invalid Turn.");
        }

        rightPanel.Opacity = 1f;
        leftPanel.Opacity = 1f;
    }
}