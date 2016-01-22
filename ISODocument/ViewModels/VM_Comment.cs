using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISODocument.ViewModels
{
    public class VM_Comment
    {
        public string actor { get; set; }
        public string lv_name { get; set; }
        public string org_name { get; set; }
        public string operation_name { get; set; }
        public string status_name { get; set; }
        public string act_name { get; set; }
        public Nullable<System.DateTime> act_dt { get; set; }
        public string comment { get; set; }
    }
}