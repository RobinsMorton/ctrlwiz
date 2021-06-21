using PaddleSDK.Product;
using System;

namespace NVSXBOX
{
    public class ActivationChangedArgs : EventArgs
    {
        public bool IsActivated { get; set; }
        public VerificationState State { get; set; }
    }
}
