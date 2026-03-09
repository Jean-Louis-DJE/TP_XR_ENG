using UnityEngine;

public class SolarSystemView : MonoBehaviour
{
    /// <summary>
    /// Retourne la position actuelle du GameObject dans l'espace mondial.
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Modifie la position du GameObject dans l'espace mondial.
    /// </summary>
    /// <param name="newPosition">Les nouvelles coordonnées X, Y, Z.</param>
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    /// <summary>
    /// Retourne la rotation actuelle du GameObject dans l'espace mondial.
    /// </summary>
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    /// <summary>
    /// Définit la rotation absolue du GameObject dans l'espace mondial.
    /// </summary>
    /// <param name="newRotation">La nouvelle rotation à appliquer.</param>
    public void SetRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }

    /// <summary>
    /// Applique une rotation relative à la rotation actuelle.
    /// </summary>
    /// <param name="rotationDelta">Rotation à appliquer par rapport à l'état courant.</param>
    public void ApplyRotation(Quaternion rotationDelta)
    {
        transform.rotation = rotationDelta * transform.rotation;
    }

    /// <summary>
    /// Modifie l'échelle de manière uniforme avec un seul facteur.
    /// </summary>
    /// <param name="scaleFactor">Facteur d'échelle uniforme.</param>
    public void SetUniformScale(float scaleFactor)
    {
        transform.localScale = Vector3.one * scaleFactor;
    }
}