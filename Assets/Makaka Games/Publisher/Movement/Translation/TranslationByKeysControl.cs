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
public class TranslationByKeysControl : MonoBehaviour
{
    [Header("X")]
    public bool isXAxisParent = false;
    public Transform xAxis;
    public string xAxisName = "Horizontal";
    public float xAxisSpeed = 4f;

    [Header("Y (Q & E keys)")]
    public bool isYAxisParent = false;
    public Transform yAxis;
    public float yAxisSpeed = 5f;

    [Header("Z")]
    public bool isZAxisParent = false;
    public Transform zAxis;
    public string zAxisName = "Vertical";
    public float zAxisSpeed = 4f;

    private void LateUpdate()
    {
        TranslateXUpdate();
        TranslateYUpdate();
        TranslateZUpdate();
    }

    private void TranslateXUpdate()
    {
        if (xAxis)
        {
            (isXAxisParent ? xAxis.parent : xAxis).Translate(
                Input.GetAxis(xAxisName) * xAxisSpeed * Time.deltaTime,
                0f,
                0f);
        }
    }

    private void TranslateZUpdate()
    {
        if (zAxis)
        {
            (isZAxisParent ? zAxis.parent : zAxis).Translate(
                0f,
                0f,
                Input.GetAxis(zAxisName) * zAxisSpeed * Time.deltaTime);
        }
    }

    private void TranslateYUpdate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            TranslateYAxis(-yAxisSpeed);
        }

        if (Input.GetKey(KeyCode.E))
        {
            TranslateYAxis(yAxisSpeed);
        }
    }

    private void TranslateYAxis(float speed)
    {
        if (yAxis)
        {
            (isYAxisParent ? yAxis.parent : yAxis).Translate(
                0f, speed * Time.deltaTime, 0f);
        }
    }

}
