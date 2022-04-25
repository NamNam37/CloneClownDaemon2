using System;
using System.Collections.Generic;
using System.IO;
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
        public void Use(Configuration config)
        {
            FileManager filman = new FileManager(config);
            MetadataManager mdman = new MetadataManager(config);

            int rollback = mdman.GetPackageCount();
            diffToBeUsed = new List<SnapShotRow>();
            usedPackageName = mdman.GetCurrentPackageName();
            int backupCount = mdman.GetBackupCount();
            string backupDate = DateTime.Now.ToString("G").Replace('/', '-').Replace(':', '_');
            for (int i = 0; i < config.dests.Count; i++)
            {
                for (int j = 0; j < config.sources.Count; j++)
                {
                    currentDest = Path.Combine(config.dests[i], config.name, usedPackageName, $"{config.type}_{backupCount} {backupDate}", config.sources[j].Split('/').Last()).ToString();
                    filman.MkDir(currentDest);
                    mdman.DeleteOldPackage(Path.Combine(config.dests[i], config.name));
                    newSnapshot = filman.CreateSnapshot(config.sources[j], j);
                    mdman.SetSnapshot(newSnapshot, j);
                    diffToBeUsed = newSnapshot;
                    if (backupCount > 0 && config.type != Configuration.Type.Full)
                    {
                        if (config.type == Configuration.Type.Differencial)
                            used = mdman.GetSnapshot(0, j);
                        if (config.type == Configuration.Type.Incremental)
                            used = mdman.GetSnapshot(backupCount - 1, j);

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
                    foreach (var item in diffToBeUsed)
                    {
                        if (!item.isFile)
                        {
                            filman.MkDir(currentDest, item.path);
                        }

                    }
                    foreach (var item in diffToBeUsed)
                    {
                        if (item.isFile)
                        {
                            filman.MkDir(currentDest, item.path.Substring(0, item.path.Length - item.path.Split(Path.DirectorySeparatorChar).Last().Length));
                            
                            filman.CopyFile(Path.Combine(config.sources[j], item.path),
                                Path.Combine(currentDest, item.path));
                        }
                    }

                }
            }
            mdman.SetBackupCount(mdman.GetBackupCount() + 1);


            /*
            
            List<SnapShotRow> used = mdman.GetSnapshot(folderCounts-1, destID);
            List<SnapShotRow> newSnapshot = filman.CreateSnapshot(config.paths[destID].source);
            List<SnapShotRow> diffToBeUsed = new List<SnapShotRow>();
            mdman.SetSnapshot(newSnapshot, destID);

            for (int d = 0; d < newSnapshot.Count; d++)
            {
                bool found = false;
                for (int f = 0; f < used.Count; f++)
                {
                    if (used[f].path == newSnapshot[d].path && used[f].modifyTime.Subtract(newSnapshot[d].modifyTime).TotalSeconds > -2)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    diffToBeUsed.Add(newSnapshot[d]);
                }
            }
            string backupFolder = $"IncrBackup_{folderCounts}";
            string configFolderName = $"_{config.name}";

                filman.MkDir(Path.Combine(paths.dest, backupFolder + configFolderName));
                foreach (var snapShotRow in diffToBeUsed)
                {
                    if (!snapShotRow.isFile)
                    {
                        filman.MkDir(Path.Combine(paths.dest, backupFolder + configFolderName, snapShotRow.path.Remove(0, paths.source.Length + 1)).Replace('\\', '/'));
                    }

                }
                foreach (var snapShotRow in diffToBeUsed)
                {
                    if (snapShotRow.isFile)
                    {
                        filman.CopyFile(Path.Combine(paths.source, snapShotRow.path.Remove(0, paths.source.Length + 1)).Replace('\\', '/'),
                            Path.Combine(paths.dest, backupFolder + configFolderName, snapShotRow.path.Remove(0, paths.source.Length + 1)).Replace('\\', '/'));
                    }
                }
        */
        }
    }
}
