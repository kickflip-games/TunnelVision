using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject[] StartScreenObjects;
    public GameObject[] EndScreenObjects;
    public GameObject ScoreObject;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        ToggleStartScreen(true);
        ToggleEndScreen(false);
        ToggleScore(false);
    }

    void ToggleStartScreen(bool state)
    {
        foreach (var go in StartScreenObjects)
        {
            go.SetActive(state);
        }
    }

    void ToggleEndScreen(bool state)
    {
        foreach (var go in EndScreenObjects)
        {
            go.SetActive(state);
        }
    }

    void ToggleScore(bool state)
    {
        ScoreObject.SetActive(state);
    }
    
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
