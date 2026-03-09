using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Slider))]
public class SliderManager : MonoBehaviour
{
    [Header("Events")]
    [Tooltip("Événement déclenché avec la valeur brute du Slider (telle que définie dans l'Inspecteur)")]
    public UnityEvent<float> OnValueChanged;

    private Slider _slider;

    public float Value => _slider != null ? _slider.value : 0f;
    public float MinValue => _slider != null ? _slider.minValue : 0f;
    public float MaxValue => _slider != null ? _slider.maxValue : 1f;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        OnValueChanged?.Invoke(value);
        Log($"Nouvelle valeur : {value:F2}");
    }

    private void Log(string message)
    {
        Debug.Log($"[SliderManager] {message}");
    }

    public void SetValue(float value)
    {
        if (_slider == null) _slider = GetComponent<Slider>();
        _slider.value = value;
    }
}