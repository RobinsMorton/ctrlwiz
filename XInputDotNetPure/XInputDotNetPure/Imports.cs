using System.Runtime.InteropServices;

namespace XInputDotNetPure
{
    class Imports
    {
        internal const string DLLName = "XInputInterface";

        [DllImport(DLLName)]
        public static extern uint XInputGamePadGetState(uint playerIndex, out GamePadState.RawState state);

        [DllImport(DLLName)]
        public static extern void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor);
    }
}