using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Émet un événement lorsqu'une planète est sélectionnée via un interactable XR
/// et applique un material de sélection.
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class PlanetSelectionEmitter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Vue planète associée à cet objet interactif.")]
    [SerializeField] private PlanetView planetView;

    [Tooltip("Material appliqué lorsque la planète est sélectionnée.")]
    [SerializeField] private Material selectedMaterial;


    public event Action<PlanetView> PlanetSelected;
    public event Action<PlanetView> PlanetDeselected;

    private XRSimpleInteractable _interactable;

    private Renderer[] _renderers;
    private Material[][] _originalMaterials;

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();

        if (planetView == null)
        {
            planetView = GetComponent<PlanetView>();
        }

        if (planetView == null)
        {
            Debug.LogError(
                $"[PlanetSelectionEmitter][{gameObject.name}] Aucune PlanetView assignée ou trouvée.",
                this
            );
        }

        // On récupère tous les renderers de la planète
        _renderers = GetComponentsInChildren<Renderer>();

        // On sauvegarde les matériaux d'origine
        _originalMaterials = new Material[_renderers.Length][];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].materials;
        }
    }

    private void OnEnable()
    {
        if (_interactable == null) return;

        _interactable.selectEntered.AddListener(HandleSelectEntered);
        _interactable.selectExited.AddListener(HandleSelectExited);
    }

    private void OnDisable()
    {
        if (_interactable == null) return;

        _interactable.selectEntered.RemoveListener(HandleSelectEntered);
        _interactable.selectExited.RemoveListener(HandleSelectExited);
    }

    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
        if (planetView == null) return;

        string interactorName = args.interactorObject?.transform?.name ?? "UnknownInteractor";

        Log($"Planète sélectionnée par {interactorName} : {planetView.name}");

        ApplySelectedMaterial();

        PlanetSelected?.Invoke(planetView);
    }

    private void HandleSelectExited(SelectExitEventArgs args)
    {
        if (planetView == null) return;

        string interactorName = args.interactorObject?.transform?.name ?? "UnknownInteractor";

        Log($"Planète désélectionnée par {interactorName} : {planetView.name}");

        RestoreOriginalMaterials();

        PlanetDeselected?.Invoke(planetView);
    }

    private void ApplySelectedMaterial()
    {
        if (selectedMaterial == null) return;

        foreach (var renderer in _renderers)
        {
            Material[] mats = renderer.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = selectedMaterial;
            }

            renderer.materials = mats;
        }
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].materials = _originalMaterials[i];
        }
    }

    private void Log(string message)
    {
        Debug.Log($"[PlanetSelectionEmitter][{gameObject.name}] {message}", this);
    }
}