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
        public void Use(Configs config)
        {
            FileManager filman = new FileManager(config);
            MetadataManager mdman = new MetadataManager(config);

            diffToBeUsed = new List<SnapShotRow>();
            usedPackageName = mdman.GetCurrentPackageName();
            int backupCount = mdman.GetBackupCount();
            string backupDate = DateTime.Now.ToString("G").Replace('/', '-').Replace(':', '_');
            for (int i = 0; i < config.dests.Count; i++)
            {
                for (int j = 0; j < config.sources.Count; j++)
                {
                    currentDest = Path.Combine(config.dests[i].path, config.configName, usedPackageName, $"{config.type}_{backupCount} {backupDate}",
                        config.sources[j].path.Split(Path.DirectorySeparatorChar).Last().Split(Path.AltDirectorySeparatorChar).Last()).ToString();
                    Console.WriteLine(config.dests[i].path);
                    Console.WriteLine(config.configName);
                    Console.WriteLine(usedPackageName);
                    Console.WriteLine($"{config.type}_{backupCount} {backupDate}");
                    Console.WriteLine(config.sources[j].path.Split(Path.DirectorySeparatorChar).Last());
                    filman.MkDir(currentDest);
                    mdman.DeleteOldPackage(Path.Combine(config.dests[i].path, config.configName));
                    newSnapshot = filman.CreateSnapshot(config.sources[j].path, j);
                    mdman.SetSnapshot(newSnapshot, j);
                    diffToBeUsed = newSnapshot;
                    if (backupCount > 0 && config.type != Configs.Type.full)
                    {
                        if (config.type == Configs.Type.differencial)
                            used = mdman.GetSnapshot(0, j);
                        if (config.type == Configs.Type.incremental)
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

                            filman.CopyFile(Path.Combine(config.sources[j].path, item.path),
                                Path.Combine(currentDest, item.path));
                        }
                    }

                }
                if (config.isZIP)
                {
                    string zipPath = Path.Combine(config.dests[i].path, config.configName, usedPackageName, $"{config.type}_{backupCount} {backupDate}");
                    ZipFile.CreateFromDirectory(zipPath, zipPath + ".zip");
                    filman.Delete(zipPath);
                }
            }
            mdman.SetBackupCount(mdman.GetBackupCount() + 1);
        }
    }
}
