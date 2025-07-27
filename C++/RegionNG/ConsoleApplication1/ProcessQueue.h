#include <iostream>
#include <queue>
#include <vector>
#include <functional>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <chrono>
#include <atomic>
#include <memory>
#include <unordered_map>
#include <algorithm>
#include <string>
#include <deque>
#include <print>

template <typename T>
class ProcessQueue
{
public:
    using Action = std::function<void(std::shared_ptr<T>)>;

    ProcessQueue(Action action, int threadCount, int capacity, const std::string& name)
        : _action(action), _capacity(capacity), _name(name), _stop(false) {
        InitThreads(threadCount);
    }

    ~ProcessQueue() {
        {
            std::unique_lock<std::mutex> lock(_mutex);
            _stop = true;
        }
        _cond.notify_all();

        for (auto& thread : _threads) {
            if (thread.joinable()) {
                thread.join();
            }
        }
    }

    void Enqueue(std::shared_ptr<T> item) {
        {
            std::unique_lock<std::mutex> lock(_mutex);
            if (_queue.size() < _capacity) {
                _queue.push(item);
            }
            else {
                std::println("Queue is full, dropping item.");
                return;
            }
        }
        _cond.notify_one();
    }

private:
    void InitThreads(int threadCount) {
        for (int i = 0; i < threadCount; ++i) {
            _threads.emplace_back(&ProcessQueue::WorkerThreadProc, this);
        }
    }

    void WorkerThreadProc() {
        while (true) {
            std::shared_ptr<T> item;
            {
                std::unique_lock<std::mutex> lock(_mutex);
                _cond.wait(lock, [this]() { return !_queue.empty() || _stop; });

                if (_stop && _queue.empty()) {
                    return;
                }

                item = _queue.front();
                _queue.pop();
            }

            _action(item);
        }
    }

    Action _action;
    std::queue<std::shared_ptr<T>> _queue;
    std::vector<std::thread> _threads;
    std::mutex _mutex;
    std::condition_variable _cond;
    size_t _capacity;
    std::string _name;
    bool _stop;
};
