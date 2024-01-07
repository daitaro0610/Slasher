using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening.Plugin
{
    public static class PluginManager
    {
        /// <summary>
        /// Type�ɉ������v���O�C���𐶐�����
        /// </summary>
        public static ITweenPlugin CreatePlugin<TValue, TPlugOptions>() where TPlugOptions : struct, IPlugOptions
        {
            if (typeof(float) == typeof(TValue))
            {
                return new KDFloatPlugin();
            }
            else if (typeof(Vector2) == typeof(TValue))
            {
                return new KDVector2Plugin();
            }
            else if (typeof(Vector3) == typeof(TValue))
            {
                if (typeof(ShakeOptions) == typeof(TPlugOptions))
                    return new KDShakePlugin();
                else return new KDVector3Plugin();
            }
            else if (typeof(Color) == typeof(TValue))
            {
                return new KDColorPlugin();
            }
            else if (typeof(Quaternion) == typeof(TValue))
            {
                return new KDQuaternionPlugin();
            }
            else
            {
                Debug.LogError($"�s���ȃ^�C�v�ł�{typeof(TValue).Name}");
                return null;
            }
        }
    }
}