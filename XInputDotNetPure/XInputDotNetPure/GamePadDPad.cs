namespace XInputDotNetPure
{
    public struct GamePadDPad
    {
        internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }

        public ButtonState Up { get; }

        public ButtonState Down { get; }

        public ButtonState Left { get; }

        public ButtonState Right { get; }
    }
}