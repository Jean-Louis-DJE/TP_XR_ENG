using System;
using UnityEngine;

public class PlanetEphemerisService : IPlanetEphemerisService
{
    public Vector3 GetPlanetPosition(PlanetData.Planet planet, DateTime date)
    {
        Vector3 p = PlanetData.GetPlanetPosition(planet, date);
        var offset = 0.5f;
        return new Vector3(p.x, p.z+offset, p.y);
    }
}
