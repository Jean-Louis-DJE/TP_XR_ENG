using System;
using UnityEngine;

public class PlanetEphemerisService : IPlanetEphemerisService
{
    public Vector3 GetPlanetPosition(PlanetData.Planet planet, DateTime date)
    {
        Vector3 p = PlanetData.GetPlanetPosition(planet, date);
        return new Vector3(p.x, p.z, p.y);
    }
}
