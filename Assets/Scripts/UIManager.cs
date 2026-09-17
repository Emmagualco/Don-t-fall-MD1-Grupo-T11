using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla los paneles de interfaz del prototipo: mensajes de ayuda
/// (tooltips) que explican los controles, la pantalla de muerte al caer al
/// vacío y la pantalla de nivel completado. Requiere que los paneles de UI
/// (cada uno con un CanvasGroup y un Text) sean asignados desde el Inspector.
/// Debe existir una única instancia de este componente por escena.
/// </summary>
public class UIManager : MonoBehaviour
{
    private const float PanelFadeSeconds = 0.25f;
    private const string VoidDeathMessage = "Has caído al vacío...\nReiniciando nivel";
    private const string LevelCompleteMessage = "¡Nivel completado!";

    /// <summary>Instancia activa del UIManager en la escena actual.</summary>
    public static UIManager Instance { get; private set; }

    [Header("Tooltip de ayuda (movimiento, salto, paredes, etc.)")]
    [SerializeField] private CanvasGroup tooltipGroup;
    [SerializeField] private Text tooltipText;

    [Header("Pantalla de muerte (caída al vacío)")]
    [SerializeField] private CanvasGroup deathGroup;
    [SerializeField] private Text deathText;
    [SerializeField] private float deathScreenHoldSeconds = 1.5f;

    [Header("Pantalla de nivel completado")]
    [SerializeField] private CanvasGroup levelCompleteGroup;
    [SerializeField] private Text levelCompleteText;
    [SerializeField] private float levelCompleteHoldSeconds = 1.5f;

    private Coroutine activeTooltipRoutine;

    private void Awake()
    {
        Instance = this;
        HideImmediately(tooltipGroup);
        HideImmediately(deathGroup);
        HideImmediately(levelCompleteGroup);
    }

    /// <summary>Muestra un mensaje de ayuda temporal en pantalla (por ejemplo, cómo moverse o saltar).</summary>
    public void ShowTooltip(string message, float displaySeconds)
    {
        if (tooltipGroup == null || tooltipText == null)
        {
            return;
        }

        if (activeTooltipRoutine != null)
        {
            StopCoroutine(activeTooltipRoutine);
        }
        activeTooltipRoutine = StartCoroutine(RunTooltip(message, displaySeconds));
    }

    /// <summary>Muestra la pantalla de "caíste al vacío" y luego ejecuta la acción de reinicio.</summary>
    public void ShowDeathScreen(Action onFinished)
    {
        if (deathText != null)
        {
            deathText.text = VoidDeathMessage;
        }
        StartCoroutine(RunEndScreen(deathGroup, deathScreenHoldSeconds, onFinished));
    }

    /// <summary>Muestra la pantalla de "nivel completado" y luego ejecuta la acción de avance de nivel.</summary>
    public void ShowLevelComplete(Action onFinished)
    {
        if (levelCompleteText != null)
        {
            levelCompleteText.text = LevelCompleteMessage;
        }
        StartCoroutine(RunEndScreen(levelCompleteGroup, levelCompleteHoldSeconds, onFinished));
    }

    private IEnumerator RunTooltip(string message, float displaySeconds)
    {
        tooltipText.text = message;
        yield return Fade(tooltipGroup, 1f);
        yield return new WaitForSeconds(displaySeconds);
        yield return Fade(tooltipGroup, 0f);
    }

    private IEnumerator RunEndScreen(CanvasGroup group, float holdSeconds, Action onFinished)
    {
        yield return Fade(group, 1f);
        yield return new WaitForSeconds(holdSeconds);
        onFinished?.Invoke();
    }

    private IEnumerator Fade(CanvasGroup group, float targetAlpha)
    {
        if (group == null)
        {
            yield break;
        }

        group.blocksRaycasts = targetAlpha > 0f;
        group.interactable = targetAlpha > 0f;

        float startAlpha = group.alpha;
        float elapsedSeconds = 0f;
        while (elapsedSeconds < PanelFadeSeconds)
        {
            elapsedSeconds += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedSeconds / PanelFadeSeconds);
            yield return null;
        }
        group.alpha = targetAlpha;
    }

    private void HideImmediately(CanvasGroup group)
    {
        if (group == null)
        {
            return;
        }
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }
}
