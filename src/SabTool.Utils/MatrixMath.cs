using System.Numerics;

namespace SabTool.Utils;
public static class MatrixMath
{
    // DirectX to OpengGl coordinate system
    private static readonly Matrix4x4 DxToOpenGl = new(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1);

    public static Matrix4x4 ConvertDxToOpenGl(Matrix4x4 matrix)
    {
        // Blender import of gltf either doesn't work properly or the matrix above is wrong or wrongly applied
        // For now don't modify anything, but use it in your code so we can fix it later
        return matrix;            
        // return matrix * DxToOpenGl;
    }

    public static Vector3 ConvertDxToOpenGl(Vector3 translation)
    {
        // Blender import of gltf either doesn't work properly or the matrix above is wrong or wrongly applied
        // For now don't modify anything, but use it in your code so we can fix it later
        return translation;
        //return new Vector3(-translation.X, translation.Y, translation.Z);
    }
}
