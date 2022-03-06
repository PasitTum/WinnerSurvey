using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WinnerSurvey.Models.L2.DBSWGE2;

namespace WinnerSurvey.Models.L1
{
    public class mWGE
    {
        public mWGE1 mwge1 { get; set; }

        public mWGE2 mwge2 { get; set; }

        public string Year { get; set; }

        public DateTime Date { get; set; }

        public string Code { get; set; }        

        public Byte[] Data { get; set; }

        public List<C_WGE_SURVEY_REPORT> List_Survey_Report { get; set; }

        public List<Detail> mwge3 { get; set; }
        public C_WGE_SURVEY_REPORT sURVEY_REPORT { get; set; }
    }

    public class mWGE1
    {
        public List<C_WGE_SURVEY1> ListSurvey1 { get; set; }

        public List<C_WGE_SURVEY2> ListSurvey2 { get; set; }

        public OCRD OCRD { get; set; }
    }

    public class mWGE2
    {
        public C_WGE_SURVEY1_2 CS1 { get; set; }

        public C_WGE_SURVEY2_2 CS2 { get; set; }

        public OCRD2 OCRD { get; set; }

        public List<OCRD2> ListOCRD { get; set; }

        public List<C_WGE_SURVEY1_2> ListSurvey1 { get; set; }

        public List<C_WGE_SURVEY2_2> ListSurvey2 { get; set; }
    }
    public class Detail : C_WGE_SURVEY1
    {
        public string Name { get; set; }
        public string product1 { get; set; }
        public string product2 { get; set; }
        public string product3 { get; set; }
        public string product4 { get; set; }
        public string product5 { get; set; }
        public string product6 { get; set; }
        public string product7 { get; set; }
        public string send1 { get; set; }
        public string send2 { get; set; }
        public string send3 { get; set; }
        public string send4 { get; set; }
        public string service1 { get; set; }
        public string service2 { get; set; }
        public string service3 { get; set; }
        public string service4 { get; set; }
        public string contact1 { get; set; }
        public string contact2 { get; set; }
        public string contact3 { get; set; }
    }
}