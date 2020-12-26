using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBroker : MonoBehaviour
{
    public static List<string> forcedKeyDowns = new List<string> {};
    public static List<string> forcedKeyUps = new List<string> {};
    public static List<string> forcedKey = new List<string> {};

    public static bool GetKeyDown(KeyCode keyToPress)
    {
        return Input.GetKeyDown(keyToPress) || forcedKeyDowns.Contains(keyToPress.ToString());
    }
    public static bool GetKeyUp(KeyCode keyToPress)
    {
        return Input.GetKeyUp(keyToPress) || forcedKeyUps.Contains(keyToPress.ToString());
    }
    public static bool GetKey(KeyCode keyToPress)
    {
        return Input.GetKey(keyToPress) || forcedKey.Contains(keyToPress.ToString());
    }
    public static void SetKeyDown(List<string> keys)
    {
        forcedKeyDowns = keys;
    }
    public static void SetKeyUp(List<string> keys)
    {
        forcedKeyUps = keys;
    }
    public static void SetKey(List<string> keys)
    {
        forcedKey = keys;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}