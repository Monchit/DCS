//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DCS_EmailReminder
{
    using System;
    using System.Collections.Generic;
    
    public partial class TM_DocType
    {
        public TM_DocType()
        {
            this.TD_Document = new HashSet<TD_Document>();
        }
    
        public string doc_type_short { get; set; }
        public string doc_type_full { get; set; }
        public byte doc_lv { get; set; }
        public bool copy_flag { get; set; }
        public byte review_year { get; set; }
        public string update_by { get; set; }
        public System.DateTime update_dt { get; set; }
    
        public virtual ICollection<TD_Document> TD_Document { get; set; }
    }
}
