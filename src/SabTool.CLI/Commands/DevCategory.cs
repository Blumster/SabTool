namespace SabTool.CLI.Commands;

using SabTool.CLI.Base;

public sealed class DevCategory : BaseCategory
{
    public override string Key => "dev";
    public override string Shortcut => "d";
    public override string Usage => "<sub command>";

    public sealed class GenerateContainerSourceCommand : BaseCommand
    {
        public override string Key { get; } = "generate-container-source";
        public override string Shortcut { get; } = "gcs";
        public override string Usage { get; } = "<container name> <structure name>";

        public override bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() < 2)
            {
                Console.WriteLine("ERROR: Not enough arguments given!");
                return false;
            }

            switch (arguments.ElementAt(0).ToLowerInvariant())
            {
                case "pbllistinv":
                    EchoPblListInv(arguments.ElementAt(1));
                    break;

                case "pbltree":
                    EchoPblTree(arguments.ElementAt(1), arguments.ElementAt(2));
                    break;

                case "pblqueue":
                    EchoPblQueue(arguments.ElementAt(1), arguments.ElementAt(2));
                    break;

                case "pbldynarray":
                    EchoPblDynArray(arguments.ElementAt(1), arguments.ElementAt(2), arguments.ElementAt(3));
                    break;
            }

            return true;
        }

        private static void EchoPblListInv(string structType)
        {
            var originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblListInv_{structType}_::Node");
            Console.WriteLine("{");
            Console.WriteLine($"  PblListInv_{structType}_::Node* _pNext;");
            Console.WriteLine($"  PblListInv_{structType}_::Node* _pPrev;");
            Console.WriteLine($"  {originalStructType}* _pObject;");
            Console.WriteLine("};");
            Console.WriteLine();
            Console.WriteLine($"struct __cppobj PblListInv_{structType}_");
            Console.WriteLine("{");
            Console.WriteLine($"  PblListInv_{structType}_::Node _head;");
            Console.WriteLine("  volatile int _iCount;");
            Console.WriteLine("};");
        }

        private static void EchoPblTree(string structType, string keyType)
        {
            var originalKeyType = keyType;

            keyType = keyType.Replace(':', '_');
            keyType = keyType.Replace('<', '_');
            keyType = keyType.Replace('>', '_');
            keyType = keyType.Replace("*", "Ptr");

            var originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblTree_{structType}_{keyType}_1_PblCriticalSection_::Node : PblRedBlackNode");
            Console.WriteLine("{");
            Console.WriteLine($"  {originalKeyType} _key;");
            Console.WriteLine($"  {originalStructType} _data;");
            Console.WriteLine("};");
            Console.WriteLine();
            Console.WriteLine($"struct __cppobj PblTree_{structType}_{keyType}_1_PblCriticalSection_;");
            Console.WriteLine();
            Console.WriteLine($"struct /*VFT*/ PblTree_{structType}_{keyType}_1_PblCriticalSection__vtbl");
            Console.WriteLine("{");
            Console.WriteLine($"  void (__thiscall *DoNothing)(PblTree_{structType}_{keyType}_1_PblCriticalSection_ *this);");
            Console.WriteLine("};");
            Console.WriteLine($"struct __cppobj PblTree_{structType}_{keyType}_1_PblCriticalSection_");
            Console.WriteLine("{");
            Console.WriteLine($"  PblTree_{structType}_{keyType}_1_PblCriticalSection__vtbl *__vftable /*VFT*/;");
            Console.WriteLine("  unsigned int _count;");
            Console.WriteLine($"  PblTree_{structType}_{keyType}_1_PblCriticalSection_::Node *_pHead;");
            Console.WriteLine("  unsigned int _ShouldCallDeleteOnNodes;");
            Console.WriteLine("  PblCriticalSection m_Section;");
            Console.WriteLine("};");
        }

        private static void EchoPblQueue(string structType, string size)
        {
            var originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblQueue_{structType}_{size}_");
            Console.WriteLine("{");
            Console.WriteLine("  bool _bFull;");
            Console.WriteLine("  int _iStart;");
            Console.WriteLine("  int _iEnd;");
            Console.WriteLine($"  {originalStructType} _Storage[{size}];");
            Console.WriteLine("};");
        }

        private static void EchoPblDynArray(string structType, string unk1, string unk2)
        {
            var originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblDynArrayBase_{structType}_unsigned_short_{unk1}_{unk2}_");
            Console.WriteLine("{");
            Console.WriteLine($"  {originalStructType}* _pArray;");
            Console.WriteLine("  unsigned __int16 _Count;");
            Console.WriteLine("  unsigned __int16 _Capacity;");
            Console.WriteLine("  unsigned __int32 : 24;");
            Console.WriteLine("  __int32 _MemPool : 8;");
            Console.WriteLine("};");
            Console.WriteLine();
            Console.WriteLine($"struct __cppobj PblDynArray_{structType}_{unk1}_{unk2}_ : PblDynArrayBase_{structType}_unsigned_short_{unk1}_{unk2}_");
            Console.WriteLine("{");
            Console.WriteLine("};");
        }
    }
}
