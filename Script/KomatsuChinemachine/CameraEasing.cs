using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEasing
{
    public enum Easing
    {
        Cut,
        Liner,
        EaseIn,
        EaseOut,
        EaseInOut,
    }
    private Easing m_Easing;
    public Easing CurrentEasing => m_Easing;

    public CameraEasing()
    {

    }   
}
