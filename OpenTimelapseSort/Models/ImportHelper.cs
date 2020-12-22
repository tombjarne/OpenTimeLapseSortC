using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTimelapseSort.Models
{
    class ImportHelper
    {
        string target { get; }
        string name { get; }

        public ImportHelper(string target, string name)
        {
            this.target = target;
            this.name = name;
        }
    }
}
