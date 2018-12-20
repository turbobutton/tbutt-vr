using UnityEngine;
using System.Collections;

using TButt;

public static class TBTransformExtensions
{
    /// <summary>
    /// Resets the local position of a transform to Vector3.zero, and local rotation to Quaternion.identity.
    /// </summary>
    /// <param name="t"></param>
    public static void ResetLocalPositionAndRotation(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Makes the transform a child of the parent you pass it under, and zeroes out its local position and rotation.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="parent"></param>
    public static void MakeZeroedChildOf(this Transform t, Transform parent)
    {
        t.parent = parent;
        t.ResetLocalPositionAndRotation();
    }

    /// <summary>
    /// Billboards an object to TBCameraRig's center
    /// </summary>
    /// <param name="billboard"></param>
    public static void FaceCamera(this Transform billboard)
    {
#if DEBUGMODE
        if (TButt.Marketing.TBCaptureCamera.instance != null)
        {
            billboard.LookAt(billboard.position + (billboard.position - TButt.Marketing.TBCaptureCamera.instance.transform.position));
            return;
        }
#endif
        billboard.LookAt(billboard.position + (billboard.position - TBCameraRig.instance.GetCenter().position));
    }
}