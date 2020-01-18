using System;
using System.Collections.Generic;
using System.Text;

namespace SabTool.Utils.Misc
{
    public class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => new T());

        public static T Instance
        {
            get
            {
                return _lazyInstance.Value;
            }
        }

        protected Singleton()
        {
        }
    }
}
