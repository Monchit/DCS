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
    
    public partial class TM_User
    {
        public string empcode { get; set; }
        public byte utype_id { get; set; }
        public System.DateTime update_dt { get; set; }
        public string update_by { get; set; }
    
        public virtual TM_UserType TM_UserType { get; set; }
    }
}
