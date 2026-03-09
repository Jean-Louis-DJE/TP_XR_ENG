using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandPanelView : MonoBehaviour
{
    [Header("Date")]
    [SerializeField] private Button datePreviousButton;
    [SerializeField] private Button dateNextButton;
    [SerializeField] private TMP_Text dateText;

    [Header("Speed")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TMP_Text speedText;

    [Header("Playback")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;

    [Header("Options")]
    [SerializeField] private Toggle orbitsToggle;

    [Header("Reset")]
    [SerializeField] private Button resetScaleButton;
    [SerializeField] private Button resetViewButton;

    [Header("Date Settings")]
    [SerializeField] private string dateFormat = "dd/MM/yyyy";
    [SerializeField] private int dateStepDays = 1;

    [Header("Speed Settings")]
    [SerializeField] private int[] speedValues = new[] { 1, 10, 100 };

    public event Action<DateTime> DateChanged;
    public event Action<int> SpeedChanged;
    public event Action PlayClicked;
    public event Action PauseClicked;
    public event Action<bool> OrbitsToggled;
    public event Action ResetScaleClicked;
    public event Action ResetViewClicked;

    private DateTime _currentDate = new DateTime(2026, 1, 1);
    private int _currentSpeed = 1;

    private void Awake()
    {
        Log("Initializing command panel", "info");

        RefreshDateText();
        RefreshSpeedText();
        ConfigureSpeedSlider();
    }

    private void OnEnable()
    {
        if (datePreviousButton != null) datePreviousButton.onClick.AddListener(HandlePreviousDateClicked);
        else Log("Date previous button missing", "warn");

        if (dateNextButton != null) dateNextButton.onClick.AddListener(HandleNextDateClicked);
        else Log("Date next button missing", "warn");

        if (speedSlider != null) speedSlider.onValueChanged.AddListener(HandleSpeedSliderChanged);
        else Log("Speed slider missing", "warn");

        if (playButton != null) playButton.onClick.AddListener(HandlePlayClicked);
        else Log("Play button missing", "warn");

        if (pauseButton != null) pauseButton.onClick.AddListener(HandlePauseClicked);
        else Log("Pause button missing", "warn");

        if (orbitsToggle != null) orbitsToggle.onValueChanged.AddListener(HandleOrbitsToggled);
        else Log("Orbit toggle missing", "warn");

        if (resetScaleButton != null) resetScaleButton.onClick.AddListener(HandleResetScaleClicked);
        else Log("Reset scale button missing", "warn");

        if (resetViewButton != null) resetViewButton.onClick.AddListener(HandleResetViewClicked);
        else Log("Reset view button missing", "warn");
    }

    private void OnDisable()
    {
        if (datePreviousButton != null) datePreviousButton.onClick.RemoveListener(HandlePreviousDateClicked);
        if (dateNextButton != null) dateNextButton.onClick.RemoveListener(HandleNextDateClicked);
        if (speedSlider != null) speedSlider.onValueChanged.RemoveListener(HandleSpeedSliderChanged);
        if (playButton != null) playButton.onClick.RemoveListener(HandlePlayClicked);
        if (pauseButton != null) pauseButton.onClick.RemoveListener(HandlePauseClicked);
        if (orbitsToggle != null) orbitsToggle.onValueChanged.RemoveListener(HandleOrbitsToggled);
        if (resetScaleButton != null) resetScaleButton.onClick.RemoveListener(HandleResetScaleClicked);
        if (resetViewButton != null) resetViewButton.onClick.RemoveListener(HandleResetViewClicked);
    }

    public void SetDate(DateTime date)
    {
        _currentDate = date;
        RefreshDateText();
        Log($"Date set externally → {_currentDate}", "info");
    }

    public void SetSpeed(int speed)
    {
        int index = GetSpeedIndex(speed);
        _currentSpeed = speedValues[index];

        RefreshSpeedText();

        if (speedSlider != null)
            speedSlider.SetValueWithoutNotify(index);

        Log($"Speed set externally → x{_currentSpeed}", "info");
    }

    private void HandlePreviousDateClicked()
    {
        _currentDate = _currentDate.AddDays(-dateStepDays);
        RefreshDateText();

        Log($"Date decreased → {_currentDate}", "input");
        DateChanged?.Invoke(_currentDate);
    }

    private void HandleNextDateClicked()
    {
        _currentDate = _currentDate.AddDays(dateStepDays);
        RefreshDateText();

        Log($"Date increased → {_currentDate}", "input");
        DateChanged?.Invoke(_currentDate);
    }

    private void HandleSpeedSliderChanged(float rawValue)
    {
        int index = Mathf.RoundToInt(rawValue);
        index = Mathf.Clamp(index, 0, speedValues.Length - 1);

        _currentSpeed = speedValues[index];

        RefreshSpeedText();

        Log($"Speed changed → x{_currentSpeed}", "input");
        SpeedChanged?.Invoke(_currentSpeed);
    }

    private void HandlePlayClicked()
    {
        Log("Play pressed", "input");
        PlayClicked?.Invoke();
    }

    private void HandlePauseClicked()
    {
        Log("Pause pressed", "input");
        PauseClicked?.Invoke();
    }

    private void HandleOrbitsToggled(bool isOn)
    {
        Log($"Orbits toggled → {(isOn ? "ON" : "OFF")}", "input");
        OrbitsToggled?.Invoke(isOn);
    }

    private void HandleResetScaleClicked()
    {
        Log("Reset scale pressed", "input");
        ResetScaleClicked?.Invoke();
    }

    private void HandleResetViewClicked()
    {
        Log("Reset view pressed", "input");
        ResetViewClicked?.Invoke();
    }

    private void RefreshDateText()
    {
        if (dateText == null)
        {
            Log("Date TMP_Text missing", "warn");
            return;
        }

        dateText.text = _currentDate.ToString(dateFormat);
    }

    private void RefreshSpeedText()
    {
        if (speedText == null)
        {
            Log("Speed TMP_Text missing", "warn");
            return;
        }

        speedText.text = $"x{_currentSpeed}";
    }

    private void ConfigureSpeedSlider()
    {
        if (speedSlider == null || speedValues == null || speedValues.Length == 0)
        {
            Log("Speed slider configuration invalid", "warn");
            return;
        }

        speedSlider.wholeNumbers = true;
        speedSlider.minValue = 0;
        speedSlider.maxValue = speedValues.Length - 1;

        int index = GetSpeedIndex(_currentSpeed);
        speedSlider.SetValueWithoutNotify(index);

        Log("Speed slider configured", "info");
    }

    private int GetSpeedIndex(int speed)
    {
        for (int i = 0; i < speedValues.Length; i++)
        {
            if (speedValues[i] == speed)
                return i;
        }

        Log($"Speed value {speed} not found, defaulting to x1", "warn");
        return 0;
    }

    public void SetOrbitsToggle(bool value)
    {
        if (orbitsToggle == null)
        {
            Log("Orbit toggle missing", "warn");
            return;
        }

        orbitsToggle.SetIsOnWithoutNotify(value);

        Log($"Orbits toggle set externally → {(value ? "ON" : "OFF")}", "info");
    }

    private void Log(string message, string level)
    {
        string core;

        switch (level.ToLower())
        {
            case "input":
                core = "INPUT";
                break;

            case "warn":
                core = "WARN";
                break;

            default:
                core = "INFO";
                break;
        }

        Debug.Log($"[{core}] [CommandPanelView] {message}", this);
    }
}