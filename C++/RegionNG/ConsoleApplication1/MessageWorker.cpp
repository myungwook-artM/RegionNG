#include "MessageWorker.h"



void MessageWorker::EnqueueLambda(Action pAction)
{
    processQueue.Enqueue(std::make_shared<ActionMsg>(pAction));
}

void MessageWorker::AddOneTimeTimer(int64_t delayMs, Action pAction) {
    if (delayMs > 0) {
        ProcessTimer::AddOneTimeTimer(milliseconds(delayMs), [this, pAction](int id) {
            AddOneTimeTimer(0, pAction);
            return true;
            });
        return;
    }

    EnqueueLambda(pAction);
}

void MessageWorker::AddPeriodicTimer(int64_t delayMs, Action pAction)
{
    ProcessTimer::AddPeriodicTimer(milliseconds(delayMs), [this, pAction](int id) {
        EnqueueLambda(pAction);
        return true;
        });
}


MessageWorkerPack::MessageWorkerPack(int threadCount)
{
    remainCount = threadCount;
    for (int i = 0; i < threadCount; ++i)
    {
        auto msgWorker = std::make_shared<MessageWorker>();

        msgWorker->EnqueueLambda([this, msgWorker](int Id) {
            std::lock_guard<std::mutex> guard(lock);
            int threadId = Util::GetCurrentThreadId();
            messageWorkerMap.insert(std::make_pair(threadId, msgWorker));
            threadIdList.push_back(threadId);
            remainCount--;

            //std::println("CreateMessageWorker id:{}", threadId);
            });
    }
}

std::shared_ptr<MessageWorker> MessageWorkerPack::GetWorker(int key)
{
    while (remainCount != 0) {
        std::this_thread::sleep_for(milliseconds(10));
    }

    std::lock_guard<std::mutex> guard(lock);
    int threadId = threadIdList[key % threadIdList.size()];
    return messageWorkerMap[threadId];
}
