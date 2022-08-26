using System.Numerics;

namespace SabTool.Utils
{
    public static class MatrixMath
    {
        // DirectX to OpengGl coordinate system
        private static readonly Matrix4x4 DxToOpenGl = new(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1);

        public static Matrix4x4 ConvertDxToOpenGl(Matrix4x4 matrix)
        {
            return matrix * DxToOpenGl;
        }

        public static Vector3 ConvertDxToOpenGl(Vector3 translation)
        {
            return new Vector3(-translation.X, translation.Y, translation.Z);
        }
    }
}
