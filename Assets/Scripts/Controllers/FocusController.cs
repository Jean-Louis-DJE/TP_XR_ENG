using System;
using System.Linq;
using UnityEngine;

public class PlanetFocusController : IDisposable
{ 
    private readonly PlanetSelectionEmitter _selectionEmitter;
    private readonly BoardView _boardView;
    private readonly TMPTextView _textView;
    private readonly CameraFocusManager _cameraFocusManager;
    private readonly SolarSystemConfig _config;
    private readonly TimeModel _timeModel;

    private PlanetView _selectedPlanet;
    private bool _isFocused;

    private Vector3 _savedBoardPosition;
    private Quaternion _savedBoardRotation;

    private Vector3 _savedPlanetLocalPosition;
    private Quaternion _savedPlanetLocalRotation;

    public PlanetFocusController(
        PlanetSelectionEmitter selectionEmitter,
        BoardView boardView,
        TMPTextView textView,
        CameraFocusManager cameraFocusManager,
        SolarSystemConfig config,
        TimeModel timeModel)
    {
        _selectionEmitter = selectionEmitter ?? throw new ArgumentNullException(nameof(selectionEmitter));
        _boardView = boardView ?? throw new ArgumentNullException(nameof(boardView));
        _textView = textView ?? throw new ArgumentNullException(nameof(textView));
        _cameraFocusManager = cameraFocusManager ?? throw new ArgumentNullException(nameof(cameraFocusManager));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _timeModel = timeModel ?? throw new ArgumentNullException(nameof(timeModel));

        _selectionEmitter.PlanetSelected += HandlePlanetSelected;
        _selectionEmitter.PlanetDeselected += HandlePlanetDeselected;

        Log("Initialisé. Écoute des événements de sélection activée.");
    }

    private void HandlePlanetSelected(PlanetView planetView)
    {
        if (planetView == null)
        {
            Log("Sélection ignorée : PlanetView null.", "warning");
            return;
        }

        if (_isFocused)
        {
            Log("Sélection ignorée : un focus est déjà actif.", "warning");
            return;
        }

        _selectedPlanet = planetView;
        _isFocused = true;

        _timeModel.Pause();

        SaveCurrentState();
        FocusCameraAbovePlanet(_selectedPlanet);
        PlaceBoardAndPlanetForFocus(_selectedPlanet);
        UpdateBoardText(_selectedPlanet);

        Log($"Focus appliqué sur {_selectedPlanet.name}.", "output");
    }

    private void HandlePlanetDeselected(PlanetView planetView)
    {
        if (!_isFocused || _selectedPlanet == null)
        {
            Log("Désélection ignorée : aucun focus actif.", "warning");
            return;
        }

        if (planetView != _selectedPlanet)
        {
            Log("Désélection ignorée : la planète ne correspond pas au focus courant.", "warning");
            return;
        }

        RestorePreviousState();
        _timeModel.Play();

        Log($"Focus terminé sur {_selectedPlanet.name}.", "output");

        _selectedPlanet = null;
        _isFocused = false;
    }

    private void SaveCurrentState()
    {
        _cameraFocusManager.SaveCurrentPoseAsReturnPose();

        _savedBoardPosition = _boardView.transform.position;
        _savedBoardRotation = _boardView.transform.rotation;

        _savedPlanetLocalPosition = _selectedPlanet.transform.localPosition;
        _savedPlanetLocalRotation = _selectedPlanet.transform.localRotation;

        Log("État courant sauvegardé : XR Origin, board, planète.");
    }

    private void RestorePreviousState()
    {
        _cameraFocusManager.RestorePreviousPose();

        _boardView.SetPose(_savedBoardPosition, _savedBoardRotation);
        _selectedPlanet.SetPosition(_savedPlanetLocalPosition);
        _selectedPlanet.transform.localRotation = _savedPlanetLocalRotation;

        Log("État précédent restauré.");
    }

