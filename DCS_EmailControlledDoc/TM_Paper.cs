//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DCS_EmailControlledDoc
{
    using System;
    using System.Collections.Generic;
    
    public partial class TM_Paper
    {
        public TM_Paper()
        {
            this.TD_DocCopy = new HashSet<TD_DocCopy>();
        }
    
        public byte paper_id { get; set; }
        public string paper_name { get; set; }
        public System.DateTime update_dt { get; set; }
        public string update_by { get; set; }
    
        public virtual ICollection<TD_DocCopy> TD_DocCopy { get; set; }
    }
}
