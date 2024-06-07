using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public Transform[] patrolPoints;  // Points de patrouille
    private int currentPatrolIndex;   // Index actuel du point de patrouille
    private NavMeshAgent agent;       // R�f�rence au NavMeshAgent
    public Transform player;          // R�f�rence au joueur
    public float chaseDistance = 10.0f; // Distance � partir de laquelle le monstre commence � poursuivre le joueur
    public float fieldOfView = 45.0f; // Champ de vision du monstre en degr�s
    public float maxHeightDifference = 4.0f; // Diff�rence de hauteur maximale pour voir le joueur

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolIndex = 0;
        GotoNextPatrolPoint();
    }

    void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Update()
    {
        if (this.gameObject.GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            this.gameObject.GetComponent<Animator>().SetInteger("moving", 1);
        }
        // Si le monstre est proche du joueur et peut le voir, commence � le poursuivre
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= chaseDistance && CanSeePlayer())
        {
            agent.destination = player.position;
        }
        else
        {
            // Si le monstre est arriv� � son point de patrouille, passe au suivant
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNextPatrolPoint();
            }
        }
    }

    bool CanSeePlayer()
    {
        // V�rifiez la diff�rence de hauteur
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);
        if (heightDifference > maxHeightDifference)
        {
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // V�rifiez si le joueur est dans le champ de vision
        if (angle < fieldOfView / 2)
        {
            RaycastHit hit;
            // Effectuer un raycast vers le joueur
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, chaseDistance))
            {
                return true;
            }
        }
        return false;
    }
}
