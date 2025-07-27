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
/// �޸� �׽�Ʈ
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
/// ���μ��� ť �׽�Ʈ
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
/// Ÿ�̸� �׽�Ʈ
/// </summary>


void TimerTest()
{
    std::println("TimerTest--------------------------------------");

	// Ÿ�̸� ������ ����
	ProcessTimer::InitProcessTimer();

    // 2�� ���� �����ϴ� Ÿ�̸� �߰�
    ProcessTimer::AddOneTimeTimer(milliseconds(2000), [](int id) 
    {
        std::println("2000ms OneTimeTimer");
        return true;
    });

    // 1�� ���� �����ϴ� Ÿ�̸� �߰�
    ProcessTimer::AddOneTimeTimer(milliseconds(1000), [](int id) 
    {
        std::println("1000ms OneTimeTimer");
        return true;
    });

    // 1�ʸ��� �ݺ��ϴ� Ÿ�̸� �߰�
    ProcessTimer::AddPeriodicTimer(milliseconds(1000), [](int id)
    {
        std::println("1000ms PeriodicTimer");
        return true;
    });

    // 5�� ���� ProcessTimer ���� �ǵ��� ��
    ProcessTimer::AddOneTimeTimer(milliseconds(5000), [](int id) 
    {
        //ProcessTimer::StopThread();
        return true;
    });

}


/// <summary>
/// ������ ���������� �׽�Ʈ
/// </summary>

struct SyncObject
{
    int id;
};

// ����, DB, Timer �����忡�� SyncObject ��ü�� ������� ó����
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


