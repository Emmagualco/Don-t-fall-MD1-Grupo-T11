using UnityEngine;

using UnityEngine.InputSystem;

public class HighJump : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isOnMegaPad;
    private float currentPadForce; // Aquí guardamos la fuerza de la plataforma actual

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(groundCheckPoint.position, checkRadius, groundLayer);
        
        isOnMegaPad = false;

        foreach (var collider in hitColliders)
        {
            // 1. Buscamos si la plataforma tiene el componente de propiedades
            JumpPadProperties padProps = collider.GetComponent<JumpPadProperties>();

            // 2. Si lo tiene, activamos el salto y leemos su fuerza personalizada
            if (padProps != null)
            {
                isOnMegaPad = true;
                currentPadForce = padProps.padForce; // ¡Aquí obtenemos el valor único de este objeto!
                break;
            }
        }

        // 3. Al presionar espacio, usamos la fuerza específica de esa plataforma
        if (isOnMegaPad && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Reseteamos la velocidad vertical vieja para que el salto siempre sea perfecto
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            // Aplicamos la fuerza de la plataforma actual
            rb.AddForce(Vector3.up * currentPadForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
        }
    }
}
