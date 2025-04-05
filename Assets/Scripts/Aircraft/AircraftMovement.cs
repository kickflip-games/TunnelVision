using UnityEngine;
using UnityEngine.InputSystem; // For Mouse, Keyboard, and AttitudeSensor
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

    // Input smoothing fields
    private Mouse mouse;
    private AttitudeSensor attitudeSensor;
    private bool isGyroAvailable = false;
    [SerializeField]
    private bool useGyroscope = true;  // Enable/disable gyroscope input
    private Quaternion initialGyro = Quaternion.identity;
    [SerializeField]
    private float gyroSensitivity = 1.0f; // Scale factor for gyroscope input

    [SerializeField]
    private float smoothTime = 0.1f;
    private float horizontalVelocity = 0f;
    private float verticalVelocity = 0f;
    private float horizontalDelta = 0f;
    private float verticalDelta = 0f;

    [Header("Sensitivity Settings")]
    [SerializeField]
    private float mouseSensitivity = 0.1f;    // Mouse input multiplier
    [SerializeField]
    private float keyboardSensitivity = 3f;   // WASD input multiplier

    /// <summary>
    /// Check if the aircraft is within the spawn area.
    /// </summary>
    public bool IsInSpawn
    {
        get
        {
            float distanceFromSpawn = (transform.parent.position - WorldGenerator.SpawnPosition).sqrMagnitude;
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

        // Initialize mouse input.
        mouse = Mouse.current;

        // Try to get the gyroscope (AttitudeSensor) from the SensorCamera plugin.
        if (useGyroscope)
        {
            attitudeSensor = AttitudeSensor.current;
            if (attitudeSensor != null)
            {
                isGyroAvailable = true;
                // Store the initial gyro reading as a baseline.
                initialGyro = attitudeSensor.attitude.ReadValue();
            }
        }
    }

    public void Lock() { _lock = true; }
    public void Unlock() { _lock = false; }

    /// <summary>
    /// Returns true if the aircraft’s rotation is such that trails should be activated.
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
    /// Applies rotation based on combined input deltas.
    /// </summary>
    /// <param name="hDelta">Horizontal delta</param>
    /// <param name="vDelta">Vertical delta</param>
    void TurnAndRotate(float hDelta, float vDelta)
    {
        float scale = (Time.timeScale != 1) ? (1 / Time.timeScale) * 0.5f : 1;
        float rate = Time.deltaTime * 64f * TurnSpeed * scale;
        float scaledH = hDelta;
        float scaledV = vDelta;

        // Vertical rotation (pitch)
        Vector3 verticalTurn = Vector3.right * rate * scaledV;
        if (Options.Invert)
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + verticalTurn);
        else
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - verticalTurn);

        // Horizontal rotation (roll)
        Vector3 horizontalTurn = Vector3.forward * rate * scaledH;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles - horizontalTurn);

        // Yaw rotation via the parent transform
        transform.parent.Rotate(Vector3.up * rate * scaledH);
    }

    /// <summary>
    /// Normalizes an angle to the range [-180, 180].
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    // Update is called once per frame
    void Update()
    {
        if (_lock)
            return;

        MoveForward();
        AdjustTrails();

        // --- Mouse Input ---
        Vector2 mouseInput = Vector2.zero;
        if (mouse != null)
        {
            mouseInput = mouse.delta.ReadValue() * mouseSensitivity;
        }

        // --- Keyboard (WASD) Input ---
        Vector2 keyboardInput = new Vector2(
            Input.GetAxisRaw("Horizontal") * keyboardSensitivity,
            Input.GetAxisRaw("Vertical") * keyboardSensitivity);

        // Combine mouse and keyboard input.
        Vector2 combinedInput = mouseInput + keyboardInput;

        // Smooth the combined input.
        horizontalDelta = Mathf.SmoothDamp(horizontalDelta, combinedInput.x, ref horizontalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
        verticalDelta = Mathf.SmoothDamp(verticalDelta, combinedInput.y, ref verticalVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

        // --- Gyroscope Input ---
        Vector2 gyroDelta = Vector2.zero;
        if (useGyroscope && isGyroAvailable)
        {
            Quaternion currentGyro = attitudeSensor.attitude.ReadValue();
            // Calculate the delta relative to the initial calibration.
            Quaternion deltaGyro = Quaternion.Inverse(initialGyro) * currentGyro;
            Vector3 deltaEuler = deltaGyro.eulerAngles;
            // Normalize Euler angles to the range [-180, 180].
            deltaEuler.x = NormalizeAngle(deltaEuler.x);
            deltaEuler.y = NormalizeAngle(deltaEuler.y);
            deltaEuler.z = NormalizeAngle(deltaEuler.z);
            // Here we assume that the device's pitch (x) controls vertical rotation (affecting aircraft pitch)
            // and the device's roll (z) controls horizontal rotation (affecting aircraft roll/yaw).
            gyroDelta = new Vector2(deltaEuler.z, deltaEuler.x) * gyroSensitivity;
        }

        // Combine the smoothed input with the gyroscope delta.
        Vector2 finalInput = new Vector2(horizontalDelta, verticalDelta) + gyroDelta;

        // Apply the combined rotation.
        TurnAndRotate(finalInput.x, finalInput.y);
    }
}
