using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;

//using Autodesk.Navisworks.Api.DocumentParts;
//using Autodesk.Navisworks.Api.Clash;
using XInputDotNetPure;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using System.IO;
//using System.Numerics;
//using System.Windows.Forms;

//using COMBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
//using COMApi = Autodesk.Navisworks.Api.Interop.ComApi;

using NvwApplication = Autodesk.Navisworks.Api.Application;
using System.Threading.Tasks;
using Autodesk.Navisworks.Api.DocumentParts;
using System.Diagnostics;
using Autodesk.Windows;

namespace NVSXBOX
{

    #region Create Ribbon
    [Plugin("NVSXBOX.CmdViewpoint", "VIATechnik", DisplayName = "CtrlWiz")]
    [Strings("CustomRibbon.name")]
    [RibbonLayout("CustomRibbon.xaml")]
    [RibbonTab("ID_CustomTab_1", DisplayName = "CtrlWiz")]
    [Command("ID_Button_1", CanToggle = true, DisplayName = " Activate\n Controller ", ExtendedToolTip = "", Icon = "XboxIcon.ico", LargeIcon = "XboxIcon.ico", ToolTip = "")]
    [Command("ID_Button_2", DisplayName = " Controller\n Map ", ExtendedToolTip = "", Icon = "Help28.png", LargeIcon = "Help28.png", ToolTip = "")]
    [Command("ID_Button_3", DisplayName = " Feature\n  Request  ", ExtendedToolTip = "", Icon = "Idea.png", LargeIcon = "Idea.png", ToolTip = "")]
    [Command("ID_Button_4", DisplayName = " Speed\n  Setting  ", ExtendedToolTip = "", Icon = "Setting.ico", LargeIcon = "Setting.ico", ToolTip = "")]
    [Command("ID_Button_5", DisplayName = " License ", ExtendedToolTip = "", Icon = "License.ico", LargeIcon = "License.ico", ToolTip = "", LoadForCanExecute = true)]

    #endregion

    public class CmdViewpoint : CommandHandlerPlugin
    {
        private static bool isProductActivated = false;
        private static InAppCheckout inAppCheckout = null;

        protected static DateTime MaxDate;
        protected static int KeyLock;
        //protected static bool isConnectInternet = false;

        private GamePadState state;
        private GamePadState prevState;
        private bool isrun = false;
        private double currentZ;

        public static Document oDoc;
        Viewpoint oCurrentViewCopy;
        //Document FPCdoc;


        private bool isMoving = false;
        private bool isRotating = false;

        //System.Numerics.Vector2 mv;
        //FormDebug formDebug;
        //COMApi.InwOpState10 m_state = null;
        //COMApi.InwOpClashElement m_clash = null;

        //public static string textDebug="abc";
        //FormDebug formDebug;
        //private string FPCPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"XRVinhSphere.fbx";

        #region declare FPS 
        private static long lastTime = System.Environment.TickCount;
        private static int fps = 1;
        private static int frames;
        private static float deltaTime = 0.005f;



        private static Stopwatch sw;
        private static long TimeDeltatime;
        private double Speed = 1;
        private static double ConstantUnit;
        public static double SpeedSetting = 4.00;
        public static double AngularSetting = 45.00;

        private bool firsttime = false;

        #endregion

        private int IDCurrentVpts;
        private int CurrCountAllVPts;
        public IList<Viewpoint> oAllVPts;


        #region Tool Plugin
        static bool isTool = false;
        #endregion

        //System.Diagnostics.Stopwatch stopWatch;
        //COMApi.InwNvViewPoint2 oV;

        //public override CommandState CanExecuteCommand(string name)
        //{
        //    if (!isProductActivated)
        //    {
        //        inAppCheckout = new InAppCheckout();

        //        inAppCheckout.StartPaddle();

        //        isProductActivated = inAppCheckout.IsProductActivated;
        //    }

        //    if (name == "ID_Button_5" && !isProductActivated)
        //    {
        //        return new CommandState(false) { IsVisible = false };
        //    }
        //    else
        //    {
        //        return new CommandState();
        //    }
        //    //RibbonTabCollection oTabs = Autodesk.Windows.ComponentManager.Ribbon.Tabs;
        //    //RibbonTab ribbonTab= oTabs.FirstOrDefault(t => t.AutomationName == "CtrlWiz");
        //    //RibbonPanel ribbonPanel= ribbonTab.Panels.FirstOrDefault();
        //    //RibbonItem ribbonItem= ribbonPanel.Source.FindItem("NVSXBOX.CmdViewpoint.VIATechnik.ID_Button_5");
        //    //ribbonItem.IsVisible = false;
        //}

