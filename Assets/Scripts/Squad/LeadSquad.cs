using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LeadSquad : MonoBehaviour
{
    public NavMeshAgent squadAgent;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 6;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask enemyMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float edgeDistance = 0.5f;

    public GameObject waypointMark;
    RaycastHit hit;

    public GameObject[] enemies;

    public Transform[] enemieLocations;
    int enemyIndex;

    public Transform[] waypoints;
    private int destPoint;

    Vector3 enemyLastPosition = Vector3.zero;
    Vector3 enemyPosition;

    float waitTime;
    float M_timeToRotate;
    bool enemyInRange;
    bool enemyNear;
    bool caughtEnemy;
    bool isFollowing;
    bool isPatrolling;
    bool isActivate;

    public float damage;
    public float enemyDamage;

    private Material originalMaterial;
    public Material highlightMaterial;

    public float coverDistance = 10f;

    // Start is called before the first frame update
    void Start()
    {
        enemyPosition = Vector3.zero;
        isFollowing = true;
        caughtEnemy = false;
        enemyInRange = false;
        enemyNear = false;
        isPatrolling = false;
        isActivate = true;

        waitTime = startWaitTime;
        M_timeToRotate = timeToRotate;

        enemyIndex = 0;

        squadAgent = GetComponent<NavMeshAgent>();
        squadAgent.isStopped = false;
        squadAgent.speed = speedWalk;
        squadAgent.SetDestination(hit.point);
        squadAgent.stoppingDistance = 0.5f;

        enemies[enemyIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        EnviromentView();
        ClickAttack();       
        Click();
        MoveToPoint();

        if (!isActivate)
        {                        
            if (!isPatrolling && enemies[enemyIndex].activeInHierarchy)
            {
                AttackEnemies();
            }
            else
            {
                Patrolling();
            }
        }
        else
        {
            if (!isFollowing && enemies[enemyIndex].activeInHierarchy)
            {
                AttackEnemies();                             
            }
            else
            {
                FollowPlayer();
            }
        }
    }

    // Move squad to waypoint
    public void MoveToPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    squadAgent.destination = hit.point;
                    waypointMark.transform.position = hit.point;
          
                    print("clicked");
                }
            }
        }
    }

    //Click enemy to attack
    public void ClickAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if(originalMaterial == null)
                    {
                        originalMaterial = hit.collider.GetComponent<Renderer>().material;
                    }

                    hit.collider.GetComponent<Renderer>().material = highlightMaterial;

                    print("clicked enemy");
                    AttackEnemies();
                    LookingEnemy(enemyLastPosition);
                }
            }
        }
    }

    //Click to change from following to patrolling
    public void Click()
    {
        if (Input.GetMouseButtonDown(1) && isActivate)
        {
            isActivate = false;
        }
        else if (Input.GetMouseButtonDown(1) && !isActivate)
        {
            isActivate = true;
        }
    }

    public void Patrolling()
    {
        if (enemyNear)
        {
            if (M_timeToRotate <= 0)
            {
                Move(speedWalk);
                LookingEnemy(enemyLastPosition);
            }
            else
            {
                Stop();
                M_timeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            enemyNear = false;
            enemyLastPosition = Vector3.zero;
            squadAgent.destination = waypoints[destPoint].position;
            NextWaypoint();
            Move(speedWalk);
        }
    }

    public void FollowPlayer()
    {
        squadAgent.SetDestination(hit.point);
        squadAgent.stoppingDistance = 0.5f;

        if (enemyNear)
        {
            if (M_timeToRotate <= 0)
            {
                Move(speedWalk);
                LookingEnemy(enemyLastPosition);
            }
            else
            {
                Stop();
                M_timeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            enemyNear = false;
            enemyLastPosition = Vector3.zero;
            squadAgent.SetDestination(hit.point);
            squadAgent.stoppingDistance = 0.5f;
        }
    }

    private void AttackEnemies()
    {
        enemyNear = false;
        enemyLastPosition = Vector3.zero;

        if (!caughtEnemy)
        {
            Move(speedRun);
            squadAgent.SetDestination(enemieLocations[enemyIndex].position);
            squadAgent.stoppingDistance = 0.5f;
        }
        if (squadAgent.remainingDistance <= squadAgent.stoppingDistance)
        {
            if (waitTime <= 0 && !caughtEnemy && Vector3.Distance(transform.position, enemies[enemyIndex].transform.position) >= 6f)
            {
                isFollowing = true;
                isPatrolling = true;
                enemyNear = false;
                Move(speedWalk);
                M_timeToRotate = timeToRotate;
                waitTime = startWaitTime;
                squadAgent.SetDestination(hit.point);
            }
        }
    }

    public void NextEnemy()
    {
        enemyIndex = (enemyIndex + 1) % enemieLocations.Length;
        squadAgent.SetDestination(enemieLocations[enemyIndex].position);
    }

    public void NextWaypoint()
    {
        if (!squadAgent.pathPending && squadAgent.remainingDistance < 0.5f)
        {
            destPoint = (destPoint + 1) % waypoints.Length;
            squadAgent.destination = waypoints[destPoint].position;
        }

    }

    void Move(float speed)
    {
        squadAgent.isStopped = false;
        squadAgent.speed = speed;
    }

    void Stop()
    {
        squadAgent.isStopped = true;
        squadAgent.speed = 0;
    }

    void LookingEnemy(Vector3 enemy)
    {
        squadAgent.SetDestination(enemy);
        if (Vector3.Distance(transform.position, enemy) <= 0.3)
        {
            if (waitTime <= 0)
            {
                enemyNear = false;
                Move(speedWalk);
                squadAgent.SetDestination(hit.point);
                waitTime = startWaitTime;
                M_timeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] _enemyInRange = Physics.OverlapSphere(transform.position, viewRadius, enemyMask);

        for (int i = 0; i < _enemyInRange.Length; i++)
        {
            Transform enemy = _enemyInRange[i].transform;
            Vector3 dirToEnemy = (enemy.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToEnemy) < viewAngle / 2)
            {
                float dsToEnemy = Vector3.Distance(transform.position, enemy.position);
                if (!Physics.Raycast(transform.position, dirToEnemy, dsToEnemy, obstacleMask))
                {
                    enemyInRange = true;
                    isFollowing = false;
                    isPatrolling = false;
                }
                else
                {
                    enemyInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, enemy.position) > viewRadius)
            {
                enemyInRange = false;
            }
            if (enemyInRange)
            {
                enemyPosition = enemy.transform.position;
            }
        }
    }

    public void EnemyDead()
    {
        enemies[enemyIndex].SetActive(false);
        NextEnemy();
        isFollowing = true;
        isPatrolling = true;
    }

    void TakeDamage()
    {
        HealthController.currentHealth -= damage;
        //print(HealthController.currentHealth);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            if (enemyInRange)
            {
                TakeDamage();
                EnemyDead();
            }
        }
    }
}
