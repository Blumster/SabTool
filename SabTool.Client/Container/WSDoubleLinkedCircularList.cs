namespace SabTool.Client.Container
{
    public class WSDoubleLinkedListEntry<T>
            where T : class
    {
        public WSDoubleLinkedListEntry<T> Previous { get; set; }
        public WSDoubleLinkedListEntry<T> Next { get; set; }
        public T Data { get; set; }
    }

    public class WSDoubleLinkedCircularList<T>
        where T : class
    {
        public WSDoubleLinkedListEntry<T> Head { get; set; }
        public uint Count { get; set; }

        public WSDoubleLinkedCircularList()
        {
            Head = new WSDoubleLinkedListEntry<T>();
            Head.Next = Head;
            Head.Previous = Head;

            Count = 0u;
        }
    }
}
