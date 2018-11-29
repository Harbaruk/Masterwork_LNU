using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starter.API.BackgroundJobs
{
    public class BackgroundJob
    {
        private readonly Queue<Action> _actionQueue;

        public BackgroundJob()
        {
            _actionQueue = new Queue<Action>();
        }

        public void AddAction(Action action)
        {
            _actionQueue.Enqueue(action);
        }

        public void Start()
        {
            foreach (var item in _actionQueue)
            {
                item.Invoke();
            }
        }
    }
}