/*
================================
Assets for Unity by Makaka Games
================================
 
[Online  Docs -> Updated]: https://makaka.org/unity-assets
[Offline Docs - PDF file]: find it in the package folder.

[Support]: https://makaka.org/support
*/

using UnityEngine;

[HelpURL("https://makaka.org/unity-assets")]
public class BreathControl : MonoBehaviour
{
    public Vector3 period = new(0f, 1.2f, 0f);

    public Vector3 amplitude = new(0f, 0.2f, 0f);
    
    private Vector3 distanceCurrent;
    
    private Vector3 positionOnStart;

    protected void Start()
    {
        positionOnStart = transform.position;
    }

    protected void Update()
    {
        distanceCurrent = new Vector3(
            period.x == 0f ? 0f : Mathf.Sin(Time.timeSinceLevelLoad / period.x),
            period.y == 0f ? 0f : Mathf.Sin(Time.timeSinceLevelLoad / period.y),
            period.z == 0f ? 0f : Mathf.Sin(Time.timeSinceLevelLoad / period.z)
            );

        transform.position =
            positionOnStart + Vector3.Scale(amplitude, distanceCurrent);
    }

}
