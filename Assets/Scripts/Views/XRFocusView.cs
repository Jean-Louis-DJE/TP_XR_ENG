using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Permet de déplacer la caméra XR vers une pose exacte en déplaçant le XR Origin,
/// puis de revenir à la pose d'origine avant le focus.
/// 
/// À placer sur la caméra du XR Rig.
/// </summary>
public class CameraFocusManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("XR Origin contenant cette caméra. Si vide, sera recherché automatiquement dans les parents.")]
    [SerializeField] private XROrigin xrOrigin;

    [Header("Debug")]
    [SerializeField] private bool enableLogs = true;

    private Transform _cameraTransform;
    private bool _hasSavedPose;
    private bool _isFocused;

    private Vector3 _savedOriginPosition;
    private Quaternion _savedOriginRotation;

    private void Awake()
    {
        _cameraTransform = transform;

        if (xrOrigin == null)
        {
            xrOrigin = GetComponentInParent<XROrigin>();
        }

        if (xrOrigin == null)
        {
            Debug.LogError(
                $"[CameraFocusManager][{gameObject.name}] Aucun XROrigin trouvé dans les parents.",
                this
            );
            enabled = false;
            return;
        }

        Log($"Initialisé. XR Origin détecté : {xrOrigin.name}");
    }

    /// <summary>
    /// Déplace la caméra XR vers une pose monde exacte.
    /// La pose précédente du XR Origin est sauvegardée au premier focus.
    /// </summary>
    /// <param name="targetWorldPosition">Position monde exacte voulue pour la caméra.</param>
    /// <param name="targetWorldRotation">Rotation monde exacte voulue pour la caméra.</param>
    public void FocusCamera(Vector3 targetWorldPosition, Quaternion targetWorldRotation)
    {
        if (xrOrigin == null)
        {
            Log("Focus ignoré : XR Origin manquant.");
            return;
        }

        SaveOriginPoseIfNeeded();

        Transform originTransform = xrOrigin.transform;

        Vector3 cameraLocalPosition = _cameraTransform.localPosition;
        Quaternion cameraLocalRotation = _cameraTransform.localRotation;

        Quaternion targetOriginRotation = targetWorldRotation * Quaternion.Inverse(cameraLocalRotation);
        Vector3 targetOriginPosition = targetWorldPosition - (targetOriginRotation * cameraLocalPosition);

        originTransform.SetPositionAndRotation(targetOriginPosition, targetOriginRotation);

        _isFocused = true;

        Log(
            $"Focus appliqué | " +
            $"Caméra cible pos={targetWorldPosition} rot={targetWorldRotation.eulerAngles} | " +
            $"Origin pos={targetOriginPosition} rot={targetOriginRotation.eulerAngles}"
        );
    }

    /// <summary>
    /// Fait un focus sur un Transform en utilisant sa position et sa rotation monde.
    /// </summary>
    /// <param name="target">Transform cible.</param>
    public void FocusOnTransform(Transform target)
    {
        if (target == null)
        {
            Log("Focus ignoré : target null.");
            return;
        }

        FocusCamera(target.position, target.rotation);
    }

    /// <summary>
    /// Fait un focus sur une planète avec un offset monde optionnel.
    /// Pratique pour cadrer légèrement devant/au-dessus de la planète.
    /// </summary>
    /// <param name="target">Transform cible.</param>
    /// <param name="worldOffset">Offset en espace monde ajouté à la position cible.</param>
    public void FocusOnTransformWithWorldOffset(Transform target, Vector3 worldOffset)
    {
        if (target == null)
        {
            Log("Focus ignoré : target null.");
            return;
        }

        FocusCamera(target.position + worldOffset, target.rotation);
    }

    /// <summary>
    /// Ramène le XR Origin à sa pose sauvegardée avant le premier focus.
    /// </summary>
    public void RestorePreviousPose()
    {
        if (!_hasSavedPose || xrOrigin == null)
        {
            Log("Restore ignoré : aucune pose sauvegardée.");
            return;
        }

        xrOrigin.transform.SetPositionAndRotation(_savedOriginPosition, _savedOriginRotation);
        _isFocused = false;

        Log(
            $"Pose restaurée | Origin pos={_savedOriginPosition} rot={_savedOriginRotation.eulerAngles}"
        );
    }

    /// <summary>
    /// Efface la pose sauvegardée. Le prochain focus sauvegardera une nouvelle pose de départ.
    /// </summary>
    public void ClearSavedPose()
    {
        _hasSavedPose = false;
        _isFocused = false;
        Log("Pose sauvegardée effacée.");
    }

    /// <summary>
    /// Sauvegarde immédiatement la pose actuelle du XR Origin comme pose de retour.
    /// </summary>
    public void SaveCurrentPoseAsReturnPose()
    {
        if (xrOrigin == null) return;

        _savedOriginPosition = xrOrigin.transform.position;
        _savedOriginRotation = xrOrigin.transform.rotation;
        _hasSavedPose = true;

        Log(
            $"Pose actuelle sauvegardée | Origin pos={_savedOriginPosition} rot={_savedOriginRotation.eulerAngles}"
        );
    }

    private void SaveOriginPoseIfNeeded()
    {
        if (_hasSavedPose) return;

        _savedOriginPosition = xrOrigin.transform.position;
        _savedOriginRotation = xrOrigin.transform.rotation;
        _hasSavedPose = true;

        Log(
            $"Pose de départ sauvegardée | Origin pos={_savedOriginPosition} rot={_savedOriginRotation.eulerAngles}"
        );
    }

    private void Log(string message)
    {
        if (!enableLogs) return;
        Debug.Log($"[CameraFocusManager][{gameObject.name}] {message}", this);
    }
}