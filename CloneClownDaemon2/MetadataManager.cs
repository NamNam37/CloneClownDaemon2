using CloneClownAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CloneClownDaemon2
{
    public class MetadataManager
    {
        private string configName { get; set; }
        private string metadataPath { get; set; }
        private string metadataPathSS { get; set; }
        private string metadataPathFC { get; set; }
        private string metadataPathPN { get; set; }
        private Configs config { get; set; }
        public MetadataManager(Configs config)
        {
            this.configName = config.configName;
            this.config = config;
            this.metadataPath = $"../../../";
            this.metadataPathFC = metadataPath + $"{configName}/folderCount.txt";
            this.metadataPathPN = metadataPath + $"{configName}/PackageNames.txt";
            this.metadataPathSS = metadataPath + $"{configName}/snapshots/";
        }
        public void InitMetadata()
        {
            FileManager filman = new FileManager(config);
            if (!File.Exists(metadataPathFC) || GetFCRawText().Length == 0)
            {
                filman.MkDir(metadataPath, configName);
                filman.MkFile(metadataPathFC); 

                using (StreamWriter writer = new StreamWriter(metadataPathFC))
                {
                    writer.WriteLine($"0;0");
                }
            }
            if (!File.Exists(metadataPathPN))
            {
                filman.MkDir(metadataPath, configName);
                filman.MkFile(metadataPathPN);
            }
            if (!Directory.Exists(metadataPathSS))
            {
                filman.MkDir(metadataPathSS);
            }
        }
        public List<SnapShotRow> GetSnapshot(int backupID, int destID)
        {
            List<SnapShotRow> data = new List<SnapShotRow>();
            using (StreamReader reader = new StreamReader($"{metadataPathSS}{backupID}_{destID}.txt"))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');
                    SnapShotRow linessRow = new SnapShotRow("", default, false);
                    if (line.Length != 0)
                    {
                        try
                        {
                            linessRow = new SnapShotRow(line[0], DateTime.Parse(line[1]), bool.Parse(line[2]));
                        }
                        catch
                        {
                            throw new FileLoadException("Metadata are corrupted");
                        }

                    }
                    data.Add(linessRow);
                }
            }
            return data;
        }
        public void SetSnapshot(List<SnapShotRow> ssRows, int backupID)
        {
            FileManager filman = new FileManager(config);

            int backupCount = GetBackupCount();

            if (!File.Exists($"{metadataPathSS}{backupCount}_{backupID}.txt") && ssRows != null)
            {
                filman.MkFile($"{metadataPathSS}{backupCount}_{backupID}.txt");
                using (StreamWriter writer = new StreamWriter($"{metadataPathSS}{backupCount}_{backupID}.txt"))
                {
                    for (int q = 0; q < ssRows.Count; q++)
                    {
                        writer.WriteLine($"{ssRows[q].path};{ssRows[q].modifyTime};{ssRows[q].isFile}");
                    }
                }
            }
        }
        private string GetFCRawText()
        {
            string output;
            using (StreamReader reader = new StreamReader(metadataPathFC))
            {
                output = reader.ReadLine();
            }
            return output;
        }
        public int GetBackupCount()
        {
            int output;
            using (StreamReader reader = new StreamReader(metadataPathFC))
            {
                output = int.Parse(reader.ReadLine().Split(';')[0]);
            }
            return output;
        }
        public int GetPackageCount()
        {
            int output;
            using (StreamReader reader = new StreamReader(metadataPathFC))
            {
                output = int.Parse(reader.ReadLine().Split(';')[1]);
            }
            return output;
        }
        public List<string> GetPackageNames()
        {
            string line;
            using (StreamReader reader = new StreamReader(metadataPathPN))
            {
                line = reader.ReadLine();
                if (line == null)
                    return new List<string>();
                if (line.Contains(';'))
                {
                    return line.Split(';').ToList();
                }
                return new List<string>() { line };
            }
            
        }
        public string GetCurrentPackageName()
        {
            List<string> a = GetPackageNames();
            a.Add(DateTime.Now.ToString("G").Replace('/', '-').Replace(':', '_'));
            if (GetPackageNames().Count == 0)
            {
                SetPackageNames(a);
                return a.Last().ToString();
            }

            if (GetBackupCount() >= config.backupCount)
            {
                SetBackupCount(0);
                SetPackageCount(GetPackageCount()+1);
                SetPackageNames(a);
                return a.Last().ToString();
            }
            return GetPackageNames().Last();

        }
        public void DeleteOldPackage(string packagesRoute)
        {
            if (GetPackageCount() >= config.backupCount + 1)
            {
                string firstPackage = GetPackageNames().First();
                new FileManager(config).Delete(Path.Combine(packagesRoute, firstPackage));
                SetPackageCount(GetPackageCount()-1);
                List<string> packages = GetPackageNames();
                packages.RemoveAt(0);
                SetPackageNames(packages);
            }
        }
        public void SetBackupCount(int value)
        {
            string[] folderCounts = GetFCRawText().Split(';');
            using (StreamWriter writer = new StreamWriter(metadataPathFC))
            {
                writer.WriteLine($"{value};{folderCounts[1]}");
            }
        }
        public void SetPackageCount(int value)
        {
            string[] folderCounts = GetFCRawText().Split(';');
            using (StreamWriter writer = new StreamWriter(metadataPathFC))
            {
                writer.WriteLine($"{folderCounts[0]};{value}");
            }
        }
        public void SetPackageNames(List<string> packageNames)
        {
            StringBuilder packageNamesBuilder = new StringBuilder();
            for (int i = 0; i < packageNames.Count; i++)
            {
                packageNamesBuilder.Append(packageNames[i]);
                packageNamesBuilder.Append(';');
            }
            if (packageNames.Count > 0)
                packageNamesBuilder.Remove(packageNamesBuilder.Length - 1, 1);

            
            string packageNamesToSave = packageNamesBuilder.ToString();
            using (StreamWriter writer = new StreamWriter(metadataPathPN))
            {
                writer.WriteLine(packageNamesToSave);
            }
        }

    }
}
