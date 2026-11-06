
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueNPC : MonoBehaviour, IInteractable
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAnimations playerAnimations;

    [Header("Player")]
    [SerializeField] private BurrowMovementIA burrowMovement;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject exclamation;

    [Header("Dialogue")]
    [SerializeField, TextArea(3, 4)]
    private string[] dialogueLines;

    [SerializeField] private float typingTime = 0.03f;
    [SerializeField] private string interactionPrompt = "Presiona E para hablar";

    [Header("Block System Settings")]
    [SerializeField] private int linesPerBlock ;
    public UnityEvent OnBlockCompleted; // Se dispara cada vez que completa un bloque de 3 líneas
    public UnityEvent OnAllDialoguesCompleted; // Se dispara cuando termine las 9 líneas en total
    private bool isPlayerInRange;
    private bool didDialogueStart;
    private bool isTyping;
    private int lineIndex;
    private int currentBlockEndIndex;
    private Coroutine typingCoroutine;

    private bool canInteract = true;

    // Indica si el jugador terminó el diálogo al menos una vez.
    public bool HasCompletedDialogue { get; private set; }

    public string InteractionPrompt => interactionPrompt;

    private void Awake()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (playerAnimations == null)
            playerAnimations = FindFirstObjectByType<PlayerAnimations>();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (exclamation != null)
            exclamation.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        if (!canInteract || (burrowMovement != null && burrowMovement.IsMoving))
        {
            return;
        }
        if (playerController == null)
        {
            Debug.LogWarning(
                "DialogueNPC: No se encontró PlayerController.",
                this
            );
            return;
        }

        if (didDialogueStart)
        {
            if (isTyping)
                FinishCurrentLine();
            else
                NextDialogueLine();

            return;
        }

        StartDialogue();
    }

    // NUEVO: Método para desbloquear el diálogo cuando Burrow llega al waypoint
    public void UnlockInteraction()
    {
        canInteract = true;

        // Opcional: si el jugador sigue en el rango, volvemos a mostrar la exclamación
        if (exclamation != null && isPlayerInRange && lineIndex < dialogueLines.Length)
        {
            exclamation.SetActive(true);
        }
    }
    private void StartDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning(
                "DialogueNPC: No hay líneas de diálogo configuradas.",
                this
            );
            return;
        }

        playerController.canMove = false;

        didDialogueStart = true;
        currentBlockEndIndex = Mathf.Min(lineIndex + linesPerBlock, dialogueLines.Length);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (exclamation != null)
            exclamation.SetActive(false);

        if (playerAnimations != null)
            playerAnimations.PlayWave();

        ShowLine();
    }

    private void NextDialogueLine()
    {
        lineIndex++;

        // Si llegó al final del bloque actual de 3 líneas
        if (lineIndex >= currentBlockEndIndex)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
        }
    }

    private void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;

        if (dialogueText == null)
        {
            Debug.LogWarning(
                "DialogueNPC: No se asignó Dialogue Text.",
                this
            );

            isTyping = false;
            typingCoroutine = null;
            yield break;
        }

        dialogueText.text = string.Empty;

        string line = dialogueLines[lineIndex];

        foreach (char character in line)
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(typingTime);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void FinishCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (dialogueText != null)
            dialogueText.text = dialogueLines[lineIndex];

        isTyping = false;
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        didDialogueStart = false;
        isTyping = false;
        canInteract = false;

        // Notifica que el jugador terminó la conversación.
        HasCompletedDialogue = true;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (playerController != null)
            playerController.canMove = true;

        if (exclamation != null)
            exclamation.SetActive(false);

        if (lineIndex >= dialogueLines.Length)
        {
            HasCompletedDialogue = true;
            OnAllDialoguesCompleted?.Invoke();
            Debug.Log("Diálogo totalmente completado: " + gameObject.name);
        }
        else
        {
            // Dispara el movimiento hacia el siguiente punto
            OnBlockCompleted?.Invoke();
            Debug.Log("Bloque de diálogo completado. Listo para moverse.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInRange = true;

        if (exclamation != null && !didDialogueStart && burrowMovement.IsMoving==false)
            exclamation.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInRange = false;

        if (exclamation != null && !didDialogueStart)
            exclamation.SetActive(false);
    }

    private void OnDisable()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = null;
        isTyping = false;

        if (playerController != null && didDialogueStart)
            playerController.canMove = true;

        didDialogueStart = false;
    }
}
