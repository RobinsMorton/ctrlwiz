namespace XInputDotNetPure
{
    public struct GamePadTriggers
    {
        internal GamePadTriggers(float left, float right)
        {
            Left = left;
            Right = right;
        }

        public float Left { get; }

        public float Right { get; }
    }
}