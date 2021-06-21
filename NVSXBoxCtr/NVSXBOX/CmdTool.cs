using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using System.Runtime.InteropServices;
using System;

namespace NVSXBOX
{
    #region ToolPlugin
    [Plugin("NVSXBOX.CmdTool", "ATLAS")]
    public class CmdTool : ToolPlugin
    {
        public ModelItem clickedModel = null;
        public ModelItem raycastModel = null;

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hwnd, ref System.Windows.Point lpPoint);

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        struct POINT
        {
            public FIXED x;
            public FIXED y;
        }
        struct FIXED
        {
            public short fract;
            public short value;
        }

        public override bool MouseDown(View view, KeyModifiers modifiers, ushort button, int x, int y, double timeOffset)
        {
            // get current selection
            PickItemResult itemResult = view.PickItemFromPoint(x, y);

            if (itemResult != null)
            {
                clickedModel = itemResult.ModelItem;
                Application.ActiveDocument.ActiveView.RequestDelayedRedraw(ViewRedrawRequests.Render);
            }

            return false;
        }

        //public Point3D GetClosestGridIntersection()
        //{
        //    if (clickedModel != null)
        //    {
        //        //Get bounding box of the selection

        //        BoundingBox3D bb3d = clickedModel.BoundingBox();

        //        GridSystem oGS = Autodesk.Navisworks.Api.Application.ActiveDocument.Grids.ActiveSystem;

        //        if (oGS != null)//get the closest grid intersection point
        //        {

        //            GridIntersection oGridIntersection = oGS.ClosestIntersection(bb3d.Center);
        //            return oGridIntersection.Position;
        //        }

        //    }
        //    return null;
        //}

        public override void Render(View view, Graphics graphics)
        {
            Application.ActiveDocument.ActiveView.RequestDelayedRedraw(ViewRedrawRequests.Render);

            Color color = Color.White;//FromByteRGB(255, 122, 0);
            graphics.Color(color, 1);
            //graphics.LineWidth(0.1);
            //graphics.DepthMask(false);
            graphics.LineWidth(5);

            if (clickedModel != null)
            {
                //color for graphics
                BoundingBox3D boundingBox = clickedModel.BoundingBox();

                Point3D origin = boundingBox.Min;
                Vector3D xVector = new Vector3D((boundingBox.Max - boundingBox.Min).X, 0, 0);
                Vector3D yVector = new Vector3D(0, (boundingBox.Max - boundingBox.Min).Y, 0);
                Vector3D zVector = new Vector3D(0, 0, (boundingBox.Max - boundingBox.Min).Z);
                graphics.Cuboid(origin, xVector, yVector, zVector, true);
            }

            if (raycastModel != null)
                Cirle(graphics, 1.5);
            else
                Cirle(graphics,1);
        }

        private void Cirle(Graphics gr,double size)
        {
            Vector3D CrhNormal = NavisUtils.Instance.getViewDir(CmdViewpoint.oDoc.CurrentViewpoint.ToViewpoint());

            Vector3D PerpenPdir = new Vector3D(CrhNormal.Y, -CrhNormal.X, CrhNormal.Z);

            Point3D mCrh = CmdViewpoint.oDoc.CurrentViewpoint.ToViewpoint().Position;

            Point3D CrhCenter = new Point3D(0.2 * CrhNormal.X + mCrh.X, 0.2 * CrhNormal.Y + mCrh.Y, 0.2 * CrhNormal.Z + mCrh.Z);

            //Point3D start1 = new Point3D(1*CrhCenter.X + PerpenPdir.X, 1 * CrhCenter.Y + PerpenPdir.Y, 1 * CrhCenter.Z + PerpenPdir.Z);
            //Point3D end1 = new Point3D(-1 * CrhCenter.X + PerpenPdir.X, -1 * CrhCenter.Y + PerpenPdir.Y, -1 * CrhCenter.Z + PerpenPdir.Z);
            gr.Point(CrhCenter);
            gr.Circle(CrhCenter, 1 * CrhNormal, 0.002*size, false);
        }
    }
    #endregion

    //#region CmdRender
    //[Plugin("NVSXBOX.CmdRender", "ATLAS")]
    //public class CmdRender : RenderPlugin
    //{
    //    public override void OverlayRender(View view, Graphics graphics)
    //    {
    //       Vector3D CrhNormal = NavisUtils.Instance.getViewDir(CmdViewpoint.oDoc.CurrentViewpoint.ToViewpoint());
    //        //Color skyBlue = Color.FromByteRGB(130, 202, 255);
    //        //graphics.Color(skyBlue, 0.7);
    //        //Point2D bottomLeft = new Point2D(20, 20);
    //        //Point2D topRight = new Point2D(view.Width - 20, 60);
    //        //graphics.Rectangle(bottomLeft, topRight, true);


    //        Point3D mCrh =CmdViewpoint.oDoc.CurrentViewpoint.ToViewpoint().Position;
    //        Point3D CrhPos = new Point3D(5*mCrh.X,5*mCrh.Y,5*mCrh.Z);

    //        graphics.Circle(CrhPos, CrhNormal, 5, true);
    //    }
    //}
    //#endregion


}
