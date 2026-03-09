using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderValueObserver : MonoBehaviour
{
    public event Action<float> OnMappedValueChanged;

    [Header("Mapped Value Range")]
    [SerializeField, Min(0.0001f)] private float minValue = 0.1f;
    [SerializeField, Min(0.0001f)] private float maxValue = 10f;

    [Header("Optional")]
    [SerializeField] private bool emitOnStart = true;

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();

        if (maxValue <= minValue)
        {
            Log(" MaxValue must be greater than minValue. Adjusting automatically.", "warning");
            maxValue = minValue + 0.001f;
        }

        // On force le slider à travailler sur [0,1]
        slider.minValue = 0f;
        slider.maxValue = 1f;

        Log($"Initialization => mapped range [{minValue}, {maxValue}]");
    }

    void OnEnable()
    {
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void Start()
    {
        if (emitOnStart)
            EmitMappedValue(slider.value);
    }

    void OnDisable()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float normalizedValue)
    {
        EmitMappedValue(normalizedValue);
    }

    private void EmitMappedValue(float normalizedValue)
    {
        float mappedValue = Mathf.Lerp(minValue, maxValue, normalizedValue);
        OnMappedValueChanged?.Invoke(mappedValue);

        Log($"Slider: {normalizedValue:F3} => Mapped Value: {mappedValue:F4}");
    }

    public void SetNormalizedValue(float normalizedValue)
    {
        if (slider == null)
            return;

        slider.value = Mathf.Clamp01(normalizedValue);
        Log("Clamp01", "warning");
    }

    public void SetMappedValue(float value)
    {
        if (slider == null)
            return;

        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);
        slider.value = normalizedValue;
        Log("Mappage", "warning");
    }

    public float GetMappedValue()
    {
        if (slider == null)
            return minValue;

        return Mathf.Lerp(minValue, maxValue, slider.value);
    }

    private void Log(string message, string level = "general")
{
    string core;

    switch (level.ToLower())
    {
        case "input":       // entrées utilisateur
            core = "INPUT";
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

    Debug.Log($"[{core}] [UI Slider Value Observer] {message}");
}
}