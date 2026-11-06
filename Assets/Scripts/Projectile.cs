
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetimeSeconds = 3f;

    private bool hasImpacted;

    private void Start()
    {
        Destroy(gameObject, lifetimeSeconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    private void HandleImpact(GameObject hitObject)
    {
        if (hasImpacted)
            return;

        hasImpacted = true;

        IProjectileHittable target =
            hitObject.GetComponentInParent<IProjectileHittable>();

        if (target != null)
        {
            target.OnProjectileHit(gameObject);
        }

        Destroy(gameObject);
    }
}

