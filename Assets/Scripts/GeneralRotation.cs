using UnityEngine;

/// <summary>
/// Makes a rotating platform carry the player around with it while they stand on it.
/// The player has a non-kinematic Rigidbody driven by PlayerController in FixedUpdate,
/// so this must move the player through the Rigidbody (MovePosition) on the same timing
/// instead of writing to Transform directly, otherwise the physics engine and this script
/// fight over the player's position every frame and the player feels "stuck".
/// </summary>
public class TwistPlattform : MonoBehaviour
{
    // Arrastra aquí al jugador desde la Jerarquía
    public Transform jugador;

    // Pon la misma velocidad que usas en tu script de rotación
    public float rotationvelocity = 50f;

    private bool uplayer = false;
    private Rigidbody jugadorRigidbody;

    private void Awake()
    {
        if (jugador != null)
        {
            jugadorRigidbody = jugador.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        // Si el jugador está parado encima, lo hacemos girar a la par, moviendo su Rigidbody
        // (no su Transform) para no pelear con la física del PlayerController.
        if (uplayer && jugadorRigidbody != null)
        {
            float deltaAngle = rotationvelocity * Time.fixedDeltaTime;
            Quaternion rotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
            Vector3 offsetFromPivot = jugadorRigidbody.position - transform.position;
            Vector3 rotatedPosition = transform.position + rotation * offsetFromPivot;
            jugadorRigidbody.MovePosition(rotatedPosition);
        }
    }

    // Detectamos cuando pisa la plataforma
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            uplayer = true;
        }
    }

    // Detectamos cuando salta o se baja
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            uplayer = false;
        }
    }
}