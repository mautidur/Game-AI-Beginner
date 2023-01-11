using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GuardController : MonoBehaviour
{
    //FSM
    enum State { Patrol, Investigate, Chase };
    State curState = State.Patrol;
    
    //info player
    public Transform player;
    
    //fov setting
    float fovDist = 20.0f;
    float fovAngle = 45.0f;

    //chasing setting
    public float chasingSpeed = 20.0f;
    public float chasingRotSpeed = 2.0f;
    public float chasingAccuracy = 5.0f;

    //patrol setting
    public float patrolDistance = 10.0f;
    float patrolWait = 5.0f;
    float patrolTimePassed = 0;

    //investigate setting
    Vector3 lastPlaceSeen;

    void Start()
    {
    patrolTimePassed = patrolWait;
    lastPlaceSeen = this.transform.position;
    }

    void Update()
    {
        State tmpstate = curState;
        //fov logic
        if (ICanSee(player))
        {
            curState = State.Chase;
            lastPlaceSeen = player.position;
            Debug.Log("liat player di "+ player.position);
        }
        else
        {
            if(curState == State.Chase)
            {
                curState = State.Investigate;
            }
            //Debug.Log("nggak ngeliat player");
        }
        //state check
        switch (curState)
        {
            case State.Patrol:
                Patrol();
                GetComponent<Renderer>().material.color = Color.cyan;
                break;
            case State.Investigate:
                Investigate();
                GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case State.Chase:
                Chase(player);
                GetComponent<Renderer>().material.color = Color.red;
                break;
        }

        if (tmpstate != curState)
            Debug.Log("guard's state: "+ curState);
    }

        bool ICanSee(Transform player)
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        RaycastHit hit;
        if (
            Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.gameObject.tag == "Player" && direction.magnitude < fovDist && angle < fovAngle
            )
            {
                return true;
            }
        return false;
    }

    void Chase(Transform player)
    {
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().Stop();
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();

        Vector3 direction = player.position - this.transform.position;
       this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * this.chasingRotSpeed);

        if (direction.magnitude > this.chasingAccuracy)
        {
            this.transform.Translate(0,0, Time.deltaTime * this.chasingSpeed);
        }
    }

    public void InvestigatePoint(Vector3 point)
    {
        lastPlaceSeen = point;
        curState = State.Investigate;
    }

    void Investigate()
    {
        if (transform.position == lastPlaceSeen)
        {
            curState = State.Patrol;
        }
        else
        {
            this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(lastPlaceSeen);
            Debug.Log("Guard's state: " + curState + " di " + lastPlaceSeen);
        }
    }
       void Patrol()
    {
        patrolTimePassed += Time.deltaTime;

        if (patrolTimePassed > patrolWait)
        {
            patrolTimePassed = 0;
            Vector3 patrollingPoint = lastPlaceSeen;

            patrollingPoint += new Vector3(Random.Range(-patrolDistance, patrolDistance), 0, Random.Range(-patrolDistance, patrolDistance));

            this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(patrollingPoint);
            Debug.Log("Guard's state: patroli di " + patrollingPoint);
        }
    }
}
