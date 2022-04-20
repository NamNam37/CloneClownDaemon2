using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CloneClownDaemon2
{
    public class MetadataManager
    {
        private string configName { get; set; }
        private string metadataPath { get; set; }
        private string metadataPathSS { get; set; }
        private string metadataPathFC { get; set; }
        private string metadataPathPN { get; set; }
        private Configuration config { get; set; }
        public MetadataManager(Configuration config)
        {
            this.configName = config.name;
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

                using (StreamWriter writer = new StreamWriter(metadataPathPN))
                {
                    writer.WriteLine($"0;0");
                }
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
    }
}
