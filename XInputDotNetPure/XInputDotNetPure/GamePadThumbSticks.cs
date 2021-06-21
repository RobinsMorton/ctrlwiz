namespace XInputDotNetPure
{

    public struct GamePadThumbSticks
    {
        public struct StickValue
        {
            internal StickValue(float x, float y)
            {
                X = x;
                Y = y;
            }

            public float X { get; }

            public float Y { get; }
        }

        internal GamePadThumbSticks(StickValue left, StickValue right)
        {
            Left = left;
            Right = right;
        }

        public StickValue Left { get; }

        public StickValue Right { get; }
    }
}