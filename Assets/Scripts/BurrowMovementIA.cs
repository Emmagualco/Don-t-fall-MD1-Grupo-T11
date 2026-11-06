using UnityEngine;
using UnityEngine.AI;
public class BurrowMovementIA : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private DialogueNPC dialogueNPC;
    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    public bool IsMoving => isMoving;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (dialogueNPC == null)
            dialogueNPC = GetComponent<DialogueNPC>();
    }

    public void MoveToNextPoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex < waypoints.Length)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            isMoving = true;
            currentWaypointIndex++;
        }
    }

    private void Update()
    {
        if (!isMoving) return;

        // Revisa si llegó al waypoint destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                isMoving = false;
                OnReachedWaypoint();
                
            }
        }
        

        if (animator != null)
        {
            if (isMoving == true) 
            {
                animator.SetBool("Walk", isMoving);

            }
            else
            {
                animator.SetBool("Walk", isMoving);
            }
        }
    }
    private void OnReachedWaypoint()
    {
        // Tomamos el waypoint actual al que acaba de llegar y le copiamos su rotación de golpe
        Transform targetWaypoint = waypoints[currentWaypointIndex - 1];
        transform.rotation = targetWaypoint.rotation;

        // Habilitamos nuevamente el diálogo en el script DialogueNPC
        if (dialogueNPC != null)
        {
            dialogueNPC.UnlockInteraction();
        }

        Debug.Log("Burrow llegó a la parada, rotó instantáneamente y habilitó el diálogo.");
    }
}


