using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class EnemyPatrol : MonoBehaviour {

    public float wanderRadius;
    public float wanderTimer;
 
    private Transform target;
    private NavMeshAgent agent;
    private float timer;


    void Start() {
        agent = GetComponent < NavMeshAgent > ();
        GetComponent<Animator>().SetBool("Walking", false);
        timer = wanderTimer;
    }

  // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;
 
        if (timer >= wanderTimer) {
            timer=-3;
            GetComponent<Animator>().SetBool("Walking", false);
            StartCoroutine(Pause());
        }
    }
    IEnumerator Pause(){
        yield return new WaitForSeconds(Random.Range(1,3)*1.0f);
        GetComponent<Animator>().SetBool("Walking", true);
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
 
        randDirection += origin;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
        return navHit.position;
    }
}