#include <vector>
#include <memory>
#include <thread>
#include <mutex>


template <typename T>
class ObjectPool {
public:
    std::shared_ptr<T> Get() {
        std::lock_guard<std::mutex> lock(poolMutex);
        if (!pool.empty()) {
            auto obj = pool.back();
            pool.pop_back();
            return obj;
        }
        return std::make_shared<T>();
    }

    void Return(std::shared_ptr<T> obj) {
        std::lock_guard<std::mutex> lock(poolMutex);
        pool.push_back(std::move(obj));
    }

private:
    std::vector<std::shared_ptr<T>> pool;
    std::mutex poolMutex;
};



template <typename T>
class ObjectPoolBucket {
public:
    T* Pop() {
        auto obj = objectPool.Get();
        bucketList.push_back(obj);
        return obj.get();
    }

    void Return() {
        for (auto& obj : bucketList) {
            objectPool.Return(obj);
        }
        bucketList.clear();
    }

private:
    std::vector<std::shared_ptr<T>> bucketList;
    ObjectPool<T> objectPool;
};

// RegionMemoryPool class for thread-local object pooling
template <typename T>
class RegionMemoryPool {
public:
    static ObjectPoolBucket<T>& GetBucketPool() {
        static thread_local ObjectPoolBucket<T> bucketPool;
        return bucketPool;
    }
};
