using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    float walkspeed = 0.0f;
    [FMODUnity.EventRef] public string[] _EventPath;
    private Animator animator;
    private bool Setup = false;
    private bool Attacking = false;
    public int Damage = 10;
    private bool stepCol = false;
    private float Counting = 0.0f;
    public bool IsAI = false;

    public GameObject _Player;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("playFootsteps", 0, 0.7f);

    }

    // Update is called once per frame
    void Update()
    {
        if (!Setup)
        {
            if (Player.AllPlayers.Count > 0)
            {
                agent = GetComponent<NavMeshAgent>();
                target = Player.AllPlayers[0].GetObject().transform;
                animator = this.GetComponent<Animator>();
                Setup = true;
            }
        }
        else if (Setup && !Attacking)
        {
            if (agent.velocity.magnitude < 0.5f)
                animator.SetBool("IsWalking", false);
            else if (agent.velocity.magnitude < 5)
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", true);
                walkspeed = 1.0f;
            }
            else
            {
                animator.SetBool("IsRunning", true);
                walkspeed = 1.5f;
            }

            agent.SetDestination(target.position);

            float distance = Vector3.Distance(target.position, transform.position);
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            if (agent.velocity.x == 0 && agent.velocity.y == 0)
            {
                CancelInvoke();
                stepCol = true;
            }
            else
            {
                if (stepCol == true)
                {
                    InvokeRepeating("playFootsteps", 0, 0.7f);
                    stepCol = false;
                }
            }

            Player Target = Player.AllPlayers[0];
            Transform Targetstrans = Target.GetObject().transform;
            Transform Casterstrans = gameObject.transform;
            float Distance = Vector3.Distance(Casterstrans.position, Targetstrans.position);

            if (Distance <= 4)
            {
                Player.AllPlayers[0].SetHealth(Player.AllPlayers[0].GetHealth() - Damage);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", true);
                agent.isStopped = true;
                Attacking = true;
            }
        }
        else if (Attacking)
        {
            Counting += Time.deltaTime;
            if (Counting >= 1.2f)
            {

                Attacking = false;
                agent.isStopped = false;
                Counting = 0;
                animator.SetBool("IsAttacking", false);
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == _Player.name)
        {
            //Debug.Log("Collision");
            StartCoroutine(playerCollision());
        }
    }


    void playFootsteps()
    {
        FMODUnity.RuntimeManager.PlayOneShot(_EventPath[0], GetComponent<Transform>().position);

    }

    IEnumerator playerCollision()
    {
        FMODUnity.RuntimeManager.PlayOneShot(_EventPath[1], GetComponent<Transform>().position);
        yield return new WaitForSeconds(2);
    }
}
