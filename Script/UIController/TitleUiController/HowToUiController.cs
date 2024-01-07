using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToUiController : UiControllerBase
{
    [SerializeField]
    private GraphicVisibilityController m_BackGround;

    public override void OnStart()
    {
        m_BackGround.SetFadeActive(true);
    }

    public override void OnReset()
    {
        m_BackGround.SetFadeActive(false);
    }
}
