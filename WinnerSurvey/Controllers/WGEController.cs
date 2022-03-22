using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
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
    public class WGEController : Controller
    {
        InterfaceLog log = new InterfaceLog(ConfigurationManager.AppSettings["LogsPath"]);

        public ActionResult Detail(int year)
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

                        int saleemp = 0;
                        string usercode = Session["Code"].ToString();

                        if (Session["Type"].ToString() == "Admin")
                        {
                            var oData = (from a in db.C_WGE_SURVEY1
                                         join b in db.OCRD on a.Cardcode equals b.CardCode into bb
                                         from b in bb.DefaultIfEmpty()
                                         join c in db.OSLP on b.SlpCode equals c.SlpCode into cc
                                         from c in cc.DefaultIfEmpty()
                                         where a.Year == year.ToString()
                                         select new C_WGE_SURVEY1_2
                                         {
                                             ID = a.ID,
                                             NumberID = a.NumberID,
                                             Cardcode = a.Cardcode,
                                             Cardname = a.Cardname,
                                             Date = a.Date,
                                             SaleCode = c.SlpName,
                                             Code = a.Code,
                                             SlpNo = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.salesPrson).FirstOrDefault().ToString()
                                         }).AsEnumerable().Select(o => new C_WGE_SURVEY1_2
                                         {
                                             ID = o.ID,
                                             NumberID = o.NumberID,
                                             Cardcode = o.Cardcode,
                                             Cardname = o.Cardname,
                                             Code = o.Code,
                                             Date = o.Date,
                                             SlpNo = o.SlpNo,
                                             SaleCode = (o.SaleCode == null) ? db.OSLP.Where(c => c.SlpCode.ToString() == o.SlpNo).Select(v => v.SlpName).FirstOrDefault() : o.SaleCode,      
                                             EncryptPassword = EncodePasswordToBase64(o.NumberID),
                                             QRCode = db.OHEM.Where(z=>z.ExtEmpNo == o.Code).Select(x=>x.firstName + " " +x.lastName).FirstOrDefault()
                                         }).Distinct();

                            var oData2 = (from b in db.C_WGE_SURVEY2
                                          where b.Year == year.ToString()
                                          select new C_WGE_SURVEY2_2
                                          {
                                              ID = b.ID,
                                              NumberID = b.NumberID,
                                              Name = b.Name,
                                              Date = b.Date,
                                              Code = b.Code
                                          }).AsEnumerable().Select(o => new C_WGE_SURVEY2_2
                                          {
                                              ID = o.ID,
                                              NumberID = o.NumberID,
                                              Name = o.Name,
                                              Date = o.Date,
                                              Code = o.Code,
                                              EncryptPassword = EncodePasswordToBase64(o.NumberID),
                                              QRCode = db.OHEM.Where(z => z.ExtEmpNo == o.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                          }).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListSurvey1 = oData.ToList();
                                mData.mwge2.ListSurvey2 = oData2.ToList();
                                mData.Year = year.ToString();
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
                                         where a.Year == year.ToString() && (b.SlpCode == saleemp || a.Code == usercode)
                                         select new C_WGE_SURVEY1_2
                                         {
                                             ID = a.ID,
                                             NumberID = a.NumberID,
                                             Cardcode = a.Cardcode,
                                             Cardname = a.Cardname,
                                             Date = a.Date,
                                             SaleCode = c.SlpName, 
                                             Code = a.Code,
                                             SlpNo = db.OHEM.Where(z => z.ExtEmpNo == a.Code).Select(x => x.salesPrson).FirstOrDefault().ToString()
                                         }).AsEnumerable().Select(o => new C_WGE_SURVEY1_2
                                         {
                                             ID = o.ID,
                                             NumberID = o.NumberID,
                                             Cardcode = o.Cardcode,
                                             Cardname = o.Cardname,
                                             SaleCode = (o.SaleCode == null) ? db.OSLP.Where(c => c.SlpCode.ToString() == o.SlpNo).Select(v => v.SlpName).FirstOrDefault() : o.SaleCode,
                                             Code = o.Code,
                                             Date = o.Date,
                                             EncryptPassword = EncodePasswordToBase64(o.NumberID),
                                             QRCode = db.OHEM.Where(z => z.ExtEmpNo == o.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                         }).Distinct();

                            var oData2 = (from b in db.C_WGE_SURVEY2
                                          where b.Year == year.ToString() && b.Code == usercode
                                          select new C_WGE_SURVEY2_2
                                          {
                                              ID = b.ID,
                                              NumberID = b.NumberID,
                                              Name = b.Name,
                                              Date = b.Date,
                                              Code = b.Code
                                          }).AsEnumerable().Select(o => new C_WGE_SURVEY2_2
                                          {
                                              ID = o.ID,
                                              NumberID = o.NumberID,
                                              Name = o.Name,
                                              Date = o.Date,
                                              Code = o.Code,
                                              EncryptPassword = EncodePasswordToBase64(o.NumberID),
                                              QRCode = db.OHEM.Where(z => z.ExtEmpNo == o.Code).Select(x => x.firstName + " " + x.lastName).FirstOrDefault()
                                          }).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListSurvey1 = oData.ToList();
                                mData.mwge2.ListSurvey2 = oData2.ToList();
                                mData.Year = year.ToString();
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

        public ActionResult Customer(int year)
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

                        int saleemp = 0;

                        if (Session["Type"].ToString() == "Admin")
                        {
                            var oData = (from d in db.OCRD
                                         join f in db.CRD1 on d.CardCode equals f.CardCode into ff
                                         from f in ff.DefaultIfEmpty()
                                         join g in db.OSLP on d.SlpCode equals g.SlpCode into gg
                                         from g in gg.DefaultIfEmpty()
                                         where !d.CardCode.StartsWith("cp") && d.frozenFor == "N" && f.Address.Equals("sh001") && (d.CardCode.StartsWith("c1") || d.CardCode.StartsWith("c2") || d.CardCode.StartsWith("c3") || d.CardCode.StartsWith("c4"))
                                         select new OCRD2
                                         {
                                             CardCode = d.CardCode,
                                             CardName = d.CardName,
                                             U_WGE_Property1 = d.U_WGE_Property1,
                                             Street = f.Street,
                                             SaleCode = g.SlpName
                                         }).AsEnumerable().Select(o => new OCRD2
                                         {
                                             CardCode = o.CardCode,
                                             CardName = o.CardName,
                                             U_WGE_Property1 = o.U_WGE_Property1,
                                             Street = o.Street,
                                             SaleCode = o.SaleCode,
                                             EncryptPassword = EncodePasswordToBase64(o.CardCode)
                                         }).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListOCRD = oData.ToList();
                                mData.Year = year.ToString();
                            }

                            return View(mData);
                        }
                        else
                        {
                            saleemp = Convert.ToInt32(Session["SaleEMP"].ToString());

                            var oData = (from d in db.OCRD
                                         join f in db.CRD1 on d.CardCode equals f.CardCode into ff
                                         from f in ff.DefaultIfEmpty()
                                         join g in db.OSLP on d.SlpCode equals g.SlpCode into gg
                                         from g in gg.DefaultIfEmpty()
                                         where !d.CardCode.StartsWith("cp") && d.frozenFor == "N" && d.SlpCode == saleemp && f.Address.Equals("sh001") && (d.CardCode.StartsWith("c1") || d.CardCode.StartsWith("c2") || d.CardCode.StartsWith("c3") || d.CardCode.StartsWith("c4"))
                                         select new OCRD2
                                         {
                                             CardCode = d.CardCode,
                                             CardName = d.CardName,
                                             U_WGE_Property1 = d.U_WGE_Property1,
                                             Street = f.Street,
                                             SaleCode = g.SlpName
                                         }).AsEnumerable().Select(o => new OCRD2
                                         {
                                             CardCode = o.CardCode,
                                             CardName = o.CardName,
                                             U_WGE_Property1 = o.U_WGE_Property1,
                                             Street = o.Street,
                                             SaleCode = o.SaleCode,
                                             EncryptPassword = EncodePasswordToBase64(o.CardCode)
                                         }).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListOCRD = oData.ToList();
                                mData.Year = year.ToString();
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

        public ActionResult WinnerSurvey1(int year, string cardcode, string numberid,string code)
        {
            mWGE mData = new mWGE();
            mData.mwge1 = new mWGE1();
            mData.mwge2 = new mWGE2();
            if (year == null)
            {

                return RedirectToAction("WinnerSurvey1", "WGE", new { year = DateTime.Now.Year });
            }
            else
            {
                    try
                    {
                        using (var db = new DBSEntities())
                        {
                    

                                mData.Year = year.ToString();
                                mData.Decode = cardcode;
                                mData.Code = code;
                                mData.NumberID = numberid;
                                if (cardcode != null && numberid == null)
                                {
                                    string decode = DecodeFrom64(cardcode);

                                    var oData = (from a in db.C_WGE_SURVEY1
                                                 join b in db.C_WGE_SURVEY_DETAIL on a.NumberID equals b.NumberID
                                                 where a.Year == year.ToString() && a.Cardcode == decode
                                                 select new C_WGE_SURVEY1_2
                                                 {
                                                     NumberID = a.NumberID,
                                                     Date = a.Date,
                                                     Cardcode = a.Cardcode,
                                                     Cardname = a.Cardname,
                                                     District = a.District,
                                                     Province = a.Province,
                                                     Name = a.Name,
                                                     Department = a.Department,
                                                     Email = a.Email,
                                                     Tel = a.Tel,
                                                     Year = a.Year,
                                                     RateProduct_1 = a.RateProduct_1,
                                                     RateProduct_2 = a.RateProduct_2,
                                                     RateProduct_3 = a.RateProduct_3,
                                                     RateProduct_4 = a.RateProduct_4,
                                                     RateProduct_5 = a.RateProduct_5,
                                                     RateProduct_6 = a.RateProduct_6,
                                                     RateProduct_7 = a.RateProduct_7,
                                                     RateService_1 = a.RateService_1,
                                                     RateService_2 = a.RateService_2,
                                                     RateService_3 = a.RateService_3,
                                                     RateService_4 = a.RateService_4,
                                                     RateTransport_1 = a.RateTransport_1,
                                                     RateTransport_2 = a.RateTransport_2,
                                                     RateTransport_3 = a.RateTransport_3,
                                                     RateTransport_4 = a.RateTransport_4,
                                                     RateCom_1 = a.RateCom_1,
                                                     RateCom_2 = a.RateCom_2,
                                                     RateCom_3 = a.RateCom_3,
                                                     RemarkProduct = a.RemarkProduct,
                                                     RemarkService = a.RemarkService,
                                                     RemarkTransport = a.RemarkTransport,
                                                     RemarkCom = a.RemarkCom,
                                                     Detail = b.Detail
                                                 });

                                    var oData2 = (from a in db.OCRD
                                                  join b in db.CRD1 on a.CardCode equals b.CardCode
                                                  where a.CardCode == decode
                                                  select a).FirstOrDefault();

                                    if (oData2 != null)
                                    {
                                        if (oData != null)
                                        {
                                            mData.mwge2.ListSurvey1 = oData.ToList();
                                        }

                                        mData.mwge1.OCRD = oData2;
                                        mData.Date = DateTime.Now;
                                        

                                        return View(mData);
                                    }
                                    else
                                    {
                                        if (Session["Name"] != null)
                                        {
                                            TempData["msg"] = "<script>alert('Not Found!');</script>";
                                            return RedirectToAction("Index", "Home");
                                        }
                                        else
                                        {
                                            TempData["msg"] = "<script>alert('Not Found!');</script>";
                                            return RedirectToAction("Index2", "Home");
                                        }
                                    }
                                }
                                else if (cardcode == null && numberid != null && code != null) //กรณีไม่มี cardcode
                                {
                                    string decode = DecodeFrom64(numberid);

                                    var oData3 = (from a in db.C_WGE_SURVEY1
                                                  join b in db.C_WGE_SURVEY_DETAIL on a.NumberID equals b.NumberID
                                                  where a.Year == year.ToString() && a.NumberID == decode
                                                  select new C_WGE_SURVEY1_2
                                                  {
                                                      NumberID = a.NumberID,
                                                      Date = a.Date,
                                                      Cardcode = a.Cardcode,
                                                      Cardname = a.Cardname,
                                                      District = a.District,
                                                      Province = a.Province,
                                                      Name = a.Name,
                                                      Department = a.Department,
                                                      Email = a.Email,
                                                      Tel = a.Tel,
                                                      Year = a.Year,
                                                      RateProduct_1 = a.RateProduct_1,
                                                      RateProduct_2 = a.RateProduct_2,
                                                      RateProduct_3 = a.RateProduct_3,
                                                      RateProduct_4 = a.RateProduct_4,
                                                      RateProduct_5 = a.RateProduct_5,
                                                      RateProduct_6 = a.RateProduct_6,
                                                      RateProduct_7 = a.RateProduct_7,
                                                      RateService_1 = a.RateService_1,
                                                      RateService_2 = a.RateService_2,
                                                      RateService_3 = a.RateService_3,
                                                      RateService_4 = a.RateService_4,
                                                      RateTransport_1 = a.RateTransport_1,
                                                      RateTransport_2 = a.RateTransport_2,
                                                      RateTransport_3 = a.RateTransport_3,
                                                      RateTransport_4 = a.RateTransport_4,
                                                      RateCom_1 = a.RateCom_1,
                                                      RateCom_2 = a.RateCom_2,
                                                      RateCom_3 = a.RateCom_3,
                                                      RemarkProduct = a.RemarkProduct,
                                                      RemarkService = a.RemarkService,
                                                      RemarkTransport = a.RemarkTransport,
                                                      RemarkCom = a.RemarkCom,
                                                      Detail = b.Detail
                                                  });


                                    mData.mwge2.ListSurvey1 = oData3.ToList();

                                    if (mData.mwge2.ListSurvey1.Count > 0)
                                    {
                                        mData.Date = DateTime.Now;
                                        return View(mData);
                                    }
                                    else
                                    {
                                        if (Session["Name"] != null)
                                        {
                                            TempData["msg"] = "<script>alert('Not Found!');</script>";
                                            return RedirectToAction("Index", "Home");
                                        }
                                        else
                                        {
                                            return RedirectToAction("WinnerSurvey1", new { year = year, code = code ,cardcode = cardcode});
                                        }
                                    }
                                }
                                else
                                {
                                    mData.Date = DateTime.Now;
                                    return View(mData);
                                    
                                }
                    
                    
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Session["Name"] != null)
                        {
                            TempData["msg"] = "<script>alert('Error!');</script>";
                            log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["msg"] = "<script>alert('Error!');</script>";
                            log.WriteLog(string.Format("{0} : {1} {2}/{3}", "Guest", "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                            return RedirectToAction("Index2", "Home");
                        }
                    }
            }
        }

        [HttpPost]
        public ActionResult getWinnerSurvey1(mWGE data, string[] checkbox, string textOther)
        {
            try
            {
                using (var db = new DBSEntities())
                {
                    C_WGE_SURVEY1 dbs = new C_WGE_SURVEY1();
                    C_WGE_SURVEY_DETAIL dbs2 = new C_WGE_SURVEY_DETAIL();

                    string datetime = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    dbs.NumberID = datetime;
                    dbs.Date = DateTime.Now;
                    dbs.Cardcode = data.mwge2.CS1.Cardcode;
                    dbs.Cardname = data.mwge2.CS1.Cardname;
                    dbs.District = data.mwge2.CS1.District;
                    dbs.Province = data.mwge2.CS1.Province;
                    dbs.Name = data.mwge2.CS1.Name;
                    dbs.Department = data.mwge2.CS1.Department;
                    dbs.Email = data.mwge2.CS1.Email;
                    dbs.Tel = data.mwge2.CS1.Tel;
                    dbs.Year = data.Year;

                    string decode = (data.Code != null) ? DecodeFrom64(data.Code) : null;
                    

                    if (decode != null)
                    {
                        var Slp_N = (from a in db.OHEM
                                     join b in db.OSLP on a.salesPrson equals b.SlpCode
                                     where a.ExtEmpNo == decode
                                     select b.SlpName).FirstOrDefault();
                        dbs.SlpName = Slp_N;
                        dbs.Code = decode;
                    }                    
  
                    foreach (var item in checkbox)
                    {
                        dbs2.NumberID = datetime;

                        if (item == "other")
                            dbs2.Detail = textOther;
                        else
                            dbs2.Detail = item;

                        db.C_WGE_SURVEY_DETAIL.Add(dbs2);
                        db.SaveChanges();
                    }

                    dbs.RateProduct_1 = data.mwge2.CS1.RateProduct_1;
                    dbs.RateProduct_2 = data.mwge2.CS1.RateProduct_2;
                    dbs.RateProduct_3 = data.mwge2.CS1.RateProduct_3;
                    dbs.RateProduct_4 = data.mwge2.CS1.RateProduct_4;
                    dbs.RateProduct_5 = data.mwge2.CS1.RateProduct_5;
                    dbs.RateProduct_6 = data.mwge2.CS1.RateProduct_6;
                    dbs.RateProduct_7 = data.mwge2.CS1.RateProduct_7;

                    dbs.RateTransport_1 = data.mwge2.CS1.RateTransport_1;
                    dbs.RateTransport_2 = data.mwge2.CS1.RateTransport_2;
                    dbs.RateTransport_3 = data.mwge2.CS1.RateTransport_3;
                    dbs.RateTransport_4 = data.mwge2.CS1.RateTransport_4;

                    dbs.RateService_1 = data.mwge2.CS1.RateService_1;
                    dbs.RateService_2 = data.mwge2.CS1.RateService_2;
                    dbs.RateService_3 = data.mwge2.CS1.RateService_3;
                    dbs.RateService_4 = data.mwge2.CS1.RateService_4;

                    dbs.RateCom_1 = data.mwge2.CS1.RateCom_1;
                    dbs.RateCom_2 = data.mwge2.CS1.RateCom_2;
                    dbs.RateCom_3 = data.mwge2.CS1.RateCom_3;

                    dbs.RemarkProduct = data.mwge2.CS1.RemarkProduct;
                    dbs.RemarkTransport = data.mwge2.CS1.RemarkTransport;
                    dbs.RemarkService = data.mwge2.CS1.RemarkService;
                    dbs.RemarkCom = data.mwge2.CS1.RemarkCom;
                    dbs.SubmitDate = DateTime.Now;

                    db.C_WGE_SURVEY1.Add(dbs);
                    db.SaveChanges();

                    TempData["msg"] = "<script>alert('Success!');</script>";
                    return RedirectToAction("WinnerSurvey1", new { year = data.Year, numberid = EncodePasswordToBase64(datetime), code= data.Code });
                }
            }
            catch (Exception ex)
            {
                if (Session["Name"] != null)
                {
                    TempData["msg"] = "<script>alert('Error!');</script>";
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["msg"] = "<script>alert('Error!');</script>";
                    log.WriteLog(string.Format("{0} : {1} {2}/{3}", "Guest", "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index2", "Home");
                }
            }
        }

        public ActionResult WinnerSurvey2(int year, string numberid,string code)
        {
            mWGE mData = new mWGE();
            mData.mwge1 = new mWGE1();
            mData.mwge2 = new mWGE2();

            try
            {
                using (var db = new DBSEntities())
                {
                    mData.Year = year.ToString();

                    if (numberid != null)
                    {
                        string decode = DecodeFrom64(numberid);

                        var oData3 = (from a in db.C_WGE_SURVEY2
                                      join b in db.C_WGE_SURVEY_DETAIL on a.NumberID equals b.NumberID
                                      where a.Year == year.ToString() && a.NumberID == decode
                                      select new C_WGE_SURVEY2_2
                                      {
                                          NumberID = a.NumberID,
                                          Date = a.Date,
                                          Name = a.Name,
                                          Email = a.Email,
                                          Tel = a.Tel,
                                          Age = a.Age,
                                          Salary = a.Salary,
                                          Year = a.Year,
                                          Rate_1 = a.Rate_1,
                                          Rate_2 = a.Rate_2,
                                          Rate_3 = a.Rate_3,
                                          Rate_4 = a.Rate_4,
                                          Rate_5 = a.Rate_5,
                                          Rate_6 = a.Rate_6,
                                          Rate_7 = a.Rate_7,
                                          Rate_8 = a.Rate_8,
                                          Rate_9 = a.Rate_9,
                                          Rate_10 = a.Rate_10,
                                          Remark = a.Remark,
                                          Detail = b.Detail
                                      });

                        mData.mwge2.ListSurvey2 = oData3.ToList();

                        if (mData.mwge2.ListSurvey2.Count > 0)
                        {
                            mData.Date = DateTime.Now;
                            return View(mData);
                        }
                        else
                        {
                            if (Session["Name"] != null)
                            {
                                TempData["msg"] = "<script>alert('Not Found!');</script>";
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["msg"] = "<script>alert('Not Found!');</script>";
                                return RedirectToAction("Index2", "Home");
                            }
                        }
                    }
                    else
                    {
                        mData.Date = DateTime.Now;

                        if (code != null)
                        {
                            string Code = code;
                            mData.Code = Code;

                            return View(mData);
                        }
                        else
                        {
                            return View(mData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Session["Name"] != null)
                {
                    TempData["msg"] = "<script>alert('Error!');</script>";
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["msg"] = "<script>alert('Error!');</script>";
                    log.WriteLog(string.Format("{0} : {1} {2}/{3}", "Guest", "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index2", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult getWinnerSurvey2(mWGE data, string[] checkbox, string textOther)
        {
            try
            {
                using (var db = new DBSEntities())
                {
                    C_WGE_SURVEY2 dbs = new C_WGE_SURVEY2();
                    C_WGE_SURVEY_DETAIL dbs2 = new C_WGE_SURVEY_DETAIL();

                    string datetime = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                    dbs.NumberID = datetime;
                    dbs.Date = DateTime.Now;
                    dbs.Name = data.mwge2.CS2.Name;
                    dbs.Email = data.mwge2.CS2.Email;
                    dbs.Tel = data.mwge2.CS2.Tel;
                    dbs.Age = data.mwge2.CS2.Age;
                    dbs.Salary = data.mwge2.CS2.Salary;
                    dbs.Year = data.Year;

                    string decode = (data.Code != null) ? DecodeFrom64(data.Code) : null;

                    if (decode != null)
                    {
                        dbs.Code = decode;
                    }

                    foreach (var item in checkbox)
                    {
                        dbs2.NumberID = datetime;
                        
                        if (item == "other")
                        {
                            dbs2.Detail = textOther;
                        }
                        else
                        {
                            dbs2.Detail = item;
                        }

                            db.C_WGE_SURVEY_DETAIL.Add(dbs2);
                            db.SaveChanges();
                        
                    }

                    dbs.Rate_1 = data.mwge2.CS2.Rate_1;
                    dbs.Rate_2 = data.mwge2.CS2.Rate_2;
                    dbs.Rate_3 = data.mwge2.CS2.Rate_3;
                    dbs.Rate_4 = data.mwge2.CS2.Rate_4;
                    dbs.Rate_5 = data.mwge2.CS2.Rate_5;
                    dbs.Rate_6 = data.mwge2.CS2.Rate_6;
                    dbs.Rate_7 = data.mwge2.CS2.Rate_7;
                    dbs.Rate_8 = data.mwge2.CS2.Rate_8;
                    dbs.Rate_9 = data.mwge2.CS2.Rate_9;
                    dbs.Rate_10 = data.mwge2.CS2.Rate_10;

                    dbs.Remark = data.mwge2.CS2.Remark;
                    dbs.SubmitDate = DateTime.Now;

                    db.C_WGE_SURVEY2.Add(dbs);
                    db.SaveChanges();

                    TempData["msg"] = "<script>alert('Success!');</script>";
                    return RedirectToAction("WinnerSurvey2", new { year = data.Year, numberid = EncodePasswordToBase64(datetime) });
                }
            }
            catch (Exception ex)
            {
                if (Session["Name"] != null)
                {
                    TempData["msg"] = "<script>alert('Error!');</script>";
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["msg"] = "<script>alert('Error!');</script>";
                    log.WriteLog(string.Format("{0} : {1} {2}/{3}", "Guest", "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index2", "Home");
                }
            }
        }

        public ActionResult SendEmail(int year)
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

                        int saleemp = 0;

                        if (Session["Type"].ToString() == "Admin")
                        {
                            var oData = (from d in db.OCRD
                                         join f in db.CRD1 on d.CardCode equals f.CardCode into ff
                                         from f in ff.DefaultIfEmpty()
                                         join g in db.C_WGE_SURVEY_EMAIL on d.CardCode equals g.CardCode into gg
                                         from g in gg.DefaultIfEmpty()
                                         join h in db.OSLP on d.SlpCode equals h.SlpCode into hh
                                         from h in hh.DefaultIfEmpty()
                                         where !d.CardCode.StartsWith("cp") && d.frozenFor == "N" && f.Address.Equals("sh001") && ( g.Year == year || g.Year == null ) && (d.CardCode.StartsWith("c1") || d.CardCode.StartsWith("c2") || d.CardCode.StartsWith("c3") || d.CardCode.StartsWith("c4"))
                                         select new OCRD2
                                         {
                                             CardCode = d.CardCode,
                                             CardName = d.CardName,
                                             U_WGE_Property1 = d.U_WGE_Property1,
                                             Street = f.Street,
                                             Email = g.Email,
                                             SaleCode = h.SlpName
                                         }).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListOCRD = oData.ToList();
                                mData.Year = year.ToString();
                            }

                            return View(mData);
                        }
                        else
                        {
                            saleemp = Convert.ToInt32(Session["SaleEMP"].ToString());

                            var oData = (from d in db.OCRD
                                         join f in db.CRD1 on d.CardCode equals f.CardCode into ff
                                         from f in ff.DefaultIfEmpty()
                                         join g in db.C_WGE_SURVEY_EMAIL on d.CardCode equals g.CardCode into gg
                                         from g in gg.DefaultIfEmpty()
                                         join h in db.OSLP on d.SlpCode equals h.SlpCode into hh
                                         from h in hh.DefaultIfEmpty()
                                         where !d.CardCode.StartsWith("cp") && d.frozenFor == "N" && d.SlpCode == saleemp && f.Address.Equals("sh001") && (g.Year == year || g.Year == null) && (d.CardCode.StartsWith("c1") || d.CardCode.StartsWith("c2") || d.CardCode.StartsWith("c3") || d.CardCode.StartsWith("c4"))
                                         select new OCRD2
                                         {
                                             CardCode = d.CardCode,
                                             CardName = d.CardName,
                                             U_WGE_Property1 = d.U_WGE_Property1,
                                             Street = f.Street,
                                             Email = g.Email,
                                             SaleCode = h.SlpName
                                         }).Distinct();

                            if (oData != null)
                            {
                                mData.mwge2.ListOCRD = oData.ToList();
                                mData.Year = year.ToString();
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
        public ActionResult getSendEmail(int year, string[] checkbox1,string cardcode)
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
                        if(cardcode != null) //edit email
                        {
                            TempData["Data"] = cardcode;
                            TempData["Year"] = year;
                            return RedirectToAction("EmailEdit", "WGE");
                        }
                        else //send email
                        {
                            int total = 0;
                            int countsuccess = 0;
                            int countfail = 0;

                            foreach (var item in checkbox1)
                            {                               
                                OCRD mData = db.OCRD.Where(o => o.CardCode.Equals(item)).FirstOrDefault();
                                string encode = EncodePasswordToBase64(item);
                                string link = "http://110.170.165.101:10470/WGE/WinnerSurvey1?&cardcode=" + encode + "&year=" + year;

                                C_WGE_SURVEY_EMAIL mData2 = db.C_WGE_SURVEY_EMAIL.Where(o => o.CardCode.Equals(item)).FirstOrDefault();

                                if (mData2 != null)
                                {
                                    string email = mData2.Email;
                                    var senderEmail = new MailAddress("info@winnergroup.co.th", "Sales Support(Winner Survey)");
                                    var receiverEmail = new MailAddress(email, "Receiver");
                                    var subject = "Winnergroup Enterprise PLC.";
                                    var newline = "<br />";
                                    var newline2 = "<br /><br />";

                                    SmtpClient smtp = new SmtpClient();
                                    smtp.UseDefaultCredentials = false;
                                    smtp.Credentials = new System.Net.NetworkCredential("info@winnergroup.co.th", "A123456a");
                                    smtp.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
                                    smtp.Host = "smtp.office365.com";
                                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    smtp.EnableSsl = true;

                                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                                    {
                                        Subject = subject,
                                        Body = "เรียน  คู่ค้าธุรกิจ/ผู้ใช้บริการสินค้า ทุกท่าน" + newline2 + "ทางบริษัท WGE ได้ดำเนินการธุรกิจตามนโยบาย ISO9001 เพื่อให้เป็นไปตามนโยบาย  ทางบริษัทจึงขอส่งแบบสำรวจความพึงพอใจเพื่อวัดระดับความพึงพอใจของผู้ใช้สินค้าของบริษัททุกท่าน ตลอดจนรับทราบความคิดเห็น / ข้อเสนอแนะต่างๆ ที่มีต่อสินค้าและบริการของบริษัท เพื่อนำไปพัฒนาและยกระดับมาตรฐานในการให้บริการต่อไป" +
                                                newline2 + "คลิกที่นี่ เพื่อตอบแบบสำรวจ : " + link + newline2 + "รบกวนดำเนินการตอบกลับภายในวันที่ 27 ธ.ค. 2565" + newline2 + "ขอบคุณในความร่วมมือ" + newline2 + "Sales Support /  Company LOGO"
                                    })
                                    {
                                        mess.IsBodyHtml = true;
                                        smtp.Send(mess);
                                        log.WriteLog("Send : " + item + " Success!!");
                                    }

                                    countsuccess++;
                                }
                                else
                                {
                                    countfail++;
                                }

                                total++;
                            }

                            string message = string.Format("Send Email => total : {0} ,success : {1} , fail : {2}", total, countsuccess, countfail);
                            TempData["msg"] = "<script>alert('"+message+"');</script>";
                            return RedirectToAction("Index", "Home");
                        }                                                                         
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

        public ActionResult EmailEdit()
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (var db = new DBSEntities())
                {
                    try
                    {
                        mWGE mData = new mWGE();
                        mData.mwge1 = new mWGE1();
                        mData.mwge2 = new mWGE2();

                        string cardcode = TempData["data"].ToString();
                        int year = Convert.ToInt32(TempData["Year"].ToString());
                        var oData = (from a in db.OCRD
                                     join b in db.C_WGE_SURVEY_EMAIL on a.CardCode equals b.CardCode into bb
                                     from b in bb.DefaultIfEmpty()
                                     where a.CardCode == cardcode && b.Year==year
                                     select new OCRD2
                                     {
                                         CardCode = a.CardCode,
                                         CardName = a.CardName,
                                         Email = b.Email
                                     }).FirstOrDefault();
                       
                        if (oData != null)
                        {
                            mData.mwge2.OCRD = oData;
                            mData.Year = TempData["Year"].ToString();
                            return View(mData);
                        }
                        else
                        {
                            TempData["msg"] = "<script>alert('Not Found!');</script>";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                        return RedirectToAction("Index", "Home");
                    }
                }                   
            }          
        }

        [HttpPost]
        public ActionResult getEmail(string email,string cardcode,int year)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                using (var db = new DBSEntities())
                {
                    C_WGE_SURVEY_EMAIL dbs = new C_WGE_SURVEY_EMAIL();

                    var oData = (from a in db.C_WGE_SURVEY_EMAIL
                                 where a.CardCode == cardcode && a.Year == year
                                 select a).FirstOrDefault();

                    if (oData == null) //insert
                    {
                        dbs.CardCode = cardcode;
                        dbs.Year = year;
                        dbs.Email = email;
                        db.C_WGE_SURVEY_EMAIL.Add(dbs);
                        db.SaveChanges();

                        TempData["msg"] = "<script>alert('Success');</script>";
                    }
                    else //edit
                    {
                        //dbs.CardCode = cardcode;
                        oData.Email = email;
                        oData.Year = year;
                        db.SaveChanges();

                        TempData["msg"] = "<script>alert('Success');</script>";
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString()));
                TempData["msg"] = "<script>alert('Error');</script>";
            }

            return RedirectToAction("SendEmail",new { year = year } );
        }

        public ActionResult QRCoder1(int year)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    mWGE mData = new mWGE();

                    string code = (Session["Code"].ToString() != null) ? EncodePasswordToBase64(Session["Code"].ToString()) : null;

                    if(code != null)
                    {
                        string address = "http://110.170.165.101:10470/WGE/WinnerSurvey1?year=" + year+"&code="+ code;

                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(address, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        mData.Data = (byte[])new ImageConverter().ConvertTo(qrCodeImage, typeof(byte[]));

                        return View(mData);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('QR Code Error!');</script>";
                        return RedirectToAction("Index", "Home");
                    }                                  
                }
                catch (Exception ex)
                {
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult QRCoder2(int year)
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    mWGE mData = new mWGE();

                    string code = (Session["Code"].ToString() != null) ? EncodePasswordToBase64(Session["Code"].ToString()) : null;

                    if (code != null)
                    {
                        string address = "http://110.170.165.101:10470/WGE/WinnerSurvey2?year=" + year + "&code=" + code;

                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(address, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        mData.Data = (byte[])new ImageConverter().ConvertTo(qrCodeImage, typeof(byte[]));

                        return View(mData);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('QR Code Error!');</script>";
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog(string.Format("{0}({1}) : {2} {3}/{4}", Session["Name"].ToString(), Session["Code"].ToString(), "Error", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString()));
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}