    private void FocusCameraAbovePlanet(PlanetView planetView)
    {
        Vector3 planetWorldPosition = planetView.transform.position;

        float contentHeight = 1.6f;
        float cameraDistance = 2.4f;

        // Point central surélevé où seront placés le board et la planète
        Vector3 focusCenter = planetWorldPosition + Vector3.up * contentHeight;

        // Direction horizontale fixe pour regarder la scène de face
        Vector3 cameraForward = Vector3.forward;

        // Caméra placée en face du contenu, à distance raisonnable
        Vector3 targetCameraPosition = focusCenter - cameraForward * cameraDistance;

        // Caméra orientée vers le centre du contenu, sans inclinaison
        Quaternion targetCameraRotation = Quaternion.LookRotation(cameraForward, Vector3.up);

        _cameraFocusManager.FocusCamera(targetCameraPosition, targetCameraRotation);

        Log($"Caméra focalisée proprement en face de {planetView.name}.");
    }

    private void PlaceBoardAndPlanetForFocus(PlanetView planetView)
    {
        Transform xrCamera = _cameraFocusManager.transform;

        float sideOffset = 0.55f;
        float contentDistanceFromCamera = 2.4f;
        float contentHeightOffset = -1.0f;

        // Centre de la zone de présentation, en face de la caméra
        Vector3 focusCenter =
            xrCamera.position +
            xrCamera.forward * contentDistanceFromCamera +
            Vector3.up * contentHeightOffset;

        // Placement horizontal : planète à gauche, board à droite
        Vector3 boardWorldPosition = focusCenter + Vector3.right * sideOffset;
        Vector3 planetWorldPosition = focusCenter - Vector3.right * sideOffset;

        // Orientation parfaitement verticale :
        // on regarde la caméra uniquement sur le plan horizontal
        Vector3 boardLookDirection = xrCamera.position - boardWorldPosition;
        boardLookDirection.y = 0f;
        if (boardLookDirection.sqrMagnitude < 0.0001f)
        {
            boardLookDirection = Vector3.back;
        }

        Vector3 planetLookDirection = xrCamera.position - planetWorldPosition;
        planetLookDirection.y = 0f;
        if (planetLookDirection.sqrMagnitude < 0.0001f)
        {
            planetLookDirection = Vector3.back;
        }

        Quaternion boardWorldRotation = Quaternion.LookRotation(boardLookDirection.normalized, Vector3.up);
        Quaternion planetWorldRotation = Quaternion.LookRotation(planetLookDirection.normalized, Vector3.up);

        _boardView.SetPose(boardWorldPosition, boardWorldRotation);

        SetPlanetWorldPosition(planetView, planetWorldPosition);
        SetPlanetWorldRotation(planetView, planetWorldRotation);

        Log("Board et planète repositionnés verticalement pour le focus.");
    }

    private void SetPlanetWorldPosition(PlanetView planetView, Vector3 worldPosition)
    {
        Transform parent = planetView.transform.parent;
        Vector3 localPosition = parent != null
            ? parent.InverseTransformPoint(worldPosition)
            : worldPosition;

        planetView.SetPosition(localPosition);
    }

    private void SetPlanetWorldRotation(PlanetView planetView, Quaternion worldRotation)
    {
        Transform parent = planetView.transform.parent;
        Quaternion localRotation = parent != null
            ? Quaternion.Inverse(parent.rotation) * worldRotation
            : worldRotation;

        planetView.transform.localRotation = localRotation;
    }

    private void UpdateBoardText(PlanetView planetView)
    {
        var info = _config.planets?.FirstOrDefault(p => p.planet == planetView.planet);

        if (info == null)
        {
            _textView.SetText(
                $"-Name: {planetView.planet}\n" +
                $"-Current time: {_timeModel.CurrentTime:dd/MM/yyyy HH:mm:ss}"
            );
            return;
        }

        string content =
            $"-Name: {info.displayName}\n" +
            $"-Distance to Sun: {info.distanceToSunAU} AU\n" +
            $"-Orbital period: {info.orbitalPeriodDays} days\n" +
            $"-Diameter: {info.diameterKm} km\n\n" +
            $"-Current time: {_timeModel.CurrentTime:dd/MM/yyyy HH:mm:ss}";

        _textView.SetText(content);
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

        Debug.Log($"[{core}] [PlanetSelectionEmitter] {message}");
    }

    public void Dispose()
    {
        _selectionEmitter.PlanetSelected -= HandlePlanetSelected;
        _selectionEmitter.PlanetDeselected -= HandlePlanetDeselected;

        Log("Disposed.");
    }
}