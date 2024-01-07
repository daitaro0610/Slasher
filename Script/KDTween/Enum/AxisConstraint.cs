using System;

#pragma warning disable 1591
namespace KD.Tweening
{
    [Flags]
    public enum AxisConstraint
    {
        None = 0,
        X = 2,
        Y = 4,
        Z = 8,
        W = 16,
        XY = 32
    }
}