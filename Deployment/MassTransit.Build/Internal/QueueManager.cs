namespace MassTransit.Build
{
    using System.Messaging;
    using System.Security.Principal;

    public class QueueManager
    {
        private readonly MessageQueueAccessRights _consumerRights = MessageQueueAccessRights.DeleteMessage
            | MessageQueueAccessRights.PeekMessage 
            | MessageQueueAccessRights.ReceiveMessage
            | MessageQueueAccessRights.WriteMessage;

        private readonly MessageQueueAccessRights _publisherRights = MessageQueueAccessRights.WriteMessage;
        private readonly MessageQueueAccessRights _adminRights = MessageQueueAccessRights.FullControl;

        private readonly string _publisherRole = "MT-Publisher";
        private readonly string _consumerRole = "MT-Consumer";

        public void CreateQueue(string path)
        {
            CreateQueue(path, false);
        }
        public void CreateQueue(string path, bool isTransactional)
        {
            MessageQueue  q = MessageQueue.Create(path, isTransactional);
            q.SetPermissions(_consumerRole, _consumerRights);
            q.SetPermissions(_publisherRole, _publisherRights);
            
            
            WindowsBuiltInRole role = WindowsBuiltInRole.Administrator;
            string builtInRoleAsString = role.ToString();
            q.SetPermissions(builtInRoleAsString, _adminRights);
        }

        public void RemoveQueue(string path)
        {
            MessageQueue.Delete(path);
        }


    }
}
