using UnityEngine;
using UnityEngine.InputSystem; // For Mouse.current
using System.Collections.Generic;
using Assets;
using Assets.Generation;

public class AircraftMovement : MonoBehaviour
{
    public float Speed = 16;
    public float TurnSpeed = 4f;

    public List<AircraftTrail> Trails = new List<AircraftTrail>();
    public GameObject[] TrailPositionsMarkers;
    public GameObject Trail;

    private bool _lock = false;
    private float _speed = 0;

    // --- Fields for smoothed rotation input ---
    private Mouse mouse;
    [SerializeField]
    private float smoothTime = 0.1f;
    private float horizontalVelocity = 0f;
    private float verticalVelocity = 0f;
    private float horizontalDelta = 0f;
    private float verticalDelta = 0f;

    [Header("Sensitivity Settings")]
    [SerializeField]
    private float mouseSensitivity = 0.1f;    // Sensitivity multiplier for mouse input

    [SerializeField]
    private float keyboardSensitivity = 3f;   // Sensitivity multiplier for keyboard (WASD) input

    /// <summary>
    /// Check if the aircraft is within the spawn area (used to check if it should be given score)
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

        // Try to initialize mouse input from the new Input System.
        mouse = Mouse.current;
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
    /// Returns true if the aircraft is turning enough to display trails.
    /// </summary>
    bool IsTurning
    {
        get
        {
            float zAngle = transform.localRotation.eulerAngles.z;
            float xAngle = transform.localRotation.eulerAngles.x;
            if ((zAngle > 45 && zAngle < 135) || (zAngle > 225 && zAngle < 315) ||
                (xAngle > 45 && xAngle < 90) || (xAngle > 270 && xAngle < 315))
                return true;
            return false;
        }
    }

    void MoveForward()
    {
        _speed = Mathf.Lerp(_speed, Speed, Time.deltaTime * 0.25f);
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

    /// <summary>
    /// Rotates the aircraft based on input delta values.
    /// The sensitivity parameter allows using different multipliers for mouse vs. keyboard.
    /// </summary>
    /// <param name="hDelta">Smoothed horizontal input</param>
    /// <param name="vDelta">Smoothed vertical input</param>
    /// <param name="sensitivity">Sensitivity multiplier</param>
    void TurnAndRotate(float hDelta, float vDelta, float sensitivity)
    {
        float scale = (Time.timeScale != 1) ? (1 / Time.timeScale) * 0.5f : 1;
        float rate = Time.deltaTime * 64f * TurnSpeed * scale;

        float scaledH = hDelta * sensitivity;
        float scaledV = vDelta * sensitivity;

        // Vertical rotation (pitch)
        Vector3 verticalTurn = Vector3.right * rate * scaledV;
        if (Options.Invert)
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + verticalTurn);
        else
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - verticalTurn);

        // Horizontal rotation (roll)
        Vector3 horizontalTurn = Vector3.forward * rate * scaledH;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - horizontalTurn);

        // Yaw rotation by rotating the parent transform
        transform.parent.Rotate(Vector3.up * rate * scaledH);
    }

    // Update is called once per frame
    void Update()
    {
        if (_lock)
            return;

        MoveForward();
        AdjustTrails();

        // Use mouse input if available; otherwise, fallback to WASD (keyboard) input.
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            float targetHorizontalDelta = mouseDelta.x;
            float targetVerticalDelta = mouseDelta.y;

            horizontalDelta = Mathf.SmoothDamp(horizontalDelta, targetHorizontalDelta, ref horizontalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
            verticalDelta = Mathf.SmoothDamp(verticalDelta, targetVerticalDelta, ref verticalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

            TurnAndRotate(horizontalDelta, verticalDelta, mouseSensitivity);
        }
        else
        {
            float hInput = Input.GetAxisRaw("Horizontal");
            float vInput = Input.GetAxisRaw("Vertical");

            horizontalDelta = Mathf.SmoothDamp(horizontalDelta, hInput, ref horizontalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
            verticalDelta = Mathf.SmoothDamp(verticalDelta, vInput, ref verticalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

            TurnAndRotate(horizontalDelta, verticalDelta, keyboardSensitivity);
        }
    }
}
