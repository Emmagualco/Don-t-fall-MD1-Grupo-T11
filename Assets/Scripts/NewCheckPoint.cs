using UnityEngine;

public class NewCheckPoint : MonoBehaviour, ICheckPoint
{
    
    public Transform newCheckPoint()
    {
        Debug.Log("newCheckPoint");
        return transform;
    }
}
