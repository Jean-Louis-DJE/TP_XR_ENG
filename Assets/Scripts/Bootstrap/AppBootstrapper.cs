using UnityEngine;
using System;

public class AppBootstrapper : MonoBehaviour
{
    public SolarSystemConfig config;

    public PlanetView[] planets;

    TimeModel timeModel;
    PlanetSystemController controller;

    TimeController timeController;

    void Start()
    {
        Debug.Log("[BOOT] Initializing application");

        timeModel = new TimeModel();
        timeController = gameObject.AddComponent<TimeController>();

        var ephemeris = new PlanetEphemerisService();

        controller = new PlanetSystemController(
            timeModel,
            ephemeris,
            planets
        );

        timeController.Init(timeModel);

    }
}
