using UnityEngine;
using KD.Tweening.Plugin;

namespace KD.Tweening
{
    public abstract class IKDTween
    {
        internal Ease Easing;
        internal UpdateType UpdateType;

        internal float Delay;

        //Loop
        internal int LoopCount;
        internal LoopType LoopType;

        //Parameter
        internal float Duration;
        internal float OvershootOrAmplitude;
        internal float Period;

        internal KDSequence SequenceParent;
        internal bool IsSequence = false;

        internal abstract void StartValueUpdate();

       internal abstract void Update(float deltaTime);
    }
}
