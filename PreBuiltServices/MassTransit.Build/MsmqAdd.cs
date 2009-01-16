namespace MassTransit.Build
{
    using global::NAnt.Core;
    using global::NAnt.Core.Attributes;
    using Microsoft.Build.Framework;

    [TaskName("msmq-add")]
    public class MsmqAdd :
        Task, ITask
    {
        private readonly QueueManager qm = new QueueManager();

        [TaskAttribute("path")]
        public string Path { get; set; }

        [TaskAttribute("isTransactional")]
        public bool IsTransactional { get; set; }

        protected override void ExecuteTask()
        {
            qm.CreateQueue(Path, IsTransactional);
        }

        bool ITask.Execute()
        {
            qm.CreateQueue(Path, IsTransactional);
            return true;
        }

        public IBuildEngine BuildEngine
        {
            get; set;
        }

        public ITaskHost HostObject
        {
            get; set;
        }
    }
}