using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace numericsbase.utils
{
    public class WorkConsumer: IDisposable
    {
        private ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        private ManualResetEvent queuehasdataevent = new ManualResetEvent(false);
        private ManualResetEvent emptyqueueevent = new ManualResetEvent(true);
        private int number_of_threads;
        private List<Task> workers = new List<Task>();        
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        private void consume(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                queuehasdataevent.WaitOne(500);
                Action f;

                if (queue.IsEmpty)
                {
                    queuehasdataevent.Reset();
                    emptyqueueevent.Set();
                }
                else if (queue.TryDequeue(out f))
                {
                    f();
                }
                
            }
            
        }
        
        public WorkConsumer(int number_threads)
        {
            var token = tokenSource.Token;
            number_of_threads = number_threads;
            for(int i = 0; i < number_threads; ++i)
            {
                workers.Add(Task.Factory.StartNew(()=>consume(token),token));
            }
        }
        public void Enqueue(Action pf)
        {            
            queue.Enqueue(pf);
            queuehasdataevent.Set();
            emptyqueueevent.Reset();
        }

        public void waitforEmptyQueue()
        {
            emptyqueueevent.WaitOne();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        tokenSource.Cancel();
                        Task.WaitAll(workers.ToArray());
                    }
                    catch (AggregateException e)
                    {
                        Console.WriteLine("\nAggregateException thrown with the following inner exceptions:");
                        // Display information about each exception. 
                        foreach (var v in e.InnerExceptions)
                        {
                            if (v is TaskCanceledException)
                                Console.WriteLine("   TaskCanceledException: Task {0}",
                                                  ((TaskCanceledException)v).Task.Id);
                            else
                                Console.WriteLine("   Exception: {0}", v.GetType().Name);
                        }
                        Console.WriteLine();
                    }
                    finally
                    {
                        tokenSource.Dispose();
                    }

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
