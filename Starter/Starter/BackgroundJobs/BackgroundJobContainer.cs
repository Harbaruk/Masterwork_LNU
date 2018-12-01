using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Starter.API.BackgroundJobs
{
    public class BackgroundJobContainer
    {
        private readonly Queue<BackgroundJob> _jobs;
        public BackgroundJobContainer()
        {
            _jobs = new Queue<BackgroundJob>();
        }

        public void Add(BackgroundJob job)
        {
            _jobs.Enqueue(job);
        }

        public void Run()
        {
            foreach (var job in _jobs)
            {
                Task.Factory.StartNew(() =>
                {
                    job.Start();
                });
            }
        }
    }
}