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
    
    public partial class TM_GroupCode
    {
        public TM_GroupCode()
        {
            this.TD_Document = new HashSet<TD_Document>();
        }
    
        public string group_code { get; set; }
        public int group_id { get; set; }
        public string update_by { get; set; }
        public System.DateTime update_dt { get; set; }
        public string responsible { get; set; }
    
        public virtual ICollection<TD_Document> TD_Document { get; set; }
    }
}
