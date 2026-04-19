using UnityEngine;
using System.Reflection;
using System;

public class ErrorFinder : MonoBehaviour
{
    void Start()
    {
        // This looks through the entire game's memory to find every script named "VD"
        var types = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in types)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "VD")
                {
                    Debug.Log("<color=red>FOUND VD AT: </color>" + type.Assembly.FullName);
                }
            }
        }
    }
}