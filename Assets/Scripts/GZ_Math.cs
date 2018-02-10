using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GZ_Math
{

    public static IEnumerator PlayAnim_CR(float playTime, System.Action<float> process, System.Action callback)
    {
        float timer = 0;

        while (timer < playTime)
        {
            timer += Time.deltaTime;
            process(timer / playTime);
            yield return null;
        }

        callback();
    }

    public static  IEnumerator WaitFor_CR(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
    public static float GetDegreeBtwVectors(Vector3 aVec, Vector3 bVec, Vector3 upVector)
    {
        var dirNum = GZ_Math.GetAngleDir(aVec, bVec, upVector);
        float angle = Mathf.Acos(Vector2.Dot(aVec.normalized, bVec.normalized)) * Mathf.Rad2Deg;

        if (dirNum == 1)
            angle = 360 - angle;

        return angle;
    }
    
    public static float GetAngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}

