using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class ThreadLocalMemory<T> where T : class, new()
    {
        public T Instance => _instance.Value;

        private ThreadLocal<T> _instance = new ThreadLocal<T>(() =>
        {
            var instance = new T();
            return instance;
        });

    }

    public class ThreadMemoryPool<T> where T : class, new()
    {
        public static ThreadLocalMemory<ObjectPool<T>> _threadLocalMemory = new();

        public static void Push(T value)
        {
            _threadLocalMemory.Instance.Return(value);
        }

        public static T Pop()
        {
            return _threadLocalMemory.Instance.Get();
        }

    }

}
