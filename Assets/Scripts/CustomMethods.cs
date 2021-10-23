using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CustomMethods
{
    public static float AnimParameter(float nowPar, float setPar, float speed)
    {
        if (nowPar > setPar)
        {
            nowPar -= speed * Time.deltaTime;
            if (nowPar < setPar)
                nowPar = setPar;
        }

        if (nowPar < setPar)
        {
            nowPar += speed * Time.deltaTime;
            if (nowPar > setPar)
                nowPar = setPar;
        }

        return nowPar;
    }
}
