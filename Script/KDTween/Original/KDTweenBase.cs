using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin;

namespace KD.Tweening
{
    public abstract class KDTweenBase<TValue, TPlugOptions> : IKDTween where TPlugOptions : struct, IPlugOptions
    {
        internal KDGetter<TValue> Getter;
        internal KDSetter<TValue> Setter;
        internal TValue StartValue;
        internal TValue EndValue;
        internal TValue ChangeValue;

        protected TPlugOptions m_Options;
        protected KDTweenPlugin<TValue, TPlugOptions> m_Plugin;
        protected ITweenPlugin m_IPlugin;

        //CallBack
        public KDTweenCallback onPlay;
        public KDTweenCallback onStart;
        public KDTweenCallback onUpdate;
        public KDTweenCallback onComplete;
        public KDTweenCallback onStepComplete;
        public KDTweenCallback onKill;

        internal bool IsPause = false;
        internal bool IsComplete = false;
        internal bool IsRelative;


        internal bool IsUnscaled = false;

        internal GameObject LinkObject;
        internal bool IsLink;



        internal virtual void Init()
        {

            IsPause = false;
            IsComplete = false;
            IsRelative = false;
            IsLink = false;
            LinkObject = null;
            onPlay = onStart = onUpdate = onStepComplete = onKill = onComplete = null;
            Delay = 0;
            LoopCount = 0;
            Easing = Ease.Linear;

            m_IPlugin = PluginManager.CreatePlugin<TValue, TPlugOptions>();

            m_Plugin = m_IPlugin as KDTweenPlugin<TValue, TPlugOptions>;
            m_Plugin.Init(this);
        }

        internal override void StartValueUpdate()
        {
            StartValue = Getter();
            m_Plugin.Init(this);
        }

        internal override void Update(float deltaTime) { }
    }
}