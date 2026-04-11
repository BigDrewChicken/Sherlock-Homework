using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameToSelector : SceneLoader
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            LoadScene();
        }
    }
}
