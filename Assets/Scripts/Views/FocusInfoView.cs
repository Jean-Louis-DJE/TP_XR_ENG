using TMPro;
using UnityEngine;

/// <summary>
/// Vue simple pour afficher du texte dans un composant TextMeshPro.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TMPTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;

    private void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
        }
    }

    /// <summary>
    /// Met à jour le texte affiché.
    /// </summary>
    public void SetText(string value)
    {
        if (textComponent == null)
        {
            Debug.LogWarning($"[TMPTextView][{gameObject.name}] TMP_Text manquant.", this);
            return;
        }

        textComponent.text = value;
    }

    /// <summary>
    /// Efface le texte affiché.
    /// </summary>
    public void Clear()
    {
        SetText(string.Empty);
    }
}