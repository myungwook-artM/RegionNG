using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class Singleton<T> where T : class , new()
    {
        public static readonly Lazy<T> lazy = new Lazy<T>(() => new T());
        public static T Instance {  get { return lazy.Value; } }
    }
}
