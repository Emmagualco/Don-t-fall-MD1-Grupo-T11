using System.Collections;
using UnityEngine;

public class ToggleVisibilityAndCollision : MonoBehaviour
{
    [Header("Settings")]
    // Public variable to change the time interval from the Inspector
    public float toggleInterval = 2.0f;

    // True = Starts visible | False = Starts hidden
    public bool startVisible = true;

    // References to the components we need to change
    private MeshRenderer objectRenderer;
    private Collider objectCollider;

    void Start()
    {
        // Get the Renderer component to control visibility
        objectRenderer = GetComponent<MeshRenderer>();

        // Get the Collider component to control physical touch
        objectCollider = GetComponent<Collider>();

        // Check if the components exist to prevent errors
        if (objectRenderer == null || objectCollider == null)
        {
            Debug.LogError("Missing MeshRenderer or Collider on " + gameObject.name);
            return;
        }

        // Set the initial state based on your choice in the Inspector
        objectRenderer.enabled = startVisible;
        objectCollider.enabled = startVisible;

        // Start the repeating timer logic
        StartCoroutine(ToggleCycle());
    }

    IEnumerator ToggleCycle()
    {
        // This loop runs forever while the object is active
        while (true)
        {
            // Wait for the amount of seconds set in 'toggleInterval'
            yield return new WaitForSeconds(toggleInterval);

            // Toggle the state (if true, becomes false. If false, becomes true)
            objectRenderer.enabled = !objectRenderer.enabled;
            objectCollider.enabled = !objectCollider.enabled;
        }
    }
}

