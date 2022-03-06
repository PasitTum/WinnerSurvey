using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WinnerSurvey.Models.L1;

namespace WinnerSurvey.Models.L2
{
    public class DBSWGE2
    {
        public class OCRD2 : OCRD
        {
            public string SlpName { get; set; }
            public string U_ISS_SalesName { get; set; }
            public string Street { get; set; }
            public string EncryptPassword { get; set; }
            public string Email { get; set; }
            public string SaleCode { get; set; }
        }

        public class OHEM2 : OHEM
        {

        }

        public class C_WGE_SURVEY1_2 : C_WGE_SURVEY1
        {
            public string NumberID1 { get; set; }
            public string Detail { get; set; }
            public string EncryptPassword { get; set; }
            public string SaleCode { get; set; }
            public string QRCode { get; set; }
            public string SlpNo { get; set; }
        }

        public class C_WGE_SURVEY2_2 : C_WGE_SURVEY2
        {
            public string NumberID2 { get; set; }
            public string Detail { get; set; }
            public string EncryptPassword { get; set; }
            public string QRCode { get; set; }
        }

        public class C_WGE_SURVEY_DETAIL_2 : C_WGE_SURVEY_DETAIL
        {

        }
    }
}