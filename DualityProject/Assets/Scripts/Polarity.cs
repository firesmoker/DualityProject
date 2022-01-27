using System;

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
    }
}
