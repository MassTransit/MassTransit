namespace MassTransit.Build
{
    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;

    public class SecurityCheck
    {
        public void ReadAcls()
        {
            DirectoryInfo dr = new DirectoryInfo(".");
            DirectorySecurity ds = dr.GetAccessControl();
            AuthorizationRuleCollection arc = ds.GetAccessRules(true, true, typeof (NTAccount));
            StringBuilder sb = new StringBuilder();
            foreach (FileSystemAccessRule ar in arc)
            {
                sb.AppendLine(ar.IdentityReference.Value);
            }
            string bob = sb.ToString();

            
        }

        public void Start(string directory)
        {
            if(!Directory.Exists(directory)) throw new Exception("Directory doesn't exist");

            DirectoryInfo di = new DirectoryInfo(directory);
            foreach (DirectoryInfo info in di.GetDirectories())
            {
                Recurse(info);
            }
            foreach (FileInfo file in di.GetFiles())
            {
                Print(file);
            }
        }
        public void Recurse(DirectoryInfo di)
        {
            
        }

        public void Print(FileInfo fileInfo)
        {
            
        }
    }
}