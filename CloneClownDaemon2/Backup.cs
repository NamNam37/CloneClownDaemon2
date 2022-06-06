using CloneClownAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class Backup
    {
        private List<SnapShotRow> used { get; set; }
        private List<SnapShotRow> newSnapshot { get; set; }
        private List<SnapShotRow> diffToBeUsed { get; set; }
        private string currentDest { get; set; }
        private string usedPackageName { get; set; }
        private Configs config { get; set; }
        int backupCount { get; set; }
        string backupDate { get; set; }
        User thisUser { get; set; }
        public Backup(Configs config, User thisUser)
        {
            this.thisUser = thisUser;
            this.config = config;
            MetadataManager mdman = new MetadataManager(config);
            diffToBeUsed = new List<SnapShotRow>();
            usedPackageName = mdman.GetCurrentPackageName();
            backupCount = mdman.GetBackupCount();
            backupDate = DateTime.Now.ToString("G").Replace('/', '-').Replace(':', '_');
        }
        public async Task Use()
        {
            if (config.dests.Count == 0)
                await new Logger().FailedLog(thisUser, config, 3);

            if (config.sources.Count == 0)
                await new Logger().FailedLog(thisUser, config, 4);


            for (int i = 0; i < config.dests.Count; i++)
            {
                for (int j = 0; j < config.sources.Count; j++)
                {
                    InitLocalDest(i, j);
                    InitFtpDest(i, j);
                    CreateSnapshot(j);

                    CopyToLocal(i, j);
                    CopyToFtp(i, j);
                }
                ZipDest(i);
            }
            MetadataManager mdman = new MetadataManager(config);
            mdman.SetBackupCount(mdman.GetBackupCount() + 1);
        }
        private void InitFtpDest(int destOrder, int sourceOrder)
        {
            if (config.dests[destOrder].type == DestF.Type.ftp)
            {
                string hostname = config.dests[destOrder].hostname;
                string login = config.dests[destOrder].login;
                string password = config.dests[destOrder].password;
                int port = config.dests[destOrder].port;
                FtpService ftpService = new FtpService(hostname, login, password, port);
                MetadataManager mdman = new MetadataManager(config);
                string destName = config.dests[destOrder].path;
                string backupFolder = $"{config.type}_{backupCount} {backupDate}";
                string sourceName = config.sources[sourceOrder].path.Split(Path.DirectorySeparatorChar).Last().Split(Path.AltDirectorySeparatorChar).Last();
                currentDest = Path.Combine(destName, config.configName, usedPackageName, backupFolder, sourceName).ToString();
                ftpService.MkDir(currentDest);
                newSnapshot = ftpService.CreateSnapshot(config.sources[sourceOrder].path, sourceOrder, config);
                mdman.SetSnapshot(newSnapshot, sourceOrder);
                diffToBeUsed = newSnapshot;
            }
        }
        private void CopyToLocal(int destOrder, int sourceOrder)
        {
            if (config.dests[destOrder].type == DestF.Type.local)
            {
                CreateDestFolder();
                CopyFilesToFolders(sourceOrder);
            }
        }
        private void CopyToFtp(int destOrder, int sourceOrder)
        {
            if (config.dests[destOrder].type == DestF.Type.ftp)
            {
                CreateDestFolderFtp(destOrder);
                CopyFilesToFoldersFtp(destOrder, sourceOrder);
            }
        }
        private void CopyFilesToFoldersFtp(int destOrder, int sourceOrder)
        {
            string hostname = config.dests[destOrder].hostname;
            string login = config.dests[destOrder].login;
            string password = config.dests[destOrder].password;
            int port = config.dests[destOrder].port;
            FtpService ftpService = new FtpService(hostname, login, password, port);
            foreach (var item in diffToBeUsed)
            {
                if (item.isFile)
                {
                    ftpService.MkDir(currentDest, item.path.Substring(0, item.path.Length - item.path.Split(Path.DirectorySeparatorChar).Last().Length));

                    ftpService.UploadFile(Path.Combine(config.sources[sourceOrder].path, item.path), Path.Combine(currentDest, item.path));
                }
            }
        }
        private void CreateDestFolderFtp(int destOrder)
        {
            string hostname = config.dests[destOrder].hostname;
            string login = config.dests[destOrder].login;
            string password = config.dests[destOrder].password;
            int port = config.dests[destOrder].port;
            FtpService ftpService = new FtpService(hostname, login, password, port);
            foreach (var item in diffToBeUsed)
            {
                if (!item.isFile)
                {
                    ftpService.MkDir(currentDest, item.path);
                }

            }
        }
        private void InitLocalDest(int destOrder, int sourceOrder)
        {
            if (config.dests[destOrder].type == DestF.Type.local)
            {
                FileManager filman = new FileManager(config);
                MetadataManager mdman = new MetadataManager(config);
                string destName = config.dests[destOrder].path;
                string backupFolder = $"{config.type}_{backupCount} {backupDate}";
                string sourceName = config.sources[sourceOrder].path.Split(Path.DirectorySeparatorChar).Last().Split(Path.AltDirectorySeparatorChar).Last();
                currentDest = Path.Combine(destName, config.configName, usedPackageName, backupFolder, sourceName).ToString();
                filman.MkDir(currentDest);
                mdman.DeleteOldPackage(Path.Combine(config.dests[destOrder].path, config.configName));
                newSnapshot = filman.CreateSnapshot(config.sources[sourceOrder].path, sourceOrder);
                mdman.SetSnapshot(newSnapshot, sourceOrder);
                diffToBeUsed = newSnapshot;
            }
        }
        private void CreateSnapshot(int SourceOrder)
        {
            MetadataManager mdman = new MetadataManager(config);
  
            if (backupCount > 0 && config.type != Configs.Type.full)
            {
                if (config.type == Configs.Type.differencial)
                    used = mdman.GetSnapshot(0, SourceOrder);
                if (config.type == Configs.Type.incremental)
                    used = mdman.GetSnapshot(backupCount - 1, SourceOrder);

                diffToBeUsed = new List<SnapShotRow>();
                for (int d = 0; d < newSnapshot.Count; d++)
                {
                    bool found = false;
                    for (int f = 0; f < used.Count; f++)
                    {
                        if ((used[f].path == newSnapshot[d].path) && (used[f].modifyTime.Subtract(newSnapshot[d].modifyTime).TotalSeconds > -2))
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        diffToBeUsed.Add(newSnapshot[d]);
                    }
                }
            }
        }
        private void CreateDestFolder()
        {
            FileManager filman = new FileManager(config);
            foreach (var item in diffToBeUsed)
            {
                if (!item.isFile)
                {
                    filman.MkDir(currentDest, item.path);
                }

            }
        }
        private void CopyFilesToFolders(int sourceOrder)
        {
            FileManager filman = new FileManager(config);
            foreach (var item in diffToBeUsed)
            {
                if (item.isFile)
                {
                    filman.MkDir(currentDest, item.path.Substring(0, item.path.Length - item.path.Split(Path.DirectorySeparatorChar).Last().Length));

                    filman.CopyFile(Path.Combine(config.sources[sourceOrder].path, item.path), Path.Combine(currentDest, item.path));
                }
            }
        }
        private void ZipDest(int destOrder)
        {
            FileManager filman = new FileManager(config);

            if (config.isZIP && config.dests[destOrder].type == DestF.Type.local) //temp
            {
                string zipPath = Path.Combine(config.dests[destOrder].path, config.configName, usedPackageName, $"{config.type}_{backupCount} {backupDate}");
                ZipFile.CreateFromDirectory(zipPath, zipPath + ".zip");
                filman.Delete(zipPath);
            }
        }
    }
}
