#include <print>
#include "MemoryPoolTest.h"
#include "SingleMemoryPool.h"
#include "ThreadMemoryPool.h"
#include "MemoryPoolTest.h"
#include "ProcessQueue.h"
#include "TimerThread.h"
#include "Util.h"
#include "GlobalPipeline.h"

/// <summary>
/// 메모리 테스트
/// </summary>
/// 
struct MemoryPoolItem
{
    /*
    ~MemoryPoolItem()
    {
        std::println("destroy MemroyPoolItem");
    }
    */
	int _idx = 0;
};


static void ExecFunc1(std::shared_ptr<MemoryPoolItem> item)
{
	auto singleObj = SingleMemoryPool<MemoryPoolItem>::Pop();
	SingleMemoryPool<MemoryPoolItem>::Push(singleObj);

	auto threadObj = ThreadMemoryPool<MemoryPoolItem>::Pop();
	ThreadMemoryPool<MemoryPoolItem>::Push(threadObj);

    std::println("idx:{} Id:{}", item->_idx, Util::GetCurrentThreadId());
}

void MemoryPoolTest()
{
	ProcessQueue<MemoryPoolItem> processQueue(ExecFunc1, 4, 100, "MemoryPoolTest");
	for (int i = 0; i < 100; i++)
	{
        processQueue.Enqueue(std::make_shared<MemoryPoolItem>(i));
	}
}


/// <summary>
/// 프로세스 큐 테스트
/// </summary>
struct TestItem
{
	int _idx = 0;
};

static void ExecFunc2(std::shared_ptr<TestItem> item)
{
	std::println("idx:{} Id:{}", item->_idx, Util::GetCurrentThreadId());
}

void ProcessQueueTest()
{
    std::println("ProcessQueueTest--------------------------------------");

	ProcessQueue<TestItem> processQueue(ExecFunc2, 4, 10, "ProcessQueueTest");

	for (int i = 0; i < 10; i++)
	{
		processQueue.Enqueue(std::make_shared<TestItem>(i));
	}

}


/// <summary>
/// 타이머 테스트
/// </summary>


void TimerTest()
{
    std::println("TimerTest--------------------------------------");

	// 타이머 쓰레드 생성
	ProcessTimer::InitProcessTimer();

    // 2초 이후 실행하는 타이머 추가
    ProcessTimer::AddOneTimeTimer(milliseconds(2000), [](int id) 
    {
        std::println("2000ms OneTimeTimer");
        return true;
    });

    // 1초 이후 실행하는 타이머 추가
    ProcessTimer::AddOneTimeTimer(milliseconds(1000), [](int id) 
    {
        std::println("1000ms OneTimeTimer");
        return true;
    });

    // 1초마다 반복하는 타이머 추가
    ProcessTimer::AddPeriodicTimer(milliseconds(1000), [](int id)
    {
        std::println("1000ms PeriodicTimer");
        return true;
    });

    // 5초 이후 ProcessTimer 종료 되도록 함
    ProcessTimer::AddOneTimeTimer(milliseconds(5000), [](int id) 
    {
        //ProcessTimer::StopThread();
        return true;
    });

}


/// <summary>
/// 쓰레드 파이프라인 테스트
/// </summary>

struct SyncObject
{
    int id;
};

// 로직, DB, Timer 쓰레드에서 SyncObject 객체를 순서대로 처리함
void GlobalPipelineTest()
{
    std::println("GlobalPipelineTest--------------------------------------");

    auto syncObj = new SyncObject(1);
    auto pipeline = new GlobalPipeline();

    pipeline->PushJob(1, GlobalPipelineType::LOGIC_THREAD, [syncObj](int id) {
        std::println("Executing logic thread job with ThreadId:{} SyncId:{}",Util::GetCurrentThreadId(), syncObj->id++);
        return true;
        });

    pipeline->PushJob(2, GlobalPipelineType::DB_THREAD, [syncObj](int id) {
        std::println("Executing DB thread job with ThreadId:{} SyncId:{}", Util::GetCurrentThreadId(), syncObj->id++);
        return true;
        });

    pipeline->PushJob(3, GlobalPipelineType::TIMER_THREAD, [syncObj](int id) {
        std::println("Executing timer thread job with ThreadId:{} SyncId:{}", Util::GetCurrentThreadId(), syncObj->id++);
        return true;
        });

    pipeline->BatchJob();

}


