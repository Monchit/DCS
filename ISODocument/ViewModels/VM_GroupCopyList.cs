using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISODocument.ViewModels
{
    public class VM_GroupCopyList
    {
        public string doc_no { get; set; }
        public byte rev_no { get; set; }
        public string doc_name { get; set; }
        public DateTime eff_date { get; set; }
        public int round { get; set; }
        public int qty { get; set; }
    }
}