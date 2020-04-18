using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftMaster : MonoBehaviour
{
    private AircraftMovement _movement;
    private AircraftTextureManager _textureManager;

    public AudioClip ExplosionAudioClip;
    public GameObject [] RenderObjects;
    
    bool _isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        _movement = GetComponentInChildren<AircraftMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DestroyShip(){
        if (!_isAlive)
            return; // already being destroyed 

        _movement.Lock ();
        // TimeControl.Lose ();
        AircraftExplosion exp = gameObject.AddComponent<AircraftExplosion>();
        exp.SetReferences(
            explosionAudio: ExplosionAudioClip,
            renderedGameObjects: RenderObjects
        );
        _isAlive = true;
    }
}
