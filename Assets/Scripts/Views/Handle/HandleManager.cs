using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class TransformRealtimeObserver : MonoBehaviour
{

    public event Action<Vector3> OnTablePositionOffsetChanged;
    public event Action<Quaternion> OnTableRotationOffsetChanged;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Renderer objectRenderer;
    private Color initialColor;

    private bool isSelected;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
            initialColor = objectRenderer.material.color;
        
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        Debug.Log("Initialization => Pos : "+initialPosition+" Rotation : "+initialRotation+" Color : "+initialColor);
    }   

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        grabInteractable.selectEntered.AddListener(OnSelected);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelected);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    void Update()
    {
        if (!isSelected)
            return;

        if (Vector3.Distance(transform.position, lastPosition) > 0.001f)
        {
            EmitCurrentPositionState();
        }

        if (Quaternion.Angle(transform.rotation, lastRotation) > 0.1f)
        {
            EmitCurrentRotationState();
        }
    }

    private void EmitCurrentPositionState()
    {
        Vector3 currentPosition = transform.position ;
        Vector3 offset = currentPosition - lastPosition;
        OnTablePositionOffsetChanged?.Invoke(offset);
        lastPosition = currentPosition;
        Log("Offset Position : "+offset);
    }

    private void EmitCurrentRotationState()
    {
        Quaternion currentRotation = transform.rotation ;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(lastRotation);
        OnTableRotationOffsetChanged?.Invoke(deltaRotation);
        lastRotation = currentRotation;
        Log("Delta Rotation : "+deltaRotation);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        isSelected = true;

        lastPosition = transform.position;
        lastRotation = transform.rotation;

        Log("Object grabbed", "input");

        if (objectRenderer != null)
            objectRenderer.material.color = Color.red;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isSelected = false;

        Log("Object released", "input");

        if (objectRenderer != null)
            objectRenderer.material.color = initialColor;
        
        transform.SetPositionAndRotation(initialPosition, initialRotation);
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

        Debug.Log($"[{core}] [Transform Realtime Observer] {message}");
    }
}