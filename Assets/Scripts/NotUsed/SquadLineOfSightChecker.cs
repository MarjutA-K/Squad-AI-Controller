using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquadLineOfSightChecker : MonoBehaviour
{
    public NavMeshAgent squad;
    public int frameInterval = 3;
    public int faceEnemyFactor = 50;

    Vector3 randomPosition;
    Vector3 coverPoint;
    public float rangeRandPoint = 6f;
    public bool isHiding = false;

    public LayerMask coverLayer;
    Vector3 coverObj;
    public LayerMask visibleLayer;

    private float maxCovDist = 30;
    public bool coverIsClose;
    public bool coverNotReached = true;

    public float distToCoverPos = 1f;
    public float distToCoverObj = 20f;

    public float rangeDist = 15f;
    public bool enemyInRange = false;

    private int testCoverPos = 10;

    private bool isCovering;

    public Transform enemy;


    bool RandomPoint(Vector3 center, float rangeRandPoint, out Vector3 resultCover)
    {
        for (int i = 0; i < testCoverPos; i++)
        {
            randomPosition = center + Random.insideUnitSphere * rangeRandPoint;
            Vector3 direction = GameObject.FindGameObjectWithTag("Enemy").transform.position - randomPosition;
            RaycastHit hitTestCov;
            if (Physics.Raycast(randomPosition, direction.normalized, out hitTestCov, rangeRandPoint, visibleLayer))
            {
                if (hitTestCov.collider.gameObject.layer == 6)
                {
                    resultCover = randomPosition;
                    return true;
                }
            }
        }

        resultCover = Vector3.zero;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        squad = GetComponent<NavMeshAgent>();
     
        isCovering = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCovering)
        {
            if (squad.isActiveAndEnabled)
            {
                if (Time.frameCount % frameInterval == 0)
                {
                    float distance = ((GameObject.FindGameObjectWithTag("Enemy").transform.position - transform.position).sqrMagnitude);

                    if (distance < rangeDist * rangeDist)
                    {
                        enemyInRange = true;
                    }
                    else
                    {
                        enemyInRange = false;
                    }
                }

                if (enemyInRange)
                {
                    CheckCoverDist();

                    if (coverIsClose)
                    {
                        if (coverNotReached)
                        {
                            squad.SetDestination(coverObj);
                        }

                        if (!coverNotReached)
                        {
                            TakeCover();

                            FacePlayer();
                        }
                    }

                    if (!coverIsClose)
                    {
                        //
                    }
                }
            }
        }      
    }

    public void Cover()
    {
        isCovering = true;
    }

    public void CheckCoverDist()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxCovDist, coverLayer);
        Collider nearestCollider = null;
        float minSqrDistance = Mathf.Infinity;

        Vector3 AI_position = transform.position;

        for(int i = 0; i < colliders.Length; i++)
        {
            float sqrDistanceToCenter = (AI_position - colliders[i].transform.position).sqrMagnitude;
            if(sqrDistanceToCenter < minSqrDistance)
            {
                minSqrDistance = sqrDistanceToCenter;
                nearestCollider = colliders[i];

                float coverDistance = (nearestCollider.transform.position - AI_position).sqrMagnitude;

                if(coverDistance <= maxCovDist*maxCovDist)
                {
                    coverIsClose = true;
                    coverObj = nearestCollider.transform.position;
                    if(coverDistance <= distToCoverObj * distToCoverObj)
                    {
                        coverNotReached = false;
                    }
                    else if(coverDistance > distToCoverObj*distToCoverObj)
                    {
                        coverNotReached = true;
                    }
                }
                if(coverDistance >= maxCovDist*maxCovDist)
                {
                    coverIsClose = false;
                }
            }
        }
        if(colliders.Length < 1)
        {
            coverIsClose = false;
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (GameObject.FindGameObjectWithTag("Enemy").transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * faceEnemyFactor);
    }

    void TakeCover()
    {
        if(RandomPoint(transform.position, rangeRandPoint, out coverPoint))
        {
            if(squad.isActiveAndEnabled)
            {
                squad.SetDestination(coverPoint);
                if((coverPoint - transform.position).sqrMagnitude <= distToCoverPos*distToCoverPos)
                {
                    isHiding = true;
                }
            }
        }
    }
}
