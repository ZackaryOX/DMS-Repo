using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChupaAI : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent; 
    float walkspeed = 0.0f;
    [FMODUnity.EventRef] public string[] _EventPath;
    private Animator animator;
    private bool Setup = false;

    private bool stepCol = false;

    bool _crawlerWalk = true;

    bool _2ndFloor = false; 

    public GameObject _Player;

    //public GameObject _activeAudio;

    FMOD.Studio.EventInstance _crawlerWSound;

    // Start is called before the first frame update
    void Start()
    {

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
        else if (Setup)
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
                stepCol = true;
            }
            else
            {
                if (stepCol == true)
                {
                   
                    stepCol = false;
                }
            }

            //Debug.Log(agent.velocity);
        }

        if (_crawlerWalk == true)
        {
            _crawlerWSound = FMODUnity.RuntimeManager.CreateInstance(_EventPath[0]);
            _crawlerWSound.start();

            _crawlerWalk = false;
        }
        _crawlerWSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player.AllPlayers[0].GetName() == other.name)
        {
            Player.AllPlayers[0].SetSanity(Player.AllPlayers[0].GetSanity() - 5);
        }
       
    }

}
