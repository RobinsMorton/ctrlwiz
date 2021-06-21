using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PaddleSDK;
using PaddleSDK.Checkout;
using PaddleSDK.Product;

namespace NVSXBOX
{
    public class InAppCheckout : IDisposable
    {
        // Your Paddle SDK Config from the Vendor Dashboard
        private static readonly string vendorId = "104612";
        private readonly string productId = "576239";
        //private static string productId = "585987";
        private static readonly string apiKey = "32f0fa8cbb119f39b7dc72f5479abc2d";
        private static InAppCheckoutStatus status = InAppCheckoutStatus.Default;
        private static bool isRunning = false;

        private static readonly PaddleProductConfig productInfo = new PaddleProductConfig { ProductName = InAppCheckout.ProductName, VendorName = InAppCheckout.VendorName };
        private static readonly string tempFileName = Path.Combine(Path.GetTempPath(), "EE42E371-CA4D-4428-87A9-113AF9E50063");

        private static PaddleProduct product = PaddleProduct.CreateProduct(ProductId, ProductType.SDKProduct, productInfo);

        public bool IsProductActivated { get; private set; }
        public bool IsAccessible { get; private set; }
        public static string ProductId => "576239";
        public static string ProductName => "CtrlWiz - Xbox Controller for Navis";
        public static string VendorId => "104612";
        public static string VendorName => "VIATechnik";

        private string ApiKey => "32f0fa8cbb119f39b7dc72f5479abc2d";

        public event EventHandler<ActivationChangedArgs> ActivationChanged;
        public event EventHandler<InAppCheckoutArgs> VerificationCompleteEvent;
        public event EventHandler<InAppCheckoutArgs> InAppCheckoutCompleteEvent;


        public PaddleProduct GetProduct { get { return product; } private set { } }

