
using SabTool.CLI.Base;

namespace SabTool.CLI.Commands;
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
                case "pbllist":
                    EchoPblList(arguments.ElementAt(1));
                    break;

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

                case "pblsortedlistinv":
                    EchoPblSortedListInv(arguments.ElementAt(1));
                    break;

                default:
                    Console.WriteLine($"Unknown container type: {arguments.ElementAt(0)}");
                    break;
            }

            return true;
        }

        private static void EchoPblList(string structType)
        {
            string originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace(' ', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblList_{structType}_::Node");
            Console.WriteLine("{");
            Console.WriteLine($"  {originalStructType} _data;");
            Console.WriteLine($"  PblList_{structType}_::Node* _pNext;");
            Console.WriteLine($"  PblList_{structType}_::Node* _pPrev;");
            Console.WriteLine("};");
            Console.WriteLine();
            Console.WriteLine($"struct __cppobj PblList_{structType}_");
            Console.WriteLine("{");
            Console.WriteLine("  int _count;");
            Console.WriteLine($"  PblList_{structType}_::Node* _pLast;");
            Console.WriteLine($"  PblList_{structType}_::Node* _pFirst;");
            Console.WriteLine("};");
        }

        private static void EchoPblListInv(string structType)
        {
            string originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace(' ', '_');
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

        private static void EchoPblSortedListInv(string structType)
        {
            string originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace(' ', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblSortedListInv_{structType}_::Node");
            Console.WriteLine("{");
            Console.WriteLine($"  PblSortedListInv_{structType}_::Node* _pNext;");
            Console.WriteLine($"  PblSortedListInv_{structType}_::Node* _pPrev;");
            Console.WriteLine($"  {originalStructType}* _pObject;");
            Console.WriteLine("};");
            Console.WriteLine();
            Console.WriteLine($"struct __cppobj PblSortedListInv_{structType}_");
            Console.WriteLine("{");
            Console.WriteLine($"  PblSortedListInv_{structType}_::Node _head;");
            Console.WriteLine("  int _iCount;");
            Console.WriteLine("};");
        }

        private static void EchoPblTree(string structType, string keyType)
        {
            string originalKeyType = keyType;

            keyType = keyType.Replace(':', '_');
            keyType = keyType.Replace('<', '_');
            keyType = keyType.Replace('>', '_');
            keyType = keyType.Replace(' ', '_');
            keyType = keyType.Replace("*", "Ptr");

            string originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace(' ', '_');
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
            string originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace(' ', '_');
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
            string originalStructType = structType;

            structType = structType.Replace(':', '_');
            structType = structType.Replace('<', '_');
            structType = structType.Replace('>', '_');
            structType = structType.Replace(' ', '_');
            structType = structType.Replace("*", "Ptr");

            Console.WriteLine($"struct __cppobj PblDynArrayBase_{structType}_unsigned_short_{unk1}_{unk2}_");
            Console.WriteLine("{");
            Console.WriteLine($"  {originalStructType}* Array;");
            Console.WriteLine("  unsigned __int16 Count;");
            Console.WriteLine("  unsigned __int16 Capacity;");
            Console.WriteLine("  _BYTE MemPool;");
            Console.WriteLine("  _BYTE gap[3];");
            //Console.WriteLine("  unsigned __int32 : 24;");
            //Console.WriteLine("  __int32 MemPool : 8;");
            Console.WriteLine("};");
            Console.WriteLine();
            Console.WriteLine($"struct __cppobj PblDynArray_{structType}_{unk1}_{unk2}_ : PblDynArrayBase_{structType}_unsigned_short_{unk1}_{unk2}_");
            Console.WriteLine("{");
            Console.WriteLine("};");
        }
    }
}
