using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

/// <summary>
/// This class makes the audio listener follow the camera position and rotation, as if it was attached to the camera. Some audio spatializers generate errors 
/// when the camera is scaled, so this works around that issue.
/// </summary>
public class TBAudioListener : MonoBehaviour
{
	void Update ()
    {
        transform.SetPositionAndRotation(TBCameraRig.instance.GetCenter().position, TBCameraRig.instance.GetCenter().rotation);
	}
}
