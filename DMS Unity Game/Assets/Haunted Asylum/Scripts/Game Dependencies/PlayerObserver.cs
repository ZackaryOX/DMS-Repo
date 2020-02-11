using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PlayerObserver
{
    
    private static int IDCount = 0; 
    protected PlayerObserver(Player subject)
    {
        //Assign an ID because it will be used to index the dictionary of observers
        //mainly used for removing an observer but also helps us find a specific one 
        //if needed.
        AssignID();
        Subject = subject;
        Subject.AttachObserver(this);
    }

    protected void AssignID()
    {
        MyID = IDCount;
        IDCount++;
    }

    public virtual void Update()
    {

    }

    public int GetID()
    {
        return MyID;
    }


    protected int MyID;
    protected Player Subject;
}


public class StatObserver : PlayerObserver
{
    private float HealthToReport;
    private float StaminaToReport;
    private float SanityToReport;
    private Vector3 DataToReport;
    public StatObserver(Player temp) : base(temp)
    {

    }
    public Vector3 GetData()
    {
        return DataToReport;
    }
    public override void Update()
    {
        float temphealth = Subject.GetHealth();
        float tempstamina = Subject.GetStamina();
        float tempsanity = Subject.GetSanity();

        //Checks to see if any of the 3 values have changed, if so it will save 
        //those to the corresponding variables and also construct our Data Vector3
        //which stores Health, Stamina, Sanity in that order!!!
        DataToReport = 
            temphealth != HealthToReport || 
            tempstamina != StaminaToReport || 
            tempsanity != SanityToReport  ?
            new Vector3(HealthToReport = temphealth,StaminaToReport = tempstamina, SanityToReport = tempsanity) : DataToReport;

        

    }
}

public class ScoreObserver : PlayerObserver
{
    private float ScoreToReport;
    public ScoreObserver(Player temp) : base(temp)
    {

    }

    public float GetScore()
    {
        return ScoreToReport;

    }

    public override void Update()
    {
        float tempscore = Subject.GetScore();

        ScoreToReport = tempscore != ScoreToReport ? tempscore : ScoreToReport;
    }
}

//----------------------------------------


abstract public class GhostObserver
{

    private static int IDCount = 0;
    protected GhostObserver(Ghost subject)
    {
        //Assign an ID because it will be used to index the dictionary of observers
        //mainly used for removing an observer but also helps us find a specific one 
        //if needed.
        AssignID();
        Subject = subject;
        Subject.AttachObserver(this);
    }

    protected void AssignID()
    {
        MyID = IDCount;
        IDCount++;
    }

    public virtual void Update()
    {

    }

    public int GetID()
    {
        return MyID;
    }


    protected int MyID;
    protected Ghost Subject;
}


public class GhostStatObserver : GhostObserver
{
    private float HealthToReport;
    private float StaminaToReport;
    private float SanityToReport;
    private Vector2 DataToReport;
    public GhostStatObserver(Ghost temp) : base(temp)
    {

    }
    public Vector3 GetData()
    {
        return DataToReport;
    }
    public override void Update()
    {
        float temphealth = Subject.GetHealth();
        float tempstamina = Subject.GetStamina();

        //Checks to see if any of the 3 values have changed, if so it will save 
        //those to the corresponding variables and also construct our Data Vector3
        //which stores Health, Stamina, Sanity in that order!!!
        DataToReport =
            temphealth != HealthToReport ||
            tempstamina != StaminaToReport ?
            new Vector2(HealthToReport = temphealth, StaminaToReport = tempstamina) : DataToReport;



    }
}

public class GhostScoreObserver : GhostObserver
{
    private float ScoreToReport;
    public GhostScoreObserver(Ghost temp) : base(temp)
    {

    }

    public float GetScore()
    {
        return ScoreToReport;

    }

    public override void Update()
    {
        float tempscore = Subject.GetScore();

        ScoreToReport = tempscore != ScoreToReport ? tempscore : ScoreToReport;
    }
}