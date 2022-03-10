using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WinnerSurvey.Class;
using WinnerSurvey.Models.L1;
using static WinnerSurvey.Models.L2.DBSWGE2;

namespace WinnerSurvey.Controllers
{
    public class CRController : Controller
    {
        InterfaceLog log = new InterfaceLog(ConfigurationManager.AppSettings["LogsPath"]);

        // GET: CR
        public ActionResult Report1(int Year)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {

                //Sales Support Supervisor
                //Senior Sales Support Officer
                //Sales Supoort Officer (Sales)
                //Sales Coordinator (MT)
                //Sales Coordinator (FS)
                //Sales Coordinator (ID)
                try
                {
                    using (var db = new DBSEntities())
                    {
                        mWGE mData = new mWGE();
                        mData.mwge1 = new mWGE1();
                        mData.mwge2 = new mWGE2();

                        int saleemp = 0;
                        string usercode = Session["Code"].ToString();
                        

                        if (Session["Type"].ToString() == "Admin")
                        {
                            string cardcode;
                            string jobtitle = Session["JobTitle"].ToString();
                            string slpcode;
                            switch (jobtitle)
                            {
                                case "Sales Coordinator (ID)":
                                    cardcode = "c1";
                                    slpcode = "s1";
                                    break;
                                case "Sales Coordinator (FS)":
                                    cardcode = "c3";
                                    slpcode = "s3";
                                    break;
                                case "Sales Coordinator (MT)":
                                    cardcode = "c4";
                                    slpcode = "s4";
                                    break;
                                case "Senior Sales Support Officer":
                                    cardcode = "";
                                    slpcode = "";
                                    break;
                                case "Sales Support Supervisor":
                                case "IT Programmer":
                                case "IT Manager (Corporate Service)":
                                    cardcode = null;
                                    slpcode = null;
                                    break;
                                default:
                                    cardcode = "";
                                    slpcode = "";
                                    break;
                            }
                            if(cardcode =="c1" || cardcode == "c3" || cardcode == "c4")
                            {
                                var oData = (from a in db.C_WGE_SURVEY1
                                             join b in db.OCRD on a.Cardcode equals b.CardCode into bb
                                             from b in bb.DefaultIfEmpty()
                                             join c in db.OSLP on b.SlpCode equals c.SlpCode into cc
                                             from c in cc.DefaultIfEmpty()
                                             where a.Year == Year.ToString() && (a.SlpName.Contains(slpcode) || a.Cardcode.Contains(cardcode) ) &&
                                             a.Generate_Report == null
                                             select new C_WGE_SURVEY1_2
                                             {
                                                 ID = a.ID,
                                                 NumberID = a.NumberID,
                                                 Cardcode = a.Cardcode,
                                                 Cardname = a.Cardname,
                                                 Date = a.Date,
                                                 SaleCode = (c.SlpName == null) ? db.OSLP.Where(c => c.SlpCode.ToString() == (db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.salesPrson).FirstOrDefault().ToString())).Select(v => v.SlpName).FirstOrDefault() : c.SlpName,
                                                 Code = a.Code,
                                                 QRCode = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                             }).OrderByDescending(z => z.ID).Distinct();
                                if (oData != null)
                                {
                                    mData.mwge2.ListSurvey1 = oData.ToList();
                                    mData.Year = Year.ToString();
                                }
                            }
                            else
                            {
                                var oData = (from a in db.C_WGE_SURVEY1
                                             join b in db.OCRD on a.Cardcode equals b.CardCode into bb
                                             from b in bb.DefaultIfEmpty()
                                             join c in db.OSLP on b.SlpCode equals c.SlpCode into cc
                                             from c in cc.DefaultIfEmpty()
                                             where a.Year == Year.ToString()  && a.Generate_Report == null
                                             select new C_WGE_SURVEY1_2
                                             {
                                                 ID = a.ID,
                                                 NumberID = a.NumberID,
                                                 Cardcode = a.Cardcode,
                                                 Cardname = a.Cardname,
                                                 Date = a.Date,
                                                 SaleCode = (c.SlpName == null) ? db.OSLP.Where(c => c.SlpCode.ToString() == (db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.salesPrson).FirstOrDefault().ToString())).Select(v => v.SlpName).FirstOrDefault() : c.SlpName,
                                                 Code = a.Code,
                                                 QRCode = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                             }).OrderByDescending(z => z.ID).Distinct();
                                if (oData != null)
                                {
                                    mData.mwge2.ListSurvey1 = oData.ToList();
                                    mData.Year = Year.ToString();
                                }
                            }
                                
                            

                            return View(mData);
                        }
                        else
                        {
                            saleemp = Convert.ToInt32(Session["SaleEMP"].ToString());

                            var oData = (from a in db.C_WGE_SURVEY1
                                         join b in db.OCRD on a.Cardcode equals b.CardCode into bb
                                         from b in bb.DefaultIfEmpty()
                                         join c in db.OSLP on b.SlpCode equals c.SlpCode into cc
                                         from c in cc.DefaultIfEmpty()
                                         where a.Year == Year.ToString() && (b.SlpCode == saleemp || a.Code == usercode)
                                         select new C_WGE_SURVEY1_2
                                         {
                                             ID = a.ID,
                                             NumberID = a.NumberID,
                                             Cardcode = a.Cardcode,
                                             Cardname = a.Cardname,
                                             Date = a.Date,
                                             SaleCode = (c.SlpName == null) ? db.OSLP.Where(c => c.SlpCode.ToString() == (db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.salesPrson).FirstOrDefault().ToString())).Select(v => v.SlpName).FirstOrDefault() : c.SlpName,
                                             Code = a.Code,
                                             QRCode = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                         }).OrderByDescending(z => z.ID).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListSurvey1 = oData.ToList();
                                mData.Year = Year.ToString();
                            }

