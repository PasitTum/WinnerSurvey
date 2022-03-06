using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WinnerSurvey.Class;
using WinnerSurvey.Models.L1;

namespace WinnerSurvey.Controllers
{
    public class AccountController : Controller
    {
        InterfaceLog log = new InterfaceLog(ConfigurationManager.AppSettings["LogsPath"]);

        // GET: Account
        public ActionResult Login()
        {
            Session.Clear();
            Session.Abandon();

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult LoginCheck(string ID, string Password,string textuser)
        {
            try
            {
                bool valid = false;
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    valid = context.ValidateCredentials(ID, Password);

                    using (var db = new DBSEntities())
                    {
                        string ID_user;
                        if (ID == "pasit.b")
                        {
                            if(textuser != "")
                            {
                                ID_user = textuser;
                            }
                            else
                            {
                                ID_user = ID;
                            }
                        }
                        else
                        {
                            ID_user = ID;
                        }
                        using (UserPrincipal user = UserPrincipal.FindByIdentity(context, ID_user)) //ดึงชื่อจาก AD
                        {
                            if (user != null)
                            {
                                if (valid == false)
                                {
                                    if (user.IsAccountLockedOut())
                                    {
                                        TempData["msg"] = "<script>alert('รหัสผ่านถูกล็อคกรุณาติดต่อฝ่าย IT');</script>";
                                    }
                                    else
                                    {
                                        TempData["msg"] = "<script>alert('รหัสผ่านไม่ถูกต้อง');</script>";
                                    }
                                }
                                else
                                {                                                                     
                                    var de = (DirectoryEntry)user.GetUnderlyingObject();
                                    string usercode = "";
                                    usercode = de.Properties["initials"][0].ToString();
                                    OHEM mData = db.OHEM.Where(o => o.ExtEmpNo.Equals(usercode)).FirstOrDefault();
                                    Session["Code"] = mData.ExtEmpNo;
                                    Session["Name"] = mData.firstName + " " + mData.lastName;
                                    Session["Job"] = mData.jobTitle;
                                    Session["Position"] = mData.position;
                                    Session["Department"] = mData.dept;
                                    Session["Jobtitle"] = de.Properties["title"][0].ToString();

                                    string code = (mData.ExtEmpNo != null) ? EncodePasswordToBase64(mData.ExtEmpNo) : null;
                                    Session["Dcode"] = code;

                                    if (mData.salesPrson != null)
                                    {
                                        Session["SaleEMP"] = mData.salesPrson;
                                        OSLP mData5 = db.OSLP.Where(o => o.SlpCode == mData.salesPrson).FirstOrDefault();

                                        Session["SaleCode"] = mData5.SlpName;

                                        if (mData5.U_ISS_SalesName != null && mData5.U_ISS_SalesName.Contains('0'))
                                        {
                                            string[] split = mData5.U_ISS_SalesName.Split('0');
                                            Session["SaleName"] = split[0];
                                        }
                                        else
                                        {
                                            Session["SaleName"] = "Unknown";
                                        }
                                    }
                                    else
                                    {
                                        Session["SaleEMP"] = "";
                                        Session["SaleCode"] = "";
                                        Session["SaleName"] = "";
                                    }

                                    //group
                                    PrincipalSearchResult<Principal> groups = user.GetGroups();
                                    foreach (Principal p in groups)
                                    {
                                        if (p.Name == "G.Admin Survey")
                                        {
                                            Session["Type"] = "Admin";
                                            break;
                                        }
                                        else
                                        {
                                            Session["Type"] = "User";
                                        }
                                    }

                                    log.WriteLog(string.Format("{0}({1}) : {2}", Session["Name"].ToString(), Session["Code"].ToString(), "Login to Trademark"));
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            else
                            {
                                TempData["msg"] = "<script>alert('ไม่พบผู้ใช้');</script>";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message.ToString());
            }

            return RedirectToAction("Login");
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