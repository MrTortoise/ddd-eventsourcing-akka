using System;
using System.Threading;

namespace ConcurrencyExamples
{
    public class SimpleThreadPoolExample
    {
        public SomeStateData _data;
        
        /// <summary>
        /// We can fire of pieces of work easily enough right?
        /// </summary>
        /// <remarks>
        /// How do we get progress?
        /// How do we cancel it?
        /// How do we monitor it?
        /// What about state? There is no isolation here. 
        /// </remarks>
        public void ThreadPoolExample()
        {
            ThreadPool.QueueUserWorkItem(Run);
        }

        private void Run(object state)
        {
            throw new NotImplementedException();
        }
    }
}