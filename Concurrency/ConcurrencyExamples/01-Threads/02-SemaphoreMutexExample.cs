using System.Threading;

namespace ConcurrencyExamples
{
    /// <summary>
    /// Semaphors are a more general form of mutex. A mutex is a new Semaphore(1,1)
    /// Semaphores allow you to have multiple threads accessing the same resource
    /// However you can constrain the number of concurrent threads that have access
    /// at any one time.
    /// </summary>
    /// <remarks>
    /// Personally I only want one thread to access a 'shared resource' at a time.
    /// If it isn't a value type everything gets really fugly quick.
    /// 
    /// </remarks>
    public class SemaphoreMutexExample
    {
        private Semaphore semaphor = new Semaphore(2, 8, "exampleSemaphor");
        private SomeSharedResource domainObject;

        public void DoLotsOfConcurrentWork()
        {
            for (int i = 0; i < 40; i++)              
            {
                ThreadPool.QueueUserWorkItem(() =>
                {
                    semaphor.WaitOne(); // this is a mutex
                    domainObject.DoSomething();
                    semaphor.Release();
                })); 
            }
        }
    }

    internal class SomeSharedResource
    {
        public void DoSomething()
        {
          // do some side effect
            Thread.Sleep(1000);
        }
    }
}