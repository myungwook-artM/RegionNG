// See https://aka.ms/new-console-template for more information
using RegionNG;
using RegionNG.World;

//Console.ReadLine();
// 메모리 풀 테스트
//MemoryPoolTest.Test();
//Console.ReadLine();

// 프로세스 큐 테스트 
// N개의 쓰레드가 하나의 큐에 있는 데이터를 동시 처리를 목적으로 개발
ProcessQueueTest.Test();
Console.ReadLine();

// 비동기 큐 테스트
ProcessQueueTest.TestAsync(1);
ProcessQueueTest.TestAsync(2);
Console.ReadLine();

// 타이머 테스트
// 1초 마다 실행, 혹시 3초 이후 한번 실행 같은 로직을 간단히 처리하는걸 목적으로 개발
TimerTest.Test();
Console.ReadLine();

// 글로벌 파이프 라인
// 여러 쓰레드의 잡업을 하나의 큐로 만들어 비동기 프로그래밍을 쉽게 가능하게 함

GlobalPipelineTest.Test();
Console.ReadLine();

// 월드 업데이트 테스트
WorldMap.Test();

/*****************************************************************
 * 호연 게임에서는 lockless 프로그래밍을 위해서 
 * 하나의 존에 있는 유저와 몬스터는 지정된 하나의 쓰레드 큐(ProcessQueueAsync) 에서만 로직을 처리한다. 
 * 그러므로 유저의 요청 패킷, 몬스터, 존 업데이트는 하나의 큐에서 순서대로 처리되므로 
 * 다른 쓰레드에서 메모리 접근이 안되며 Lock 없이 프로그래밍이 가능하다.
 *  
 * 서로 다른 쓰레드에 존재하는 유저의 처리가 필요한 경우
 * GlobalPipeline 객체를 사용하여 각 유저를 처리하는 쓰레드에 Job 을 넘겨서 처리하고 다시 받아 온다.
 * 
 *  
 * 타이머 쓰레드의 경우 주기적으로 실행해야 하는 객체 및 함수를
 * 직관적으로 지정할수 있으면 편리하게 사용하기 위한 목적으로 개발됨
 * 
 *  * 
 ******************************************************************/






