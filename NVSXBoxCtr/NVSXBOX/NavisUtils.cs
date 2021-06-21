using System;
using System.IO;
using System.Numerics;
using Autodesk.Navisworks.Api;
using Microsoft.Win32;

namespace NVSXBOX
{
    public class NavisUtils : Singleton<NavisUtils>
    {
        public Vector3 Point3DtoV3(Point3D p) => new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        public Quaternion Rot3DtoQuaternion(Rotation3D r) => new Quaternion((float)r.A, (float)r.B, (float)r.C, (float)r.D);

        public Point3D V3toPoint3D(Vector3 v) => new Point3D(v.X, v.Y, v.Z);

        public Vector3D Point3DtoV3D(Point3D p) => new Vector3D(p.X, p.Y, p.Z);
        public Point3D V3DtoPoint3D(Vector3D v) => new Point3D(v.X, v.Y, v.Z);

        public Vector3 V3DtoV3(Vector3D v3d) => new Vector3((float)v3d.X, (float)v3d.Y, (float)v3d.Z);

        public Vector3D V3toV3D(Vector3 v3) => new Vector3D(v3.X, v3.Y, v3.Z);

        public Vector3D getViewDir(Viewpoint oVP)
        {

            Rotation3D oRot = oVP.Rotation;
            // calculate view direction
            Rotation3D oNegtiveZ = new Rotation3D(0, 0, -1, 0);

            Rotation3D otempRot = MultiplyRotation3D(oNegtiveZ, oRot.Invert());



            Rotation3D oViewDirRot = MultiplyRotation3D(oRot, otempRot);

            // get view direction

            Vector3D oViewDir = new Vector3D(oViewDirRot.A, oViewDirRot.B, oViewDirRot.C);
            oViewDir.Normalize();

            return new Vector3D(oViewDir.X, oViewDir.Y, oViewDir.Z);
        }


        public double AngleBetween(Vector3 vector1, Vector3 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        public Rotation3D MultiplyRotation3D(Rotation3D r2, Rotation3D r1)
        {
            Rotation3D oRot = new Rotation3D(r2.D * r1.A + r2.A * r1.D + r2.B * r1.C - r2.C * r1.B,
                                            r2.D * r1.B + r2.B * r1.D + r2.C * r1.A - r2.A * r1.C,
                                            r2.D * r1.C + r2.C * r1.D + r2.A * r1.B - r2.B * r1.A,
                                            r2.D * r1.D - r2.A * r1.A - r2.B * r1.B - r2.C * r1.C);

            oRot.Normalize();
            return oRot;
        }

        public Vector3D VLerp(Vector3D from, Vector3D to, double time)
        {
            Vector3 rs = Vector3.Lerp(new Vector3((float)from.X, (float)from.Y, (float)from.Z),
                                        new Vector3((float)to.X, (float)to.Y, (float)to.Z),
                                        (float)time);

            return new Vector3D(rs.X, rs.Y, rs.Z);
        }

        //public void SetStartDate(DateTime value)
        //{
        //    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\CtrWiz");
        //    key.SetValue("StartDate", value);
        //    key.Close();
        //}

        //public DateTime GetStartDate()
        //{

        //    RegistryKey value = Registry.CurrentUser.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\CtrWiz");
        //    if (value != null)
        //    {
        //        return value.GetValue("StartDate") as DateTime;
        //    }
        //}
    }
}
