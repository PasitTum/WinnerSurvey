//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WinnerSurvey.Models.L1
{
    using System;
    using System.Collections.Generic;
    
    public partial class C_WGE_SURVEY_REPORT
    {
        public int ID { get; set; }
        public Nullable<int> ID_Survey1 { get; set; }
        public string Year_Survey1 { get; set; }
        public Nullable<int> ID_Survey2 { get; set; }
        public string Year_Survey2 { get; set; }
        public string ID_Report { get; set; }
        public string User_Accept { get; set; }
        public Nullable<System.DateTime> Date_Accept { get; set; }
        public string Head_Approve { get; set; }
        public Nullable<System.DateTime> Date_Approve { get; set; }
        public string UserPosition { get; set; }
        public string ApprovePosition { get; set; }
        public string Dept { get; set; }
        public string pathUser { get; set; }
        public string pathApprove { get; set; }
    }
}
