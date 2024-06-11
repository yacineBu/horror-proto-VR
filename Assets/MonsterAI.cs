using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    public Transform[] patrolPoints;  // Points de patrouille
    private int currentPatrolIndex;   // Index actuel du point de patrouille
    private NavMeshAgent agent;       // Référence au NavMeshAgent
    public Transform player;          // Référence au joueur
    public float chaseDistance = 3.0f; // Distance à partir de laquelle le monstre commence à poursuivre le joueur
    public float fieldOfView = 45.0f; // Champ de vision du monstre en degrés
    private bool end;

    public float jumpScareDistance = 2.0f; // Distance pour le jumpscare
    public GameObject jumpScareCamera; // Caméra pour le jumpscare
    public float shakeDuration = 1.0f; // Durée du tremblement
    public float shakeMagnitude = 0.2f; // Magnitude du tremblement

    void Start()
    {
        end = false;
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

        CheckForJumpScare();

        if (!end) {
            // Si le monstre est proche du joueur et peut le voir, commence à le poursuivre
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= chaseDistance && CanSeePlayer())
            {
                agent.destination = player.position;
            }
            else
            {
                // Si le monstre est arrivé à son point de patrouille, passe au suivant
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GotoNextPatrolPoint();
                }
            }
        }
    }

    void CheckForJumpScare()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= jumpScareDistance)
        {
            this.gameObject.GetComponent<Animator>().SetInteger("moving", 8);
            jumpScareCamera.SetActive(true);
            StartCoroutine(CameraShake());
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.destination = player.position ;
            end = true;
            StartCoroutine(RestartSceneAfterDelay(3f));
        }
        else
        {
            jumpScareCamera.SetActive(false);
            agent.isStopped = false;
        }
    }
    IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator CameraShake()
    {
        Vector3 originalPosition = jumpScareCamera.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude;

            jumpScareCamera.transform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        jumpScareCamera.transform.localPosition = originalPosition;
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // Vérifiez si le joueur est dans le champ de vision
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
