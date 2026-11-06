using UnityEngine;

public class WeaponSpin : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeedDegreesPerSecond = 90f;

    [SerializeField]
    private float bobHeight = 0.15f;

    [SerializeField]
    private float bobSpeed = 2f;

    private Vector3 startLocalPosition;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeedDegreesPerSecond * Time.deltaTime, Space.World);

        float verticalOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + new Vector3(0f, verticalOffset, 0f);
    }
}
