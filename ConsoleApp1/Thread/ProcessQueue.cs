
using System;
using System.Collections.Generic;



public class ProcessQueue<T> 
{
	private Queue<T> _queue;
	private Action<T> _action;
	private AutoResetEvent _event = new AutoResetEvent(false);
    private object _lock = new object();
    public int Count => _queue.Count;

    public ProcessQueue(Action<T> lamdaFunc, int threadCount, int capacity, string name)
    {
        _queue = new Queue<T>(capacity);
        _action = lamdaFunc;
        InitThread(threadCount, name);
    }

    public void InitThread(int threadCount, string queueName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            var thread = new ProcessQueue<T>.WorkerThread(this, queueName);
        }
    }
   

	/// <summary>
	/// 
	/// </summary>
	private void WaitEvent()
	{
        _event.WaitOne();
	}

    private void ProcessItems()
    {
        while (TryDequeue(out var item))
        {
            ProcessItem(item);
        }
    }
    private bool TryDequeue(out T item)
    {
        lock (_lock)
        {
            if (_queue.Count > 0)
            {
                item = _queue.Dequeue();
                return true;
            }
            else
            {
                item = default(T);
                return false;
            }
        }
    }
    private void ProcessItem(T item)
    {
        _action?.Invoke(item);
    }


    public void Enqueue(T item)
	{
		lock (_lock)
		{
			_queue.Enqueue(item);
		}
		_event.Set();
	}

   
    public class WorkerThread
	{
		private ProcessQueue<T> _queue;

		public WorkerThread(ProcessQueue<T> queue, string typeName)
		{
			_queue = queue;
			ManagedThread.CreateThread(ThreadProc, $"ProcessQueue({typeName})");
		}

		private void ThreadProc()
		{
			while(true)
			{
				if(_queue.Count == 0 )
				{
					_queue.WaitEvent();
					continue;
				}
				_queue.ProcessItems();
			}
		}
    }
    

	
}

