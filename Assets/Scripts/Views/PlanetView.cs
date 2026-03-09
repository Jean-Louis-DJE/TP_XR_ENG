using UnityEngine;

public class PlanetView : MonoBehaviour
{
    public PlanetData.Planet planet;

    [SerializeField] private LineRenderer orbitRenderer;

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public void SetRotation(Quaternion rotation)
    {
        transform.localRotation = rotation;
    }

    public Quaternion GetRotation()
    {
        return transform.localRotation;
    }

    public void SetUniformScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void DrawOrbit(Vector3[] points)
    {
        if (orbitRenderer == null)
        {
            Debug.LogWarning($"[PlanetView] LineRenderer manquant pour {name}");
            return;
        }

        orbitRenderer.useWorldSpace = false;

        Vector3[] correctedPoints = new Vector3[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            correctedPoints[i] = points[i] - transform.localPosition;
        }

        orbitRenderer.positionCount = correctedPoints.Length;
        orbitRenderer.SetPositions(correctedPoints);
    }
}