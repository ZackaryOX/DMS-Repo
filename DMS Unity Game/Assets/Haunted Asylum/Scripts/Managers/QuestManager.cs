using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    //Public
    public Image background;
    public Text text;

    private List<Quest> Quests = new List<Quest>() { };
    private List<Image> Images = new List<Image>() { };
    private List<Text> Texts = new List<Text>() { };

    //Quest objectives
    public GameObject waypoint1;
    public GameObject waypoint2;
    public GameObject waypoint3;
    public GameObject waypoint4;
    public GameObject flashlightobj;
    public GameObject keyobj;
    public GameObject keycardobj;
    public GameObject waypoint5;
    public GameObject waypoint6;
    private PickUp flashlight;
    private PickUp key;
    private PickUp keycard;

    //Quests
    private WaypointQuest q1;
    private WaypointQuest q2;
    private PickUpQuest q3;
    private WaypointQuest q4;
    private PickUpQuest q5;
    private WaypointQuest q6;
    private UseQuest q7;
    private PickUpQuest q8;
    private UseQuest q9;
    private WaypointQuest q10;
    private WaypointQuest q11;

    // Start is called before the first frame update
    void Start()
    {
        flashlight = PickUp.AllItems[flashlightobj.name];
        key = PickUp.AllItems[keyobj.name];
        keycard = PickUp.AllItems[keycardobj.name];

        q1 = new WaypointQuest("— Leave cell —", waypoint1);
        q2 = new WaypointQuest("— Walk to drawer —", waypoint2);
        q3 = new PickUpQuest("— Open drawer and pickup flashlight —", flashlight);
        q4 = new WaypointQuest("— Walk to key —", waypoint3);
        q5 = new PickUpQuest("— Pickup key —", key);
        q6 = new WaypointQuest("— Walk to door —", waypoint4);
        q7 = new UseQuest("— Use key and open the door —", keyobj);
        q8 = new PickUpQuest("— Find and pickup keycard —", keycard);
        q9 = new UseQuest("— Use keycard to open a door —", keycardobj);
        q10 = new WaypointQuest("— Find and activate fusebox —", waypoint5);
        q11 = new WaypointQuest("— Escape —", waypoint6);



        Quests.Add(q1);
        Quests.Add(q2);
        Quests.Add(q3);
        Quests.Add(q4);
        Quests.Add(q5);
        Quests.Add(q6);
        Quests.Add(q7);
        Quests.Add(q8);
        Quests.Add(q9);
        Quests.Add(q10);
        Quests.Add(q11);
        Quests[0].Activate();

        //Create UI
        for (int i = 0; i < Quests.Count; i++)
        {
            Images.Add(Instantiate(background, background.transform.parent));

            Texts.Add(Instantiate(text, text.transform.parent));
            Texts[i].text = Quests[i].getName();
        }
        Images[0].transform.localPosition = new Vector3(0, -300, 0);
        Texts[0].transform.localPosition = new Vector3(0, -300, 0);
        Images[0].color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        //Remove quest if completed
        if (Quests.Count > 0)
        {
            //Check only top quest
            Quests[0].Update();

            if (Quests[0].getCompletion() == true)
            {
                //Remove the active quest
                Quests.RemoveAt(0);
                Images[0].transform.localPosition = new Vector3(-1000, -1000, 0);
                Images.RemoveAt(0);
                Texts[0].transform.localPosition = new Vector3(-1000, -1000, 0);
                Texts.RemoveAt(0);

                if (Quests.Count == 0)
                {
                    //Deactivate this quest
                    this.gameObject.SetActive(false);
                }
                else
                {
                    //Move next quest to screen
                    Images[0].transform.localPosition = new Vector3(0, -300, 0);
                    Texts[0].transform.localPosition = new Vector3(0, -300, 0);
                    //Activate next quest
                    Quests[0].Activate();
                }
            }
        }
    }
}
