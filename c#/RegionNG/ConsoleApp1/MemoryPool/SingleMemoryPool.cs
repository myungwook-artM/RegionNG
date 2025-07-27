using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RegionNG
{

   
    public class ObjectPool<T> where T : class, new()
    {
        Queue<T> _queue = new Queue<T>();

        public T Get()
        {
            if (true == _queue.TryDequeue(out var obj))
                return obj;

            return new T();            
        }

        public void Return(T obj)
        {
             _queue.Enqueue(obj);
       
        }
    }


    public class SingleMemoryPool<T> where T : class, new()
    {
        private static readonly ObjectPool<T> _objectPool = new ObjectPool<T>();
        private static object _lock = new object();

        public SingleMemoryPool() { }

        public static void Push(T value)
        {
            lock (_lock)
            {
                _objectPool.Return(value);
            }
                
        }

        public static T Pop()
        {
            lock (_lock)
            {
                return _objectPool.Get();
            }
                
        }
    }
}
