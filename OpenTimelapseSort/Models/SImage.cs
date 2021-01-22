using System.Diagnostics;
using System.IO;

namespace OpenTimelapseSort.Models
{
    public class SImage
    {
        public string Id { get; set; }
        public string DirectoryId { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public long FileTime { get; set; }
        public long FileSize { get; set; }
        public string ParentInstance { get; set; }
        public SDirectory ParentDirectory { get; set; }
        public double Lumen { get; set; }
        public long Colors { get; set; }

        public SImage(string name, string target, string parentInstance)
        {
            Name = name;
            Target = target;
            ParentInstance = parentInstance;
            FileTime = File.GetLastWriteTime(target).ToFileTime();
            Debug.WriteLine(FileTime);
        }
    }
}