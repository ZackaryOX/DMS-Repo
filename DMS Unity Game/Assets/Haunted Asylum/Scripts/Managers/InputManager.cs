using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InputManager
{
    private static Dictionary<string, KeyCode> Keys = new Dictionary<string, KeyCode>();

    bool waitingForKey = false;
    string waitingKeyName;
    Text waitingKeyText;

    //Set up keys
    public InputManager()
    {
        Keys["forward"] = KeyCode.W;
        Keys["backward"] = KeyCode.S;
        Keys["right"] = KeyCode.D;
        Keys["left"] = KeyCode.A;
        Keys["jump"] = KeyCode.Space;
        Keys["run"] = KeyCode.LeftShift;
        Keys["interact"] = KeyCode.E;
        Keys["use"] = KeyCode.Mouse0;

        Keys["one"] = KeyCode.Alpha1;
        Keys["two"] = KeyCode.Alpha2;
        Keys["three"] = KeyCode.Alpha3;
        Keys["four"] = KeyCode.Alpha4;
        Keys["five"] = KeyCode.Alpha5;
        Keys["six"] = KeyCode.Alpha6;
        Keys["seven"] = KeyCode.Alpha7;
        Keys["eight"] = KeyCode.Alpha8;
        Keys["nine"] = KeyCode.Alpha9;
        Keys["zero"] = KeyCode.Alpha0;

        Keys["escape"] = KeyCode.Escape;
    }


    //Check for Input
    public void Update()
    {
        if (waitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc))
                    {
                        //Key from dictionary = new key
                        Keys[waitingKeyName] = kc;

                        //Change Button text to new key
                        waitingKeyText.text = kc.ToString();

                        waitingForKey = false;
                        break;
                    }
                }
            }
        }
    }

    //Get reference for Text object
    public void StoreText(Text KeyText)
    {
        if (!waitingForKey)
            waitingKeyText = KeyText;
        waitingKeyName = KeyText.name;
        waitingForKey = true;
    }

    public bool GetKey(string temp)
    {
        return Input.GetKey(Keys[temp]);
    }

    public bool GetKeyDown(string temp)
    {
        return Input.GetKeyDown(Keys[temp]);
    }

    public bool GetKeyUp(string temp)
    {
        return Input.GetKeyUp(Keys[temp]);
    }

    public float GetScrollWheel()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }

    public float GetMouseX()
    {
        return Input.GetAxis("Mouse X");
    }

    public float GetMouseY()
    {
        return Input.GetAxis("Mouse Y");
    }

    public void ResetKeys()
    {
        waitingForKey = false;

        Keys["forward"] = KeyCode.W;
        Keys["backward"] = KeyCode.S;
        Keys["right"] = KeyCode.D;
        Keys["left"] = KeyCode.A;
        Keys["jump"] = KeyCode.Space;
        Keys["run"] = KeyCode.LeftShift;
        Keys["interact"] = KeyCode.E;
        Keys["use"] = KeyCode.Mouse0;

        Keys["one"] = KeyCode.Alpha1;
        Keys["two"] = KeyCode.Alpha2;
        Keys["three"] = KeyCode.Alpha3;
        Keys["four"] = KeyCode.Alpha4;
        Keys["five"] = KeyCode.Alpha5;
        Keys["six"] = KeyCode.Alpha6;
        Keys["seven"] = KeyCode.Alpha7;
        Keys["eight"] = KeyCode.Alpha8;
        Keys["nine"] = KeyCode.Alpha9;
        Keys["zero"] = KeyCode.Alpha0;

        Keys["escape"] = KeyCode.Escape;
    }
}
