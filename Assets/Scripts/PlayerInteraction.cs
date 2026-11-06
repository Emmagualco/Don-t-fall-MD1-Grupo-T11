
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private const float PromptDisplaySeconds = 2f;

    [Header("Configuración de interacción")]
    [SerializeField] private float interactionDistance = 10f;
    [SerializeField] private Transform interactionCamera;
    [SerializeField] private LayerMask interactionLayerMask = ~0;

    private IInteractable currentTarget;
    private IInteractable previousTarget;

    private void Awake()
    {
        if (interactionCamera == null && Camera.main != null)
            interactionCamera = Camera.main.transform;
    }

    private void Update()
    {
        DetectInteractable();
        ShowPromptOnNewTarget();
        ReadInteractionInput();

        previousTarget = currentTarget;
    }

    private void DetectInteractable()
    {
        currentTarget = null;

        if (interactionCamera == null)
            return;

        RaycastHit[] hits = Physics.RaycastAll(
            interactionCamera.position,
            interactionCamera.forward,
            interactionDistance,
            interactionLayerMask,
            QueryTriggerInteraction.Collide
        );

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // Ignora los colliders del propio jugador.
            if (hit.collider.transform == transform ||
                hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            IInteractable interactable =
                hit.collider.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            currentTarget = interactable;
            break;
        }
    }

    private void ShowPromptOnNewTarget()
    {
        if (currentTarget == null ||
            currentTarget == previousTarget ||
            UIManager.Instance == null)
        {
            return;
        }

        UIManager.Instance.ShowTooltip(
            currentTarget.InteractionPrompt,
            PromptDisplaySeconds
        );
    }

    private void ReadInteractionInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            !keyboard.eKey.wasPressedThisFrame ||
            currentTarget == null)
        {
            return;
        }

        Debug.Log(
            "Interactuando con: " + currentTarget.GetType().Name
        );

        PlayerAnimations playerAnimations =
            GetComponentInChildren<PlayerAnimations>();

        if (playerAnimations == null)
            playerAnimations = GetComponentInParent<PlayerAnimations>();

        // La animación de saludo ahora se ejecuta desde DialogueNPC.
        // Evitamos reproducirla dos veces.

        if (currentTarget is InteractableObject)
        {
            if (playerAnimations != null)
                playerAnimations.PlayPickup();
        }

        currentTarget.Interact(gameObject);
    }
}
