# 1€ Filter for Quaternions - Unity Implementation

## About
The "1€ Filter" is a low-pass filter for real-time noisy signals, as described in the [CHI 2012 Paper](https://gery.casiez.net/publications/CHI2012-casiez.pdf) by Géry Casiez, Nicolas Roussel, and Daniel Vogel.

I implemented this filter in Unity and modified it to filter quaternions for use in an existing project. The Unity implementation builds upon the [C# implementation](https://gery.casiez.net/1euro/OneEuroFilter.cs) by Mitsuru Furuta.

## Usage
The **OneEuroFilterQuat.cs** file contains a C# class for the 1€ Filter. Copy the file to the Assets folder in your Unity project to use it.

### Instantiating the filter
Creating the *OneEuroFilterQuat* object requires two parameters:
1. Minumum cutoff frequency
2. Beta (speed coeffcient)

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    OneEuroFilterQuat rotationFilter;
    float fcmin; //minimum cutoff frequency
    float beta; //speed coefficient

    void Start()
    {
        //setting the parameters
        fcmin = 1.0f;
        beta = 0.5f;

        //instantiating the filter
        rotationFilter = new OneEuroFilterQuat(fcmin,beta); 
    }
```

### Using the filter
To filter quaternions, use the **Filter** method, which requires two arguments:
1. The quaternion you want to filter
2. Sampling period (time difference between the current and previous sample of the input signal)
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    OneEuroFilterQuat rotationFilter;
    float fcmin; //minimum cutoff frequency
    float beta; //speed coefficient
    
    Quaternion quat; //noisy input quaternion
    Quaternion filteredQuat; //filtered quaternion
    float dt; //sampling period

    void Start()
    {
        //setting the parameters
        fcmin = 1.0f;
        beta = 0.5f;

        //instantiating the filter
        rotationFilter = new OneEuroFilterQuat(fcmin,beta); 
    }

    void Update()
    {
        //random quaternion created each frame
        quat = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        
        dt = Time.deltaTime;

        //filter the quaternion
        filteredQuat = rotationFilter.Filter(quat, dt); 
    }
```
