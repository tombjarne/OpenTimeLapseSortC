using System;
using System.Collections.Generic;

namespace OpenTimelapseSort.Models
{
    public class SImport
    {
        public string Id { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }
        public virtual List<SDirectory> Directories { get; set; }
        public DateTime Timestamp { get; set; }
        public string ImportDate { get; set; }

        public SImport()
        {
            Timestamp = DateTime.Today;
            ImportDate = Timestamp.ToShortDateString();
        }
    }
}