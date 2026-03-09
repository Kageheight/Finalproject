using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSightClient.Models
{
    public class ScanLog
    {
        public string FilePath { get; set; } = "";
        public DateTime ScanTime { get; set; }
        public string Result { get; set; } = "";
    }
}