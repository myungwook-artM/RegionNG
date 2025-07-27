#include "TimerThread.h"



class TimerPermanent : public IOTimerBase {
public:
    void TimerExpired(int id) override
    {
        throw std::logic_error("Not implemented");
    }
};

class TimerOneTime : public IOTimerBase {
public:
    void TimerExpired(int id) override
    {
        if (func) {
            func(id);
        }
    }

};

class TimerPeriodic : public IOTimerBase {
public:
    void TimerExpired(int id) override
    {
        if (func) {
            func(id);
        }

        execTime = steady_clock::now() + delayMs;
        ProcessTimer::AddTimer(shared_from_this());
    }
};


void ProcessTimer::InitProcessTimer()
{
    auto permanentTimer = std::make_shared<TimerPermanent>();
    permanentTimer->execTime = time_point<steady_clock>::max();
    _bufferQueue.push(permanentTimer);

    isRunning = true;
    timerThread = std::thread(&ProcessTimer::TimerCheckerThreadProc);
}

void ProcessTimer::StopThread()
{
    isRunning = false;
}

void ProcessTimer::AddTimer(std::shared_ptr<IOTimerBase> timerObject)
{
    std::lock_guard<std::recursive_mutex> lock(queueMutex);
    timerObject->execTime = steady_clock::now() + timerObject->delayMs;
    _bufferQueue.push(timerObject);
}

void ProcessTimer::AddPeriodicTimer(milliseconds delayMs, IOTimerBase::TimerCallback timerFunc)
{
    auto periodicTimer = std::make_shared<TimerPeriodic>();
    periodicTimer->delayMs = delayMs;
    periodicTimer->func = timerFunc;
    AddTimer(periodicTimer);
}

void ProcessTimer::AddOneTimeTimer(milliseconds delayMs, IOTimerBase::TimerCallback timerFunc)
{
    auto oneTimeTimer = std::make_shared<TimerOneTime>();
    oneTimeTimer->delayMs = delayMs;
    oneTimeTimer->func = timerFunc;
    AddTimer(oneTimeTimer);
}


void ProcessTimer::TimerCheckerThreadProc() {

    while (isRunning) {

        std::lock_guard<std::recursive_mutex> lock(queueMutex);

        time_point<steady_clock> nowTime = steady_clock::now();
        std::shared_ptr<IOTimerBase> timerObj;

        while (true)
        {
            if (_bufferQueue.size() <= 0)
            {
                break;
            }

            timerObj = _bufferQueue.front();
            _bufferQueue.pop();

            if (timerObj->execTime <= nowTime)
            {
                ProcessQueueThread(timerObj);
                continue;
            }
            else
            {
                timerQueue.push(timerObj);

                if (topTime > timerObj->execTime)
                {
                    topTime = timerObj->execTime;
                }
            }
        }

        if (topTime <= nowTime)
        {
            while (true)
            {
                auto topObj = timerQueue.top();
                timerQueue.pop();
                ProcessQueueThread(topObj);

                topObj = timerQueue.top();
                if (topObj->execTime > nowTime)
                {
                    topTime = topObj->execTime;
                    break;
                }
            }
        }

        std::this_thread::sleep_for(milliseconds(1));

    }
}


void ProcessTimer::ProcessQueueThread(std::shared_ptr<IOTimerBase> timerObject)
{
    timerObject->TimerExpired(0);
}