                            return View(mData);
                        }                       
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult getReport1(string Year, string[] checkbox1)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    using(var db= new DBSEntities())
                    {
                        //สร้างรายการสรุป
                        C_WGE_SURVEY_REPORT dbs = new C_WGE_SURVEY_REPORT();
                        string gcode = Guid.NewGuid().ToString();
                        
                        string cardcode="";
                        string slpcode;
                        string path1 = "/Files/S" + Session["Code"].ToString() + ".jpg"; ;
                        switch (Session["Jobtitle"])
                        {
                            case "Sales Coordinator (ID)":
                                cardcode = "c1";
                                slpcode = "s1";
                                break;
                            case "Sales Coordinator (FS)":
                                cardcode = "c3";
                                slpcode = "s3";
                                break;
                            case "Sales Coordinator (MT)":
                                cardcode = "c4";
                                slpcode = "s4";
                                break;
                            default:
                                cardcode = null;
                                slpcode = null;
                                break;
                        }
                        if (cardcode != null)
                        {
                            var oData = (from a in db.C_WGE_SURVEY1 where a.Year == Year && a.Cardcode.Contains(cardcode) || a.SlpName.Contains(slpcode) select a).ToList();
                            foreach (var oitem in oData)
                            {
                                oitem.Generate_Report = gcode;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                             
                            foreach (var oitem in checkbox1)
                            {
                                var oitem2 = (from a in db.C_WGE_SURVEY1
                                             where a.Year == Year && a.NumberID.Contains(oitem)
                                             select a).FirstOrDefault();
                                oitem2.Generate_Report = gcode;
                                db.SaveChanges();
                            }
                        }
                        dbs.pathUser = path1;
                        dbs.ID_Report = gcode;
                        dbs.Year_Survey1 = Year;
                        dbs.User_Accept = Session["Name"].ToString();
                        dbs.Date_Accept = DateTime.Now;
                        dbs.UserPosition = Session["JobTitle"].ToString();
                        dbs.Dept = cardcode;

                        db.C_WGE_SURVEY_REPORT.Add(dbs);
                        db.SaveChanges();
                        

                        log.WriteLog(string.Format("{0}({1}) : {2} {3}", Session["Name"].ToString(), Session["Code"].ToString(), "Report1", System.Reflection.MethodBase.GetCurrentMethod().Name));

                        return RedirectToAction("Approve1", "CR", new { Year = Year });
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog(ex.ToString());
                    TempData["msg"] = "<script>alert('error!');</script>"+ex;
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult Report2(int Year)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    using (var db = new DBSEntities())
                    {
                        mWGE mData = new mWGE();
                        mData.mwge1 = new mWGE1();
                        mData.mwge2 = new mWGE2();

                        string usercode = Session["Code"].ToString();

                        if (Session["Type"].ToString() == "Admin")
                        {
                            var oData = (from a in db.C_WGE_SURVEY2
                                         where a.Year == Year.ToString()
                                         select new C_WGE_SURVEY2_2
                                         {
                                             ID = a.ID,
                                             NumberID = a.NumberID,
                                             Name = a.Name,
                                             Date = a.Date,
                                             Code = a.Code,
                                             QRCode = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                         }).OrderByDescending(z => z.ID).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListSurvey2 = oData.ToList();
                                mData.Year = Year.ToString();
                            }

                            return View(mData);
                        }
                        else
                        {
                            var oData = (from a in db.C_WGE_SURVEY2
                                         where a.Year == Year.ToString() && a.Code == usercode
                                         select new C_WGE_SURVEY2_2
                                         {
                                             ID = a.ID,
                                             NumberID = a.NumberID,
                                             Name = a.Name,
                                             Date = a.Date,
                                             Code = a.Code,
                                             QRCode = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                         }).OrderByDescending(z => z.ID).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListSurvey2 = oData.ToList();
                                mData.Year = Year.ToString();
                            }

                            return View(mData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult getReport2(int Year,string[] checkbox1)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Report"), "CrystalReport2.rpt"));

                    rd.SetParameterValue("@year", Year);
                    rd.SetParameterValue("@ID", checkbox1);

                    rd.SetDatabaseLogon("sa", "Wge123456*", "SVERP01", "DBS_WGE");

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();

                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);

                    rd.Close();
                    rd.Dispose();

                    log.WriteLog(string.Format("{0}({1}) : {2} {3}", Session["Name"].ToString(), Session["Code"].ToString(), "Report1", System.Reflection.MethodBase.GetCurrentMethod().Name));

                    return File(stream, "application/pdf", "Report2.pdf");
                }
                catch (Exception ex)
                {
                    log.WriteLog(ex.ToString());
                    TempData["msg"] = "<script>alert('error!');</script>";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult Approve1(int Year)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    using (var db = new DBSEntities())
                    {
                        mWGE mData = new mWGE();
                        string cardcode = "";
                        switch (Session["Jobtitle"])
                        {
                            case "Sales Coordinator (ID)":
                            case "Head of Food Industry Sales":
                                cardcode = "c1";
                                break;
                            case "Sales Coordinator (FS)":
                            case "Head of Food Service Sales":
                                cardcode = "c3";
                                break;
                            case "Sales Coordinator (MT)":
                            case "Head of Modern Trade":
                                cardcode = "c4";
                                break;
                            case "Sales Support Supervisor":
                            case "IT Programmer":
                            case "IT Manager (Corporate Service)":
                                cardcode = null;
                                break;
                        }

                        if (Session["Type"].ToString() == "Admin")
                        {
                            
                            if (cardcode == "c1" || cardcode == "c3" || cardcode == "c4" )
                            {
                                var oData = (from a in db.C_WGE_SURVEY_REPORT
                                             where a.Dept.Contains(cardcode)  && a.Year_Survey1 == Year.ToString()
                                             select a).ToList();
                                mData.List_Survey_Report = oData;
                            }
                            else
                            {
                                var oData = (from a in db.C_WGE_SURVEY_REPORT
                                             where a.Year_Survey1 == Year.ToString()
                                             select a).ToList();
                                mData.List_Survey_Report = oData;
                            }


                            mData.Year = Year.ToString();
                            return View(mData);
                        }
                        else
                        {
                            string salecode = Session["SaleCode"].ToString();
                            string checkcode ;
                            switch (Session["Jobtitle"])
                            {
                                case "Sales Coordinator (ID)":
                                case "Head of Food Industry Sales":
                                    checkcode = "S1";
                                    break;
                                case "Sales Coordinator (FS)":
                                case "Head of Food Service Sales":
                                    checkcode = "S3";
                                    break;
                                case "Sales Coordinator (MT)":
                                case "Head of Modern Trade":
                                    checkcode = "S4";
                                    break;
                                case "Sales Support Supervisor":
                                case "IT Programmer":
                                case "IT Manager (Corporate Service)":
                                    checkcode = "";
                                    break;
                                default:
                                    checkcode = salecode.Substring(0, 2);
                                    break;
                            }

                            string dept = "";
                            switch (checkcode)
                            {
                                case "S1":
                                    dept = "c1";
                                    break;
                                case "S3":
                                    dept = "c3";
                                    break;
                                case "S4":
                                    dept = "c4";
                                    break;
                                default:
                                    dept = "";
                                    break;
                            }
                            if (dept == "c1" || dept == "c3" || dept == "c4" || dept == "c")
                            {
                                var oData = (from a in db.C_WGE_SURVEY_REPORT
                                             where a.Dept.Contains(dept) && a.Year_Survey1 == Year.ToString()
                                             select a).ToList();
                                mData.List_Survey_Report = oData;
                            }
                            else
                            {
                                var oData = (from a in db.C_WGE_SURVEY_REPORT
                                             where a.Year_Survey1 == Year.ToString()
                                             select a).ToList();
                                mData.List_Survey_Report = oData;
                            }


                            mData.Year = Year.ToString();
                            return View(mData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog(ex.ToString());
                    TempData["msg"] = "<script>alert('error!');</script>";
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        
        [HttpPost]
        public ActionResult getApprove1(string button,string ID_Report,string print,string Approve, string Sendemail)
        {
            try
            {
                using(var db = new DBSEntities())
                {
                    //ยกเลิกรายการ
                    if (button != null && (ID_Report == null && print == null && Approve == null && Sendemail == null))
                    {
                        var oData = (from a in db.C_WGE_SURVEY1 where a.Generate_Report == button select a).ToList();
                        foreach (var oItem in oData)
                        {
                            var oData2 = (from a in db.C_WGE_SURVEY1 where a.ID == oItem.ID select a).FirstOrDefault();
                            oData2.Generate_Report = null;
                            db.SaveChanges();
                        }
                        var oData3 = (from a in db.C_WGE_SURVEY_REPORT where a.ID_Report == button select a).FirstOrDefault();
                        string Year = oData3.Year_Survey1;
                        db.C_WGE_SURVEY_REPORT.Remove(oData3);

                        db.SaveChanges();
                        return RedirectToAction("Approve1", "CR", new { Year = Year });
                    }
                    //ตรวจสอบรายการ ปุ่มนในหน้า Approve1
                    else if (ID_Report != null && (button == null && print == null && Approve == null && Sendemail == null))
                    {
                        return RedirectToAction("ApproveDetail1", "CR", new { ID_Report = ID_Report });
                    }
                    //ปริ้นรายการ
                    else if (button == null && (ID_Report == null && print != null && Approve == null && Sendemail == null))
                    {
                        ReportDocument rd = new ReportDocument();
                        rd.Load(Path.Combine(Server.MapPath("~/Report"), "CrystalReport1.rpt"));
                        string path = AppDomain.CurrentDomain.BaseDirectory;

                        rd.SetParameterValue("@year", print);
                        rd.SetParameterValue("@path", path);
                        rd.SetDatabaseLogon("sa", "Wge123456*", "SVERP01", "DBS_WGE");

                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);

                        rd.Close();
                        rd.Dispose();

                        log.WriteLog(string.Format("{0}({1}) : {2} {3}", Session["Name"].ToString(), Session["Code"].ToString(), "Report1", System.Reflection.MethodBase.GetCurrentMethod().Name));

                        return File(stream, "application/pdf", "Report2.pdf");
                    }
                    //Head Approve ใบสรุป
                    else if (Approve != null && (ID_Report == null && button == null && print == null && Sendemail == null))
                    {
                        var oData4 = (from a in db.C_WGE_SURVEY_REPORT where a.ID_Report == Approve select a).FirstOrDefault();

                        string email = "pasit.b@winnergroup.co.th";
                        string path2 = "";
                        switch (Session["Jobtitle"])
                        {
                            case "Head of Food Industry Sales":
                                path2 = "/Files/S49001.jpg";
                                break;
                            case "Head of Food Service Sales":
                                path2 = "/Files/S60012.jpg";
                                break;
                            case "Head of Modern Trade":
                                path2 = "/Files/S63032.jpg";
                                break;
                            case "Head of Marketing":
                                path2 = "/Files/S64042.jpg";
                                break;
                        }

                        oData4.Head_Approve = Session["Name"].ToString();
                        oData4.Date_Approve = DateTime.Now;
                        oData4.ApprovePosition = Session["JobTitle"].ToString();
                        oData4.pathApprove = path2;

                        db.SaveChanges();
                        
                        switch (oData4.User_Accept)
                        {
                            case "Narisa Samitaman":
                                email = "narisa.s@winnergroup.co.th";
                                break;
                            case "Supaporn Bunnunyang":
                                email = "supaporn.b@winnergroup.co.th";
                                break;
                            case "Pajaree Ruensuk":
                                email = "pajaree.r@winnergroup.co.th";
                                break;
                            case "Panjaphat Budsaya-arunroj":
                                email = "panjaphat.b@winnergroup.co.th";
                                break;
                            default:
                                email = "salessupport@winnergroup.co.th";
                                break;
                        }
                        getSendEmail(oData4.Year_Survey1, email , "approve", Approve);
                        
                        return RedirectToAction("ApproveDetail1", "CR", new { ID_Report = Approve });
                    }
                    else if (Sendemail != null && (ID_Report == null && button == null && print == null && Approve == null))
                    {
                        var oData5 = (from a in db.C_WGE_SURVEY_REPORT where a.ID_Report == Sendemail select a).FirstOrDefault();
                        string email = "";
                        switch (oData5.Dept)
                        {
                            case "c1":
                                email = "yot.s@winnergroup.co.th";
                                break;
                            case "c3":
                                email = "chulawan.c@winnergroup.co.th";
                                break;
                            case "c4":
                                email = "nuttaporn.r@winnergroup.co.th";
                                break;
                            default:
                                email = null;
                                break;
                        }
                            getSendEmail(oData5.Year_Survey1, email, "summary",Sendemail);
                        

                        return RedirectToAction("Approve1", "CR", new { Year = oData5.Year_Survey1 });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLog(ex.ToString());
                TempData["msg"] = "<script>alert('error!');</scrpit>";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ApproveDetail1 (string ID_Report,mWGE mData)
        {
            try
            {
                if(Session["Name"].ToString() != null)
                {
                    
                    using (var db = new DBSEntities())
                    {
                        var checkdata = (from a in db.C_WGE_SURVEY_REPORT
                                         where a.ID_Report == ID_Report
                                         select a).FirstOrDefault();
                        if (checkdata == null)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            var oData = (from a in db.C_WGE_SURVEY1
                                         where a.Generate_Report == ID_Report
                                         select new Detail
                                         {
                                             Name = a.Cardname,
                                             product1 = a.RateProduct_1,
                                             product2 = a.RateProduct_2,
                                             product3 = a.RateProduct_3,
                                             product4 = a.RateProduct_4,
                                             product5 = a.RateProduct_5,
                                             product6 = a.RateProduct_6,
                                             product7 = a.RateProduct_7,
                                             send1 = a.RateTransport_1,
                                             send2 = a.RateTransport_2,
                                             send3 = a.RateTransport_3,
                                             send4 = a.RateTransport_4,
                                             service1 = a.RateService_1,
                                             service2 = a.RateService_2,
                                             service3 = a.RateService_3,
                                             service4 = a.RateService_4,
                                             contact1 = a.RateCom_1,
                                             contact2 = a.RateCom_2,
                                             contact3 = a.RateCom_3
                                         }).ToList();

                            if (oData.Count() > 0)
                            {

                                mData.mwge3 = oData;
                            }
                            var oData2 = (from a in db.C_WGE_SURVEY_REPORT where a.ID_Report == ID_Report select a).FirstOrDefault();
                            mData.Date = DateTime.Now;
                            mData.sURVEY_REPORT = oData2;
                            return View(mData);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }
               
            }
            catch(Exception ex)
            {
                log.WriteLog(ex.ToString());
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult getSendEmail(string year, string emailHD, string namesend, string ID_Report)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    using (var db = new DBSEntities())
                    {
                            int total = 0;
                            int countsuccess = 0;
                            int countfail = 0;
                            var oData = (from a in db.C_WGE_SURVEY_REPORT where a.ID_Report == ID_Report select a).FirstOrDefault();
                        string name;
                        switch (oData.Dept)
                        {
                            case "c1":
                                name = "Head of Food Industry Sales";
                                break;
                            case "c3":
                                name = "Head of Food Service Sales";
                                break;
                            case "c4":
                                name = "Head of Modern Trade";
                                break;
                            default:
                                name = "";
                                break;
                        }
                        string email = emailHD;
                        var senderEmail = new MailAddress("info@winnergroup.co.th", "Sales Support(Winner Survey)");
                        var receiverEmail = new MailAddress(email, "Receiver");
                        MailAddress copy = new MailAddress("pasit.b@winnergroup.co.th");
                        var password = "WGE51001*xx";
                        var subject = "รายงานสรุปแบบสอบถามความพึงพอใจกลุ่มลูกค้าผู้ผลิตหรือผู้จัดจำหน่ายปี " + oData.Year_Survey1;
                        var newline = "<br />";
                        var newline2 = "<br /><br />";
                        var link = "http://110.170.165.101:10470";
                        string body;
                        switch (namesend)
                        {
                            case "summary":
                                body = "เรียน " + name
                                                + newline2 +
                                                "   ระบบได้ทำการสรุปแบบสอบถามความพึงพอใจกลุ่มลูกค้าผู้ผลิตหรือผู้จัดจำหน่ายปี " + oData.Year_Survey1 + " เรียบร้อบแล้ว" +
                                                newline + "โปรดตรวจสอบตาม Link ด้านล่าง :" + newline2 + link + newline2 +
                                                "<font color='red'>" + "***ขั้นตอนการดำเนินการ " + "</front>" + newline +
                                                "ล็อกอินเข้าสู่ระบบ -> ไปยังเมนู Report -> เลือกหัวข้อ 'สรุปแบบสอบถาม " + oData.Year_Survey1 + "'" +
                                                newline2 + "<font color='black'>ผู้สรุป" + newline + oData.User_Accept + newline + oData.UserPosition;
                                break;
                            case "approve":
                                body = "เรียน "+oData.User_Accept+newline2
                                    +"คุณ "+oData.Head_Approve+"ตำแหน่ง : "+oData.ApprovePosition +" ได้ทำการอนุมัติ "+newline+"'สรุปรายงานแบบสอบถามความพึงพอใจกลุ่มลูกค้าผู้ผลิตหรือผู้จัดจำหน่ายปี "+oData.Year_Survey1+"' เรียบร้อยแล้ว"
                                    +newline+"สามารถตรวจสอบได้ตาม link ด้านล่าง:"+newline+link;
                                break;
                            default:
                                body = "";
                                break;
                        }
                                    

                                    var smtp = new SmtpClient
                                    {
                                        Host = "mail.winnergroup.co.th",
                                        Port = 587,
                                        EnableSsl = true,
                                        DeliveryMethod = SmtpDeliveryMethod.Network,
                                        UseDefaultCredentials = false,
                                        Credentials = new NetworkCredential(senderEmail.Address, password)
                                    };
                                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                                    {
                                        Subject = subject,
                                        Body = body
                                    })
                                    {
                                        mess.IsBodyHtml = true;
                                        mess.CC.Add(copy);
                                        smtp.Send(mess);
                                        log.WriteLog("Send : "  + " Success!!");
                                    }


                            string message = string.Format("Send Email => total : {0} ,success : {1} , fail : {2}", total, countsuccess, countfail);
                            TempData["msg"] = "<script>alert('" + message + "');</script>";
                            log.WriteLog(string.Format("{0}({1}) : {2} {3} Email-{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Send", System.Reflection.MethodBase.GetCurrentMethod().Name,email));
                            return RedirectToAction("Index", "Home");
                        
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    TempData["msg"] = "<script>alert('error!!');</script>";
                    return RedirectToAction("SendEmail", new { year = year });
                }
            }
        }
    }
}