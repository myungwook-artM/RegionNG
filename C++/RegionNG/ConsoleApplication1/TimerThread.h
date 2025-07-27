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
#include <list>

using namespace std::chrono;

class IOTimerBase  : public std::enable_shared_from_this<IOTimerBase> {
public:
    using TimerCallback = std::function<bool(int)>;

    virtual ~IOTimerBase() = default;
    virtual void TimerExpired(int id) = 0;

    TimerCallback func;
    milliseconds delayMs{ 0 };
    time_point<steady_clock> execTime;
    time_point<steady_clock> endTime = time_point<steady_clock>::max();
};


class ProcessTimer {
public:
    static void InitProcessTimer();
    static void StopThread();
    static void AddTimer(std::shared_ptr<IOTimerBase> timerObject);
    static void AddPeriodicTimer(milliseconds delayMs, IOTimerBase::TimerCallback timerFunc);
    static void AddOneTimeTimer(milliseconds delayMs, IOTimerBase::TimerCallback timerFunc);

private:
     
    struct TimerComparator {
        bool operator()(const std::shared_ptr<IOTimerBase>& lhs, const std::shared_ptr<IOTimerBase>& rhs) const {
            return lhs->execTime > rhs->execTime;
        }
    };

    static void TimerCheckerThreadProc();
    static void ProcessQueueThread(std::shared_ptr<IOTimerBase> timerObject);


    static inline std::priority_queue<std::shared_ptr<IOTimerBase>, std::vector<std::shared_ptr<IOTimerBase>>, TimerComparator> timerQueue;
    static inline std::recursive_mutex queueMutex;
    static inline std::atomic<bool> isRunning = false;
    static inline std::thread timerThread;
    static inline std::queue<std::shared_ptr<IOTimerBase>> _bufferQueue;
    static inline time_point<steady_clock> topTime = time_point<steady_clock>::max();
    //static inline ProcessQueue<std::shared_ptr<IOTimerBase>> processQueue = ProcessQueue<std::shared_ptr<IOTimerBase>>(
    //    ProcessQueueThread,1, 1024, "TimerThread");

};
