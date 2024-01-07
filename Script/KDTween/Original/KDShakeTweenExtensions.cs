using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening
{
    public static class KDShakeTweenExtensions 
    {
        #region Transform

        private const float RANDOM_MAX_VALUE = 100f;

        public static Vector3 RandomVector3Value()
        {
            return new Vector3(
                Random.Range(0, RANDOM_MAX_VALUE),
                Random.Range(0, RANDOM_MAX_VALUE),
                Random.Range(0, RANDOM_MAX_VALUE));
        }

        /// <summary>
        /// ���ׂĂ̕����ɃV�F�C�N����
        /// </summary>
        /// <param name="target">�����������I�u�W�F�N�g</param>
        /// <param name="strength">�U���̋���</param>
        /// <param name="vibrato">�U���̕��@(��)</param>
        /// <param name="duration">����������</param>
        /// <param name="fadeOut">���X�ɗh���}���邩�ǂ���</param>
        /// <param name="randomOffset">�����_���l�̃V�[�h</param>
        /// <returns></returns>
        public static KDShakeTween<Vector3,ShakeOptions> TweenShakePosition3D(this Transform target,float strength, float vibrato,float duration,bool fadeOut = true,Vector3? randomOffset = null)
        {
            if (randomOffset == null)
                randomOffset = RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.None,
                    fadeOut = fadeOut
                });
        }

        /// <summary>
        /// �c�������ɃV�F�C�N����
        /// </summary>
        /// <param name="target">�����������I�u�W�F�N�g</param>
        /// <param name="strength">�U���̋���</param>
        /// <param name="vibrato">�U���̕��@(��)</param>
        /// <param name="duration">����������</param>
        /// <param name="fadeOut">���X�ɗh���}���邩�ǂ���</param>
        /// <param name="randomOffset">�����_���l�̃V�[�h</param>
        /// <returns></returns>
        public static KDShakeTween<Vector3, ShakeOptions> TweenShakePosition2D(this Transform target, float strength, float vibrato, float duration, bool fadeOut = true, Vector3? randomOffset = null)
        {
            if (randomOffset == null)
                randomOffset = RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.XY,
                    fadeOut = fadeOut
                });
        }

        /// <summary>
        /// X�������̂݃V�F�C�N����
        /// </summary>
        /// <param name="target">�����������I�u�W�F�N�g</param>
        /// <param name="strength">�U���̋���</param>
        /// <param name="vibrato">�U���̕��@(��)</param>
        /// <param name="duration">����������</param>
        /// <param name="fadeOut">���X�ɗh���}���邩�ǂ���</param>
        /// <param name="randomOffset">�����_���l�̃V�[�h</param>
        /// <returns></returns>
        public static KDShakeTween<Vector3, ShakeOptions> TweenShakePositionX(this Transform target, float strength, float vibrato, float duration, bool fadeOut = true, Vector3? randomOffset = null)
        {
            if (randomOffset == null)
                randomOffset = RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.X,
                    fadeOut = fadeOut
                });
        }

        /// <summary>
        /// Y�������̂݃V�F�C�N����
        /// </summary>
        /// <param name="target">�����������I�u�W�F�N�g</param>
        /// <param name="strength">�U���̋���</param>
        /// <param name="vibrato">�U���̕��@(��)</param>
        /// <param name="duration">����������</param>
        /// <param name="fadeOut">���X�ɗh���}���邩�ǂ���</param>
        /// <param name="randomOffset">�����_���l�̃V�[�h</param>
        /// <returns></returns>
        public static KDShakeTween<Vector3, ShakeOptions> TweenShakePositionY(this Transform target, float strength, float vibrato, float duration, bool fadeOut = true, Vector3? randomOffset = null)
        {
            if (randomOffset == null)
                randomOffset = RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    fadeOut = fadeOut
                });
        }

        /// <summary>
        /// Z�������̂݃V�F�C�N����
        /// </summary>
        /// <param name="target">�����������I�u�W�F�N�g</param>
        /// <param name="strength">�U���̋���</param>
        /// <param name="vibrato">�U���̕��@(��)</param>
        /// <param name="duration">����������</param>
        /// <param name="fadeOut">���X�ɗh���}���邩�ǂ���</param>
        /// <param name="randomOffset">�����_���l�̃V�[�h</param>
        /// <returns></returns>
        public static KDShakeTween<Vector3, ShakeOptions> TweenShakePositionZ(this Transform target, float strength, float vibrato, float duration, bool fadeOut = true, Vector3? randomOffset = null)
        {
            if (randomOffset == null)
                randomOffset = RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.Z,
                    fadeOut = fadeOut
                });
        }

        #endregion

        #region Vector

        /// <summary>
        /// ���ׂĂ̕����ɃV�F�C�N����
        /// </summary>
        /// <param name="target">�����������I�u�W�F�N�g</param>
        /// <param name="strength">�U���̋���</param>
        /// <param name="vibrato">�U���̕��@(��)</param>
        /// <param name="duration">����������</param>
        /// <param name="fadeOut">���X�ɗh���}���邩�ǂ���</param>
        /// <param name="randomOffset">�����_���l�̃V�[�h</param>
        /// <returns></returns>
        public static KDShakeTween<Vector3, ShakeOptions> TweenShakePosition3D(this Vector3 target, float strength, float vibrato, float duration, bool fadeOut = true, Vector3? randomOffset = null)
        {
            if (randomOffset == null)
                randomOffset = RandomVector3Value();

            return KDTweenManager.Instance.SetUp(
                () => target,
                (x) => target = x,
                strength,
                vibrato,
                (Vector3)randomOffset,
                duration,
                new ShakeOptions
                {
                    axisConstraint = AxisConstraint.None,
                    fadeOut = fadeOut
                });
        }

        #endregion
    }

}