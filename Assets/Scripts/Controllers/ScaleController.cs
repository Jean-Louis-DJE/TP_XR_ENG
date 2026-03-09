using System;
using UnityEngine;

public class PlanetScaleController : IDisposable
{
    private readonly SolarSystemView _view;
    private readonly SolarSystemConfig _config;
    private readonly SliderManager _sliderManager;

    private bool _isDisposed;

    public PlanetScaleController(
        SolarSystemView view,
        SolarSystemConfig config,
        SliderManager sliderManager)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _sliderManager = sliderManager ?? throw new ArgumentNullException(nameof(sliderManager));

        ValidateConfig();

        _sliderManager.OnValueChanged.AddListener(Scale);

        Log("Initialisé. Abonnement à SliderManager.OnValueChanged effectué.");

        Scale(_sliderManager.Value);
        Log($"Synchronisation initiale appliquée avec la valeur du slider : {_sliderManager.Value:F2}");
    }

    public void Scale(float sliderValue)
    {
        if (_isDisposed)
        {
            Log("Appel ignoré : controller déjà disposed.", "warning");
            return;
        }

        float normalizedValue = NormalizeSliderValue(sliderValue);
        float targetScale = Mathf.Lerp(_config.minScale, _config.maxScale, normalizedValue);

        _view.SetUniformScale(targetScale);

        Log(
            $"Slider brut: {sliderValue:F2} | " +
            $"normalisé: {normalizedValue:F2} | " +
            $"scale appliqué: {targetScale:F2}",
            "warning"
        );
    }

    private float NormalizeSliderValue(float rawValue)
    {
        if (Mathf.Approximately(_sliderManager.MinValue, _sliderManager.MaxValue))
        {
            Log("Le Slider a minValue == maxValue. Fallback sur 0.", "warning");
            return 0f;
        }

        return Mathf.InverseLerp(_sliderManager.MinValue, _sliderManager.MaxValue, rawValue);
    }

    private void ValidateConfig()
    {
        if (_config.minScale <= 0f)
            throw new ArgumentOutOfRangeException(nameof(_config.minScale), "minScale doit être > 0.");

        if (_config.maxScale <= 0f)
            throw new ArgumentOutOfRangeException(nameof(_config.maxScale), "maxScale doit être > 0.");

        if (_config.maxScale < _config.minScale)
            throw new ArgumentException("maxScale doit être >= minScale.");
    }

    private void Log(string message, string level = "general")
    {
        string core;

        switch (level.ToLower())
        {
            case "input":       // entrées utilisateur
                core = "XR INPUT";
                break;

            case "warning":     // avertissements pendant le traitement
                core = "WARN";
                break;

            case "output":      // sorties / résultats
                core = "OUTPUT";
                break;

            default:            // cas général
                core = "INFO";
                break;
        }

        Debug.Log($"[{core}] [PlanetScaleController] {message}");
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _sliderManager.OnValueChanged.RemoveListener(Scale);
        _isDisposed = true;

        Log("Disposed. Désabonnement effectué.");
    }
}