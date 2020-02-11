using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class PluginTesterScript : MonoBehaviour
{
    
    Timer T;
    bool check = false;
    const string DLL_NAME = "Tutorial DLL";

    [DllImport(DLL_NAME)]
    private static extern void writeTime(float _Time);

    [DllImport(DLL_NAME)]
    private static extern float readTime();

    [DllImport(DLL_NAME)]
    private static extern void deleteLogs();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Door.AllDoors.Count > 0 && Door.AllDoors[gameObject.name].GetIsOpened() == true && check == false)
        {
            writeTime(Timer.ElapsedTime);
            check = true;
            Debug.Log(Timer.ElapsedTime);
        }

        if (Input.GetKey(KeyCode.N))
        {
            writeTime(T.ReturnTime());
        }

        if (Input.GetKey(KeyCode.Z))
        {
            Debug.Log(readTime());
        }


    }
}
