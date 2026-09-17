using UnityEngine;
// Esta línea es necesaria para usar el nuevo sistema de controles
using UnityEngine.InputSystem; 

public class ModelMovement : MonoBehaviour
{
    public Vector3 OgPosition;
    public Vector3 NewPosition;
    public float speed = 5f;
    private bool goToNewPosition = false;

    void Start()
    {
        OgPosition = transform.position;
    }

    void Update()
    {
        // Así se detecta la barra de espacio en el nuevo Input System
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            goToNewPosition = !goToNewPosition;
        }

        Vector3 destination = goToNewPosition ? NewPosition : OgPosition;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }
}