        public override int ExecuteCommand(string commandId, params string[] parameters)
        {
            //inAppCheckout = new InAppCheckout();
            //if (!inAppCheckout.StartPaddle1().Result)
            //    return 0;
            if (!isProductActivated)
            {
                inAppCheckout = new InAppCheckout();

                inAppCheckout.ActivationChanged += InAppCheckout_ActivationChanged;

                inAppCheckout.StartPaddle();

                isProductActivated = inAppCheckout.IsProductActivated;
                if (!isProductActivated) return 0;
            }

            #region Time Expired
            //try
            //{
            //    MaxDate = new DateTime(2018, 12, 31, 0, 0, 0);
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(ex.Message);
            //}
            //var Currenttime = GetCurrentTime();
            //KeyLock = DateTime.Compare(MaxDate, Currenttime);

            //if (KeyLock < 0)
            //{
            //    Form_Expired fe = new Form_Expired();
            //    fe.ShowDialog();
            //    return 0;
            //} 
            #endregion
            switch (commandId)
            {
                case "ID_Button_1":
                    {
                        if (!isrun)
                        {
                            isrun = true;
                            Button1();
                            //Button1Nosync();
                        }
                        break;
                    }
                case "ID_Button_2":
                    {
                        Button2();
                        break;
                    }
                case "ID_Button_3":
                    {
                        Button3();
                        break;
                    }
                #region Update 3
                case "ID_Button_4":
                    {
                        try
                        {
                            oDoc = NvwApplication.ActiveDocument;
                            if (oDoc != null)
                            {
                                var oVP = oDoc.CurrentViewpoint.CreateCopy();
                                oVP.LinearSpeed = SpeedSetting;
                                oVP.AngularSpeed = AngularSetting;
                                oDoc.CurrentViewpoint.CopyFrom(oVP);
                                oVP.Dispose();
                            }

                        }
                        catch (Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show(e.Message);
                            throw;
                        }

                        Form_SpeedSetting form = new Form_SpeedSetting();
                        form.ShowDialog();
                        break;
                    }
                #endregion
                case "ID_Button_5":
                    {
                        inAppCheckout.ShowLicenseDetailsWindow();
                        break;
                    }
            }

            return 0;
        }

        private void InAppCheckout_ActivationChanged(object sender, ActivationChangedArgs e)
        {
            isProductActivated = e.IsActivated;
        }

        //public override CommandState CanExecuteCommand(String commandId)
        //{
        //    CommandState state = new CommandState();
        //    switch (commandId)
        //    {
        //        // Button 1 is only enabled when Button 2 is toggled on.
        //        case "ID_Button_1":
        //            {
        //                state.IsEnabled = false;
        //                break;
        //            }
        //        case "ID_Button_2":
        //            {

        //                break;
        //            }
        //    }
        //    return state;
        //}
        private async void Button1()
        {
            //Get Current Documents
            SetConstantUnit();

            #region Update 3
            SetInitSettingUIAS();
            #endregion

            IDCurrentVpts = 0;

            await Start(1, 0.35f);
            //stopWatch = new System.Diagnostics.Stopwatch();
            GetListSaveVPts();
            CurrCountAllVPts = oAllVPts.Count;

            await Update();
        }

