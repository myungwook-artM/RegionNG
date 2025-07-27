#include <memory>
#include <thread>
#include <stack>
#include <mutex>
#include <iostream>
#include "SingleMemoryPool.h"

// A thread-local memory class template
template <typename T>
class ThreadLocalMemory {
public:
    ThreadLocalMemory() = default;

    // Get the instance for the current thread
    T& Instance() {
        if (!instance) {
            instance = std::make_unique<T>();
        }
        return *instance;
    }

private:
    thread_local static std::unique_ptr<T> instance;
};

// Definition of the thread_local instance
template <typename T>
thread_local std::unique_ptr<T> ThreadLocalMemory<T>::instance = nullptr;


template <typename T>
class ThreadMemoryPool {
public:
    static void Push(std::shared_ptr<T> value) {
        GetThreadLocalPool().Return(value);
    }

    static std::shared_ptr<T> Pop() {
        return GetThreadLocalPool().Get();
    }

private:
    static ObjectPool<T>& GetThreadLocalPool() {
        return threadLocalMemory.Instance();
    }

    static ThreadLocalMemory<ObjectPool<T>> threadLocalMemory;
};

// Definition of the static member
template <typename T>
ThreadLocalMemory<ObjectPool<T>> ThreadMemoryPool<T>::threadLocalMemory;