using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
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

    public GameObject[] enemies;

    public Transform[] enemieLocations;
    int enemyIndex;

    public Transform agentFollowPoint;

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
        squadAgent.SetDestination(agentFollowPoint.position);
        squadAgent.stoppingDistance = 0.5f;

        enemies[enemyIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        EnviromentView();
        //ClickAttack();
        Click();

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
                Follow();
                squadAgent.SetDestination(agentFollowPoint.position);
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


    public void Follow()
    {
        squadAgent.SetDestination(agentFollowPoint.position);
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
            squadAgent.SetDestination(agentFollowPoint.position);
            squadAgent.stoppingDistance = 0.5f;

            /*if (squadAgent.remainingDistance <= squadAgent.stoppingDistance)
            {
                   squadAgent.SetDestination(agentFollowPoint.position);
                   Move(speedWalk);
                   waitTime = startWaitTime;        
            }*/
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
                squadAgent.SetDestination(agentFollowPoint.position);
            }         
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

    void LookingEnemy(Vector3 enemy)
    {
        squadAgent.SetDestination(enemy);
        if (Vector3.Distance(transform.position, enemy) <= 0.3)
        {
            if (waitTime <= 0)
            {
                enemyNear = false;
                Move(speedWalk);
                squadAgent.SetDestination(agentFollowPoint.position);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemyInRange)
            {
                EnemyDead();
            }
        }
    }
}
