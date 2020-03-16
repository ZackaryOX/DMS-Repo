using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    private bool StartUp = false;
    private bool NameAssigned = false;
    private bool Updating = false;
    public float WaitTime = 0.2f;


    public GameObject PlayerContainer;
    public GameObject PlayerModel;
    public GameObject PlayerCam;

    public GameObject GhostContainer;
    public GameObject GhostModel;
    public GameObject GhostCam;

    Vector3 PrevPos = new Vector3(0, 0, 0);
    Vector3 PrevRot = new Vector3(0, 0, 0);
    NetworkedPlayer ClientToReplicate;

    GameObject Avatar;
    // Start is called before the first frame update
    NetworkWrapper ThisWrapper;
    void Start()
    {
        ThisWrapper = gameObject.GetComponent<NetworkWrapper>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            WaitTime -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            WaitTime += 0.1f;
        }
        if (ThisWrapper.ClientRunning && !NameAssigned)
        {
            if (ThisWrapper.Name == "PLAYER")
            {
                //Current client
                Avatar = PlayerModel;
                PlayerContainer.SetActive(true);
                Avatar.SetActive(true);
                PlayerCam.SetActive(true);

                //Other client
                GhostContainer.SetActive(true);
                GhostModel.SetActive(true);
                GhostCam.SetActive(false);
                GhostModel.AddComponent<NetworkedPlayer>();
                ClientToReplicate = GhostModel.GetComponent<NetworkedPlayer>();
                GhostModel.GetComponent<GhostCharacter>().IsClone = true;


                NameAssigned = true;
            }
            else if (ThisWrapper.Name == "GHOST")
            {
                //Current client
                Avatar = GhostModel;
                GhostContainer.SetActive(true);
                Avatar.SetActive(true);
                GhostCam.SetActive(true);


                //Other client
                PlayerContainer.SetActive(true);
                PlayerModel.SetActive(true);
                PlayerCam.SetActive(false);
                PlayerModel.AddComponent<NetworkedPlayer>();
                ClientToReplicate = PlayerModel.GetComponent<NetworkedPlayer>();
                PlayerModel.GetComponent<Character>().IsClone = true;

                NameAssigned = true;
            }
        }


        if (!StartUp && NameAssigned)
        {
            Updating = true;
            StartCoroutine("WaitAndUpdate");

            StartUp = true;
        }
        HandleMessage();
    }

    IEnumerator WaitAndUpdate()
    {
        while (Updating)
        {
            if (ThisWrapper.Name == "PLAYER")
            {
                UpdateNetPlayer();
            }
            else if (ThisWrapper.Name == "GHOST")
            {
                UpdateNetGhost();
            }
            yield return new WaitForSeconds(WaitTime);
        }
    }

    void HandleMessage()
    {
        string Message = ThisWrapper.GetInboundMessage();
        StringReader strRdr = new StringReader(Message);

        string Header = strRdr.ReadLine();

        if (Header == "NULL")
        {
        }
        else if (Header == "CS")
        {
            string Type = strRdr.ReadLine();
            //Debug.Log("Type: " + Type);
            if (Type == "UPDP")
            {
                float xpos = float.Parse(strRdr.ReadLine());
                float ypos = float.Parse(strRdr.ReadLine());
                float zpos = float.Parse(strRdr.ReadLine());
                float xrot = float.Parse(strRdr.ReadLine());
                float yrot = float.Parse(strRdr.ReadLine());
                float zrot = float.Parse(strRdr.ReadLine());

                int walking = int.Parse(strRdr.ReadLine());
                int running = int.Parse(strRdr.ReadLine());

                Vector3 Pos = new Vector3(xpos, ypos, zpos);
                Vector3 Rot = new Vector3(xrot, yrot, zrot);
                Vector3 Anims = new Vector3(walking, running, 0);

                ClientToReplicate.AddUpdate(Pos, Rot, Anims);
            }
            else if (Type == "UPDG")
            {
                float xpos = float.Parse(strRdr.ReadLine());
                float ypos = float.Parse(strRdr.ReadLine());
                float zpos = float.Parse(strRdr.ReadLine());
                float xrot = float.Parse(strRdr.ReadLine());
                float yrot = float.Parse(strRdr.ReadLine());
                float zrot = float.Parse(strRdr.ReadLine());

                int walking = int.Parse(strRdr.ReadLine());
                int running = int.Parse(strRdr.ReadLine());

                Vector3 Pos = new Vector3(xpos, ypos, zpos);
                Vector3 Rot = new Vector3(xrot, yrot, zrot);
                Vector3 Anims = new Vector3(walking, running, 0);

                ClientToReplicate.AddUpdate(Pos, Rot, Anims);
            }
            else if (Type == "UPDRAWER")
            {
                int NetID = int.Parse(strRdr.ReadLine());
                string Index = "Drawer" + NetID;
                GameObject.Find(Index).GetComponent<ForDrawer>().DrawerInteract();
            }
            else if (Type == "CS")
            {
                Debug.Log("CS ACTIVATED");
                string line;
                while ((line = strRdr.ReadLine()) != null)
                {
                    Debug.Log(line);
                }
            }
            ThisWrapper.RemoveInboundMessage();
        }

    }
    void UpdateNetPlayer()
    {
        Vector3 NewPos = PlayerModel.transform.position;
        Vector3 NewRot = PlayerModel.transform.eulerAngles;

        Animator Temp = PlayerModel.GetComponent<Animator>();
        int walking = Temp.GetBool("IsWalking") ? 1 : 0;
        int running = Temp.GetBool("IsRunning") ? 1 : 0;

        if (NewPos != PrevPos || NewRot != PrevRot)
        {
            PrevPos = NewPos;
            PrevRot = NewRot;
            string Msg = "UPDP";
            Msg += "\n";
            Msg += NewPos.x;
            Msg += "\n";
            Msg += NewPos.y;
            Msg += "\n";
            Msg += NewPos.z;
            Msg += "\n";
            Msg += NewRot.x;
            Msg += "\n";
            Msg += NewRot.y;
            Msg += "\n";
            Msg += NewRot.z;
            Msg += "\n";
            Msg += walking.ToString();
            Msg += "\n";
            Msg += running.ToString();
            ThisWrapper.SendServerMessage(Msg);
        }
    }

    void UpdateNetGhost()
    {
        Vector3 NewPos = GhostModel.transform.position;
        Vector3 NewRot = GhostModel.transform.eulerAngles;

        Animator Temp = GhostModel.GetComponent<Animator>();
        int walking = Temp.GetBool("IsWalking") ? 1 : 0;
        int running = Temp.GetBool("IsRunning") ? 1 : 0;

        if (NewPos != PrevPos || NewRot != PrevRot)
        {
            PrevPos = NewPos;
            PrevRot = NewRot;
            string Msg = "UPDG";
            Msg += "\n";
            Msg += NewPos.x;
            Msg += "\n";
            Msg += NewPos.y;
            Msg += "\n";
            Msg += NewPos.z;
            Msg += "\n";
            Msg += NewRot.x;
            Msg += "\n";
            Msg += NewRot.y;
            Msg += "\n";
            Msg += NewRot.z;
            Msg += "\n";
            Msg += walking.ToString();
            Msg += "\n";
            Msg += running.ToString();
            ThisWrapper.SendServerMessage(Msg);
        }
    }
}
