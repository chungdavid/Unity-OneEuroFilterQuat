/*
C# implementation of the One Euro Filter for quaternions.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneEuroFilterQuat
{
    protected bool firstTime;
    protected float minCutoff;
    protected float beta;
    protected LowPassFilterQuat xFilt;
    protected LowPassFilterQuat dxFilt;
    protected float dcutoff;

    public OneEuroFilterQuat(float _minCutoff, float _beta)
    {
        firstTime = true;
        minCutoff = _minCutoff;
        beta = _beta;

        xFilt = new LowPassFilterQuat();
        dxFilt = new LowPassFilterQuat();
        dcutoff = 1.0f;
    }

    public Quaternion Filter(Quaternion x, float dt)
    {
        Quaternion dx = Quaternion.identity;
        if (firstTime)
        {
            firstTime = false;
        }
        else
        {
            QuatFilterable.computeDerivative(dx, xFilt.Last(), x, dt);
        }

        Quaternion edx = dxFilt.Filter(dx, Alpha(dt, dcutoff));
        float dxMag = QuatFilterable.computeDerivativeMagnitude(edx);
        float cutoff = minCutoff + beta * dxMag;

        return xFilt.Filter(x, Alpha(dt, cutoff));
    }

    protected float Alpha(float dt, float cutoff)
    {
        float tau = 1.0f / (2 * Mathf.PI * cutoff);
        return 1.0f / (1.0f + tau / dt);
    }
}

public class LowPassFilterQuat
{
    protected bool firstTime;
    protected Quaternion hatXPrev;

    public LowPassFilterQuat()
    {
        firstTime = true;
    }

    public Quaternion Last()
    {
        return hatXPrev;
    }

    public Quaternion Filter(Quaternion x, float alpha)
    {
        Quaternion hatX;
        if (firstTime)
        {
            firstTime = false;
            hatX = x;
        }
        else
        {
            hatX = Quaternion.Slerp(hatXPrev, x, alpha);
        }

        
        hatXPrev = hatX;
        return hatX;
    }
}

public class QuatFilterable
{
    //compute the derivative of the quaternion
    public static void computeDerivative(Quaternion dx, Quaternion prev, Quaternion current, float dt)
    {
        float rate = 1.0f / dt;

        Quaternion inversePrev = new Quaternion(0, 0, 0, 1); 
        q_invert(inversePrev, prev); //obtain the multiplicative inverse of the previous filtered quaternion
        q_mult(dx, current, inversePrev);

        // nlerp instead of slerp
        dx[0] *= rate;
        dx[1] *= rate;
        dx[2] *= rate;
        dx[3] = dx[3] * rate + (1.0f - rate);
        dx = dx.normalized;

        //using slerp
        //dx = Quaternion.Slerp(Quaternion.identity, dx, rate);
    }

    public static float computeDerivativeMagnitude(Quaternion dx)
    {
        return 2.0f * Mathf.Acos(dx[3]);
    }


    //---------quaternion functions---------------------
    //this function copies the mutliplicative inverse of the quaternion from source to dest
    public static void q_invert(Quaternion destQuat, Quaternion srcQuat)
    {
        float srcQuatNorm = 1.0f / (srcQuat[0] * srcQuat[0] + srcQuat[1] * srcQuat[1] + srcQuat[2] * srcQuat[2] + srcQuat[3] * srcQuat[3]);
        float x = -srcQuat[0] * srcQuatNorm;
        float y = -srcQuat[1] * srcQuatNorm;
        float z = -srcQuat[2] * srcQuatNorm;
        float w = srcQuat[3] * srcQuatNorm;

        destQuat.Set(x,y,z,w);
    }

    public static void q_mult(Quaternion destQuat, Quaternion qLeft, Quaternion qRight)
    {
        destQuat = qLeft * qRight;
    } 
}