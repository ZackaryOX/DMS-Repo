using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Quest
{
    protected GameObject objective;
    protected string questName;
    protected bool isCompleted = false;

    public Quest(string name, GameObject obj)
    {
        objective = obj;
        questName = name;
    }

    public bool getCompletion() { return isCompleted; }
    public string getName() { return questName; }

    public virtual void Update()
    {
    }

    public virtual void Deactivate()
    {
    }

    public virtual void Activate()
    {
    }
}

public class WaypointQuest : Quest
{
    private Vector3 Position;
    public WaypointQuest(string name, GameObject obj) : base(name, obj)
    {
        Position = obj.transform.position;
        Deactivate();
    }


    public override void Update()
    {
        isCompleted = objective.activeSelf == false ? true : isCompleted;
    }

    public override void Deactivate()
    {
        objective.transform.position = new Vector3(-100, -100, -100);
    }

    public override void Activate()
    {
        objective.transform.position = Position;
    }
}


public class PickUpQuest : Quest
{
    private PickUp item;

    public PickUpQuest(string name, PickUp obj) : base(name, null)
    {
        item = obj;
        Deactivate();
    }

    public override void Update()
    {
        if (item.GetPicked() == true)
        {
            isCompleted = true;
        }
    }

    public override void Deactivate()
    {
        item.SetCanBePicked(false);
    }

    public override void Activate()
    {
        item.SetCanBePicked(true);
    }
}

public class UseQuest : Quest
{
    public UseQuest(string name, GameObject obj) : base(name, obj)
    {
        Deactivate();
    }

    public override void Update()
    {
        isCompleted = objective.activeSelf == false ? true : isCompleted;
    }
}