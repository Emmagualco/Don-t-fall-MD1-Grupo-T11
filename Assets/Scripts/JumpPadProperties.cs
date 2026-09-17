using UnityEngine;

public class JumpPadProperties : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("How much force this specific pad gives to the player")]
    public float padForce = 45f; // Cada plataforma puede tener un número diferente aquí
}
