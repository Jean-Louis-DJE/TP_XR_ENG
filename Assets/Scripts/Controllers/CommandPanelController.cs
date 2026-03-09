using System;
using UnityEngine;

/// <summary>
/// Contrôleur du panneau de commande VR.
/// Il orchestre les interactions entre la vue, le modèle temporel
/// et les autres composants applicatifs.
/// </summary>
public class CommandPanelController : IDisposable
{
    private readonly CommandPanelView _view;
    private readonly TimeModel _timeModel;
    private readonly SolarSystemConfig _config;
    private readonly PlanetScaleController _scaleController;
    private readonly PlanetTransformController _HandleSystemController;
    private DebugOverlay _debugOverlay;

    public CommandPanelController(
        CommandPanelView view,
        TimeModel timeModel,
        SolarSystemConfig config,
        PlanetScaleController scaleController,
        PlanetTransformController HandleSystemController,
        DebugOverlay debugOverlay
        )
    {
       
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _timeModel = timeModel ?? throw new ArgumentNullException(nameof(timeModel));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _scaleController = scaleController;
        _HandleSystemController = HandleSystemController;
        _debugOverlay = debugOverlay;

        BindEvents();
        InitializeView();

        Log("Initialisé.");
    }

    private void BindEvents()
    {
        _view.DateChanged += HandleDateChanged;
        _view.SpeedChanged += HandleSpeedChanged;
        _view.PlayClicked += HandlePlayClicked;
        _view.PauseClicked += HandlePauseClicked;
        _view.OrbitsToggled += HandleOrbitsToggled;
        _view.ResetScaleClicked += HandleResetScaleClicked;
        _view.ResetViewClicked += HandleResetViewClicked;
    }

    private void UnbindEvents()
    {
        _view.DateChanged -= HandleDateChanged;
        _view.SpeedChanged -= HandleSpeedChanged;
        _view.PlayClicked -= HandlePlayClicked;
        _view.PauseClicked -= HandlePauseClicked;
        _view.OrbitsToggled -= HandleOrbitsToggled;
        _view.ResetScaleClicked -= HandleResetScaleClicked;
        _view.ResetViewClicked -= HandleResetViewClicked;
    }

    private void InitializeView()
    {
        _view.SetDate(_timeModel.CurrentTime);
        _view.SetSpeed(1);
        _view.SetOrbitsToggle(_config.showOrbits); 

        Log("Vue synchronisée avec l'état initial.");
        
    }

    private void HandleDateChanged(DateTime newDate)
    {
        Log($"Date demandée : {newDate:dd/MM/yyyy}", "input");
        _debugOverlay?.SetLastUserAction("Date Changed");
        _timeModel.SetTime(newDate);
    }

    private void HandleSpeedChanged(int newSpeed)
    {
        Log($"Vitesse demandée : x{newSpeed}", "input");
        _debugOverlay?.SetLastUserAction("Speed Changed");
        _timeModel.SetScale(newSpeed);
    }

    private void HandlePlayClicked()
    {
        Log("Play demandé", "input");
        _debugOverlay?.SetLastUserAction("Speed Clicked");
        _timeModel.Play();
    }

    private void HandlePauseClicked()
    {
        Log("Pause demandée", "input");
        _debugOverlay?.SetLastUserAction("Pause Clicked");
        _timeModel.Pause();
    }

    private void HandleOrbitsToggled(bool isEnabled)
    {
        Log($"Toggle orbites : {(isEnabled ? "ON" : "OFF")}", "input");
        _debugOverlay?.SetLastUserAction("Orbit Toggled");
        _config.showOrbits = isEnabled ;
    }

    private void HandleResetScaleClicked()
    {
        Log("Reset scale demandé", "input");
        _debugOverlay?.SetLastUserAction("Scale Clicked");
        _scaleController.Scale(1);
    }

    private void HandleResetViewClicked()
    {
        Log("Reset view demandé", "input");
        _debugOverlay?.SetLastUserAction("Reseted View");
        _HandleSystemController.resetViewPose() ;
    }

    private void Log(string message, string level = "info")
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

        Debug.Log($"[{core}] [CommandPanelController] {message}");
    }

    public void Dispose()
    {
        UnbindEvents();
        Log("Disposed.");
    }
}