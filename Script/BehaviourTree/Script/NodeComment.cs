using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class NodeComment : ScriptableObject
{
    [TextArea] public string commentTitle = "New NodeComment";
    [TextArea] public string description = "New Comment";
    [HideInInspector] public string guid;
    [HideInInspector] public Vector3 position;

    private Vector2Int minSize = Vector2Int.one * 200;
    public Vector2Int size = Vector2Int.one* 200;
    [HideInInspector] public Vector2Int currentSize;

    public Action onDataChanged;

    public void NotifiData()
    {
        SizeCheck();

        onDataChanged?.Invoke();
    }

    private void SizeCheck()
    {
        //ƒTƒCƒY’²®
        if (minSize.x > size.x)
        {
            size.x = minSize.x;
        }
        if (minSize.y > size.y)
        {
            size.y = minSize.y;
        }

        currentSize = size;
    }
}
