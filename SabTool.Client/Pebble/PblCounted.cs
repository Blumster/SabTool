using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Client.Pebble
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class PblCounted<T>
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        private static uint _instanceCount = 0;

        public PblCounted()
        {
            ++_instanceCount;
        }

        ~PblCounted()
        {
            --_instanceCount;
        }
    }
}
