using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPosition : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject mark;
    public Transform enemy;

    public Transform player;

    bool isMoving;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        isMoving = false;
    }

    void Update()
    {

        MoveToPoint();
        //ClickAttack();

        if(isMoving)
        {
            
        }
        else
        {
            //Stop();
        }

        Cursor.visible = true;
    }

    public void MoveToPoint()
    {
        //isMoving = true;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;       

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    agent.destination = hit.point;
                    mark.transform.position = hit.point;
                    print("clicked");
                }   
            }
        }
    }
}
