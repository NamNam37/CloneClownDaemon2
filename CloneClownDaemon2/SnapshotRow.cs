using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneClownDaemon2
{
    public class SnapShotRow
    {
        public string path { get; set; }
        public DateTime modifyTime { get; set; }
        public bool isFile { get; set; }
        public SnapShotRow(string path, DateTime modifyTime, bool isFile)
        {
            this.path = path;
            this.modifyTime = modifyTime;
            this.isFile = isFile;
        }
    }
}
