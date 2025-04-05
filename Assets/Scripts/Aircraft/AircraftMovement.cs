using UnityEngine;
using UnityEngine.InputSystem; // For Mouse.current
using System.Collections.Generic;
using Assets;
using Assets.Generation;

public class AircraftMovement : MonoBehaviour
{
    public float Speed = 16;
    public float TurnSpeed = 3f;

    public List<AircraftTrail> Trails = new List<AircraftTrail>();
    public GameObject[] TrailPositionsMarkers;
    public GameObject Trail;

    private bool _lock = false;
    private float _speed = 0;

    // --- Input and smoothing variables ---
    private Mouse mouse;
    [SerializeField]
    private float smoothTime = 0.1f;
    private float horizontalVelocity = 0f;
    private float verticalVelocity = 0f;
    private float horizontalDelta = 0f;
    private float verticalDelta = 0f;

    [Header("Sensitivity Settings")]
    [SerializeField]
    private float mouseSensitivity = 0.1f;    // Scales mouse movement
    [SerializeField]
    private float keyboardSensitivity = 3f;   // Scales WASD (keyboard) input

    /// <summary>
    /// Check if the aircraft is within the spawn area (used for scoring, etc.)
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
        // Initialize mouse input from the new Input System.
        mouse = Mouse.current;
    }

    public void Lock() { _lock = true; }
    public void Unlock() { _lock = false; }

    /// <summary>
    /// Returns true if the aircraft is turning enough to display trails.
    /// </summary>
    bool IsTurning
    {
        get
        {
            float zAngle = transform.localRotation.eulerAngles.z;
            float xAngle = transform.localRotation.eulerAngles.x;
            return ((zAngle > 45 && zAngle < 135) || (zAngle > 225 && zAngle < 315) ||
                    (xAngle > 45 && xAngle < 90) || (xAngle > 270 && xAngle < 315));
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
    /// Applies the rotation to the aircraft using the combined, smoothed input.
    /// </summary>
    /// <param name="hDelta">Horizontal delta</param>
    /// <param name="vDelta">Vertical delta</param>
    void TurnAndRotate(float hDelta, float vDelta)
    {
        float scale = (Time.timeScale != 1) ? (1 / Time.timeScale) * 0.5f : 1;
        float rate = Time.deltaTime * 64f * TurnSpeed * scale;

        // Vertical rotation (pitch)
        Vector3 verticalTurn = Vector3.right * rate * vDelta;
        if (Options.Invert)
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + verticalTurn);
        else
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - verticalTurn);

        // Horizontal rotation (roll)
        Vector3 horizontalTurn = Vector3.forward * rate * hDelta;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - horizontalTurn);

        // Yaw rotation by rotating the parent transform
        transform.parent.Rotate(Vector3.up * rate * hDelta);
    }

    // Update is called once per frame
    void Update()
    {
        if (_lock)
            return;

        MoveForward();
        AdjustTrails();

        // Get mouse contribution (if available)
        Vector2 mouseInput = Vector2.zero;
        if (mouse != null)
        {
            // Multiply by mouseSensitivity here
            mouseInput = mouse.delta.ReadValue() * mouseSensitivity;
        }

        // Get keyboard contribution from WASD
        Vector2 keyboardInput = new Vector2(
            Input.GetAxisRaw("Horizontal") * keyboardSensitivity,
            Input.GetAxisRaw("Vertical") * keyboardSensitivity);

        // Combine the two input sources
        Vector2 combinedTarget = mouseInput + keyboardInput;

        // Smooth the combined input values
        horizontalDelta = Mathf.SmoothDamp(horizontalDelta, combinedTarget.x, ref horizontalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
        verticalDelta = Mathf.SmoothDamp(verticalDelta, combinedTarget.y, ref verticalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

        // Apply rotation based on the smoothed, combined delta
        TurnAndRotate(horizontalDelta, verticalDelta);
    }
}
