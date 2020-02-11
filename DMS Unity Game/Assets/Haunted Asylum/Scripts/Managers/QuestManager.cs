using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    //Public
    public Image background;
    public Text text;
    public Sprite topQuest;

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
    private PickUp flashlight;
    private PickUp key;

    //Quests
    private WaypointQuest q1;
    private WaypointQuest q2;
    private PickUpQuest q3;
    private WaypointQuest q4;
    private PickUpQuest q5;
    private WaypointQuest q6;
    private UseQuest q7;

    // Start is called before the first frame update
    void Start()
    {
        flashlight = PickUp.AllItems[flashlightobj.name];
        key = PickUp.AllItems[keyobj.name];

        q1 = new WaypointQuest("Leave cell", waypoint1);
        q2 = new WaypointQuest("Walk to drawer", waypoint2);
        q3 = new PickUpQuest("Open drawer and pickup flashlight", flashlight);
        q4 = new WaypointQuest("Walk to key", waypoint3);
        q5 = new PickUpQuest("Pickup key", key);
        q6 = new WaypointQuest("Walk to door", waypoint4);
        q7 = new UseQuest("Use key and open the door", keyobj);


        Quests.Add(q1);
        Quests.Add(q2);
        Quests.Add(q3);
        Quests.Add(q4);
        Quests.Add(q5);
        Quests.Add(q6);
        Quests.Add(q7);
        Quests[0].Activate();

        //Create UI
        for (int i = 0; i < Quests.Count; i++)
        {
            Images.Add(Instantiate(background, background.transform.parent));
            Images[i].transform.localPosition = new Vector3(660, 485 - 75 * i, 0);

            Texts.Add(Instantiate(text, text.transform.parent));
            Texts[i].transform.localPosition = new Vector3(660, 485 - 75 * i, 0);
            Texts[i].text = Quests[i].getName();
        }
        Images[0].color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        //Remove quest if completed
        if (Quests.Count > 0 && Quests[0].getCompletion() == true)
        {
            Quests.RemoveAt(0);
            Images[0].transform.localPosition = new Vector3(-1000, -1000, 0);
            Images.RemoveAt(0);
            Texts[0].transform.localPosition = new Vector3(-1000, -1000, 0);
            Texts.RemoveAt(0);

            for (int i = 0; i < Images.Count; i++)
            {
                Images[i].transform.localPosition += new Vector3(0, 75, 0);
                Texts[i].transform.localPosition += new Vector3(0, 75, 0);
            }

            if (Quests.Count == 0)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                Quests[0].Activate();
                Images[0].sprite = topQuest;
            }
        }

        if (Quests.Count != 0)
        {
            //Check only top quest
            Quests[0].Update();
        }
    }
}
