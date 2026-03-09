using System;
using UnityEngine;

public class PlanetSystemController : IDisposable
{
    private readonly TimeModel _timeModel;
    private readonly IPlanetEphemerisService _ephemeris;
    private readonly PlanetView[] _planets;

    private readonly SolarSystemConfig _config;

    public PlanetSystemController(TimeModel timeModel, IPlanetEphemerisService ephemeris, PlanetView[] planets, SolarSystemConfig config)
    {
        _timeModel = timeModel;
        _ephemeris = ephemeris;
        _planets = planets;
        _config = config;

        _timeModel.OnTimeChanged += HandleTimeChanged;

        ApplyPlanetScale();

        if (_config.showOrbits)  BuildPlanetOrbits();

        Log("Initialisé pour la gestion des orbites.");
    }

    private void HandleTimeChanged(DateTime time)
    {
        foreach (var planet in _planets)
        {
            if (planet == null) continue;
            Vector3 pos = _ephemeris.GetPlanetPosition(planet.planet, time);

            planet.SetPosition(pos*_config.distanceScale);
            Log($"Mise à jour pos : {planet.planet.ToString()} - {time} - {pos*_config.distanceScale}", "output");
        }
    }

    private void BuildPlanetOrbits()
    {
        const int samples = 5000;
        const int stepDays = 30;

        DateTime start = _timeModel.CurrentTime;

        foreach (var planet in _planets)
        {
            if (planet == null) continue;

            Vector3[] points = new Vector3[samples];

            for (int i = 0; i < samples; i++)
            {
                DateTime t = start.AddDays(i * stepDays);
                points[i] = _ephemeris.GetPlanetPosition(planet.planet, t)*_config.distanceScale;
            }

            planet.DrawOrbit(points);
        }

        Log("Orbites calculées et envoyées aux vues.");
    }

    private void ApplyPlanetScale()
    {
        float scale = _config.planetSizeScale;

        foreach (var planet in _planets)
        {
            if (planet == null) continue;

            planet.SetUniformScale(scale);
        }

        Log($"Planet scale appliqué : {scale}", "output");
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

        Debug.Log($"[{core}] [Planet System Controller] {message}");
    }
    public void Dispose()
    {
        _timeModel.OnTimeChanged -= HandleTimeChanged;
        Log("Disposed.");
    }
}