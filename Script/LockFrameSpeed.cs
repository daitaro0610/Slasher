using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFrameSpeed : MonoBehaviour
{
    private void Awake()
    {   
        //フレームレートを60に固定する
        Application.targetFrameRate = 60;
    }
}
