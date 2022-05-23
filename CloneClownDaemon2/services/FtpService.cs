using CloneClownAPI.Models;
using FluentFTP;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CloneClownDaemon2
{
    public class FtpService
    {
        FtpClient client = new FtpClient();
        List<SnapShotRow> ssRows = new List<SnapShotRow>();
        public FtpService(string hostname, string login, string password, int port)
        {
            client.Host = hostname;
            client.Credentials = new NetworkCredential(login, password);
            client.Port = port;
        }
        public void MkDir(string path)
        {
            client.Connect();
            client.CreateDirectory(path);
            client.Disconnect();
        }
        public void MkDir(string path, string dirName)
        {
            client.Connect();
            client.CreateDirectory(Path.Combine(path, dirName));
            client.Disconnect();
        }
        public void UploadFolder(string localFolder, string ftpFolder)
        {
            client.Connect();
            client.UploadDirectory(localFolder, ftpFolder);
            client.Disconnect();
        }
        public void UploadFile(string localFileDir, string FtpFileDir)
        {
            client.Connect();
            client.UploadFile(localFileDir, FtpFileDir);
            client.Disconnect();
        }
        public void DeleteFolder(string ftpPath)
        {
            client.Connect();
            client.DeleteDirectory(ftpPath);
            client.Disconnect();
        }
        public void DeleteFile(string ftpPath)
        {
            client.Connect();
            client.DeleteFile(ftpPath);
            client.Disconnect();
        }
        public List<SnapShotRow> CreateSnapshot(string path, int backupID, Configs config)
        {
            ssRows = new List<SnapShotRow>();
            CreateSubSnapshot(new DirectoryInfo(path), backupID, config);
            ssRows.RemoveRange(0, 1);
            for (int i = 0; i < ssRows.Count; i++)
            {
                if (ssRows[i].path[0] == '/' || ssRows[i].path[0] == '\\')
                {
                    ssRows[i].path = ssRows[i].path.Remove(0, 1);
                }
            }

            return ssRows;
        }
        private void CreateSubSnapshot(DirectoryInfo path, int backupID, Configs config)
        {
            ssRows.Add(new SnapShotRow(path.FullName.Remove(0, config.sources[backupID].path.Length), default, false));

            foreach (FileInfo file in path.GetFiles())
            {
                ssRows.Add(new SnapShotRow(Path.Combine(path.FullName.Remove(0, config.sources[backupID].path.Length), file.Name), file.LastWriteTime, true));
            }
            foreach (DirectoryInfo sourceSubDirs in path.GetDirectories())
            {
                DirectoryInfo subPath = new DirectoryInfo(Path.Combine(path.FullName, sourceSubDirs.Name));
                CreateSubSnapshot(subPath, backupID, config);
            }
        }
    }
}
