
using System;
using System.Threading;
using System.Threading.Tasks;

public class ManagedThread
{
	private Thread? _thread;
	private string? _name;
	private int _threadId;

	public static void CreateThread ( ThreadStart startFunc, string name, int idx = 0)
	{
		var thread = new ManagedThread();
		thread._name = name ;

		thread._thread = new Thread(() =>
		{
			thread.InitThread();
			startFunc.Invoke();		
		});
			
		thread._thread.Name = name;
		thread._thread.Start();
			
	}
		
	private void InitThread()
	{
		_threadId = Thread.CurrentThread.ManagedThreadId;
	}



}
