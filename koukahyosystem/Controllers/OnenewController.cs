using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class OnenewController : Controller
    {       
        Models.OneOnOneModel OneMdl = new Models.OneOnOneModel();
        //string cshain = "";
       
        string loginUser = "";
        string selectedyear = "";
        // GET: OneOnOne
        //対象者
        public ActionResult OneNew(string fkakunin)
        {
            DateTime dateVal = new DateTime();

            bool chk = true;
            if (Session["isAuthenticated"] != null)
            {
                dateVal = DateTime.Parse(Session["dToday"].ToString());
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);
              
                var readDate = new DateController();
                int thisYear = 0;
               
                if (Session["curr_nendou"] != null)
                {
                    thisYear = int.Parse(Session["curr_nendou"].ToString());
                }
                else
                {
                    thisYear = System.DateTime.Now.Year;
                }
                OneMdl.CurrentYear = thisYear.ToString();

                string str_start = thisYear + "/4/1";
                OneMdl.konkiStartDate = DateTime.Parse(str_start);

                string str_end = OneMdl.konkiStartDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(OneMdl.konkiStartDate.AddYears(1).Year, 04);
                OneMdl.konkiEndDate = DateTime.Parse(str_end);
                                 
                OneMdl.SelectedYear = thisYear.ToString();

                var readData = new DateController();
                OneMdl.YearList = readData.YearList("seichou");
                OneMdl.OneOnOneList = new List<Models.oneononList>();
                
                OneMdl.fpermit = chk;

                OneMdl.pagination = new List<int>();
                OneMdl.Ken_fKAKUTEI = false;
                OneMdl.Ken_taishosha = "";
                OneMdl.fSuperior = (bool)Session["fHyoukasha"];
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(OneMdl);
        }

        //面談者
        public ActionResult OneOnOneKakunin()
        {
            DateTime dateVal = new DateTime();

            bool chk = true;
            if (Session["isAuthenticated"] != null)
            {
                dateVal = DateTime.Parse(Session["dToday"].ToString());
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);

                var readDate = new DateController();
                int thisYear = 0;

                if (Session["curr_nendou"] != null)
                {
                    thisYear = int.Parse(Session["curr_nendou"].ToString());
                }
                else
                {
                    thisYear = System.DateTime.Now.Year;
                }
                OneMdl.CurrentYear = thisYear.ToString();

                string str_start = thisYear + "/4/1";
                OneMdl.konkiStartDate = DateTime.Parse(str_start);

                string str_end = OneMdl.konkiStartDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(OneMdl.konkiStartDate.AddYears(1).Year, 04);
                OneMdl.konkiEndDate = DateTime.Parse(str_end);

                OneMdl.SelectedYear = thisYear.ToString();

                var readData = new DateController();
                OneMdl.YearList = readData.YearList("seichou");
                OneMdl.OneOnOneList = new List<Models.oneononList>();

                OneMdl.fpermit = chk;

                OneMdl.pagination = new List<int>();
                OneMdl.Ken_fKAKUTEI = false;
                OneMdl.Ken_taishosha = "";
                OneMdl.fSuperior = (bool)Session["fHyoukasha"];
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(OneMdl);
        }
        //過去データデータ取得
        [HttpPost]
        public JsonResult PreviousYearAction(string yearval,string  Kentaishosha, string KensMOKUHYO, bool KenfKAKUTEI, bool fsuperior)
        {            
            if (yearval != "")
            {               
                var readDate = new DateController();
                selectedyear = readDate.PreYear(yearval);                
            }            
            OneMdl.SelectedYear = selectedyear;
            OneMdl.Ken_sMOKUHYO = KensMOKUHYO;
            //ReadOneonOneListData();           
            var result = new { cur_year = selectedyear, Ken_taishosha = Kentaishosha, Ken_sMOKUHYO = KensMOKUHYO, Ken_fKAKUTEI = KenfKAKUTEI, oneononelist = OneMdl.OneOnOneList};
            return Json(result); 
        }

        //次データデータ取得
        [HttpPost]
        public JsonResult NextYearAction(string yearval, string Kentaishosha, string KensMOKUHYO, bool KenfKAKUTEI, bool fsuperior)
        {          
            int SelectYear = int.Parse(yearval);           
            if (yearval != "")
            {
                var readDate = new DateController();
                selectedyear = readDate.NextYear(yearval, "seichou");
            }           
            OneMdl.SelectedYear = selectedyear;
            OneMdl.Ken_sMOKUHYO = KensMOKUHYO;
            //ReadOneonOneListData();
            var result = new { cur_year = selectedyear, Ken_taishosha = Kentaishosha, Ken_sMOKUHYO = KensMOKUHYO, Ken_fKAKUTEI = KenfKAKUTEI, oneononelist = OneMdl.OneOnOneList };
            return Json(result);
        }

        //選択している年度でデータ取得
        [HttpPost]
        public JsonResult SearchAction(string yearval, string Kentaishosha, string KensMOKUHYO, bool KenfKAKUTEI, bool fsuperior)
        {         
            int selectedyear = int.Parse(yearval);          
            OneMdl.SelectedYear = yearval;
            OneMdl.Ken_taishosha = Kentaishosha;
            OneMdl.Ken_fKAKUTEI = KenfKAKUTEI;
            OneMdl.Ken_sMOKUHYO = KensMOKUHYO;
            OneMdl.fSuperior = fsuperior;
            OneMdl.OneOnOneList = ReadDataList(); 
            
            var result = new { cur_year = selectedyear, Ken_taishosha = Kentaishosha, Ken_sMOKUHYO = KensMOKUHYO, Ken_fKAKUTEI =  KenfKAKUTEI, oneononelist = OneMdl.OneOnOneList };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ClearAction()
        {
            int thisyear = 0;
            //実際今年値
            if (Session["curr_nendou"] != null)
            {
                thisyear = int.Parse(Session["curr_nendou"].ToString());
            }
            else
            {
                thisyear = System.DateTime.Now.Year;
            }
            OneMdl.SelectedYear = thisyear.ToString();
            OneMdl.OneOnOneList = ReadDataList();
            var result = new { cur_year = thisyear,  oneononelist = OneMdl.OneOnOneList };
            return Json(result);
        }

        //ミーティング新規作成
        [HttpPost]
        public JsonResult CreateNewAction(string yearval, string KensMOKUHYO)
        {
            string newcMOKUHYO = AutoCode();
            var result = new { cur_year = yearval, Ken_sMOKUHYO = KensMOKUHYO, newId = newcMOKUHYO };
            return Json(result);

        }

        //リスト一覧からデータ読込リック
        [HttpPost]
        public JsonResult ReadOneonOne(string yearval, string Kentaishosha, string KensMOKUHYO, bool KenfKAKUTEI, bool fsuperior, string cMokuhyou,string cTaishosha)
        {
            
            cMokuhyou = cMokuhyou.PadLeft(5, '0');
            OneMdl.SelectedYear = yearval;
            OneMdl.Ken_taishosha = Kentaishosha;
            OneMdl.Ken_sMOKUHYO = KensMOKUHYO;
            OneMdl.Ken_fKAKUTEI = KenfKAKUTEI;
            OneMdl.fSuperior = fsuperior;
            ReadEditOneonOne(cMokuhyou, cTaishosha);
            var result = new { cur_year = yearval,
                Ken_sMOKUHYO = KensMOKUHYO,
                cMokuhyou = cMokuhyou,
                jyoutai = OneMdl.fKAKUTEI,
                kanryou = OneMdl.fKANRYOU,
                comfirmstatus = OneMdl.status,
                applyDate = OneMdl.ApplyDate,
                comifrmDate = OneMdl.dJISHIBI,
                mokuhyo = OneMdl.sMOKUHYO,
                c_action = OneMdl.Actiontask,
                l_action = OneMdl.Feedback,
                c_problem = OneMdl.Trouble_tantousha,
                l_problem = OneMdl.Trouble_Leader,
                c_notic = OneMdl.Awareness_tantousha,
                l_notic = OneMdl.Awareness_Leader,
                memo =  OneMdl.Memo,

                pre_applyDate = OneMdl.prv_date,
                pre_comifrmDate = OneMdl.prv_djishi,                
                pre_mokuhyo = OneMdl.prv_tema,
                c_pre_action = OneMdl.prv_taskaction,
                l_pre_action = OneMdl.prv_feedback,
                c_pre_problem = OneMdl.prv_trouble,
                l_pre_problem = OneMdl.prv_trouble_L,
                c_pre_notic = OneMdl.prv_awareness,
                l_pre_notic = OneMdl.prv_awareness_L,
                pre_memo = OneMdl.prv_memo

            };
            return Json(result);
        }

        //保存  //確定
        [HttpPost]
        public JsonResult SaveAction(string yearval, string Kentaishosha, string KensMOKUHYO, bool KenfKAKUTEI, bool fsuperior
            , string applyDate, string cmokuhyo, string mokuhyo
            , string c_action, string l_action, string c_problem, string l_problem
            , string c_notic, string l_notic,string l_memo, bool fkanryou, bool fkakutei,
            string ctaishosha)
        {
            DateTime dApply = DateTime.Parse(applyDate);
            string strApplydate = dApply.ToString("yyyy/MM/dd");            
            OneMdl.dHIDUKE = DateTime.Parse(strApplydate).Date;
            OneMdl.Ken_taishosha = Kentaishosha;
            OneMdl.Ken_sMOKUHYO = KensMOKUHYO;
            OneMdl.Ken_fKAKUTEI = KenfKAKUTEI;

            OneMdl.cMOKUHYO = cmokuhyo;
            OneMdl.sMOKUHYO = mokuhyo;
            OneMdl.Actiontask = c_action;
            OneMdl.Feedback = l_action;
            OneMdl.Trouble_tantousha = c_problem; 
            OneMdl.Trouble_Leader = l_problem;
            OneMdl.Awareness_tantousha = c_notic;
            OneMdl.Awareness_Leader = l_notic;
            OneMdl.Memo = l_memo;
            OneMdl.fKAKUTEI = fkakutei;
            OneMdl.fKANRYOU = fkanryou ;
            OneMdl.fSuperior = fsuperior;
            if(ctaishosha != "")
            {
                OneMdl.cTaishosha = ctaishosha.PadLeft(4, '0');
            }
            
            SaveData();

            OneMdl.SelectedYear = yearval;
            OneMdl.OneOnOneList = ReadDataList();
            var result = new { cur_year = yearval, Ken_taishosha = Kentaishosha, Ken_sMOKUHYO = KensMOKUHYO, Ken_fKAKUTEI = KenfKAKUTEI, oneononelist = OneMdl.OneOnOneList };
            return Json(result);           
        }
              

        //順番とページ付け
        [HttpPost]
        public JsonResult SortPagination(string yearval, string Kentaishosha, string KensMOKUHYO,bool KenfKAKUTEI, bool fsuperior, int pageIndex, string sortName, string sortDirection)
        {
            List<Models.oneononList> SortedOneList = new List<Models.oneononList>();
            OneMdl.SelectedYear = yearval;
            OneMdl.Ken_taishosha = Kentaishosha;
            OneMdl.Ken_sMOKUHYO = KensMOKUHYO;
            OneMdl.Ken_fKAKUTEI = KenfKAKUTEI;
            OneMdl.fSuperior = fsuperior;
            Pagination(pageIndex);
            int startIndex = (pageIndex - 1) * OneMdl.PageSize;
           
            SortedOneList =  SortingData( sortName,  sortDirection,  startIndex);
           
            var result = new { cur_year = yearval, Ken_taishosha = Kentaishosha, Ken_sMOKUHYO = KensMOKUHYO, Ken_fKAKUTEI = KenfKAKUTEI, curPageIndex = pageIndex , oneononelist = SortedOneList, pagination = OneMdl.pagination  };
            return Json(result);
        }       

        //ログイン名からID取得
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

        //OneonOneミーティング新規作成するとき自動
        private string AutoCode()
        {
            string name = Session["LoginName"].ToString();
            loginUser = FindLoginId(name);
            string MokuhyouNum = "";
            try
            {
                DataTable dt_PerConv = new DataTable();
                string sqlStr = "SELECT cMOKUHYO as cMOKUHYO FROM r_oneonone where cTAISHOSHA = '" + loginUser + "' ; ";
                var readData = new SqlDataConnController();
                dt_PerConv = readData.ReadData(sqlStr);
                //finding the missing number 
                List<int> ListMokuhyo = new List<int>();
                foreach (DataRow dr in dt_PerConv.Rows)
                {
                    ListMokuhyo.Add(int.Parse(dr["cMOKUHYO"].ToString()));
                }
                if (ListMokuhyo.Count > 0)
                {
                    var MissingNumbers = Enumerable.Range(1, 9999).Except(ListMokuhyo).ToList();
                    var ResultNum = MissingNumbers.Min();
                    MokuhyouNum = ResultNum.ToString().PadLeft(5, '0');
                }
                else
                {
                    var MissingNumbers = 1;
                    MokuhyouNum = MissingNumbers.ToString().PadLeft(5, '0');
                }
            }
            catch
            {
                throw;
            }

            return MokuhyouNum;
        }

        //リスト一覧データ取得
        private List<Models.oneononList> ReadDataList()
        {

            List<Models.oneononList> OneonOneList = new List<Models.oneononList>();
            try
            {
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);

                DataTable dt_one = new DataTable();
                string sqlStr = "";

                string str_start = OneMdl.SelectedYear + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                DateTime endDate = DateTime.Parse(str_end);

                string allhyouka = hyoukalist(loginUser); //added by lwinmar 20220519 

                sqlStr += " SELECT  ";
                sqlStr += " mo.cTAISHOSHA as cTAISHOSHA ";
                sqlStr += " ,mo.cMENDANSHA as cMENDANSHA ";
                sqlStr += " ,ms.sSHAIN as sTAISHOSHA ";
                sqlStr += " ,mo.cMOKUHYO as cMOKUHYO ";
                sqlStr += " ,ifnull(mo.sMOKUHYO ,'') as sMOKUHYO ";
                sqlStr += " ,DATE_FORMAT(ifnull(mo.dHIDUKE,''),'%Y/%m/%d') as dHIDUKE ";
                sqlStr += " ,DATE_FORMAT(ifnull(mo.dJISHIBI,''),'%Y/%m/%d') as dJISHIBI ";
                sqlStr += " ,ifnull(mo.fKANRYOU,'') as fKANRYOU ";
                sqlStr += " ,ifnull(mo.fKAKUTEI,'') as fKAKUTEI ";
                sqlStr += " FROM r_oneonone mo ";
                sqlStr += " INNER JOIN m_shain ms ON ms.cSHAIN = mo.cTAISHOSHA ";
                //sqlStr += " Where Year(dHIDUKE) = '" + curYear + "'";
                sqlStr += " where dHIDUKE BETWEEN '" + startDate.ToString("yyyy/MM/dd") + "' AND '" + endDate.ToString("yyyy/MM/dd") + "'";

                if (OneMdl.fSuperior == true)
                {
                    sqlStr += " AND mo.fKANRYOU = 1 ";                    
                    sqlStr += " AND mo.cTAISHOSHA in (" + allhyouka + ")"; //20220520 added by lwin mar   
                    //sqlStr += " AND mo.cMENDANSHA = '" + loginUser + "' "; //20220519 deleted by lwinmar 

                    //検索条件
                    if (!string.IsNullOrEmpty(OneMdl.Ken_taishosha))
                    {
                        //sqlStr += " AND ms.sSHAIN collate utf8mb4_unicode_ci LIKE '%" + OneMdl.Ken_taishosha + "%'";
                        sqlStr += " AND ms.sSHAIN LIKE '%" + OneMdl.Ken_taishosha + "%'";
                    }

                    if (!string.IsNullOrEmpty(OneMdl.Ken_sMOKUHYO))
                    {
                        sqlStr += " AND mo.sMOKUHYO collate utf8mb4_unicode_ci LIKE '%" + OneMdl.Ken_sMOKUHYO + "%'";
                    }

                    if (OneMdl.Ken_fKAKUTEI == true)
                    {
                        sqlStr += " AND mo.fKAKUTEI = '1'";
                    }
                    else
                    {
                        sqlStr += " AND mo.fKAKUTEI = 0 ";
                    }

                }
                else
                {

                    sqlStr += " AND cTAISHOSHA = '" + loginUser + "'";

                    //検索条件

                    if (!string.IsNullOrEmpty(OneMdl.Ken_sMOKUHYO))
                    {
                        sqlStr += " AND mo.sMOKUHYO collate utf8mb4_unicode_ci LIKE '%" + OneMdl.Ken_sMOKUHYO + "%'";
                    }

                    if (OneMdl.Ken_fKANRYOU == true)
                    {
                        sqlStr += " AND mo.fKANRYOU = '1'";
                    }


                    if (OneMdl.Ken_fKAKUTEI == true)
                    {
                        sqlStr += " AND mo.fKAKUTEI = '1'";
                    }

                }

                sqlStr += " ORDER BY dHIDUKE DESC , cMOKUHYO DESC ";

                var readData = new SqlDataConnController();
                dt_one = readData.ReadData(sqlStr);


                //DataTable dt_one = new DataTable();

                //if (!string.IsNullOrEmpty(kensaku))
                //{

                //    DataView dv = dt_perconver.DefaultView;
                //    dv.Sort = kensaku;
                //    dt_one = dv.ToTable();

                //}
                //else
                //{
                //    dt_one = dt_perconver;
                //}

                foreach (DataRow dr in dt_one.Rows)
                {

                    //string status = "";
                    string send = "";
                    if (dr["fKANRYOU"].ToString() == "1")
                    {
                        send = "済";
                    }
                    else
                    {
                        send = "未";
                    }

                    if (OneMdl.fSuperior == false)
                    {

                        OneonOneList.Add(new Models.oneononList
                        {
                            dHIDUKE = dr["dHIDUKE"].ToString(),
                            cMOKUHYO = dr["cMOKUHYO"].ToString(),
                            sMOKUHYO = dr["sMOKUHYO"].ToString(),
                            fKANRYOU = send,
                            dJISHIBI = dr["dJISHIBI"].ToString()

                        });
                    }
                    else
                    {
                        OneonOneList.Add(new Models.oneononList
                        {
                            dHIDUKE = dr["dHIDUKE"].ToString(),
                            cTAISHOSHA = dr["cTAISHOSHA"].ToString(),
                            sTAISHOSHA = dr["sTAISHOSHA"].ToString(),
                            cMOKUHYO = dr["cMOKUHYO"].ToString(),
                            sMOKUHYO = dr["sMOKUHYO"].ToString(),
                            dJISHIBI = dr["dJISHIBI"].ToString()
                        });
                    }

                }

            }
            catch
            {
            }
            return OneonOneList;
        }

        //リスト一覧からデータ読込機能
        private void ReadEditOneonOne(string cMOKUHYOU,string cTaishosha)
        {
            string name = "";
            if (OneMdl.fSuperior == false)
            {
                name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);
            }
            else
            {
                if (cTaishosha != "")
                {
                    loginUser = cTaishosha.PadLeft(4, '0'); ;
                }
                
            }
            
            try
            {

                DataTable dt_perConv = new DataTable();
                var readDate = new DateController();

                string sqlStr = "SELECT ";
                sqlStr += " cMOKUHYO ";
                sqlStr += " , ifnull(sMOKUHYO,'') as  sMOKUHYO ";
                sqlStr += " , ifnull(cTAISHOSHA,'') as  cTAISHOSHA ";
                sqlStr += " , ifnull(dHIDUKE,'') as  dHIDUKE ";
                sqlStr += " , ifnull(dJISHIBI,'') as  dJISHIBI ";
                sqlStr += " , ifnull(sACTIONTASK,'') as sACTIONTASK ";
                sqlStr += " , ifnull(sTROUBLE,'') as sTROUBLE ";
                sqlStr += " , ifnull(sTROUBLE_L,'') as sTROUBLE_L";
                sqlStr += " , ifnull(sAWARENESS,'') as sAWARENESS ";
                sqlStr += " , ifnull(sAWARENESS_L,'') as sAWARENESS_L ";
                sqlStr += " , ifnull(sFEEDBACK,'') as sFEEDBACK ";
                sqlStr += " , ifnull(sTODO,'') as sTODO ";
                sqlStr += " , ifnull(sMEMO, '' ) as sMEMO ";
                sqlStr += " , ifnull(fKANRYOU, '') as fKANRYOU ";
                sqlStr += " , ifnull(fKAKUTEI, '') as fKAKUTEI ";
                sqlStr += " FROM r_oneonone ";
                sqlStr += " WHERE cMOKUHYO = '" + cMOKUHYOU + "'";
                sqlStr += " AND cTAISHOSHA = '" + loginUser + "'";
                var readData = new SqlDataConnController();
                dt_perConv = readData.ReadData(sqlStr);

                foreach (DataRow dr in dt_perConv.Rows)
                {
                    if (dr["dHIDUKE"].ToString() != "")
                    {
                        OneMdl.dHIDUKE = DateTime.Parse(dr["dHIDUKE"].ToString());
                        OneMdl.ApplyDate = OneMdl.dHIDUKE.ToString("yyyy-MM-dd");

                    }
                    //対象者　//社員名
                    OneMdl.sMOKUHYO = dr["sMOKUHYO"].ToString();
                    OneMdl.Actiontask = dr["sACTIONTASK"].ToString();
                    OneMdl.Trouble_tantousha = dr["sTROUBLE"].ToString();
                    OneMdl.Trouble_Leader = dr["sTROUBLE_L"].ToString();
                    OneMdl.Awareness_tantousha = dr["sAWARENESS"].ToString();
                    OneMdl.Awareness_Leader = dr["sAWARENESS_L"].ToString();
                    OneMdl.dJISHIBI = dr["dJISHIBI"].ToString();

                    //面談者　
                    OneMdl.Feedback = dr["sFEEDBACK"].ToString();
                    OneMdl.Todo = dr["sTODO"].ToString();
                    OneMdl.Memo = dr["sMEMO"].ToString();


                    if (dr["fKANRYOU"].ToString() == "1")
                    {
                        OneMdl.fKANRYOU = true;
                        if (dr["fKAKUTEI"].ToString() != "")
                        {
                            if (dr["fKAKUTEI"].ToString() == "0")
                            {
                                //OneMdl.status = "実施状態　：　未";
                                OneMdl.fKAKUTEI = false;
                            }
                            else
                            {
                                //OneMdl.status = "実施状態　：　済";
                                OneMdl.fKAKUTEI = true;
                            }
                        }
                        else
                        {
                            //OneMdl.status = "実施状態　：　未";
                            OneMdl.fKAKUTEI = false;
                        }
                    }
                    else
                    {
                        OneMdl.status = null;
                        OneMdl.fKANRYOU = false;
                    }
                    //tantousha = dr["cTAISHOSHA"].ToString();
                }

                //最後入力された情報取得

                //string taishosha = "'" + FindLoginId(name) + "'";
                sqlStr = "";
                sqlStr += " SELECT ";
                sqlStr += "  ifnull(cMOKUHYO, '') as cMOKUHYO ";
                sqlStr += " , ifnull(sMOKUHYO,'') as  sMOKUHYO ";
                sqlStr += " , ifnull(DATE_FORMAT(dHIDUKE, '%Y/%m/%d'),'') as  dHIDUKE ";
                sqlStr += " , ifnull(DATE_FORMAT(dJISHIBI, '%Y/%m/%d'),'') as  dJISHIBI ";
                sqlStr += " , ifnull(sACTIONTASK,'') as sACTIONTASK ";
                sqlStr += " , ifnull(sTROUBLE,'') as sTROUBLE ";
                sqlStr += " , ifnull(sTROUBLE_L,'') as sTROUBLE_L";
                sqlStr += " , ifnull(sAWARENESS,'') as sAWARENESS ";
                sqlStr += " , ifnull(sAWARENESS_L,'') as sAWARENESS_L  ";
                sqlStr += " , ifnull(sFEEDBACK,'') as sFEEDBACK ";
                sqlStr += " , ifnull(sTODO,'') as sTODO ";
                sqlStr += " , ifnull(sMEMO, '' ) as sMEMO ";
                sqlStr += " FROM r_oneonone ";
                sqlStr += " WHERE cTAISHOSHA = " + loginUser;
                sqlStr += " AND cMOKUHYO <>'" + cMOKUHYOU + "'";
                sqlStr += " AND((dHIDUKE = '" + OneMdl.dHIDUKE.Date + "'  AND cMOKUHYO < '" + cMOKUHYOU + "') or(dHIDUKE < '" + OneMdl.dHIDUKE.Date + "') ) ";


                sqlStr += " ORDER BY dHIDUKE DESC , cMOKUHYO DESC ";
                sqlStr += " LIMIT 1; ";

                readData = new SqlDataConnController();
                DataTable dt = new DataTable();
                dt = readData.ReadData(sqlStr);
                foreach (DataRow pv_dr in dt.Rows)
                {

                    DateTime preDateVal = DateTime.Parse(pv_dr["dHIDUKE"].ToString());
                    OneMdl.prv_date = preDateVal.ToString("yyyy-MM-dd");

                    if (pv_dr["dJISHIBI"].ToString() != "")
                    {
                        preDateVal = DateTime.Parse(pv_dr["dJISHIBI"].ToString());
                        OneMdl.prv_djishi = preDateVal.ToString("yyyy-MM-dd");
                    }


                    if (pv_dr["sMOKUHYO"].ToString() != "")
                    {
                        OneMdl.prv_tema = pv_dr["sMOKUHYO"].ToString();
                    }

                    if (pv_dr["sACTIONTASK"].ToString() != "")
                    {
                        OneMdl.prv_taskaction = pv_dr["sACTIONTASK"].ToString();
                    }
                    if (pv_dr["sTROUBLE"].ToString() != "")
                    {
                        OneMdl.prv_trouble = pv_dr["sTROUBLE"].ToString();
                    }

                    if (pv_dr["sTROUBLE_L"].ToString() != "")
                    {

                        OneMdl.prv_trouble_L = pv_dr["sTROUBLE_L"].ToString();
                    }


                    if (pv_dr["sAWARENESS"].ToString() != "")
                    {
                        OneMdl.prv_awareness = pv_dr["sAWARENESS"].ToString();
                    }


                    if (pv_dr["sAWARENESS_L"].ToString() != "")
                    {
                        OneMdl.prv_awareness_L = pv_dr["sAWARENESS_L"].ToString();
                    }

                    if (pv_dr["sFEEDBACK"].ToString() != "")
                    {
                        OneMdl.prv_feedback = pv_dr["sFEEDBACK"].ToString();
                    }


                    if (pv_dr["sMEMO"].ToString() != "")
                    {
                        OneMdl.prv_memo = pv_dr["sMEMO"].ToString();
                    }

                }

            }
            catch
            {
            }

        }

        //保存する機能
        private bool SaveData()
        {
            bool fsave = false;
            try
            {
                string name = "";
                //dHIDUKE
                DateTime dDate = new DateTime();
                dDate = OneMdl.dHIDUKE;

                //cMOKUHYO
                string C_MOKKUHYO = OneMdl.cMOKUHYO;

                //sMOKUHYO
                string S_MOKKUHYO = OneMdl.sMOKUHYO;

                //cTAISHOSHA
                string strMendansha = "";
                strMendansha = FindHyoukasha();

                //ActionTask
                string strActionTask = "";
                if (OneMdl.Actiontask != null)
                {
                    strActionTask = OneMdl.Actiontask;
                }
                //Trouble field for tabtousha
                string strTrouble = "";
                if (OneMdl.Trouble_tantousha != null)
                {
                    strTrouble = OneMdl.Trouble_tantousha;
                }

                //Trouble field of leader 
                string strTrouble_L = "";
                if (OneMdl.Trouble_Leader != null)
                {
                    strTrouble_L = OneMdl.Trouble_Leader;
                }

                //Awareness field for tabtousha
                string strAwareness = "";
                if (OneMdl.Awareness_tantousha != null)
                {
                    strAwareness = OneMdl.Awareness_tantousha;
                }
                //Awareness field of leader 
                string strAwareness_L = "";
                if (OneMdl.Awareness_Leader != null)
                {
                    strAwareness_L = OneMdl.Awareness_Leader;
                }
                //Feedback
                string strFeedback = "";
                if (OneMdl.Feedback != null)
                {
                    strFeedback = OneMdl.Feedback;
                }
                //Todo
                string strTodo = "";
                if (OneMdl.Todo != null)
                {
                    strTodo = OneMdl.Todo;
                }
                //memo
                string strMemo = "";
                if (OneMdl.Memo != null)
                {
                    strMemo = OneMdl.Memo;
                }
               
                string strKanryou = "0";
                if (OneMdl.fKANRYOU == true)
                {
                    strKanryou = "1"; //リーダへ申請
                }

                string strKakutei = "0";
                if (OneMdl.fKAKUTEI == true)
                {
                    strKakutei = "1";//リーダ確認済
                }
                //実施日
                var curDateCon = new DateController();
                DateTime curDate = curDateCon.FindToDayDate();

                string sqlquery = "";
                if (OneMdl.fSuperior == true)
                {
                    loginUser = OneMdl.cTaishosha;
                    sqlquery += " update r_oneonone set ";
                    sqlquery += " dJISHIBI = IFNULL(dJISHIBI, '" + curDate + "') ";
                    sqlquery += " , cMENDANSHA  = '" + strMendansha + "'";//20220520 added by lwinmar
                    sqlquery += " , sTROUBLE_L  = '" + strTrouble_L + "'";
                    sqlquery += " , sAWARENESS_L = '" + strAwareness_L + "'";
                    sqlquery += " , sFEEDBACK ='" + strFeedback + "'";
                    sqlquery += " , sTODO ='" + strTodo + "', sMEMO ='" + strMemo + "' ,  fKAKUTEI = '" + strKakutei + "'";
                    sqlquery += "  Where cMOKUHYO='" + C_MOKKUHYO + "' AND cTAISHOSHA ='" + loginUser + "' ;";
                }
                else
                {
                    name = Session["LoginName"].ToString();
                    loginUser = FindLoginId(name);
                    //insert data into database
                    sqlquery += " INSERT INTO r_oneonone ( ";
                    sqlquery += " cTAISHOSHA ,cMENDANSHA , cMOKUHYO, sMOKUHYO ";
                    sqlquery += " , dHIDUKE , sACTIONTASK , sTROUBLE , sAWARENESS ";
                    sqlquery += " , fKANRYOU , fKAKUTEI ) VALUES ";
                    sqlquery += " ('" + loginUser + "' ,'" + strMendansha + "',  '" + C_MOKKUHYO + "' , '" + S_MOKKUHYO + "'";
                    sqlquery += " , '" + dDate + "' , '" + strActionTask + "' , '" + strTrouble + "' , '" + strAwareness + "'";

                    sqlquery += " , '" + strKanryou + "' ,'" + strKakutei + "')";
                    sqlquery += " ON DUPLICATE KEY UPDATE cTAISHOSHA = '" + loginUser + "'  ";
                    sqlquery += " , cMENDANSHA ='" + strMendansha + "'";
                    sqlquery += " , cMOKUHYO ='" + C_MOKKUHYO + "'";
                    sqlquery += " , sMOKUHYO ='" + S_MOKKUHYO + "'";
                    sqlquery += " , dHIDUKE = '" + dDate + "'";
                    sqlquery += " , sACTIONTASK ='" + strActionTask + "'";
                    sqlquery += " , sTROUBLE ='" + strTrouble + "'";
                    sqlquery += " , sAWARENESS ='" + strAwareness + "'";
                    sqlquery += " , fKANRYOU ='" + strKanryou + "'";
                    sqlquery += " , fKAKUTEI ='" + strKakutei + "';";
                }

                var insertdata = new SqlDataConnController();
                fsave = insertdata.inputsql(sqlquery);
            }
            catch (Exception ec)
            {
            }
           
            return fsave;
        }

        //確認者コード
        private string FindHyoukasha()
        {
            string name = Session["LoginName"].ToString();
            loginUser = FindLoginId(name);
            string id = "";
            try
            {
                DataTable dt_shain = new DataTable();
                string sqlStr = "SELECT cHYOUKASHA FROM m_shain where cSHAIN = '" + loginUser + "'";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlStr);
                if (dt_shain.Rows.Count > 0)
                {
                    id = dt_shain.Rows[0]["cHYOUKASHA"].ToString();
                }
            }
            catch
            {
            }
            return id;
        }

        #region get_hyoukalist 20220519 added by lwinmar
        public string hyoukalist(string id)
        {
            #region hyoukalist
            var readData = new SqlDataConnController();
            string hyoukaQuery = "SELECT cSHAIN FROM m_shain where cHYOUKASHA='" + id + "' and fTAISYA=0;";
            DataTable dthyouka = new DataTable();
            dthyouka = readData.ReadData(hyoukaQuery);
            string allhyouka = "";
            foreach (DataRow Lsdr in dthyouka.Rows)
            {
                allhyouka += Lsdr["cSHAIN"].ToString() + ",";//20220519 added by lwinmar

            }
            if (allhyouka.Length > 0)
            {
                allhyouka = allhyouka.Remove(allhyouka.Length - 1, 1);

            }
            return allhyouka;
            #endregion
        }
        #endregion 

        private void Pagination(int pageIndex)
        {
           
            OneMdl.OneOnOneList = ReadDataList();
            OneMdl.PageIndex = pageIndex;
            OneMdl.PageSize = 10;
            OneMdl.RecordCount = OneMdl.OneOnOneList.Count;
            if (OneMdl.OneOnOneList.Count > 10)
            {
                int totalrow = OneMdl.OneOnOneList.Count;
                int pageCount = 0;
                if ((totalrow % 10 == 0))
                {
                    pageCount = totalrow / 10;
                }
                else
                {
                    pageCount = (totalrow / 10) + 1; ;
                }

                OneMdl.pagination = new List<int>();
                for (int i = 1; i <= pageCount; i++)
                {
                    OneMdl.pagination.Add(i);
                }

            }
            
            
            
        }

        private List<Models.oneononList> SortingData(string sortName, string sortDirection,int startIndex)
        {
            List<Models.oneononList> SortedOneList = new List<Models.oneononList>();

            switch (sortName)
            {
                case "":
                    SortedOneList = OneMdl.OneOnOneList.OrderByDescending(r => r.dHIDUKE).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    break;
                case "送信日":
                    if (sortDirection == "ASC")
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderBy(r => r.dHIDUKE).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    else
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderByDescending(r => r.dHIDUKE).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    break;
                case "社員名":
                    if (sortDirection == "ASC")
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderBy(r => r.sTAISHOSHA).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    else
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderByDescending(r => r.sTAISHOSHA).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    break;
                case "目標":
                    if (sortDirection == "ASC")
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderBy(r => r.sMOKUHYO).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    else
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderByDescending(r => r.sMOKUHYO).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    break;

                case "送信状態":
                    if (sortDirection == "ASC")
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderBy(r => r.fKANRYOU).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    else
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderByDescending(r => r.fKANRYOU).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    break;

                case "実施日":
                    if (sortDirection == "ASC")
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderBy(r => r.dJISHIBI).Skip(startIndex).Take(OneMdl.PageSize).ToList();
                    }
                    else
                    {
                        SortedOneList = OneMdl.OneOnOneList.OrderByDescending(r => r.dJISHIBI).Skip(startIndex).Take(OneMdl.PageSize).ToList();

                    }
                    break;
            }

            return SortedOneList;


        }


    }
}