using UnityEngine;

public class AppBootstrapper : MonoBehaviour
{
    [Header("Global Configuration")]
    [Tooltip("Le ScriptableObject contenant les sensibilités et réglages.")]
    public SolarSystemConfig config;

    [Header("Scene References")]
    [Tooltip("L'objet racine du système solaire pour les manipulations globales.")] 
    [SerializeField] private SolarSystemView solarSystemRootView;
    
    [Tooltip("L'observateur de mouvements : cube.")]
    [SerializeField] private TransformRealtimeObserver observer;

    [Header("Scale UI")]
    [Tooltip("Le slider utilisé pour piloter le scale global du système solaire.")]
    [SerializeField] private SliderManager scaleSliderManager;

    [Header("Focus")]
    [Tooltip("Le tableau d'information à déplacer lors du focus.")]
    [SerializeField] private BoardView boardView;

    [Tooltip("La vue TMP à mettre à jour avec les informations de la planète sélectionnée.")]
    [SerializeField] private TMPTextView selectedPlanetInfoTextView;

    [Tooltip("Le manager de focus de la caméra XR.")]
    [SerializeField] private CameraFocusManager cameraFocusManager;

    [Tooltip("Liste des émetteurs de sélection présents sur les planètes.")]
    [SerializeField] private PlanetSelectionEmitter[] planetSelectionEmitters;

    [Header("Planets")]
    [Tooltip("Liste des vues de planètes à synchroniser avec les éphémérides.")]
    public PlanetView[] planets;

    // Modèles et Services
    private TimeModel _timeModel;
    private TimeController _timeController;

    // Les contrôleurs spécialisés
    private PlanetSystemController _orbitController;
    private PlanetTransformController _transformController;
    private PlanetScaleController _scaleController;
    private PlanetFocusController[] _focusControllers;

    void Start()
    {
        Debug.Log("[BOOT] Initializing application...");

        // 1. Initialisation des données et services de base
        _timeModel = new TimeModel();
        var ephemeris = new PlanetEphemerisService();

        // 2. Setup du moteur de temps (MonoBehaviour)
        _timeController = gameObject.AddComponent<TimeController>();
        _timeController.Init(_timeModel);

        // 3. Instanciation des contrôleurs
        
        // Gère les orbites et la position des planètes selon le temps
        _orbitController = new PlanetSystemController(
            _timeModel, 
            ephemeris, 
            planets,
            config
        );

        // Gère le déplacement (Position) et l'orientation (Rotation) du système
        _transformController = new PlanetTransformController(
            observer, 
            solarSystemRootView, 
            config
        );

        // Gère la mise à l'échelle (Scale) du système
        _scaleController = new PlanetScaleController(
            solarSystemRootView,
            config,
            scaleSliderManager
        );

        // Gère le focus sur les planètes sélectionnées
        if (planetSelectionEmitters != null && planetSelectionEmitters.Length > 0)
        {
            _focusControllers = new PlanetFocusController[planetSelectionEmitters.Length];

            for (int i = 0; i < planetSelectionEmitters.Length; i++)
            {
                if (planetSelectionEmitters[i] == null) continue;

                _focusControllers[i] = new PlanetFocusController(
                    planetSelectionEmitters[i],
                    boardView,
                    selectedPlanetInfoTextView,
                    cameraFocusManager,
                    config,
                    _timeModel
                );
            }
        }

        Debug.Log("[BOOT] Application Ready. All controllers initialized.");
    }

    private void OnDestroy()
    {
        Debug.Log("[BOOT] Cleaning up controllers...");

        // Libération des événements pour éviter les fuites de mémoire
        _orbitController?.Dispose();
        _transformController?.Dispose();
        _scaleController?.Dispose();

        if (_focusControllers != null)
        {
            for (int i = 0; i < _focusControllers.Length; i++)
            {
                _focusControllers[i]?.Dispose();
            }
        }
    }
}