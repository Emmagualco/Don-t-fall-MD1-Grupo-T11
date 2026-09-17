using UnityEngine;

/// <summary>
/// Proyectil simple instanciado por ProjectileLauncher: viaja en línea recta
/// gracias a su Rigidbody y se autodestruye tras un tiempo de vida o al
/// colisionar con otro objeto. Demuestra Instantiate/Destroy en tiempo de
/// ejecución (Clase 7).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Tooltip("Segundos que el proyectil vive antes de autodestruirse si no impacta nada.")]
    [SerializeField] private float lifetimeSeconds = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetimeSeconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
