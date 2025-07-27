#pragma once

#include <iostream>
#include <functional>
#include <queue>
#include <unordered_map>
#include <vector>
#include <thread>
#include <mutex>
#include <atomic>
#include <chrono>
#include <memory>
#include "TimerThread.h"
#include "ProcessQueue.h"
#include <Windows.h>
#include "Util.h"
#include <print>

struct ActionMsg {

    std::function<void(int)> action;

    void Exec() {
        if (action) {
            action(0);
        }
    }
};


class MessageWorker {
private:
    ProcessQueue<ActionMsg> processQueue;

    using Action = std::function<void(int)>;

    static void MessageProc(std::shared_ptr<ActionMsg> actionMsg) {
        actionMsg->Exec();
    }

public:
    MessageWorker() 
        : processQueue(MessageWorker::MessageProc, 1, 1024, "MessageWorker")
    {}

    void EnqueueLambda(Action pAction);
    void AddOneTimeTimer(int64_t delayMs, Action pAction);
    void AddPeriodicTimer(int64_t delayMs, Action pAction);
};


class MessageWorkerPack {
private:
    std::unordered_map<int, std::shared_ptr<MessageWorker>> messageWorkerMap;
    std::vector<int> threadIdList;
    std::mutex lock;
    std::atomic<long> remainCount;

public:
    MessageWorkerPack(int threadCount);

    std::shared_ptr<MessageWorker> GetWorker(int key);
};
