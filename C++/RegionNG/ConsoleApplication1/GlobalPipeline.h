#include <iostream>
#include <queue>
#include <functional>
#include <memory>
#include <unordered_map>
#include <mutex>
#include <thread>
#include <atomic>


// Enum for pipeline types
enum class GlobalPipelineType {
    LOGIC_THREAD = 1,
    DB_THREAD = 2,
    TIMER_THREAD = 3
};

// Class representing a pipeline job
class PipelineJob {
public:
    int key;
    GlobalPipelineType type;
    std::function<bool(int)> action;

    bool Exec() {
        return action ? action(0) : false;
    }
};


// GlobalPipeline class
class GlobalPipeline {
public:
    void PushJob(int key, GlobalPipelineType type, std::function<bool(int)> action);
    void FailJob(int key, std::function<bool(int)> action);
    void BatchJob();

private:
    void ExecJob(std::shared_ptr<PipelineJob> job);
    void ExecJobForLogicThread(std::shared_ptr<PipelineJob> job);
    void ExecJobForDBThread(std::shared_ptr<PipelineJob> job);
    void ExecJobForTimer(std::shared_ptr<PipelineJob> job);
    bool InternalExecJob(std::shared_ptr<PipelineJob> job);

    std::queue<std::shared_ptr<PipelineJob>> queue;
    std::shared_ptr<PipelineJob> failJob;
    std::recursive_mutex queueMutex;
};

