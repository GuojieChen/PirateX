using System;
using ServiceStack.DataAnnotations;

namespace PirateX.ConfigImport
{
    public class FileHash
    {
        [StringLength(100)]
        public string Id {get{return FileName;}}

        [StringLength(100)]
        public string FileName { get; set; }

        [StringLength(125)]
        public string Hash { get; set; }

        public DateTime UpdateAtUtc { get; set; }

        public FileHash()
        {
            UpdateAtUtc = DateTime.UtcNow;
        }
    }
}
