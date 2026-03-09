using System;
using UnityEngine;

[CreateAssetMenu(menuName = "XR/Solar System Config")]
public class SolarSystemConfig : ScriptableObject
{
    [Header("Scale settings")]
    public float distanceScale = 0.4f;
    public float planetSizeScale = 0.2f;
    public bool showOrbits = true;

    [Header("Planet global scale range")]
    [Min(0.0001f)] public float minScale = 0.5f;
    [Min(0.0001f)] public float maxScale = 3.0f;

    [Header("Handle Sensibility Settings")]
    public float positionSensitivity = 1.0f;
    public float rotationSensitivity = 1.0f;

    [Header("Planet metadata")]
    public PlanetInfo[] planets =
    {
        new PlanetInfo
        {
            planet = PlanetData.Planet.Mercury,
            displayName = "Mercury",
            distanceToSunAU = 0.39f,
            orbitalPeriodDays = 88f,
            diameterKm = 4879f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Venus,
            displayName = "Venus",
            distanceToSunAU = 0.72f,
            orbitalPeriodDays = 225f,
            diameterKm = 12104f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Earth,
            displayName = "Earth",
            distanceToSunAU = 1.00f,
            orbitalPeriodDays = 365f,
            diameterKm = 12742f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Mars,
            displayName = "Mars",
            distanceToSunAU = 1.52f,
            orbitalPeriodDays = 687f,
            diameterKm = 6779f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Jupiter,
            displayName = "Jupiter",
            distanceToSunAU = 5.20f,
            orbitalPeriodDays = 4333f,
            diameterKm = 139820f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Saturn,
            displayName = "Saturn",
            distanceToSunAU = 9.54f,
            orbitalPeriodDays = 10759f,
            diameterKm = 116460f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Uranus,
            displayName = "Uranus",
            distanceToSunAU = 19.19f,
            orbitalPeriodDays = 30687f,
            diameterKm = 50724f
        },
        new PlanetInfo
        {
            planet = PlanetData.Planet.Neptune,
            displayName = "Neptune",
            distanceToSunAU = 30.07f,
            orbitalPeriodDays = 60190f,
            diameterKm = 49244f
        }
    };

    [Serializable]
    public class PlanetInfo
    {
        [Header("Identity")]
        public PlanetData.Planet planet;
        public string displayName;

        [Header("Astronomical data")]
        [Tooltip("Distance moyenne au Soleil en unités astronomiques (UA).")]
        public float distanceToSunAU;

        [Tooltip("Période orbitale en jours terrestres.")]
        public float orbitalPeriodDays;

        [Tooltip("Diamètre moyen en kilomètres.")]
        public float diameterKm;
    }
}