using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TButt;
using UnityEngine.UI;

namespace TButt.Sample
{
    public class TBSampleControllerSelector : MonoBehaviour
    {
        [SerializeField]
        private TBInput.ControlType controlType;

        [SerializeField]
        private Image fillImage;

        [SerializeField]
        private Image disabledImage;

        private float _selectionVal;

        void Start()
        {
            if (TBInput.GetActiveControlType() != TBInput.ControlType.None)
                gameObject.SetActive(false);
            else
                StartCoroutine(ChooseController());
        }

        private IEnumerator ChooseController()
        {
            if (!TBInput.SupportsControlType(controlType))
            {
                disabledImage.gameObject.SetActive(true);
                yield break;
            }

            while (TBInput.GetActiveControlType() == TBInput.ControlType.None)
            {
                switch(controlType)
                {
                    case TBInput.ControlType.HandControllers:
                        AnimateHandControllers();
                        break;
                    case TBInput.ControlType.ClickRemote:
                        AnimateInput(ref _selectionVal, TBInput.Controller.ClickRemote);
                        break;
                    case TBInput.ControlType.Gamepad:
                        AnimateInput(ref _selectionVal, TBInput.Controller.Gamepad);
                        break;
                    case TBInput.ControlType.Mobile3DOFController:
                        AnimateInput(ref _selectionVal, TBInput.Controller.Mobile3DOFController);
                        break;
                }

                if (_selectionVal >= 1)
                    TBInput.SetActiveControlType(controlType);

                yield return null;
            }
        }

        private void AnimateHandControllers()
        {
            bool leftController = false;
            bool rightController = false;

            if (TBInput.GetButton(TBInput.Button.Any, TBInput.Controller.RHandController))
                rightController = true;

            if (TBInput.GetButton(TBInput.Button.Any, TBInput.Controller.LHandController))
                leftController = true;

            if (rightController || leftController)
                AddTime(ref _selectionVal);
            else
                ResolveTime(ref _selectionVal);
        }

        private void AnimateInput(ref float time, TBInput.Controller controller)
        {
            if (TBInput.GetButton(TBInput.Button.Any, controller))
            {
                AddTime(ref time);
            }
            else
            {
                ResolveTime(ref time);
            }
        }

        private void AddTime(ref float time)
        {
            time += Time.deltaTime;
            fillImage.fillAmount = time;
        }

        private void ResolveTime(ref float time)
        {
            if (time > 0)
            {
                fillImage.fillAmount = time;
                time -= Time.deltaTime;
            }
            else
            {
                fillImage.fillAmount = 0;
            }
        }
    }
}