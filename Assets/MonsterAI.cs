using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;       // R�f�rence au NavMeshAgent
    public Transform player;          // R�f�rence au joueur
    public float chaseDistance = 5.0f; // Distance � partir de laquelle le monstre commence � poursuivre le joueur
    public float fieldOfView = 45.0f; // Champ de vision du monstre en degr�s
    public float maxHeightDifference = 4.0f; // Diff�rence de hauteur maximale pour voir le joueur
    private bool end;

    public float jumpScareDistance = 2.0f; // Distance pour le jumpscare
    public GameObject jumpScareCamera; // Cam�ra pour le jumpscare
    public float shakeDuration = 1.0f; // Dur�e du tremblement
    public float shakeMagnitude = 0.2f; // Magnitude du tremblement
    public float wanderRadius = 10.0f; // Rayon pour les d�placements al�atoires
    public float wanderInterval = 5.0f; // Intervalle entre les changements de destination al�atoires

    private float nextWanderTime;

    void Start()
    {
        end = false;
        agent = GetComponent<NavMeshAgent>();
        nextWanderTime = Time.time + wanderInterval;
        SetRandomDestination();
    }

    void Update()
    {
        if (this.gameObject.GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            this.gameObject.GetComponent<Animator>().SetInteger("moving", 1);
        }

        CheckForJumpScare();

        if (!end)
        {
            if (CanSeePlayer())
            {
                agent.destination = player.position;
            }
            else
            {
                if (Time.time >= nextWanderTime && !agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    SetRandomDestination();
                    nextWanderTime = Time.time + wanderInterval;
                }
            }
        }
    }

    void CheckForJumpScare()
    {
        float distanceToPlayer = GetPathDistance(transform.position, player.position);
        if (distanceToPlayer <= jumpScareDistance)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.transform == player)
                {
                    this.gameObject.GetComponent<Animator>().SetInteger("moving", 8);
                    jumpScareCamera.SetActive(true);
                    StartCoroutine(CameraShake());
                    agent.ResetPath();
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true;
                    agent.destination = player.position;
                    end = true;
                    StartCoroutine(RestartSceneAfterDelay(3f));
                }
            }
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

        if (angle < fieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.transform == player)
                {
                    float navMeshDistance = GetPathDistance(transform.position, player.position);
                    if (navMeshDistance <= chaseDistance)
                    {
                        return true;
                    }
                }
            }
        }
        float distanceToPlayer = GetPathDistance(transform.position, player.position);
        if (distanceToPlayer < 3f) return true;
        return false;
    }

    float GetPathDistance(Vector3 startPoint, Vector3 endPoint)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path))
        {
            float distance = 0.0f;
            if (path.corners.Length > 1)
            {
                for (int i = 1; i < path.corners.Length; i++)
                {
                    distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
            }
            return distance;
        }
        return float.MaxValue;
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            // V�rifiez que la nouvelle position est au m�me niveau ou �tage
            if (Mathf.Abs(hit.position.y - transform.position.y) <= maxHeightDifference)
            {
                agent.destination = hit.position;
            }
        }
    }
}
