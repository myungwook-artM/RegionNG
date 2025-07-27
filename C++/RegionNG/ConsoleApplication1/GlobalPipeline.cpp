#include "GlobalPipeline.h"
#include "SystemThread.h"


void GlobalPipeline::PushJob(int key, GlobalPipelineType type, std::function<bool(int)> action)
{
    std::lock_guard<std::recursive_mutex> lock(queueMutex);
    queue.emplace(std::make_shared<PipelineJob>(PipelineJob{ key, type, action }));
}

void GlobalPipeline::FailJob(int key, std::function<bool(int)> action)
{
    failJob = std::make_shared<PipelineJob>(PipelineJob{ key, GlobalPipelineType::LOGIC_THREAD, action });
}

void GlobalPipeline::BatchJob()
{
    std::shared_ptr<PipelineJob> job;
    {
        std::lock_guard<std::recursive_mutex> lock(queueMutex);
        if (queue.empty()) {
            return;
        }
        job = queue.front();
        queue.pop();
    }
    ExecJob(job);
}


void GlobalPipeline::ExecJob(std::shared_ptr<PipelineJob> job)
{
    if (!job) {
        return;
    }

    switch (job->type)
    {
    case GlobalPipelineType::LOGIC_THREAD:
        ExecJobForLogicThread(job);
        break;
    case GlobalPipelineType::DB_THREAD:
        ExecJobForDBThread(job);
        break;
    case GlobalPipelineType::TIMER_THREAD:
        ExecJobForTimer(job);
        break;
    }
}

void GlobalPipeline::ExecJobForLogicThread(std::shared_ptr<PipelineJob> job) {

    auto worker = LogicThread::Instance().GetWorker(job->key);
    worker->EnqueueLambda([this, job](int id) {
        InternalExecJob(job);
        });
}

void GlobalPipeline::ExecJobForDBThread(std::shared_ptr<PipelineJob> job) {
    auto worker = DBThread::Instance().GetWorker(job->key);
    worker->EnqueueLambda([this, job](int id) {
        InternalExecJob(job);
        });

}

void GlobalPipeline::ExecJobForTimer(std::shared_ptr<PipelineJob> job) {
    ProcessTimer::AddOneTimeTimer(milliseconds(1), [this, job](int id) {
        InternalExecJob(job);
        return true;
        });
}

bool GlobalPipeline::InternalExecJob(std::shared_ptr<PipelineJob> job) {
    if (!job->Exec()) {
        if (failJob) {
            failJob->Exec();
        }
        return false;
    }
    BatchJob();
    return true;
}

