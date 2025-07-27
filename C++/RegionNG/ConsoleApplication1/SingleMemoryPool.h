#pragma once

#include <iostream>
#include <string>
#include <thread>
#include <queue>
#include <mutex>

template <typename T>
class ObjectPool {
    
public:
    std::shared_ptr<T> Get() {
        std::lock_guard<std::mutex> lock(_mutex);
        if (!_queue.empty()) {
            auto obj = _queue.front();
            _queue.pop();
            return obj;
        }
        return std::make_shared<T>();
    }

    void Return(std::shared_ptr<T> obj) {
        std::lock_guard<std::mutex> lock(_mutex);
        _queue.push(obj);
    }

    ~ObjectPool() {
        while (!_queue.empty()) {
            //auto obj = _queue.front();
            _queue.pop();
        }
    }

private:
    std::queue<std::shared_ptr<T>> _queue;
    std::mutex _mutex;
};

// SingleMemoryPool template class
template <typename T>
class SingleMemoryPool {

public:
    static void Push(std::shared_ptr<T> value) {
        std::lock_guard<std::mutex> lock(_mutex);
        _objectPool.Return(value);
    }

    static std::shared_ptr<T> Pop() {
        std::lock_guard<std::mutex> lock(_mutex);
        return _objectPool.Get();
    }

private:
    static ObjectPool<T> _objectPool;
    static std::mutex _mutex;
};

// Static member initialization
template <typename T> ObjectPool<T> SingleMemoryPool<T>::_objectPool;
template <typename T> std::mutex SingleMemoryPool<T>::_mutex;






