using System;
using System.Numerics;
using Autodesk.Revit.DB;

namespace RVTXBoxCtr.Utility
{
    internal static class ExtensionMethods
    {
        internal static XYZ RotateByAxis(this XYZ direction, XYZ axis, int angle)
        {
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis.ConvertToVector3(), angle.ConvertIntoRadians());

            return Vector3.Transform(direction.ConvertToVector3(), quaternion).ConvertToXYZ();
        }

        internal static XYZ RotateByAxis(this XYZ direction, XYZ axis, double radians)
        {
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis.ConvertToVector3(), (float)radians);

            return Vector3.Transform(direction.ConvertToVector3(), quaternion).ConvertToXYZ();
        }

        internal static Vector3 ConvertToVector3(this XYZ point)
        {
            return new Vector3((float)point.X, (float)point.Y, (float)point.Z);
        }

        internal static XYZ ConvertToXYZ(this Vector3 vector)
        {
            return new XYZ(vector.X, vector.Y, vector.Z);
        }

        private static float ConvertIntoRadians(this int angle)
        {
            return (float)(angle * Math.PI / 180);
        }

        internal static XYZ Move(this XYZ currentPoint, XYZ direction, double distance)
        {
            return currentPoint.Add(direction.Multiply(distance));
        }
    }
}