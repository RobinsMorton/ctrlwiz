namespace XInputDotNetPure
{
    public struct GamePadButtons
    {
        internal GamePadButtons(
            ButtonState start,
            ButtonState back,
            ButtonState leftStick,
            ButtonState rightStick,
            ButtonState leftShoulder,
            ButtonState rightShoulder, 
            ButtonState guide, 
            ButtonState a, 
            ButtonState b, 
            ButtonState x, 
            ButtonState y)
        {
            Start = start;
            Back = back;
            LeftStick = leftStick;
            RightStick = rightStick;
            LeftShoulder = leftShoulder;
            RightShoulder = rightShoulder;
            Guide = guide;
            A = a;
            B = b;
            X = x;
            Y = y;
        }

        public ButtonState Start { get; }

        public ButtonState Back { get; }

        public ButtonState LeftStick { get; }

        public ButtonState RightStick { get; }

        public ButtonState LeftShoulder { get; }

        public ButtonState RightShoulder { get; }

        public ButtonState Guide { get; }

        public ButtonState A { get; }

        public ButtonState B { get; }

        public ButtonState X { get; }

        public ButtonState Y { get; }
    }
}