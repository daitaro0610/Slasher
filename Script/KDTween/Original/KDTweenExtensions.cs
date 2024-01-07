using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.Tweening.Plugin.Options;

namespace KD.Tweening
{
    public static class KDTweenExtensions
    {
        #region Transform
        public static KDTween<Vector3, VectorOptions> TweenMove(this Transform target, Vector3 endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValue,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector3, VectorOptions> TweenMoveX(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(endValue, target.position.y, target.position.z);
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.X,
                    snapping = false
                }
                );


        }
        public static KDTween<Vector3, VectorOptions> TweenMoveY(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.position.x, endValue, target.position.z);
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                }
                );

        }
        public static KDTween<Vector3, VectorOptions> TweenMoveZ(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.position.x, target.position.y, endValue);
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                }
                );


        }
        public static KDTween<Vector3, VectorOptions> TweenRotate(this Transform target, Vector3 endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => NormalizedAngle(target.eulerAngles),
                (x) => target.eulerAngles = x,
                NormalizedAngle(endValue),
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                }
                );

            static Vector3 NormalizedAngle(Vector3 angle)
            {
                return new Vector3(
                     Mathf.Repeat(angle.x + 180, 360) - 180,
                     Mathf.Repeat(angle.y + 180, 360) - 180,
                     Mathf.Repeat(angle.z + 180, 360) - 180
                    );
            }
        }
        public static KDTween<Vector3, VectorOptions> TweenScale(this Transform target, Vector3 endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.localScale,
                (x) => target.localScale = x,
                endValue,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                });
        }
        public static KDTween<Vector3, VectorOptions> TweenScaleX(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(endValue, target.localScale.y, target.localScale.z);
            return KDTweenManager.Instance.SetUp(
                () => target.localScale,
                (x) => target.localScale = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.X,
                    snapping = false
                });
        }
        public static KDTween<Vector3, VectorOptions> TweenScaleY(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.localScale.x, endValue, target.localScale.z);
            return KDTweenManager.Instance.SetUp(
                () => target.localScale,
                (x) => target.localScale = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                });
        }
        public static KDTween<Vector3, VectorOptions> TweenScaleZ(this Transform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.localScale.x, target.localScale.y, endValue);
            return KDTweenManager.Instance.SetUp(
                () => target.localScale,
                (x) => target.localScale = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Z,
                    snapping = false
                });
        }
        #endregion

        #region RectTransform
        public static KDTween<Vector2, VectorOptions> TweenAnchorMove(this RectTransform target, Vector2 endVale, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.anchoredPosition,
                (x) => target.anchoredPosition = x,
                endVale,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector2, VectorOptions> TweenAnchorMoveX(this RectTransform target, float endValue, float duration)
        {
            Vector2 endValueV2 = new Vector2(endValue, target.anchoredPosition.y);

            return KDTweenManager.Instance.SetUp(
                () => target.anchoredPosition,
                (x) => target.anchoredPosition = x,
                endValueV2,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.X,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector2, VectorOptions> TweenAnchorMoveY(this RectTransform target, float endValue, float duration)
        {
            Vector2 endValueV2 = new Vector2(target.anchoredPosition.x, endValue);

            return KDTweenManager.Instance.SetUp(
                 () => target.anchoredPosition,
                 (x) => target.anchoredPosition = x,
                 endValueV2,
                 duration,
                 new VectorOptions
                 {
                     axisConstraint = AxisConstraint.Y,
                     snapping = false
                 }
                 );

        }
        public static KDTween<Vector3, VectorOptions> TweenAnchor3DMove(this RectTransform target, Vector3 endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.anchoredPosition3D,
                (x) => target.anchoredPosition3D = x,
                endValue,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector3, VectorOptions> TweenAnchor3DMoveX(this RectTransform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(endValue, target.anchoredPosition3D.y, target.anchoredPosition3D.z);
            return KDTweenManager.Instance.SetUp(
                () => target.anchoredPosition3D,
                (x) => target.anchoredPosition3D = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.X,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector3, VectorOptions> TweenAnchor3DMoveY(this RectTransform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.anchoredPosition3D.x, endValue, target.anchoredPosition3D.z);
            return KDTweenManager.Instance.SetUp(
                () => target.anchoredPosition3D,
                (x) => target.anchoredPosition3D = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector3, VectorOptions> TweenAnchor3DMoveZ(this RectTransform target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.anchoredPosition3D.x, target.anchoredPosition3D.y, endValue);
            return KDTweenManager.Instance.SetUp(
                () => target.anchoredPosition3D,
                (x) => target.anchoredPosition3D = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                }
                );
        }

        public static KDTween<Vector2, VectorOptions> TweenSizeDelta(this RectTransform target, Vector2 endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.sizeDelta,
                (x) => target.sizeDelta = x,
                endValue,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                });
        }

        #endregion

        #region Rigidbody
        public static KDTween<Vector3, VectorOptions> TweenMove(this Rigidbody target, Vector3 endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValue,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.None,
                    snapping = false
                }
                );
        }
        public static KDTween<Vector3, VectorOptions> TweenMoveX(this Rigidbody target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(endValue, target.position.y, target.position.z);
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.X,
                    snapping = false
                }
                );


        }
        public static KDTween<Vector3, VectorOptions> TweenMoveY(this Rigidbody target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.position.x, endValue, target.position.z);
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                }
                );


        }
        public static KDTween<Vector3, VectorOptions> TweenMoveZ(this Rigidbody target, float endValue, float duration)
        {
            Vector3 endValueV3 = new Vector3(target.position.x, target.position.y, endValue);
            return KDTweenManager.Instance.SetUp(
                () => target.position,
                (x) => target.position = x,
                endValueV3,
                duration,
                new VectorOptions
                {
                    axisConstraint = AxisConstraint.Y,
                    snapping = false
                }
                );
        }
        #endregion

        #region Text
        public static KDTween<Color, ColorOptions> TweenColor(this Text target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenAlpha(this Text target, float endValue, float duration)
        {
            Color endValueColor = target.color;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region Image
        public static KDTween<Color, ColorOptions> TweenColor(this Image target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenAlpha(this Image target, float endValue, float duration)
        {
            Color endValueColor = target.color;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region SpriteRenderer
        public static KDTween<Color, ColorOptions> TweenColor(this SpriteRenderer target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenAlpha(this SpriteRenderer target, float endValue, float duration)
        {
            Color endValueColor = target.color;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region Material
        public static KDTween<Color, ColorOptions> TweenColor(this Material target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenAlpha(this Material target, float endValue, float duration)
        {
            Color endValueColor = target.color;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region Outline
        public static KDTween<Color, ColorOptions> TweenColor(this Outline target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.OutlineColor,
                (x) => target.OutlineColor = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenAlpha(this Outline target, float endValue, float duration)
        {
            Color endValueColor = target.OutlineColor;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.OutlineColor,
                (x) => target.OutlineColor = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region Graphic
        public static KDTween<Color, ColorOptions> TweenColor(this Graphic target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenAlpha(this Graphic target, float endValue, float duration)
        {
            Color endValueColor = target.color;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.color,
                (x) => target.color = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region LineRenderer
        public static KDTween<Color, ColorOptions> TweenStartColor(this LineRenderer target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.startColor,
                (x) => target.startColor = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenEndColor(this LineRenderer target, Color endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.endColor,
                (x) => target.endColor = x,
                endValue,
                duration,
                new ColorOptions
                {
                    alphaOnly = false
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenStartAlpha(this LineRenderer target, float endValue, float duration)
        {
            Color endValueColor = target.startColor;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.startColor,
                (x) => target.startColor = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        public static KDTween<Color, ColorOptions> TweenEndAlpha(this LineRenderer target, float endValue, float duration)
        {
            Color endValueColor = target.endColor;
            endValueColor.a = endValue;

            return KDTweenManager.Instance.SetUp(
                () => target.endColor,
                (x) => target.endColor = x,
                endValueColor,
                duration,
                new ColorOptions
                {
                    alphaOnly = true
                }
                );
        }
        #endregion

        #region ScrollRect
        public static KDTween<float, FloatOptions> TweenHorizontalNormalizedPos(this ScrollRect target, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.horizontalNormalizedPosition,
                (x) => target.horizontalNormalizedPosition = x,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                }
                );
        }
        public static KDTween<float, FloatOptions> TweenVerticalNormalizedPos(this ScrollRect target, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.verticalNormalizedPosition,
                (x) => target.verticalNormalizedPosition = x,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                }
                );
        }
        #endregion

        #region Slider
        public static KDTween<float, FloatOptions> TweenValue(this Slider target, float endValue, float duration)
        {
            return KDTweenManager.Instance.SetUp(
                () => target.value,
                (x) => target.value = x,
                endValue,
                duration,
                new FloatOptions
                {
                    snapping = false
                });
        }
        #endregion

    }
}