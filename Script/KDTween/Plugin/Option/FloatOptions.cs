using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.Tweening.Plugin.Options
{
    public struct FloatOptions : IPlugOptions
    {
        public bool snapping;

        public void Reset()
        {
            snapping = false;
        }
    }
}