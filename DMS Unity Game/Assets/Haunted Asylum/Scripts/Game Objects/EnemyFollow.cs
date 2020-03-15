using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = Player.AllPlayers[0].GetObject().transform;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude < 0.5f)
            animator.SetBool("IsWalking", false);
        else if (agent.velocity.magnitude < 5)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsRunning", true);
        }

        agent.SetDestination(target.position);

        float distance = Vector3.Distance(target.position, transform.position);
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }
}
