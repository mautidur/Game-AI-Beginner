using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public float knockRadius = 20.0f;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                this.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);
            }
        }
        if (Input.GetKey("space"))
        {
            StartCoroutine(PlayKnock());
            GetComponent<Renderer>().material.color = Color.magenta;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, knockRadius);
            for (int i =0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag == "guard")
                {
                    hitColliders[i].GetComponent<GuardController>().InvestigatePoint(this.transform.position);
                }
            }        
        }
        else{
            GetComponent<Renderer>().material.color = Color.grey;
        }    
    }

    IEnumerator PlayKnock()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
    }
}