        #region Update 3
        public void SetInitSettingUIAS()
        {
            SpeedSetting = 4.00;
            AngularSetting = 45.00;

            try
            {
                oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                if (oDoc != null)
                {
                    var oVP = oDoc.CurrentViewpoint.CreateCopy();

                    if (SpeedSetting >= 45) SpeedSetting = 45;
                    if (AngularSetting >= 90) AngularSetting = 90;

                    oVP.LinearSpeed = SpeedSetting;
                    oVP.AngularSpeed = AngularSetting * Math.PI / 180; //Degree to Radian
                    oDoc.CurrentViewpoint.CopyFrom(oVP);
                    //oVP.Dispose();
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw;
            }
        }
        #endregion

        #region Update 3
        public void SetRuntimeSettingUIAS()
        {
            try
            {
                oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                if (oDoc != null)
                {

                    SpeedSetting = oDoc.CurrentViewpoint.ToViewpoint().LinearSpeed;
                    AngularSetting = oDoc.CurrentViewpoint.ToViewpoint().AngularSpeed * 180 / Math.PI;

                    if (SpeedSetting >= 45)
                    {
                        SpeedSetting = 45;
                        var oVP = oDoc.CurrentViewpoint.CreateCopy();
                        oVP.LinearSpeed = SpeedSetting;
                        oDoc.CurrentViewpoint.CopyFrom(oVP);
                        //oVP.Dispose();

                    }
                    if (AngularSetting >= 90)
                    {
                        AngularSetting = 90;
                        var oVP = oDoc.CurrentViewpoint.CreateCopy();
                        oVP.AngularSpeed = AngularSetting * Math.PI / 180; //Degree to Radian
                        oDoc.CurrentViewpoint.CopyFrom(oVP);
                        //oVP.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw;
            }


        }
        #endregion

        private void SetConstantUnit()
        {
            oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            if (oDoc.Units == Units.Meters) ConstantUnit = 1;
            if (oDoc.Units == Units.Inches) ConstantUnit = 39.3701;
            if (oDoc.Units == Units.Centimeters) ConstantUnit = 100;
            if (oDoc.Units == Units.Feet) ConstantUnit = 3.28084;
            if (oDoc.Units == Units.Kilometers) ConstantUnit = Math.Pow(10, -3);
            if (oDoc.Units == Units.Microinches) ConstantUnit = 39.3701 * Math.Pow(10, 6);
            if (oDoc.Units == Units.Micrometers) ConstantUnit = Math.Pow(10, 6);
            if (oDoc.Units == Units.Miles) ConstantUnit = 0.621371 * Math.Pow(10, -3);
            if (oDoc.Units == Units.Millimeters) ConstantUnit = 1000;
            if (oDoc.Units == Units.Mils) ConstantUnit = 39370.1;
            if (oDoc.Units == Units.Yards) ConstantUnit = 1.093613888889;

        }

        #region Get all list Saved Viewpoint
        private void GetListSaveVPts()
        {
            //oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            oAllVPts = new List<Viewpoint>();


            foreach (SavedItem oSVP in oDoc.SavedViewpoints.Value)
            {
                // if it is a folder/animation 
                if (oSVP.IsGroup) recurse(oSVP);
                else
                {
                    try
                    {
                        if (oSVP is SavedViewpoint)
                        {

                            SavedViewpoint oThisSVP = oSVP as SavedViewpoint;
                            if (oThisSVP != null)
                            {
                                Viewpoint oVP = oThisSVP.Viewpoint;
                                if (!oAllVPts.Contains(oVP))
                                {
                                    oAllVPts.Add(oVP);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }


            //System.Windows.Forms.MessageBox.Show(string.Format($"All Views: {oDoc.Views.Count} \n  " +
            //    $"Save View point count: {oDoc.SavedViewpoints.Value.Count}\n" +
            //    $"List VP: {oAllVPts.Count}"));

        }
        #endregion
        #region Recurse
        void recurse(SavedItem oFolder)
        {
            foreach (SavedItem item in ((GroupItem)oFolder).Children)
            {
                if (item.IsGroup)
                {
                    recurse(item);
                }
                else if (item is SavedViewpoint)
                {
                    try
                    {
                        SavedViewpoint oThisSVP = item as SavedViewpoint;
                        if (oThisSVP != null)
                        {
                            Viewpoint oVP = oThisSVP.Viewpoint;
                            if (!oAllVPts.Contains(oVP))
                            {
                                oAllVPts.Add(oVP);
                            }
                        }

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }
        #endregion


        private void Button2()
        {
            Form_Help f2 = new Form_Help();
            f2.Show();
        }

        private void Button3()
        {
            System.Diagnostics.Process.Start("https://support.buildfore.com/hc/en-us/community/topics/360001036772-CtrlWiz-Xbox-Controller-for-Navisworks-Feature-Requests");
        }

        private async void Button5_Tool()
        {
            ToolPluginRecord toolPluginRecord = (ToolPluginRecord)Application.Plugins.FindPlugin("NVSXBOX.CmdTool.ATLAS");
            Application.MainDocument.Tool.SetCustomToolPlugin(toolPluginRecord.LoadPlugin());

            await Task.WhenAll();
        }

        #region Virbration
        private async void Vibration(int delay, float p)
        {
            await Start(delay, p);
        }

        public async Task Start(int delay, float p)
        {
            GamePad.SetVibration(PlayerIndex.One, p, p);
            await Task.Delay(delay * 1000);
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
        }
        #endregion

        #region Inertia
        private async void Inertia(int delay, double vX, double vY)
        {
            await threadInnertia(delay, vX, vY);
        }
        public async Task threadInnertia(int delay, double vX, double vY)
        {
            while (delay > 16)
            {

                MoveCamera(vX, vY, 0);
                await Task.Delay(16);
                delay -= 16;
            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------
        public async Task Update()
        {
            while (isrun)
            {
                try
                {
                    //var currentTick = System.Environment.TickCount;

                    sw = Stopwatch.StartNew();

                    //Move Camera----------------------------------------------------------------------------------------
                    prevState = state;
                    state = GamePad.GetState(PlayerIndex.One);

                    if (Math.Abs(state.ThumbSticks.Left.X) >= 0.002 ||
                        Math.Abs(state.ThumbSticks.Left.Y) >= 0.002 ||
                        state.Triggers.Left >= 0.002 || state.Triggers.Right >= 0.002)
                    {
                        double mcurrentZ = currentZ;
                        if (state.Triggers.Left >= 0.002)
                        {
                            currentZ = -0.55 * state.Triggers.Left;
                        }
                        else if (state.Triggers.Right >= 0.002)
                            currentZ = 0.55 * state.Triggers.Right;
                        else
                            currentZ = 0;
                        mcurrentZ = currentZ;

                        MoveCamera(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y, mcurrentZ);

                        //tempInput = new Vector2D(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y) * Speed;
                        //mv = new System.Numerics.Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y) * (float)Speed;
                        //ab = Math.Sqrt(state.ThumbSticks.Left.X * state.ThumbSticks.Left.X + state.ThumbSticks.Left.Y * state.ThumbSticks.Left.Y);
                    }
                    else
                    {
                        if (isMoving)
                        {
                            //oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                            oCurrentViewCopy = oDoc.CurrentViewpoint.CreateCopy();

                            isMoving = false;
                        }
                        //formDebug.textBox1.Text = mv.Length().ToString(); 

                        #region momentum removed
                        //if (mv.Length() > 0.01)
                        //{
                        //    mv = System.Numerics.Vector2.Lerp(mv, System.Numerics.Vector2.Zero, 2.2f * TimeDeltatime);
                        //    MoveCamera(mv.X / Speed, mv.Y / Speed, 0);
                        //}
                        //else mv = System.Numerics.Vector2.Zero; 
                        #endregion
                    }

                    ////Rotate Camera--------------------------------------------------------------------------------------
                    if (Math.Abs(state.ThumbSticks.Right.X) >= 0.002 || Math.Abs(state.ThumbSticks.Right.Y) >= 0.002)
                    {
                        EulerAngleCamera(-state.ThumbSticks.Right.Y, -state.ThumbSticks.Right.X);
                    }
                    else
                    {
                        isRotating = false;
                    }
                    //Button Back----------------------------------------------------------------------------------------
                    #region Button Back: Finish plugin
                    if (prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed)
                    {
                        Vibration(1, 0.35f);
                        isrun = false;
                    }
                    #endregion

                    //Button Start----------------------------------------------------------------------------------------
                    #region Button Start: change viewpoint
                    if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
                    {
                        GetListSaveVPts();
#if NW2016 || NW2017
                 if (oAllVPts.Count != 0)
                {
                    if(CurrCountAllVPts==oAllVPts.Count)
                    {
                        oDoc.CurrentViewpoint.CopyFrom(oAllVPts[IDCurrentVpts]);
                        IDCurrentVpts++;
                        if (IDCurrentVpts >= oAllVPts.Count) IDCurrentVpts = 0;

                    } else
                    {
                        IDCurrentVpts = 0;
                        oDoc.CurrentViewpoint.CopyFrom(oAllVPts[IDCurrentVpts]);
                        CurrCountAllVPts = oAllVPts.Count;
                    }

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("The current version is not supported");
                }
#else

                        //For NW2018,NW2019
                        if (oAllVPts.Count != 0)
                        {
                            if (oAllVPts.Count >= CurrCountAllVPts)
                            {
                                oDoc.CurrentViewpoint.CopyFrom(oAllVPts[IDCurrentVpts]);
                                IDCurrentVpts++;

                                if (oAllVPts.Count > CurrCountAllVPts) CurrCountAllVPts = oAllVPts.Count;
                                if (IDCurrentVpts >= oAllVPts.Count) IDCurrentVpts = 0;

                            }
                            else
                            {
                                IDCurrentVpts = 0;
                                oDoc.CurrentViewpoint.CopyFrom(oAllVPts[IDCurrentVpts]);
                                CurrCountAllVPts = oAllVPts.Count;
                            }

                        }
                        else
                        {
                            if (oDoc.HomeView != null) oDoc.CurrentViewpoint.CopyFrom(oDoc.HomeView);
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Home view not found! \n Please set one");
                                oCurrentViewCopy = oDoc.CurrentViewpoint.CreateCopy();
                            }
                        }
#endif
                    }
                    #endregion


                    //Button LS-------------------------------------------------------------------------------------------
                    #region Button LS = Null
                    if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        //Speed = 2.2;
                    }

                    //Button LS Released----------------------------------------------------------------------------------------

                    if (prevState.Buttons.LeftShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Released)
                    {
                        //Speed = 1;
                    }
                    #endregion

                    //Button RS-------------------------------------------------------------------------------------------
                    #region Button RS = Null
                    if (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        Document document = Application.ActiveDocument;
                        View view = Application.ActiveDocument.ActiveView;
                        int x = view.Height / 2;
                        int y = view.Width / 2;
                        PickItemResult itemResult = Application.ActiveDocument.ActiveView.PickItemFromPoint(x, y);
                        if (itemResult != null)
                        {
                            ModelItem modelItem = itemResult.ModelItem;
                            ModelItemCollection modelItems = new ModelItemCollection
                            {
                                modelItem
                            };
                            document.CurrentSelection.Add(modelItem);
                            //Debug.WriteLine(modelItem.ClassDisplayName);
                        }

                        //Vibration(1, 0.25f);
                        //isFly = !isFly;
                        //if (!isFly)
                        //{
                        //    if (oCurrentViewCopy.Tool != Tool.NavigateWalk) oCurrentViewCopy.Tool = Tool.NavigateWalk;
                        //}
                        //else if (oCurrentViewCopy.Tool != Tool.NavigateFly) oCurrentViewCopy.Tool = Tool.NavigateFly;

                        //toogleRealism();
                        //Speed = 3.0;
                    }
                    //Button RS Released--------------------------------------------------------------------------------
                    if (prevState.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.RightShoulder == ButtonState.Released)
                    {
                        //Speed = 1;
                    }
                    #endregion

                    //Button Left Stick
                    #region Button LeftStick: Speed up
                    if (prevState.Buttons.LeftStick == ButtonState.Released && state.Buttons.LeftStick == ButtonState.Pressed)
                    {
                        Speed = 3;
                    }
                    //Button Left Stick Released
                    if (prevState.Buttons.LeftStick == ButtonState.Pressed && state.Buttons.LeftStick == ButtonState.Released)
                    {
                        Speed = 1;
                    }
                    #endregion


                    //Button A--------------------------------------------------------------------------------------------
                    #region Button A: Null
                    if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
                    {

                        Button5_Tool();
                        Vibration(1, 0.1f);
                    }
                    #endregion
                    //Button B-------------------------------------------------------------------------------------------
                    #region Button B: Null
                    if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed)
                    {
                        //Vibration(1, 0.35f);
                    }
                    #endregion
                    //Button X------------------------------------------------------------------------------------------
                    #region Button X: Null
                    if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
                    {
                        //Vibration(1, 0.35f);
                    }
                    #endregion
                    //Button Y------------------------------------------------------------------------------------------
                    #region Button Y: Null
                    if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
                    {
                        //Vibration(1, 0.35f);
                    }
                    #endregion


                    await Task.Delay(10);
                    #region Show FPS 

                    //if (currentTick - lastTime >= 1000)
                    //{
                    //    fps = frames;
                    //    frames = 0;
                    //    lastTime = currentTick;
                    //}

                    //frames++;


                    //deltaTime = (currentTick - lastTime) / 1000f;

                    //if (fps > 1)
                    //{

                    //    TimeDeltatime = (1 / (float)fps);
                    //    //if (TimeDeltatime >= 0.05) TimeDeltatime = 0.05f;
                    //    //if (TimeDeltatime <= 0.01) TimeDeltatime = 0.01f;
                    //}
                    #endregion
                    TimeDeltatime = sw.ElapsedTicks * 1000 / Stopwatch.Frequency;

                    sw.Stop();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                    throw;
                }
            }

        }

        //private void toogleRealism()
        //{
        //    isCOM = !isCOM;

        //    //if (isCOM)
        //    //{

        //    //    oV.Paradigm = COMApi.nwEParadigm.eParadigm_WALK;
        //    //    oV.Viewer.CollisionDetection = true;
        //    //    oV.Viewer.Gravity = true;
        //    //}

        //    //var mParadigm = oV.Paradigm;

        //    //IDMode++;
        //    //if (IDMode > 3) IDMode = 0;

        //    //if (IDMode == 1)
        //    //{
        //    //    oV.Paradigm = COMApi.nwEParadigm.eParadigm_WALK;
        //    //    oV.Viewer.CollisionDetection = true;
        //    //    oV.Viewer.Gravity = true;


        //    //}
        //    //else if (IDMode == 2)
        //    //{
        //    //    oV.Paradigm = COMApi.nwEParadigm.eParadigm_FLY;
        //    //    oV.Viewer.CollisionDetection = true;
        //    //}
        //    //else if(IDMode==3)
        //    //{
        //    //    oV.Paradigm = COMApi.nwEParadigm.eParadigm_FLY;
        //    //    oV.Viewer.CollisionDetection = false;
        //    //} else
        //    //    oV.Paradigm = mParadigm;
        //    //third person
        //    //oV.Viewer.CameraMode = COMApi.nwECameraMode.eCameraMode_ThirdPerson;

        //    //if (oV.Paradigm == COMApi.nwEParadigm.eParadigm_WALK)
        //    //{
        //    //    // gravity, coollison or crouch can only take effect in Walk mode.

        //    //    oV.Viewer.Gravity = true;//toogle gravity on

        //    //    oV.Viewer.CollisionDetection = true;//toogle collision detection on

        //    //    oV.Viewer.AutoCrouch = true; //toogle auto crouch on

        //    //}

        //}

        //=======================================================================================================================
        #region Move Camera
        private void MoveCamera(double x, double y, double z)
        {
            // To move the camera, we can just directly manipulate the
            // Position property. Rotation will remain unchanged, so view direction
            // is not changed.
            //if (!isCOM)

            if (!isMoving)
            {

                SetConstantUnit();
                oCurrentViewCopy = oDoc.CurrentViewpoint.CreateCopy();

                #region Update 3
                SetRuntimeSettingUIAS();
                #endregion

                isMoving = true;
            }

            #region Move Mode 1
            // get view direction
            //Vector3D oViewDir = NavisUtils.Instance.getViewDir(oCurrentViewCopy);

            //Vector3 mV3 = NavisUtils.Instance.V3DtoV3(oViewDir);

            //Vector3 PDir = Vector3.Reflect(mV3, Vector3.UnitZ);

            //Vector3 PerpenPdir = new Vector3(PDir.Y, -PDir.X, PDir.Z);

            //Vector3 pos = NavisUtils.Instance.Point3DtoV3(oCurrentViewCopy.Position);

            //pos += (float)y * (PDir * (Vector3.UnitY + Vector3.UnitX));

            //pos += (float)x * (PerpenPdir * (Vector3.UnitX + Vector3.UnitY));

            //pos += (float)z * Vector3.UnitZ;

            //Point3D newPos = NavisUtils.Instance.V3toPoint3D(pos);
            //oCurrentViewCopy.Position = newPos;
            //oDoc.CurrentViewpoint.CopyFrom(oCurrentViewCopy);
            #endregion

            #region Move Mode 2
            // get view direction
            //Vector3D oViewDir = NavisUtils.Instance.getViewDir(oCurrentViewCopy);

            //Vector3D PerpenPdir = new Vector3D(oViewDir.Y, -oViewDir.X, oViewDir.Z);

            //Vector3D pos = NavisUtils.Instance.Point3DtoV3D(oCurrentViewCopy.Position);

            //pos += y * (oViewDir * (UnitVector3D.UnitY + UnitVector3D.UnitX));

            //pos += x * (PerpenPdir * (UnitVector3D.UnitY + UnitVector3D.UnitX));

            //pos += z * UnitVector3D.UnitZ;

            //Point3D newPos = NavisUtils.Instance.V3DtoPoint3D(pos);
            //oCurrentViewCopy.Position = newPos;
            //oDoc.CurrentViewpoint.CopyFrom(oCurrentViewCopy);
            #endregion

            #region Move Mode 3



            Vector3D oViewDir = NavisUtils.Instance.getViewDir(oCurrentViewCopy);

            Vector3D PerpenPdir = new Vector3D(oViewDir.Y, -oViewDir.X, oViewDir.Z);

            Vector3D pos = NavisUtils.Instance.Point3DtoV3D(oCurrentViewCopy.Position);


            pos += (y * (oViewDir * (UnitVector3D.UnitY + UnitVector3D.UnitX)) + x * (PerpenPdir * (UnitVector3D.UnitY + UnitVector3D.UnitX))) * 0.003 * Speed * SpeedSetting * ConstantUnit * TimeDeltatime;
            pos += z * UnitVector3D.UnitZ * 0.003 * Speed * SpeedSetting * ConstantUnit * TimeDeltatime;

            //oCurrentViewCopy.Tool = Tool.NavigateWalk;


            //oDoc.Units == Units.Millimeters;

            Point3D newPos = NavisUtils.Instance.V3DtoPoint3D(pos);
            oCurrentViewCopy.Position = newPos;
            oDoc.CurrentViewpoint.CopyFrom(oCurrentViewCopy);

            #endregion

            //  correct for Forward:
            //Point3D newPos = new Point3D(oCurrentViewCopy.Position.X + PDir.X * y /*+ PDir.Y * x*/,
            //                                oCurrentViewCopy.Position.Y + /*PDir.X * x +*/ PDir.Y * y,
            //                                oCurrentViewCopy.Position.Z + z);
            //double alpha;
            //if (Vector3.Dot(PDir, NavisUtils.Instance.V3DtoV3(UnitVector3D.UnitX)) > 0)
            //    alpha = NavisUtils.Instance.AngleBetween(PDir, NavisUtils.Instance.V3DtoV3(UnitVector3D.UnitX));
            //else
            //    alpha = 180 - NavisUtils.Instance.AngleBetween(PDir, NavisUtils.Instance.V3DtoV3(UnitVector3D.UnitX));


            //Point3D newPos = new Point3D(oCurrentViewCopy.Position.X + PDir.X * Math.Cos(alpha) + PDir.Y * Math.Sin(alpha),
            //                               oCurrentViewCopy.Position.Y + PDir.X * Math.Sin(alpha) + PDir.Y * Math.Cos(alpha),
            //                               oCurrentViewCopy.Position.Z + z);

            //Point3D newPos = new Point3D(oCurrentViewCopy.Position.X + x1,
            //                               oCurrentViewCopy.Position.Y + y1,
            //                               oCurrentViewCopy.Position.Z + z);

            //Matrix4x4 m0 = Matrix4x4.CreateWorld(Vector3.Zero,PDir,Vector3.UnitZ);

            //Quaternion q1 = NavisUtils.Instance.Rot3DtoQuaternion(EulerAngleCamera(-state.ThumbSticks.Right.Y * 0.02 * Speed, -state.ThumbSticks.Right.X * 0.02 * Speed));

            //Point3D Pos = oCurrentViewCopy.Position;
            //Rotation3D rot = oCurrentViewCopy.Rotation;

            //Matrix4x4 m0 = Matrix4x4.CreateWorld(NavisUtils.Instance.Point3DtoV3(Pos), pdir, Vector3.UnitZ);

            //Matrix4x4 m1 = Matrix4x4.Transform(m0, NavisUtils.Instance.Rot3DtoQuaternion(rot));

            //Vector3 movepos = new Vector3((float)(Pos.X ), (float)(Pos.Y), (float)(Pos.Z));

            //Vector3 ConvertToLocal = Vector3.Transform(movepos, m0);

            //Vector3 movelocal = new Vector3(ConvertToLocal.X+(float)x, ConvertToLocal.Y+ (float)y, ConvertToLocal.Z + (float)z);

            //Vector3 ConvertToGlobal = Vector3.Transform(movelocal, Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ));

            //oCurrentViewCopy.Position = NavisUtils.Instance.V3toPoint3D( ConvertToGlobal);


            //textDebug = "false dsfsdfsdfsd";
            //formDebug.textBox1.Text = textDebug;

            //else
            //{


            //    m_state.BeginEdit("Move Camera");

            //    oV = (COMApi.InwNvViewPoint2)m_state.CurrentView.ViewPoint;
            //    oV.Viewer.CameraMode = COMApi.nwECameraMode.eCameraMode_FirstPerson;
            //    oV.Paradigm = COMApi.nwEParadigm.eParadigm_WALK;//eParadigm_FLY
            //    oV.Viewer.CollisionDetection = true;
            //    oV.Viewer.Gravity = true;

            //    COMApi.InwLPos3f pos = oV.Camera.Position;//cam.Position;
            //    pos.data1 += x;
            //    pos.data2 += y;
            //    pos.data3 += z;

            //    m_state.EndEdit();
            //    oV = (COMApi.InwNvViewPoint2)m_state.CurrentView.ViewPoint;
            //    //oV.Copy(); 
            //    //textDebug = OnCollisionEnter(oV.Camera).ToString();
            //    //formDebug.textBox1.Text = textDebug;

            //    //updatePos();

            //}
        }
        #endregion

        //private string OnCollisionEnter(COMApi.InwOclClashTest test)
        //{

        //    //COMApi.InwOclClashTest test = inwNvCamera as COMApi.InwOclClashTest;

        //    //m_clash = test as COMApi.InwOpClashElement;

        //    //if (m_clash != null) return true;

        //    //return false;
        //    string rs = "";
        //    foreach (COMApi.InwBase oPlugin in m_state.Plugins())
        //    {
        //        if (oPlugin.ObjectName == "nwOpClashElement")
        //        {
        //            m_clash = (COMApi.InwOpClashElement)oPlugin;
        //            rs = "nwOpClashElement";
        //        }
        //    }
        //    if (m_clash == null)
        //    {
        //       rs = "cannot find clash test plugin!";
        //    }
        //    return rs;
        //}

        #region Rotation Camera By Quaternion
        private void RotateCamera(double angle, double rotX, double rotY, double rotZ) //angle 0.1-0.01
        {
            //Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            //Viewpoint currentVP = oDoc.CurrentViewpoint;

            //  Make a copy of current viewpoint	
            Viewpoint oCurrVCopy = oDoc.CurrentViewpoint.CreateCopy();

            //  set the axis we will rotate around （Ｚ：０,０,１）
            UnitVector3D odeltaA = new UnitVector3D(rotX, rotY, rotZ);
            // Create delta of Quaternion: 
            Rotation3D delta = new Rotation3D(odeltaA, angle);
            // multifly the current Quaternion with the delta , get the new Quaternion 

            oCurrVCopy.Rotation = Multiply(oCurrVCopy.Rotation, delta);


            // Update current viewpoint
            oDoc.CurrentViewpoint.CopyFrom(oCurrVCopy);

        }
        #endregion

        #region Rotation Camera By EulerAngle
        private Rotation3D EulerAngleCamera(double XAxis, double YAxis)
        {
            if (!isRotating)
            {
                SetConstantUnit();
                oCurrentViewCopy = oDoc.CurrentViewpoint.CreateCopy();

                #region Update 3
                SetRuntimeSettingUIAS();
                #endregion

                isRotating = true;
            }

            Vector3D EulerAngel = toEulerAngle(oCurrentViewCopy.Rotation);

            EulerAngel = toEulerAngle(oCurrentViewCopy.Rotation) + new Vector3D(0.045 * YAxis * Speed * AngularSetting * TimeDeltatime / 1000, 0, 0.045 * XAxis * Speed * AngularSetting * TimeDeltatime / 1000);


            oCurrentViewCopy.Rotation = toQuaternion(EulerAngel);

            // Update current viewpoint
            oDoc.CurrentViewpoint.CopyFrom(oCurrentViewCopy);

            return oCurrentViewCopy.Rotation;
        }

        #endregion

        #region Rotation3D Multiply
        public static Rotation3D Multiply(Rotation3D r1, Rotation3D r2)
        {
            Rotation3D res = new Rotation3D(r2.D * r1.A + r2.A * r1.D + r2.B * r1.C - r2.C * r1.B,
                                            r2.D * r1.B + r2.B * r1.D + r2.C * r1.A - r2.A * r1.C,
                                            r2.D * r1.C + r2.C * r1.D + r2.A * r1.B - r2.B * r1.A,
                                            r2.D * r1.D - r2.A * r1.A - r2.B * r1.B - r2.C * r1.C);

            return res;
        }
        #endregion

        #region Quaternion To EulerAngle
        public Vector3D toEulerAngle(Rotation3D q)
        {
            double roll, pitch, yaw;
            // roll (x-axis rotation)
            double sinr_cosp = +2.0 * (q.A * q.B + q.C * q.D);
            double cosr_cosp = +1.0 - 2.0 * (q.B * q.B + q.C * q.C);
            roll = Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = +2.0 * (q.A * q.C - q.D * q.B);
            if (Math.Abs(sinp) >= 1)
            {
                if (sinp >= 0) pitch = Math.PI / 2;
                else pitch = -Math.PI / 2;
                //pitch =copysign(Math.PI / 2, sinp); // use 90 degrees if out of range
            }
            else
                pitch = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = +2.0 * (q.A * q.D + q.B * q.C);
            double cosy_cosp = +1.0 - 2.0 * (q.C * q.C + q.D * q.D);
            yaw = Math.Atan2(siny_cosp, cosy_cosp);

            return new Vector3D(roll, pitch, yaw);
        }
        #endregion

        #region Rotation To Quaternion
        public Rotation3D toQuaternion(Vector3D mEulerAnger) // yaw (Z), pitch (Y), roll (X)
        {
            double yaw, pitch, roll;
            yaw = mEulerAnger.Z;
            pitch = mEulerAnger.Y;
            roll = mEulerAnger.X;
            // Abbreviations for the various angular functions
            double cy = Math.Cos(yaw * 0.5);
            double sy = Math.Sin(yaw * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double sp = Math.Sin(pitch * 0.5);
            double cr = Math.Cos(roll * 0.5);
            double sr = Math.Sin(roll * 0.5);

            Rotation3D q = new Rotation3D(cy * cp * cr + sy * sp * sr,
                                            cy * cp * sr - sy * sp * cr,
                                            sy * cp * sr + cy * sp * cr,
                                            sy * cp * cr - cy * sp * sr);
            //q.A = cy * cp * cr + sy * sp * sr;
            //q.B = cy * cp * sr - sy * sp * cr;
            //q.C = sy * cp * sr + cy * sp * cr;
            //q.D = sy * cp * cr - cy * sp * sr;
            return q;
        }

        #endregion

        #region Get Current Time Internet
        protected static DateTime GetCurrentTime()
        {
            try
            {
                using (var response = System.Net.WebRequest.Create("http://www.google.com").GetResponse())
                {//string todaysDates =  response.Headers["date"];
                    return DateTime.ParseExact(response.Headers["date"],
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);

                }

            }
            catch (System.Net.WebException)
            {
                System.Windows.Forms.MessageBox.Show("Please connect to the internet!!!");
                return new DateTime(2018, 11, 11, 0, 0, 0); //In case something goes wrong. 
            }

        }
        #endregion

        #region Showing infor F1 for Help
        public override bool TryShowCommandHelp(String commandId)
        {
            //System.Windows.MessageBox.Show("Showing Help for command with the Id " + commandId);
            System.Diagnostics.Process.Start("https://support.buildfore.com/hc/en-us/articles/360021376292-How-to-Use-the-Xbox-Controller-for-Navisworks");
            return true;
        }
        #endregion
    }
}
