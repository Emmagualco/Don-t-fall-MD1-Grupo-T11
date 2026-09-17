using System;
using UnityEngine;

public class ReturnCheckpoint : MonoBehaviour, IKillZone
{
    [SerializeField] private Transform Player;
    
    public void teleportCheckPoint(Transform CheckPoint)
    {
        Debug.Log("Volviendo al checkpoint");
        Player.transform.position = CheckPoint.position;

    }
}
