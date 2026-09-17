using UnityEngine;

public class Rotation : MonoBehaviour
{
    // Puedes cambiar la velocidad de giro desde el Inspector de Unity
    public float InfiniteRotation = 50f; 

    void Update()
    {
        // Rota el objeto sobre el eje Y (como un trompo o un ventilador)
        transform.Rotate(Vector3.up * InfiniteRotation * Time.deltaTime);
    }
}
