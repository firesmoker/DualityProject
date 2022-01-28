using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public enum Polarity
    {
        Red,
        Blue
    }

    public static class Polarities
    {
        public static Polarity Flip(this Polarity polarity)
        {
            if (polarity == Polarity.Blue)
                return Polarity.Red;
            else
                return Polarity.Blue;
        }

        public static float EvaluateByPolarity(this AnimationCurve curve, float x, Polarity polarity)
        {
            if (polarity == Polarity.Blue)
                return curve.Evaluate(x);
            else
                return 1 - curve.Evaluate(1 - x);
        }
    }
}
