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
public class SensorCameraModeSelector : MonoBehaviour
{
    public void SelectGyro()
    {
        SensorCameraControl.ForciblySelectedSensor =
            SensorCameraControl.ForcedSensorSelection.Gyro;
    }

    public void SelectAccelerometer()
    {
        SensorCameraControl.ForciblySelectedSensor =
            SensorCameraControl.ForcedSensorSelection.Accelerometer;
    }

    public void SelectNoSensors()
    {
        SensorCameraControl.ForciblySelectedSensor =
            SensorCameraControl.ForcedSensorSelection.NoSensors;
    }

}
