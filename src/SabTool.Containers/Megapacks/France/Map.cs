using System;
using System.IO;
using System.Text;

namespace SabTool.Containers.Megapacks.France
{
    using Utils.Extensions;

    public class Map : StreamingManager
    {
        public void Read(Stream stream)
        {
            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (!br.CheckHeaderString("MAP6", reversed: true))
                    throw new Exception("Invalid FourCC header!");

                var mapFileNameWithoutExtension = Path.GetFileNameWithoutExtension("France.map");

                var franceStr = br.ReadStringWithMaxLength(256);

                m_nTotalNumBlocks = br.ReadInt32();
                FieldDA84 = br.ReadInt32();

                var count = br.ReadInt32();
                if (count > 0)
                {
                    Console.WriteLine("Int: {0}", br.ReadInt32());

                    for (var i = 0; i < count; ++i)
                    {
                        var strLen = br.ReadInt32();
                        var str = br.ReadStringWithMaxLength(strLen + 1);

                        var v11Len = br.ReadInt32();
                        var v11Arr = br.ReadBytes(v11Len);
                        var unkArr = br.ReadBytes(48);
                        var v12Len = br.ReadInt32();
                        var v12Arr = br.ReadBytes(v12Len);

                        //Console.WriteLine("Str ({0}): {1}", strLen, str);
                        //Console.WriteLine("V11 ({0}): {1}", v11Len, Encoding.UTF8.GetString(v11Arr));
                        //Console.WriteLine("V12 ({0}): {1}", v12Len, Encoding.UTF8.GetString(v12Arr));
                        //Console.WriteLine("Unk: {0}", BitConverter.ToString(unkArr));
                    }
                }

                for (var i = 0; i < 3; ++i)
                {
                    float[] arr = new float[6];
                    arr[0] = br.ReadSingle();
                    arr[1] = br.ReadSingle();
                    arr[2] = br.ReadSingle();
                    arr[3] = br.ReadSingle();
                    arr[4] = br.ReadSingle();
                    arr[5] = br.ReadSingle();
                }

                for (var i = 0; i < 2; ++i)
                {
                    var s1 = br.ReadInt16();
                    var s2 = br.ReadInt16();
                }

                Field90 = br.ReadInt16();
                var sh2 = br.ReadInt16();

                Sub9F3900(br, m_nTotalNumBlocks, mapFileNameWithoutExtension, false);
                Sub9F3BF0(br, m_nTotalNumBlocks, mapFileNameWithoutExtension, false);
                Sub9F3FA0(br, m_nTotalNumBlocks, mapFileNameWithoutExtension, false);
            }
        }

        public void Write(Stream stream)
        {

        }
    }
}
