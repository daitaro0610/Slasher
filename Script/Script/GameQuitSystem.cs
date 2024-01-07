using UnityEngine;

public class GameQuitSystem
{
    public void Update()
    {
        //Escが押された時
        if (Input.GetKey(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}
