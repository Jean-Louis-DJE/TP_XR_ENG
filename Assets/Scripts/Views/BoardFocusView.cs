using UnityEngine;

/// <summary>
/// Vue responsable du placement du panneau d'information.
/// </summary>
public class BoardView : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    /// <summary>
    /// Place le tableau à une position et orientation données.
    /// </summary>
    public void SetPose(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
    }

    /// <summary>
    /// Replace le tableau à sa position et orientation initiales.
    /// </summary>
    public void RestoreInitialPose()
    {
        transform.SetPositionAndRotation(_initialPosition, _initialRotation);
    }
}