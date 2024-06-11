using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10f;
    public float chaseRadius = 5f;
    public float randomPointRange = 20f;
    public float wanderTimer = 5f;

    public float jumpScareDistance = 2.0f; // Distance pour le jumpscare
    public GameObject jumpScareCamera; // Caméra pour le jumpscare
    public float shakeDuration = 1.0f; // Durée du tremblement
    public float shakeMagnitude = 0.2f; // Magnitude du tremblement
    public float chaseDistance = 5.0f; // Distance à partir de laquelle le monstre commence à poursuivre le joueur
    private bool end;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        end = false;
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        if (this.gameObject.GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            this.gameObject.GetComponent<Animator>().SetInteger("moving", 1);
        }

        if (!end)
        {
            timer += Time.deltaTime;

            // Check distance to the player
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (distanceToPlayer <= jumpScareDistance && CanSeePlayer())
            {
                CheckForJumpScare();
            }
            else if (distanceToPlayer <= chaseRadius || CanSeePlayer())
            {
                // Chase the player
                agent.SetDestination(player.position);
            }
            else
            {
                // Wander around
                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, randomPointRange, -1);
                    agent.SetDestination(newPos);
                    timer = 0;
                }
            }
        }
    }

    // Function to check if the monster can see the player
    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < 45f / 2)
        {
            return true;
            /*
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
            */
        }
        return false;
    }

    // Function to get a random point on the NavMesh
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    void CheckForJumpScare()
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
}


/*
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;       // Référence au NavMeshAgent
    public Transform player;          // Référence au joueur
    public float chaseDistance = 5.0f; // Distance à partir de laquelle le monstre commence à poursuivre le joueur
    public float fieldOfView = 45.0f; // Champ de vision du monstre en degrés
    public float maxHeightDifference = 4.0f; // Différence de hauteur maximale pour voir le joueur
    private bool end;

    public float jumpScareDistance = 2.0f; // Distance pour le jumpscare
    public GameObject jumpScareCamera; // Caméra pour le jumpscare
    public float shakeDuration = 1.0f; // Durée du tremblement
    public float shakeMagnitude = 0.2f; // Magnitude du tremblement
    public float wanderRadius = 10.0f; // Rayon pour les déplacements aléatoires
    public float wanderInterval = 5.0f; // Intervalle entre les changements de destination aléatoires

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
            // Vérifiez que la nouvelle position est au même niveau ou étage
            if (Mathf.Abs(hit.position.y - transform.position.y) <= maxHeightDifference)
            {
                agent.destination = hit.position;
            }
        }
    }
}
*/