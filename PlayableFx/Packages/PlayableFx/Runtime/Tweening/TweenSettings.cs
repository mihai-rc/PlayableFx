using System;

namespace PlayableFx
{
    [Serializable]
    public struct TweenSettings
    {
        public TweenConfig PositionConfig;
        public TweenConfig RotationConfig;
        public TweenConfig ScaleConfig;
    }
}