        public void StartPaddle()
        {
            // Default Product Config in case we're unable to reach our servers on first run
            try
            {
                // Initialize the SDK singleton with the config
                Paddle.Configure(apiKey, vendorId, productId, productInfo);

                // Set up events for Checkout
                Paddle.Instance.TransactionCompleteEvent += PaddleCheckoutForm_TransactionCompleteEvent;
                Paddle.Instance.TransactionErrorEvent += PaddleCheckoutForm_TransactionErrorEvent;
                Paddle.Instance.TransactionBeginEvent += PaddleCheckoutForm_TransactionBeginEvent;


                // Ask the Product to get it's latest state and info from the Paddle Platform
                product.Refresh((success) =>
                {
                    // product data was successfully refreshed
                    if (success)
                    {
                        if (!product.Activated)
                        {
                            // Product is not activated, so let's show the Product Access dialog to gatekeep your app
                            Paddle.Instance.ShowProductAccessWindowForProduct(product);
                        }
                        else
                        {
                            product.VerifyActivation((state, errorMessage) =>
                            {
                                if (state != VerificationState.Verified)
                                {
                                    product.Deactivate((isActive, msg) => { });
                                    ActivationChanged?.Invoke(this, new ActivationChangedArgs() { IsActivated = false });
                                };
                            });
                        }
                    }
                    else
                    {
                        // The SDK was unable to get the last info from the Paddle Platform.
                        // We can show the Product Access dialog with the data provided in the PaddleProductConfig object.
                        Paddle.Instance.ShowProductAccessWindowForProduct(product);
                    }
                });
                IsProductActivated = product.Activated;
                ActivationChanged?.Invoke(this, new ActivationChangedArgs() { IsActivated = IsProductActivated });

                using (FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate))
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(product.Activated);
                }
            }
            catch (Exception)
            {
                if (File.Exists(tempFileName))
                {
                    using (FileStream fileStream = new FileStream(tempFileName, FileMode.Open))
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        IsProductActivated = binaryReader.ReadBoolean();
                    }
                }
                if (!IsProductActivated)
                    MessageBox.Show("Unable to verify license.\nPlease check your internet connection.", "Connection Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        //public async Task<bool> StartPaddle1()
        //{
        //    //this.VerificationCompletEvent += InAppCheckout_VerificationCompletEvent;
        //    //LicenseVerification();
        //    isRunning = true;
        //    while (isRunning)
        //    {
        //        if (status == InAppCheckoutStatus.Default)
        //        {
        //            status = InAppCheckoutStatus.Pending;
        //            LicenseActivation();
        //        }
        //        await Task.Delay(100);
        //    }
        //    status = InAppCheckoutStatus.Default;
        //    return false;
        //}

        //private void InAppCheckout_VerificationCompletEvent(object sender, InAppCheckoutArgs e)
        //{
        //    if (e.State == VerificationState.Unverified)
        //    {
        //        product.Deactivate((isActive, msg) => { });
        //        IsAccessible = false;
        //        //InAppCheckoutCompleteEvent?.Invoke(this, new InAppCheckoutArgs() { IsAccessible = false, State = e.State });
        //    }
        //    IsAccessible = true;
        //    isRunning = false;
            
        //}

        public void ShowLicenseDetailsWindow()
        {
            if (product == null && IsProductActivated == true)
            {
                ActivationChanged?.Invoke(this, new ActivationChangedArgs() { IsActivated = false });
            }

            try
            {
                product.Refresh((success) =>
                {
                    // product data was successfully refreshed
                    if (success)
                    {
                        Paddle.Instance.ShowLicenseActivationWindowForProduct(product);
                        ActivationChanged?.Invoke(this, new ActivationChangedArgs() { IsActivated = false });
                    }
                });
            }
            catch (Exception)
            {
                // The SDK was unable to get the last info from the Paddle Platform.
                MessageBox.Show("Unable to get license details.\nPlease check your internet connection.", "Connection Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void PaddleCheckoutForm_TransactionBeginEvent(object sender, TransactionBeginEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PaddleCheckoutForm_TransactionErrorEvent(object sender, TransactionErrorEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void PaddleCheckoutForm_TransactionCompleteEvent(object sender, TransactionCompleteEventArgs e)
        {
            //LicenseVerification();
            //throw new NotImplementedException();
        }

        //public void LicenseActivation()
        //{
        //    ConfigurePaddle();
        //    // Ask the Product to get it's latest state and info from the Paddle Platform
        //    product.Refresh((success) =>
        //    {
        //        // product data was successfully refreshed
        //        if (success)
        //        {
        //            if (product.Activated)
        //            {
        //                this.VerificationCompleteEvent += InAppCheckout_VerificationCompletEvent;
        //                LicenseVerification();
        //            }
        //            else
        //            {
        //                // Product is not activated, so let's show the Product Access dialog to gatekeep your app
        //                Paddle.Instance.ShowProductAccessWindowForProduct(product);
        //                isRunning = false;
        //            }
        //        }
        //        else
        //        {
        //            // The SDK was unable to get the last info from the Paddle Platform.
        //            // We can show the Product Access dialog with the data provided in the PaddleProductConfig object.
        //            Paddle.Instance.ShowProductAccessWindowForProduct(product);
        //            isRunning = false;
        //        }
        //    });
        //}

        //public void LicenseVerification()
        //{
        //    product.VerifyActivation((verificationState, errorMessage) =>
        //    {
        //        this.VerificationCompleteEvent?.Invoke(this, new InAppCheckoutArgs() { IsAccessible = false, State = verificationState });
        //    });
        //}

        //private void ConfigurePaddle()
        //{
        //    // Initialize the SDK singleton with the config
        //    Paddle.Configure(ApiKey, VendorId, ProductId, productInfo);
        //    // Set up events for Checkout
        //    Paddle.Instance.TransactionCompleteEvent += PaddleCheckoutForm_TransactionCompleteEvent;
        //    Paddle.Instance.TransactionErrorEvent += PaddleCheckoutForm_TransactionErrorEvent;
        //    Paddle.Instance.TransactionBeginEvent += PaddleCheckoutForm_TransactionBeginEvent;
        //}

        public void Dispose()
        {
            Paddle.Instance.TransactionCompleteEvent -= PaddleCheckoutForm_TransactionCompleteEvent;
            Paddle.Instance.TransactionErrorEvent -= PaddleCheckoutForm_TransactionErrorEvent;
            Paddle.Instance.TransactionBeginEvent -= PaddleCheckoutForm_TransactionBeginEvent;
        }
    }
}
