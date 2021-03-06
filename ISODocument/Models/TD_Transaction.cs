//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISODocument.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TD_Transaction
    {
        public string doc_type_short { get; set; }
        public string group_code { get; set; }
        public int run_no { get; set; }
        public byte rev_no { get; set; }
        public byte sub_rev { get; set; }
        public byte status_id { get; set; }
        public byte lv_id { get; set; }
        public int org_id { get; set; }
        public byte operation_id { get; set; }
        public Nullable<byte> action_id { get; set; }
        public string actor { get; set; }
        public Nullable<System.DateTime> act_dt { get; set; }
        public string comment { get; set; }
        public bool active { get; set; }
    
        public virtual TD_Document TD_Document { get; set; }
        public virtual TM_Action TM_Action { get; set; }
        public virtual TM_Level TM_Level { get; set; }
        public virtual TM_Operation TM_Operation { get; set; }
        public virtual TM_Status TM_Status { get; set; }
    }
}
