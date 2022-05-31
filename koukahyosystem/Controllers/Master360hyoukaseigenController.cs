using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace koukahyosystem.Controllers
{
    public class Master360hyoukaseigenController : Controller
    {
        Models.Master360hyoukaseigen val = new Models.Master360hyoukaseigen();
        string curYear = "";
        string cshain = "";
        // GET: Master360hyoukaseigen
        public ActionResult Master360hyoukaseigen()
        {
            if (Session["isAuthenticated"] != null)
            {
                if (Session["LoginName"] != null)
                {
                    string name = Session["LoginName"].ToString();
                    cshain = FindLoginId(name);
                    if (Session["curr_nendou"] != null)
                    {
                        curYear = Session["curr_nendou"].ToString();
                    }
                    else
                    {
                        curYear = System.DateTime.Now.Year.ToString();
                    }
                }
                string is_irai = "";
                DataTable dt_irai = new DataTable();
                is_irai += "select * from r_hyouka where dNENDOU='" + curYear + "'";
                var checkirai = new SqlDataConnController();
                dt_irai = checkirai.ReadData(is_irai);
                if (dt_irai.Rows.Count > 0)
                {
                    val.txt_disabled = "disabled";
                }
                else
                {
                    val.txt_disabled = "";
                }
                var readData = new DateController();
                val.YearList = readData.YearList("Hyoukairai");
                val.cur_year = curYear;
                val.SeigenName = show_data(curYear);
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }

        #region findloginId
        private string FindLoginId(string name)
        {
            string id = "";
            try
            {
                DataTable dt_shain = new DataTable();
                string sqlStr = "SELECT cSHAIN FROM m_shain where sLOGIN = '" + name + "'";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlStr);
                if (dt_shain.Rows.Count > 0)
                {
                    id = dt_shain.Rows[0]["cSHAIN"].ToString();
                }
            }
            catch
            {
            }
            return id;
        }
        #endregion

        #region postmethod
        [HttpPost]
        public ActionResult Master360hyoukaseigen(Models.Master360hyoukaseigen model)
        {
            if (Session["isAuthenticated"] != null)
            {
                val = model;
                bool chk = false;
                if (Request["year_btn"] == "display")
                {
                    string selectedyear=model.cur_year;
                    if (int.Parse(Session["curr_nendou"].ToString()) <= int.Parse(selectedyear))
                    {
                        chk = true;
                        bool retuval = checkirai(selectedyear);
                        if (retuval == true)
                        {
                            model.txt_disabled = "disabled";
                        }
                        else
                        {
                            model.txt_disabled = "";
                        }
                    }
                    else
                    {
                        chk = false;
                        model.txt_disabled = "disabled";
                    }
                    val.SeigenName = show_data(selectedyear);
                    val.cur_year = selectedyear;
                    ModelState.Clear();
                }
                else if (Request["year_btn"] != null)
                {
                    string selectedyear = "";
                    if (Request["year_btn"] == "<")
                    {
                        var readDate = new DateController();
                        selectedyear = readDate.PreYear(model.cur_year);
                        if (int.Parse(Session["curr_nendou"].ToString()) <= int.Parse(selectedyear))
                        {
                            chk = true;
                        }
                        else
                        {
                            chk = false;
                        }
                    }
                    if (Request["year_btn"] == ">")
                    {
                        var readDate = new DateController();
                        selectedyear = readDate.NextYear(model.cur_year, "");
                        if (int.Parse(Session["curr_nendou"].ToString()) == int.Parse(selectedyear))
                        {
                            chk = true;
                        }
                        else
                        {
                            chk = false;
                        }
                    }
                    curYear = selectedyear;
                    val.SeigenName = show_data(curYear);
                    val.cur_year = selectedyear;
                    val.f_nentou = chk;
                    ModelState.Clear();

                    if (val.f_nentou == false)
                    {
                        model.txt_disabled = "disabled";
                    }
                    else
                    {
                        bool retuval = checkirai(curYear);
                        if (retuval == true)
                        {
                            model.txt_disabled = "disabled";
                        }
                        else
                        {
                            model.txt_disabled = "";
                        }
                    }
                }
                else if (Request["save_btn"] == "保存")
                {
                    bool f_save = false;
                    string currentyear = "";
                    if (model.SeigenName != null)
                    {
                        string txt_seigen = model.SeigenName;
                        int seigen = int.Parse(txt_seigen);
                        if (Session["LoginName"] != null)
                        {
                            currentyear = model.cur_year;
                            bool retuval = checkirai(currentyear);
                            if (retuval == true)
                            {
                                TempData["com_msg"] = "評価中です。保存できません。";
                                string con_sei = Convert.ToString(seigen);
                                con_sei = "";
                                model.SeigenName = con_sei;
                                model.txt_disabled = "disabled";
                            }
                            else
                            {
                                model.txt_disabled = "";
                                if (seigen == 0 || seigen > 10)
                                {
                                    TempData["com_msg"] = "制限は10人までです。";
                                    string con_sei = Convert.ToString(seigen);
                                    con_sei = "";
                                    model.SeigenName = con_sei;
                                }
                                else
                                {
                                    string sqlinsert = "";
                                    string selectdata = "";
                                    currentyear = model.cur_year;
                                    if (Session["LoginName"] != null)
                                    {
                                        string name = Session["LoginName"].ToString();
                                        cshain = FindLoginId(name);
                                        selectdata += "select * from m_360seigen where dnendou='" + currentyear + "'";
                                        DataTable dt_select = new DataTable();
                                        var showdata = new SqlDataConnController();
                                        dt_select = showdata.ReadData(selectdata);
                                        if (dt_select.Rows.Count > 0)
                                        {
                                            string updatesql = "";
                                            updatesql += "update m_360seigen set dcreate=now(),csakuseisha='" + cshain + "',nseigen='" + seigen + "' where dnendou='" + currentyear + "'";
                                            if (updatesql != "")
                                            {
                                                var updatedata = new SqlDataConnController();
                                                f_save = updatedata.inputsql(updatesql);
                                            }
                                            else
                                            {
                                                f_save = false;
                                            }
                                        }
                                        else
                                        {
                                            sqlinsert += "insert into m_360seigen values ('" + currentyear + "',now(),'" + cshain + "','" + seigen + "')";
                                            if (sqlinsert != "")
                                            {
                                                var insertdata = new SqlDataConnController();
                                                f_save = insertdata.inputsql(sqlinsert);
                                            }
                                            else
                                            {
                                                f_save = false;
                                            }
                                        }
                                    }
                                }
                                ModelState.Clear();
                            }
                            ModelState.Clear();
                        }
                    }
                    else
                    {
                        //ModelState.AddModelError("", "* 制限を入力してください。");
                    }
                }
                var readData = new DateController();
                val.YearList = readData.YearList("Hyoukairai");
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region seigenshow
        public string show_data(string yy)
        {
            Models.Master360hyoukaseigen model = new Models.Master360hyoukaseigen();
            DataTable dt_date = new DataTable();
            string get_data = "";
            string seiname = "";
            get_data += "select nseigen from m_360seigen where dnendou='" + yy + "'";
            var callsql = new SqlDataConnController();
            dt_date = callsql.ReadData(get_data);
            if (dt_date.Rows.Count > 0)
            {
                seiname = dt_date.Rows[0][0].ToString();
            }
            return seiname;
        }
        #endregion

        #region checkirai
        public Boolean checkirai(string tyear)
        {
            bool reval = false;
            string dbloginname = "";
            string is_irai = "";
            string this_year = tyear;
            string login_name = Session["LoginName"].ToString();
            dbloginname = FindLoginId(login_name);
            DataTable dt_irai = new DataTable();
            is_irai += "select * from r_hyouka where dNENDOU='" + this_year + "'";
            var checkirai = new SqlDataConnController();
            dt_irai = checkirai.ReadData(is_irai);
            if (dt_irai.Rows.Count > 0)
            {
                reval = true;
            }
            else
            {
                reval = false;
            }
            return reval;
        }
        #endregion
    }
}