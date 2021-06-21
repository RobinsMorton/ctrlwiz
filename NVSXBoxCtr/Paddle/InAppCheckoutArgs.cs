using PaddleSDK.Product;
using System;

namespace NVSXBOX
{
    public class InAppCheckoutArgs : EventArgs
    {
        public bool IsAccessible { get; set; }
        public VerificationState State { get; set; }
    }
}
