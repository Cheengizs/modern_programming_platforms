using System;
using System.Collections.Generic;
using System.Threading;

namespace TestRunner.Threading;

public class CustomThreadPool : IDisposable
{
    private class PoolTask
    {
        public Action Action { get; set; } = null!;
        public DateTime EnqueuedAt { get; set; }
    }

    private readonly Queue<PoolTask> _tasks = new Queue<PoolTask>();
    private readonly object _lock = new object();
    private readonly int _minThreads;
    private readonly int _maxThreads;
    
    private int _totalThreads = 0;
    private int _activeThreads = 0;
    private bool _isDisposed = false;
    private readonly Timer _monitorTimer;

    public CustomThreadPool(int minThreads, int maxThreads)
    {
        _minThreads = minThreads;
        _maxThreads = maxThreads;

        for (int i = 0; i < _minThreads; i++) StartNewThread();

        _monitorTimer = new Timer(MonitorStatus, null, 500, 500);
    }

    public void EnqueueTask(Action taskAction)
    {
        lock (_lock)
        {
            if (_isDisposed) return;

            _tasks.Enqueue(new PoolTask { Action = taskAction, EnqueuedAt = DateTime.Now });

            if (_activeThreads == _totalThreads && _totalThreads < _maxThreads)
            {
                StartNewThread();
            }
            Monitor.Pulse(_lock);
        }
    }

    private void StartNewThread()
    {
        _totalThreads++;
        var thread = new Thread(WorkerLoop) { IsBackground = true };
        thread.Start();
    }

    private void WorkerLoop()
    {
        while (true)
        {
            PoolTask task;
            lock (_lock)
            {
                while (_tasks.Count == 0)
                {
                    if (_isDisposed)
                    {
                        _totalThreads--;
                        return;
                    }

                    bool signaled = Monitor.Wait(_lock, TimeSpan.FromSeconds(3));
                    if (!signaled && _totalThreads > _minThreads)
                    {
                        _totalThreads--; 
                        return;
                    }
                }
                task = _tasks.Dequeue();
                _activeThreads++;
            }

            try
            {
                task.Action();
            }
            catch (Exception ex)
            {
                TestReporter.PrintError($"[Thread Fault] Recovering from error: {ex.Message}");
                lock (_lock)
                {
                    _activeThreads--;
                    _totalThreads--;
                    if (_totalThreads < _minThreads) StartNewThread();
                }
                return; 
            }

            lock (_lock) { _activeThreads--; }
        }
    }

    private void MonitorStatus(object? state)
    {
        lock (_lock)
        {
            if (_isDisposed) return;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[POOL MONITOR] Threads: {_totalThreads}/{_maxThreads} | Active: {_activeThreads} | Queue: {_tasks.Count}");
            Console.ResetColor();

            if (_tasks.Count > 0 && (DateTime.Now - _tasks.Peek().EnqueuedAt).TotalMilliseconds > 1000)
            {
                if (_totalThreads < _maxThreads)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[POOL SCALING] Task waiting too long! Spawning emergency thread.");
                    Console.ResetColor();
                    StartNewThread();
                }
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _isDisposed = true;
            _monitorTimer.Dispose();
            Monitor.PulseAll(_lock);
        }
    }
}
