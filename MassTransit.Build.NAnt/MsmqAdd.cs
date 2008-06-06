namespace MassTransit.Build.NAnt
{
    using global::NAnt.Core;
    using global::NAnt.Core.Attributes;

    [TaskName("msmq-add")]
    public class MsmqAdd :
        Task
    {
        private string _path;
        private bool _isTransactional;
        private QueueManager qm = new QueueManager();

        [TaskAttribute("path")]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [TaskAttribute("isTransactional")]
        public bool IsTransactional
        {
            get { return _isTransactional; }
            set { _isTransactional = value; }
        }

        protected override void ExecuteTask()
        {
            qm.CreateQueue(Path, IsTransactional);
        }
    }
}