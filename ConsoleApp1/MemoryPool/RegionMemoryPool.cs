using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{

    public class ObjectPoolBucket<T> where T : class, new()
    {
        List<T> _bucketList = new List<T>();
        ObjectPool<T> _objectPool = new ObjectPool<T>();

        public T Pop()
        {
            var obj =_objectPool.Get();
            _bucketList.Add(obj);
            return obj;
        }

        public void Return()
        {
            foreach( var obj in _bucketList)
            {
                _objectPool.Return(obj);
            }
        }

    }
        
    public class RegionMemoryPool<T> where T : class ,new()
    {
        [ThreadStatic]
        private static ObjectPoolBucket<T> _bucketPool;

        private static ObjectPoolBucket<T> BucketPool
        {
            get
            {
                if (_bucketPool == null)
                {
                    _bucketPool = new ObjectPoolBucket<T>();
                }
                return _bucketPool;
            }
        }

    }
}
