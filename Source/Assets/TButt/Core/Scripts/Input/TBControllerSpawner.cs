using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;

namespace TButt.Input
{
    /// <summary>
    /// Sample for spawning different controller models into the scene based on the input type. This is not a required TButt component, but is provided as an example since it's
    /// a common use case.
    /// </summary>
    public class TBControllerSpawner : MonoBehaviour
    {
        [Header("Tracked Controller Models")]
        [SerializeField]
        private GameObject leftHandController;

        [SerializeField]
        private GameObject rightHandController;

        [SerializeField]
        private GameObject mobile3DOFController;

        private IEnumerator Start()
        {
            while(TBInput.GetActiveControlType() == TBInput.ControlType.None)
            {
                yield return null;
            }

            switch (TBInput.GetActiveControlType())
            {
                case TBInput.ControlType.Mobile3DOFController:
                    Instantiate(mobile3DOFController).name = "3DOF Controller";
                    break;
                case TBInput.ControlType.HandControllers:
                    Instantiate(leftHandController).name = "Left Hand Controller";
                    Instantiate(rightHandController).name = "Right Hand Controller";
                    break;
            }
        }
    }
}