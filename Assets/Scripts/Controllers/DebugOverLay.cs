using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("TMP dédié aux données runtime : FPS, temps, vitesse, dernière action, warnings.")]
    [SerializeField] private TMP_Text statusText;

    [Tooltip("TMP dédié exclusivement aux logs capturés dans la queue.")]
    [SerializeField] private TMP_Text logsText;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.25f;

    [Header("Logs")]
    [SerializeField] private int maxLogLines = 12;
    [SerializeField] private int maxWarningLines = 4;

    [Header("Perf thresholds")]
    [SerializeField] private float suspiciousFrameTimeMs = 20f;
    [SerializeField] private float lowFpsThreshold = 50f;

    private readonly Queue<string> _logLines = new Queue<string>();
    private readonly Queue<string> _warningLines = new Queue<string>();

    private TimeModel _timeModel;

    private float _refreshTimer;
    private float _smoothedDeltaTime;

    private string _lastUserAction = "None";
    private bool _isInitialized;

    public void Init(TimeModel model)
    {
        _timeModel = model;

        _isInitialized = true;

        LogInfo("Initialized.");
        RefreshTexts(force: true);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Update()
    {
        _smoothedDeltaTime += (Time.unscaledDeltaTime - _smoothedDeltaTime) * 0.1f;

        _refreshTimer += Time.unscaledDeltaTime;
        if (_refreshTimer < refreshInterval)
            return;

        _refreshTimer = 0f;

        RefreshTexts();
    }

    /// <summary>
    /// Met à jour la dernière action utilisateur importante affichée dans l'overlay.
    /// </summary>
    public void SetLastUserAction(string action)
    {
        _lastUserAction = string.IsNullOrWhiteSpace(action) ? "None" : action;
    }

    /// <summary>
    /// Ajoute explicitement un warning métier.
    /// </summary>
    public void PushWarning(string warningMessage)
    {
        if (string.IsNullOrWhiteSpace(warningMessage))
            return;

        EnqueueLimited(_warningLines, $"[WARN] {warningMessage}", maxWarningLines);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (string.IsNullOrWhiteSpace(logString))
            return;

        bool isStructured =
            logString.Contains("[TIME]") ||
            logString.Contains("[INPUT]") ||
            logString.Contains("[XR]") ||
            logString.Contains("[PERF]") ||
            logString.Contains("[WARN]");

        bool isImportantType =
            type == LogType.Warning ||
            type == LogType.Error ||
            type == LogType.Exception;

        if (isStructured || isImportantType)
        {
            EnqueueLimited(_logLines, logString, maxLogLines);
        }

        if (type == LogType.Warning || type == LogType.Error || type == LogType.Exception)
        {
            EnqueueLimited(_warningLines, logString, maxWarningLines);
        }
    }

    private void RefreshTexts(bool force = false)
    {
        if (!force && !_isInitialized && _timeModel == null)
        {
            // On permet quand même l'affichage FPS/logs sans modèle,
            // mais on évite d'annoncer une initialisation inexistante.
        }

        RefreshStatusText();
        RefreshLogsText();
    }

    private void RefreshStatusText()
    {
        if (statusText == null)
            return;

        float frameTimeMs = _smoothedDeltaTime * 1000f;
        float fps = _smoothedDeltaTime > 0f ? 1f / _smoothedDeltaTime : 0f;

        if (frameTimeMs > suspiciousFrameTimeMs || fps < lowFpsThreshold)
        {
            PushTransientWarning($"[PERF] Suspicious perf: {fps:0.} FPS / {frameTimeMs:0.0} ms");
        }

        string currentDate = GetCurrentDateText();
        string currentSpeed = GetCurrentSpeedText();

        StringBuilder sb = new StringBuilder(256);

        sb.AppendLine($"FPS: {fps:0.} | Frame: {frameTimeMs:0.0} ms");
        sb.AppendLine($"Date: {currentDate}");
        sb.AppendLine($"Speed: {currentSpeed}");
        sb.AppendLine($"Last action: {_lastUserAction}");

        if (_warningLines.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Warnings:");
            foreach (string warning in _warningLines)
            {
                sb.AppendLine($"- {warning}");
            }
        }

        statusText.text = sb.ToString();
    }

    private void RefreshLogsText()
    {
        if (logsText == null)
            return;

        if (_logLines.Count == 0)
        {
            logsText.text = string.Empty;
            return;
        }

        StringBuilder sb = new StringBuilder(512);

        foreach (string line in _logLines)
        {
            sb.AppendLine(line);
        }

        logsText.text = sb.ToString();
    }

    private string GetCurrentDateText()
    {
        if (_timeModel == null)
            return "N/A";

        return _timeModel.CurrentTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    private string GetCurrentSpeedText()
    {
        if (_timeModel == null)
            return "x?";

        return $"x{_timeModel.TimeScale}";
    }

    private void PushTransientWarning(string warningMessage)
    {
        foreach (string line in _warningLines)
        {
            if (line == warningMessage)
                return;
        }

        EnqueueLimited(_warningLines, warningMessage, maxWarningLines);
    }

    private void EnqueueLimited(Queue<string> queue, string value, int maxCount)
    {
        queue.Enqueue(value);

        while (queue.Count > maxCount)
        {
            queue.Dequeue();
        }
    }

    private void LogInfo(string message)
    {
        Debug.Log($"[INFO] [DebugOverlay] {message}", this);
    }
}