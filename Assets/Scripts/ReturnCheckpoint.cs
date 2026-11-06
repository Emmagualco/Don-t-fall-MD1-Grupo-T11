using UnityEngine;

public class ReturnCheckpoint : MonoBehaviour, IKillZone
{
    [Header("Referencias")]
    [SerializeField] private Transform Player;

    private void Awake()
    {
        if (Player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                Player = playerObject.transform;
            }
            else
            {
                Debug.LogError(
                    "ReturnCheckpoint: No se encontró un objeto con la etiqueta Player.",
                    this
                );
            }
        }
    }

    public void teleportCheckPoint(Transform CheckPoint)
    {
        Debug.Log("Volviendo al checkpoint");

        if (Player == null)
        {
            Debug.LogError("ReturnCheckpoint: Player es null.", this);
            return;
        }

        if (CheckPoint == null)
        {
            Debug.LogError("ReturnCheckpoint: El checkpoint recibido es null.", this);
            return;
        }

        Rigidbody rb = Player.GetComponent<Rigidbody>();

        // Detiene el movimiento antes del teletransporte.
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teletransporta al jugador.
        Player.position = CheckPoint.position;

        // Restablece la velocidad después del teletransporte.
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"Jugador teletransportado a: {CheckPoint.name}", this);
    }
}
