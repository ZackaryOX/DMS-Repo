using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    private static int Objects = 0;
    //Contstructor
    public Entity(GameObject thisobject)
    {
        this.ThisObject = thisobject;
        this.Trans = thisobject.transform;
        Objects++;
        this.ID = Objects - 1;
        this.Name = "Entity" + ID.ToString();
        ThisObject.name = this.Name;

    }
    //Public
    public int GetID()
    {
        return this.ID;
    }
    public Vector3 GetPosition()
    {

        return this.Trans.position;
    }
    public Vector3 GetRotationEuler()
    {
        return this.Trans.eulerAngles;
    }
    public Quaternion GetRotationQuat()
    {
        return this.Trans.rotation;
    }
    public string GetName()
    {

        return this.Name;
    }
    public GameObject GetObject()
    {
        return ThisObject;
    }
    public void SetPosition(Vector3 temp)
    {
        this.Trans.position = temp;
    }
    public void SetRotationEuler(Vector3 temp)
    {
        this.Trans.eulerAngles = temp;
    }
    public void SetRotationQuat(Quaternion temp)
    {
        this.Trans.rotation = temp;
    }
    public void SetActive(bool temp)
    {
        ThisObject.SetActive(temp);
    }

    public virtual void Update()
    {

    }



    //Private
    protected GameObject ThisObject;
    protected int ID = 0;
    protected Transform Trans;
    protected string Name;

}