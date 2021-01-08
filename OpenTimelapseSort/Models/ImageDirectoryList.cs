using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTimelapseSort.Models
{
    class ImageDirectoryList
    {
        public int importId { get; set; }

        public List<SDirectory> directories;

        public ImageDirectoryList(int importId)
        {
            this.importId = importId;
        }
    }
}
