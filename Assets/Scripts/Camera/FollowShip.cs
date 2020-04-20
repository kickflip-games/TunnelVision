using UnityEngine;
using System.Collections;

public class FollowShip : MonoBehaviour
{
    public GameObject PlayerGameObject;
    public Vector3 Offset;
    private Vector3 _lerpPosition;

    void Start()
    {
        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        if (PlayerGameObject == null)
            return;
        _lerpPosition = Lerp(_lerpPosition, PlayerGameObject.transform.position,
            Time.deltaTime * 32f);
        this.transform.position = Lerp(this.transform.position,
            PlayerGameObject.transform.position + PlayerGameObject.transform.forward * Offset.z +
            PlayerGameObject.transform.up * Offset.y, Time.deltaTime * 8f);
        this.transform.LookAt(_lerpPosition, Vector3.up);
    }

    public Vector3 Lerp(Vector3 A, Vector3 B, float C)
    {
        return new Vector3(
            Mathf.Lerp(A.x, B.x, C),
            Mathf.Lerp(A.y, B.y, C),
            Mathf.Lerp(A.z, B.z, C)
            );
    }
}