using System;
using UnityEngine;

public class PlanetTransformController : IDisposable
{
    private readonly TransformRealtimeObserver _observer;
    private readonly SolarSystemView _view;
    private readonly SolarSystemConfig _config;

    private Pose _initialPose;

    private bool _isDisposed;
    private DebugOverlay _debugOverlay;

    public PlanetTransformController(
        TransformRealtimeObserver observer,
        SolarSystemView view,
        SolarSystemConfig config,
        DebugOverlay debugOverlay)
    {
        
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _debugOverlay = debugOverlay;

        _observer.OnTablePositionOffsetChanged += HandlePositionChanged;
        _observer.OnTableRotationOffsetChanged += HandleRotationChanged;

        _initialPose = _view.GetInitialPose();

        Log("Initialisé. Écoute des offsets de position et de rotation activée.");
    }

    private void HandlePositionChanged(Vector3 offset)
    {
        if (_isDisposed) return;

        Vector3 scaledOffset = offset * _config.positionSensitivity;
        Vector3 currentPosition = _view.GetPosition();
        Vector3 targetPosition = currentPosition + scaledOffset;

        _view.SetPosition(targetPosition);

        Log(
            $"Position update | " +
            $"offset brut: {offset} | " +
            $"offset appliqué: {scaledOffset} | " +
            $"nouvelle position: {targetPosition}",
            "output"
        );

        _debugOverlay?.SetLastUserAction("Moving Handle");
    }

    private void HandleRotationChanged(Quaternion rotationOffset)
    {
        if (_isDisposed) return;

        rotationOffset.ToAngleAxis(out float angle, out Vector3 axis);

        if (IsInvalidAxis(axis))
        {
            Log("Rotation ignorée : axe invalide détecté.", "warning");
            _debugOverlay?.PushWarning("Axe invalide détecté");
            return;
        }

        Quaternion amplifiedRotation = Quaternion.AngleAxis(
            angle * _config.rotationSensitivity,
            axis
        );

        _view.ApplyRotation(amplifiedRotation); 

        Log(
            $"Rotation update | " +
            $"angle brut: {angle:F2} | " +
            $"axe: {axis} | " +
            $"sensibilité: {_config.rotationSensitivity:F2}",
            "output"
        );
        _debugOverlay?.SetLastUserAction("Rotating Handle");
    }

    private static bool IsInvalidAxis(Vector3 axis)
    {
        return float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z);
    }

    public void resetViewPose()
    {
        _view.SetPosition(_initialPose.position);
        _view.SetRotation(_initialPose.rotation);
        Log("Reset view", "output");
        _debugOverlay?.SetLastUserAction("View Reseted");
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

        Debug.Log($"[{core}] [PlanetTransformController] {message}");
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _observer.OnTablePositionOffsetChanged -= HandlePositionChanged;
        _observer.OnTableRotationOffsetChanged -= HandleRotationChanged;

        _isDisposed = true;

        Log("Disposed. Désabonnement effectué.");
    }
}