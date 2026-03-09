using UnityEngine;

public class SolarSystemView : MonoBehaviour
{
    private Pose _initialPose;

    private void Awake()
    {
        InitializeInitialPose();
    }

    /// <summary>
    /// Enregistre la position et la rotation initiales du système solaire.
    /// </summary>
    public void InitializeInitialPose()
    {
        _initialPose = new Pose(transform.position, transform.rotation);
    }

    /// <summary>
    /// Retourne la pose initiale enregistrée (position + rotation).
    /// </summary>
    public Pose GetInitialPose()
    {
        return _initialPose;
    }

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
    public void SetRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }

    /// <summary>
    /// Applique une rotation relative à la rotation actuelle.
    /// </summary>
    public void ApplyRotation(Quaternion rotationDelta)
    {
        transform.rotation = rotationDelta * transform.rotation;
    }

    /// <summary>
    /// Modifie l'échelle de manière uniforme avec un seul facteur.
    /// </summary>
    public void SetUniformScale(float scaleFactor)
    {
        transform.localScale = Vector3.one * scaleFactor;
    }
}