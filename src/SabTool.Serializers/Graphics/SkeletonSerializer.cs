using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Newtonsoft.Json;

using SharpGLTF.Transforms;

namespace SabTool.Serializers.Graphics
{
    using Data.Graphics;
    using Json.Converters;
    using Utils.Extensions;

    public static class SkeletonSerializer
    {
        public static Skeleton DeserializeRaw(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var skeleton = new Skeleton
            {
                SomeSize = reader.ReadInt32(),
                Int4 = reader.ReadInt32(),
                Int8 = reader.ReadInt32(),
                IntC = reader.ReadInt32(),
                SomeSize2 = reader.ReadInt32(),
                Int14 = reader.ReadInt32(),
                Int18 = reader.ReadInt32(),
                Int1C = reader.ReadInt32(),
                Int20 = reader.ReadInt32(),
                Int24 = reader.ReadInt32()
            };

            stream.Position += 0x4;

            // Unused/unknown bone indexes, overwritten almost instantly after loading
            stream.Position += skeleton.NumBones;

            // Unused/unknown data, found to be zeros all the time
            stream.Position += skeleton.SomeSize;

            skeleton.BasePoses = new Matrix4x4[skeleton.NumBones];
            for (var i = 0; i < skeleton.NumBones; ++i)
            {
                skeleton.BasePoses[i] = reader.ReadMatrix4x4();
            }

            skeleton.Bones = new Bone[skeleton.NumBones];
            for (var i = 0; i < skeleton.NumBones; ++i)
            {
                skeleton.Bones[i] = BoneSerializer.DeserializeRaw(stream);
                skeleton.Bones[i].Index = (short)i;
            }

            skeleton.UnkBasePoses = new AffineTransform[skeleton.NumBones];
            for (var i = 0; i < skeleton.NumBones; ++i)
                skeleton.UnkBasePoses[i] = reader.ReadAffineTransform();

            skeleton.Indices = new short[skeleton.NumBones];
            for (var i = 0; i < skeleton.NumBones; ++i)
                skeleton.Indices[i] = reader.ReadInt16();

            stream.Position += skeleton.SomeSize2;

            // Pointer array without data to skip allocation
            stream.Position += 4 * skeleton.NumBones;

            return skeleton;
        }

        public static void SerializeRaw(Skeleton skeleton, Stream stream)
        {

        }

        public static Skeleton DeserializeJSON(Stream stream)
        {
            return null;
        }

        public static void SerializeJSON(Skeleton skeleton, Stream stream)
        {
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            writer.Write(JsonConvert.SerializeObject(skeleton, Formatting.Indented, new CrcConverter()));
        }
    }
}
