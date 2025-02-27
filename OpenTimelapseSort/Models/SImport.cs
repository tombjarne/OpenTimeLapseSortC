﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenTimelapseSort.Models
{
    public class SImport
    {
        [Key]
        public string Id { get; set; } // unique Id
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