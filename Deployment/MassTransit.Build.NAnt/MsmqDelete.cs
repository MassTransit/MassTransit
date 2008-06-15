namespace MassTransit.Build.NAnt
{
    using global::NAnt.Core;
    using global::NAnt.Core.Attributes;

    [TaskName("msmq-delete")]
    public class MsmqDelete :
        Task
    {
        private QueueManager qw = new QueueManager();
        private string _path;

        [TaskAttribute("path")]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        protected override void ExecuteTask()
        {
            qw.RemoveQueue(Path);
        }
    }
}