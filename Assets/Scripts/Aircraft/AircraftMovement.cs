/*
 * Written by Avi Vajpeyi
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Assets.Generation;
public class AircraftMovement : MonoBehaviour
{
    public float Speed = 16;
    public float TurnSpeed = 3f;


    public List<AircraftTrail> Trails = new List<AircraftTrail>();

//    public Vector3[] TrailPositions;
    public GameObject[] TrailPositionsMarkers;
    public GameObject Trail;

    private bool _lock = false;
    private float _speed = 0;
    

    /// <summary>
    /// Check if I am within the spawn area (used to check if I should be given score or not)
    /// </summary>
    public bool IsInSpawn
    {
        get
        {
            float distanceFromSpawn =
                (transform.parent.position - WorldGenerator.SpawnPosition).sqrMagnitude;
            float spawnArea = WorldGenerator.SpawnRadius * WorldGenerator.SpawnRadius;
            return distanceFromSpawn < spawnArea;
        }
    }


    void InitialiseTrails()
    {
        foreach (GameObject marker in TrailPositionsMarkers)
        {
            Vector3 pos = marker.transform.position;
            GameObject trailGo = Instantiate(Trail, pos, Quaternion.identity);
            trailGo.transform.parent = transform;
            AircraftTrail t = trailGo.GetComponent<AircraftTrail>();
            Trails.Add(t);
        }
    }


    void Start()
    {
        InitialiseTrails();
        Unlock();
    }

    public void Lock()
    {
        _lock = true;
    }

    public void Unlock()
    {
        _lock = false;
    }


    /// <summary>
    /// Is turning enough for trails to be turned on
    /// </summary>
    bool IsTurning
    {
        get
        {
            float zAngle = transform.localRotation.eulerAngles.z;
            float xAngle = transform.localRotation.eulerAngles.x;
            if (zAngle > 45 && zAngle < 135 || zAngle > 225 && zAngle < 315 ||
                xAngle > 45 && xAngle < 90 || xAngle > 270 && xAngle < 315)
                return true;
            return false;
        }
    }

    void MoveForward()
    {
        _speed = Mathf.Lerp(_speed, Speed, Time.deltaTime * .25f);
        transform.parent.position += transform.forward * Time.deltaTime * 4 * _speed;
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(Vector3.zero), Time.deltaTime * 2.5f);
    }

    void AdjustTrails()
    {
        foreach (AircraftTrail trail in Trails)
        {
            trail.ToggleTrail(turnOn: IsTurning);
        }
    }

    void TurnAndRotate(float hAxis, float vAxis)
    {
        float scale = (Time.timeScale != 1) ? (1 / Time.timeScale) * .5f : 1;
        float rate = Time.deltaTime * 64f * TurnSpeed * scale;

        // Vertical-Rotation of aircraft 
        Vector3 verticalTurn = Vector3.right * rate * vAxis;
        if (Options.Invert)
            transform.localRotation =
                Quaternion.Euler(transform.localRotation.eulerAngles + verticalTurn);
        else
            transform.localRotation =
                Quaternion.Euler(transform.localRotation.eulerAngles - verticalTurn);

        // Horizontal-Rotation of aircraft
        Vector3 horizontalTurn = Vector3.forward * rate * hAxis;
        transform.localRotation =
            Quaternion.Euler(transform.localRotation.eulerAngles - horizontalTurn);

        // Turning of aircraft
        transform.parent.Rotate(Vector3.up * rate * hAxis);
    }


    // Update is called once per frame
    void Update()
    {
        if (_lock)
            return;

        MoveForward();
        AdjustTrails();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        TurnAndRotate(hAxis: h, vAxis: v);
    }
}