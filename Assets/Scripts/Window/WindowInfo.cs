using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInfo : MonoBehaviour
{
    [SerializeField] CanvasGroup Background;
    [SerializeField] CanvasGroup BasicInfo;

    protected IEnumerator<WaitForEndOfFrame> SetBackground(float setAlpha, float speed = 2f)
    {
        if (Background != null)
        {
            Background.gameObject.SetActive(true);
            
            while (Background.alpha != setAlpha)
            {
                Background.alpha = CustomMethods.AnimParameter(Background.alpha, setAlpha, speed);
                yield return new WaitForEndOfFrame();
            }

            if (setAlpha == 0)
                Background.gameObject.SetActive(false);
        }
    }

    protected IEnumerator<WaitForEndOfFrame> SetBasicInfo(float setAlpha, float speed = 2f)
    {
        if (BasicInfo != null)
        {
            BasicInfo.gameObject.SetActive(true);

            while (BasicInfo.alpha != setAlpha)
            {
                BasicInfo.alpha = CustomMethods.AnimParameter(BasicInfo.alpha, setAlpha, speed);
                yield return new WaitForEndOfFrame();
            }

            if (setAlpha == 0)
                BasicInfo.gameObject.SetActive(false);
        }
    }

  
}
