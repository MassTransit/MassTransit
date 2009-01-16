namespace MassTransit.Build
{
    using global::NAnt.Core;
    using global::NAnt.Core.Attributes;
    using Microsoft.Build.Framework;

    [TaskName("msmq-delete")]
    public class MsmqDelete :
        Task, ITask
    {
        private readonly QueueManager qw = new QueueManager();

        [TaskAttribute("path")]
        public string Path { get; set; }

        protected override void ExecuteTask()
        {
            qw.RemoveQueue(Path);
        }

        bool ITask.Execute()
        {
            qw.RemoveQueue(Path);
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