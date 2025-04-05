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
public class RotationByKeysControl : MonoBehaviour
{
    [Header("Horizontal")]
    [Tooltip("Object for Horizontal Rotation")]
    public Transform horizontal;
    public string horizontalAxis = "Horizontal";
    public float speedHorizontal = 75f;

    [Header("Vertical")]
    [Tooltip("Object for Vertical Rotation")]
    public Transform vertical;
    public string verticalAxis = "Vertical";
    public float speedVertical = -50f;

    private void LateUpdate()
    {
        horizontal.Rotate(
            0f,
            Input.GetAxis(horizontalAxis) * speedHorizontal * Time.deltaTime,
            0f);

        vertical.Rotate(
            Input.GetAxis(verticalAxis) * speedVertical * Time.deltaTime,
            0f,
            0f);
    }

}
