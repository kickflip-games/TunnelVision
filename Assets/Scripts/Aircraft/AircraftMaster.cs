using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftMaster : MonoBehaviour
{
    private AircraftMovement _movement;
    private AircraftTextureManager _textureManager;

    public AudioClip ExplosionAudioClip;
    public GameObject [] RenderObjects;
    public GameObject ShardContainer;
    public TimeControl timeControl;
    
    private AudioSource _aircraftAudioSource;
    
    bool _isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        _movement = GetComponentInChildren<AircraftMovement>();
        _textureManager = GetComponent<AircraftTextureManager>();
        _aircraftAudioSource = gameObject.AddComponent<AudioSource>();
        timeControl = FindObjectOfType<TimeControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DestroyShip(){
        if (!_isAlive)
            return; // already being destroyed 
        
        Debug.Log("Destroying ship!");
        
        
        _isAlive = false;
        _movement.Lock ();
        timeControl.Lose ();
        AircraftExplosion exp = gameObject.AddComponent<AircraftExplosion>();
        exp.SetReferences(
            audioSource: _aircraftAudioSource,
            explosionAudio: ExplosionAudioClip,
            renderedGameObjects: RenderObjects,
            shardContainer: ShardContainer
        );
        exp.Explode();

    }
}
