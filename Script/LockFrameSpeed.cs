using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFrameSpeed : MonoBehaviour
{
    private void Awake()
    {   
        //�t���[�����[�g��60�ɌŒ肷��
        Application.targetFrameRate = 60;
    }
}
