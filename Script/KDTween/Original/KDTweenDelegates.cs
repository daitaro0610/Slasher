using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.Tweening
{
    public delegate T KDGetter<out T>();

    public delegate void KDSetter<in T>(T pNewValue);



    public delegate void KDTweenCallback();

    public delegate void KDTweenCallback<in T>(T value);

    public delegate float EaseFunction(float time, float duration, float overshootOrAmplitude, float period);
}