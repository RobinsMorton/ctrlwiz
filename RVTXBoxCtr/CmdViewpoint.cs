using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Numerics;
using Fms = System.Windows.Forms;
using XInputDotNetPure;
using RVTXBoxCtr.Utility;

namespace RVTXBoxCtr
{
    #region Attributes
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    #endregion

    public class CmdViewpoint : IExternalCommand
    {
        private GamePadState state;
        private GamePadState prevState;
        private static Stopwatch sw;
        private bool isrun = false;
        private double currentZ;
        private bool isMoving = false;
        private bool isRotating = false;
        private static long TimeDeltatime;
        private double Speed = 1;
        private static double ConstantUnit = 3.28084;
        public static double SpeedSetting = 4.00;
        public static double AngularSetting = 45.00;
        private static UIDocument uiDoc;
        private static Document doc;
        private static View activeView;
        private static View3D view3D;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiDoc = commandData.Application.ActiveUIDocument;
            doc = uiDoc.Document;
            activeView = doc.ActiveView;
            view3D = activeView as View3D;

            if (view3D != null && view3D.IsPerspective)
            {
                Initialize();
            }
            else
            {
                Fms::MessageBox.Show("Please activate the Fly mode in a Perspective view.", "Information",
                                    Fms::MessageBoxButtons.OK, Fms::MessageBoxIcon.Exclamation);
                return Result.Cancelled;
            }
            return Result.Succeeded;
        }

        private async void Initialize()
        {
            await Start(1, 0.35f);
            isrun = true;
            await Update();
        }

        private async Task Start(int delay, float p)
        {
            GamePad.SetVibration(PlayerIndex.One, p, p);
            await Task.Delay(delay * 1000);
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
        }

        private async void Vibration(int delay, float p)
        {
            await Start(delay, p);
        }

        private async Task Update()
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
                    }
                    else
                    {
                        if (isMoving)
                        {
                            isMoving = false;
                        }
                    }

                    //Rotate Camera--------------------------------------------------------------------------------------
                    if (Math.Abs(state.ThumbSticks.Right.X) >= 0.002 || Math.Abs(state.ThumbSticks.Right.Y) >= 0.002)
                    {
                        RotateCamera(-state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);
                    }
                    else
                    {
                        isRotating = false;
                    }

                    //Button Start----------------------------------------------------------------------------------------
                    if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
                    {
                        Vibration(1, 0.35f);
                        isrun = true;
                    }

                    //Button Back----------------------------------------------------------------------------------------
                    if (prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed)
                    {
                        Vibration(1, 0.35f);
                        isrun = false;
                    }

                    //Button Left Stick Pressed
                    if (prevState.Buttons.LeftStick == ButtonState.Released && state.Buttons.LeftStick == ButtonState.Pressed)
                    {
                        Speed = 3;
                    }

                    //Button Left Stick Released
                    if (prevState.Buttons.LeftStick == ButtonState.Pressed && state.Buttons.LeftStick == ButtonState.Released)
                    {
                        Speed = 1;
                    }

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
                    TaskDialog.Show("Moving", ex.Message);
                    throw;
                }
            }
        }

        private void MoveCamera(double x, double y, double z)
        {
            XYZ direction = new XYZ(x, y, z).Normalize();

            if (!isMoving)
            {
                isMoving = true;
            }

            ViewOrientation3D viewOrientation3D = view3D.GetOrientation();
            Vector3 oViewDir = viewOrientation3D.ForwardDirection.ConvertToVector3();
            //Vector3 PerpenPdir = new Vector3(oViewDir.Y, -oViewDir.X, oViewDir.Z);
            Vector3 PerpenPdir = view3D.RightDirection.ConvertToVector3();
            Vector3 pos = viewOrientation3D.EyePosition.ConvertToVector3();

            pos += ((float)y * (oViewDir * (Vector3.UnitY + Vector3.UnitX)) + (float)x * (PerpenPdir * (Vector3.UnitY + Vector3.UnitX))) * (float)(0.003 * Speed * SpeedSetting * ConstantUnit * TimeDeltatime);
            pos += (float)z * Vector3.UnitZ * (float)(0.003 * Speed * SpeedSetting * ConstantUnit * TimeDeltatime);

            using (ViewOrientation3D newViewOrientation3D = new ViewOrientation3D(
                //viewOrientation3D.EyePosition.Move(direction, 0.003 * Speed * SpeedSetting * ConstantUnit * TimeDeltatime),
                pos.ConvertToXYZ(),
                viewOrientation3D.UpDirection,
                viewOrientation3D.ForwardDirection))
            {
                view3D.SetOrientation(newViewOrientation3D);
            }
            uiDoc.RefreshActiveView();
        }

        private void RotateCamera(double XAxis, double YAxis)
        {
            if (!isRotating)
            {
                isRotating = true;
            }

            //ViewOrientation3D viewOrientation3D = view3D.GetOrientation();
            //using (ViewOrientation3D newViewOrientation3D = new ViewOrientation3D(
            //    viewOrientation3D.EyePosition,
            //    viewOrientation3D.UpDirection.RotateByAxis(view3D.RightDirection, 0.045 * YAxis * Speed * AngularSetting * TimeDeltatime / 1000),
            //    viewOrientation3D.ForwardDirection.RotateByAxis(viewOrientation3D.UpDirection, 0.045 * XAxis * Speed * AngularSetting * TimeDeltatime / 1000) +
            //    viewOrientation3D.ForwardDirection.RotateByAxis(view3D.RightDirection, 0.045 * YAxis * Speed * AngularSetting * TimeDeltatime / 1000)))

            //{
            //    view3D.SetOrientation(newViewOrientation3D);
            //}

            //uiDoc.RefreshActiveView();

            //Rotate Left-Right
            ViewOrientation3D viewOrientation3D = view3D.GetOrientation();
            using (ViewOrientation3D newViewOrientation3D = new ViewOrientation3D(
                viewOrientation3D.EyePosition,
                viewOrientation3D.UpDirection.RotateByAxis(XYZ.BasisZ, 0.045 * XAxis * Speed * AngularSetting * TimeDeltatime / 1000),
                viewOrientation3D.ForwardDirection.RotateByAxis(XYZ.BasisZ, 0.045 * XAxis * Speed * AngularSetting * TimeDeltatime / 1000)))
            {
                view3D.SetOrientation(newViewOrientation3D);
            }

            //Rotate Up-Down
            viewOrientation3D = view3D.GetOrientation();
            using (ViewOrientation3D newViewOrientation3D = new ViewOrientation3D(
                viewOrientation3D.EyePosition,
                viewOrientation3D.UpDirection.RotateByAxis(view3D.RightDirection, 0.045 * YAxis * Speed * AngularSetting * TimeDeltatime / 1000),
                viewOrientation3D.ForwardDirection.RotateByAxis(view3D.RightDirection, 0.045 * YAxis * Speed * AngularSetting * TimeDeltatime / 1000)))
            {
                view3D.SetOrientation(newViewOrientation3D);
            }
            uiDoc.RefreshActiveView();
        }
#if WorkingScope
                    public async Task Update()
                    {
                        while (isrun)
                        {
                            try
                            {
                                //var currentTick = System.Environment.TickCount;

                                //sw = Stopwatch.StartNew();

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
#endif
    }
}
