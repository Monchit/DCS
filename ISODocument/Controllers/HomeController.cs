using ISODocument.Models;
using ISODocument.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebCommonFunction;

namespace ISODocument.Controllers
{
    public class HomeController : Controller
    {
        private TNCSecurity secure = new TNCSecurity();

        private DocumentControlEntities dbDC = new DocumentControlEntities();
        private TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

        public ActionResult Index(string key = null)
        {
            if (key != null)
            {
                return Login(key);
            }
            else
            {
                //ViewBag.Menu = 0;
                return View();
            }
        }

        public ActionResult Contact()
        {
            return View();
        }

        [Check_Authen]
        public ActionResult Inprogress()
        {
            //ViewBag.Menu = 1;
            ViewBag.ULv = byte.Parse(Session["DC_ULv"].ToString());
            var paper = from a in dbDC.TM_Paper
                        select a;
            ViewBag.Paper = paper;
            return View();
        }

        [Check_Authen]
        public ActionResult Controlled()
        {
            //ViewBag.Menu = 2;
            ViewBag.ULv = byte.Parse(Session["DC_ULv"].ToString());
            return View();
        }

        [Check_Authen]
        public ActionResult Copy()
        {
            //ViewBag.Menu = 8;
            ViewBag.ULv = byte.Parse(Session["DC_ULv"].ToString());
            var get_group = from a in dbDC.V_Group
                            orderby a.group_name
                            select a;
            ViewBag.AllGroup = get_group;

            var paper = from a in dbDC.TM_Paper
                        select a;
            ViewBag.Paper = paper;
            return View();
        }

        [Check_Authen]
        public ActionResult CopyCompleted()
        {
            //ViewBag.Menu = 14;
            var paper = from a in dbDC.TM_Paper
                        select a;
            ViewBag.Paper = paper;
            return View();
        }

        [Check_Authen]
        public ActionResult ControlledShow()
        {
            if ((Session["DC_QC"] != null && Session["DC_QC"].ToString() == "1") || Session["DC_UType"].ToString() == "1")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Check_Authen]
        public ActionResult Cancelled()
        {
            //ViewBag.Menu = 3;
            return View();
        }

        [Check_Authen]
        public ActionResult IssuerCancel()
        {
            //ViewBag.Menu = 4;
            return View();
        }

        [Check_Authen]
        public ActionResult Obsolete()
        {
            //ViewBag.Menu = 5;
            return View();
        }

        [Check_Authen]
        public ActionResult FutureUse()
        {
            //ViewBag.Menu = 7;
            return View();
        }

        [Check_Authen]
        public ActionResult OverDueReview()
        {
            //ViewBag.Menu = 11;
            return View();
        }

        [Check_Authen]
        public ActionResult DCCConfirm()
        {
            return View();
        }

        [Check_Authen]
        public ActionResult DocForm()
        {
            if (Session["DC_ULv"] != null)
            {
                int lv = int.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());
                if (lv == 1)
                {
                    var gcode = from g in dbDC.TM_GroupCode
                                where g.group_id == org
                                select g;
                    ViewBag.GCode = gcode;

                    var docType = from a in dbDC.TM_DocType
                                  where a.doc_lv > 1
                                  orderby a.doc_type_full
                                  select a;
                    ViewBag.DocType = docType;
                }
                else if (lv <= 2)//Mgr. Down
                {
                    var gcode = from g in dbDC.TM_GroupCode
                                where g.group_id == org
                                select g;
                    ViewBag.GCode = gcode;

                    var docType = from a in dbDC.TM_DocType
                                  orderby a.doc_type_full
                                  select a;
                    ViewBag.DocType = docType;
                }
                else if (lv == 3)//Dept.
                {
                    var groupindept = (from a in dbTNC.tnc_costcentercode
                                       where a.dept_id == org && a.group_id != 0
                                       select a.group_id).Distinct().ToList();
                    var gcode = from a in dbDC.TM_GroupCode
                                where groupindept.Contains(a.group_id)
                                select a;
                    ViewBag.GCode = gcode;

                    var docType = from a in dbDC.TM_DocType
                                  orderby a.doc_type_full
                                  select a;
                    ViewBag.DocType = docType;
                }
            }
            //ViewBag.Menu = 6;
            return View();
        }

        [Check_Authen]
        public ActionResult DocInitial()
        {
            var gcode = from g in dbDC.TM_GroupCode
                        select g;
            ViewBag.GCode = gcode;

            var docType = from a in dbDC.TM_DocType
                          orderby a.doc_type_full
                          select a;
            ViewBag.DocType = docType;

            return View();
        }

        [Check_Authen]
        public ActionResult NewVDO()
        {
            if (Session["DC_ULv"] != null)
            {
                int lv = int.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());
                if (lv == 1)
                {
                    var gcode = from g in dbDC.TM_GroupCode
                                where g.group_id == org
                                select g;
                    ViewBag.GCode = gcode;
                }
                else if (lv <= 2)//Mgr. Down
                {
                    var gcode = from g in dbDC.TM_GroupCode
                                where g.group_id == org
                                select g;
                    ViewBag.GCode = gcode;
                }
                else if (lv == 3)//Dept.
                {
                    var groupindept = (from a in dbTNC.tnc_costcentercode
                                       where a.dept_id == org && a.group_id != 0
                                       select a.group_id).Distinct().ToList();
                    var gcode = from a in dbDC.TM_GroupCode
                                where groupindept.Contains(a.group_id)
                                select a;
                    ViewBag.GCode = gcode;
                }

                string subPath = "~/UploadFiles/VDO/" + Session["DC_Auth"].ToString() + "/";
                if (!Directory.Exists(Server.MapPath(subPath)))
                    Directory.CreateDirectory(Server.MapPath(subPath));
            }
            return View();
        }

        [Check_Authen]
        public ActionResult NewVDO1()
        {
            if (Session["DC_ULv"] != null)
            {
                int lv = int.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());
                if (lv == 1)
                {
                    var gcode = from g in dbDC.TM_GroupCode
                                where g.group_id == org
                                select g;
                    ViewBag.GCode = gcode;
                }
                else if (lv <= 2)//Mgr. Down
                {
                    var gcode = from g in dbDC.TM_GroupCode
                                where g.group_id == org
                                select g;
                    ViewBag.GCode = gcode;
                }
                else if (lv == 3)//Dept.
                {
                    var groupindept = (from a in dbTNC.tnc_costcentercode
                                       where a.dept_id == org && a.group_id != 0
                                       select a.group_id).Distinct().ToList();
                    var gcode = from a in dbDC.TM_GroupCode
                                where groupindept.Contains(a.group_id)
                                select a;
                    ViewBag.GCode = gcode;
                }
            }
            return View();
        }

        [Check_Authen]
        public ActionResult Questionair()
        {
            return View();
        }

        [Check_Authen]
        public ActionResult MgrEdit(string doctype, string groupcode, int runno, byte revno)
        {
            TD_Document td_document = dbDC.TD_Document.Find(doctype, groupcode, runno, revno);
            if (td_document == null)
            {
                return HttpNotFound();
            }
            return View(td_document);
        }

        [Check_Authen]
        public ActionResult DocEdit(string doctype, string groupcode, int runno, byte revno)
        {
            TD_Document td_document = dbDC.TD_Document.Find(doctype, groupcode, runno, revno);
            if (td_document == null)
            {
                return HttpNotFound();
            }
            return View(td_document);
        }

        [Check_Authen]
        public ActionResult DocRevise(string doctype, string groupcode, int runno, byte revno)
        {
            TD_Document td_document = dbDC.TD_Document.Find(doctype, groupcode, runno, revno);
            if (td_document == null)
            {
                return HttpNotFound();
            }
            return View(td_document);
        }

        [Check_Authen]
        public ActionResult RequestCopy()
        {
            var paper = from a in dbDC.TM_Paper
                        select a;
            ViewBag.Paper = paper;
            //ViewBag.Menu = 11;
            return View();
        }

        [Check_Authen]
        public ActionResult DocCopyRequest()
        {
            //ViewBag.Menu = 7;
            return View();
        }

        [Check_Authen]
        public ActionResult HardCopyForm()
        {
            var all_group = from a in dbDC.V_Group//dbTNC.tnc_group_master
                            orderby a.group_name
                            select a;

            ViewBag.AllGroup = all_group;
            return View();
        }

        public ActionResult ShowDocDetail(string dt, string gc, int rn, byte rv)
        {
            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == dt && a.group_code == gc
                         && a.run_no == rn && a.rev_no == rv
                         select a).FirstOrDefault();
            return View(query);
        }

        //-------------------------------------------//
        public ActionResult _FormApprove(string doctype, string groupcode, int runno, byte revno)
        {
            var get_tran = from a in dbDC.TD_Transaction
                           where a.doc_type_short == doctype && a.group_code == groupcode
                           && a.run_no == runno && a.rev_no == revno && a.active == true
                           select a;
            return PartialView(get_tran);
        }

        public ActionResult _FormCopy(string doctype, string groupcode, int runno, byte revno)
        {
            var get_doc = dbDC.TD_Document.Find(doctype, groupcode, runno, revno);
            return PartialView(get_doc);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult _FormCopy1(string doctype, string groupcode, int runno, byte revno)
        {
            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno
                         select a).FirstOrDefault();
            var paper = from a in dbDC.TM_Paper
                        select a;

            ViewBag.Paper = paper;

            return PartialView(query);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult _FormCancel(string doctype, string groupcode, int runno, byte revno)
        {
            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno
                         select a).FirstOrDefault();
            return PartialView(query);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult _FormIssuerCancel(string doctype, string groupcode, int runno, byte revno)
        {
            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno
                         select a).FirstOrDefault();
            return PartialView(query);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult _FormReview(string doctype, string groupcode, int runno, byte revno)
        {
            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno
                         select a).FirstOrDefault();
            return PartialView(query);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult _ShowDocDetail(string doctype, string groupcode, int runno, byte revno, bool check = false, byte operation = 0, byte appv = 0)
        {
            List<VM_Comment> vobj = new List<VM_Comment>();
            VM_Comment vm;
            var comment = from t in dbDC.TD_Transaction
                          where t.doc_type_short == doctype && t.group_code == groupcode
                          && t.run_no == runno && t.rev_no == revno && t.lv_id != 0 && t.status_id != 99
                          select t;

            foreach (var item in comment.OrderBy(o => o.act_dt))
            {
                vm = new VM_Comment();
                vm.actor = GetUserName(item.actor);
                vm.act_name = item.action_id != null ? item.TM_Action.action_name : "Wait";
                vm.act_dt = item.act_dt;
                vm.status_name = item.TM_Status.status_name;
                vm.lv_name = item.TM_Level.lv_name;
                vm.operation_name = item.TM_Operation.operation_name;
                vm.comment = item.comment;
                vm.org_name = GetOrgName(item.lv_id, item.org_id);
                vobj.Add(vm);
            }
            ViewBag.CommentList = vobj;
            //------------------------------------------//

            byte lv = byte.Parse(Session["DC_ULv"].ToString());
            int org = int.Parse(Session["DC_Org"].ToString());
            string user = Session["DC_Auth"].ToString();

            //------------------------------------------//
            if (Session["DC_UType"] != null && Session["DC_UType"].ToString() != "0")
            {
                TempData["ShowFile"] = "show";
            }
            else if (Session["DC_ULv"] != null && (Session["DC_ULv"].ToString() == "3" || Session["DC_ULv"].ToString() == "4"))
            {//Add Date 2016-05-03 by Monchit W.
                TempData["ShowFile"] = "show";
            }
            else
            {
                if (check)//Check Show File
                {
                    var get_dis = dbDC.TD_DistributionList.Any(a => a.doc_type_short == doctype && a.group_code == groupcode
                                  && a.run_no == runno && a.rev_no == revno && (a.group_id == org || a.group_id == 0));
                    if (get_dis)
                        TempData["ShowFile"] = "show";
                    else
                        TempData["ShowFile"] = "hide";
                }
                else
                {
                    TempData["ShowFile"] = "show";
                }
            }

            //------------------------------------------//
            //var check_cancelled = comment.Where(w => w.operation_id == 6 && w.active == true);

            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno
                         select a).FirstOrDefault();

            if (appv == 1)
            {
                if (lv == 1 || lv == 2)
                {
                    var get_approve = comment.Where(w => w.active == true && w.status_id != 0 && w.operation_id == operation
                                                && (w.lv_id == lv && w.org_id == org)).FirstOrDefault();
                    if (get_approve != null)
                    {
                        if (get_approve.status_id == 3)
                        {
                            var get_dcc = dbDC.TM_User.Any(w => (w.utype_id >= 3 && w.utype_id <= 6) && w.empcode == user);
                            if (get_dcc)
                            {
                                ViewBag.Transaction = get_approve;
                            }
                        }
                        else if (get_approve.status_id == 9)
                        {
                            ViewBag.EditDoc = get_approve;
                        }
                        else
                        {
                            ViewBag.Transaction = get_approve;
                        }
                    }
                }
                else if (lv == 3)
                {
                    var groupindept = (from a in dbTNC.tnc_costcentercode
                                       where a.dept_id == org && a.group_id != 0
                                       select a.group_id).Distinct().ToList();

                    var get_approve = comment.Where(w => w.active == true && w.status_id != 0 && w.operation_id == operation
                        && ((w.lv_id == lv && w.org_id == org) || (w.lv_id == 2 && groupindept.Contains(w.org_id)))).FirstOrDefault();
                    if (get_approve != null)
                    {
                        ViewBag.Transaction = get_approve;
                    }
                }
                else if (lv >= 4)
                {
                    var groupinplant = (from a in dbTNC.tnc_costcentercode
                                        where a.plant_id == org && a.group_id != 0
                                        select a.group_id).Distinct().ToList();
                    var deptinplant = (from b in dbTNC.tnc_costcentercode
                                       where b.plant_id == org && b.dept_id != 0
                                       select b.dept_id).Distinct().ToList();

                    var get_approve = comment.Where(w => w.active == true && w.status_id != 0 && w.operation_id == operation
                                                    && ((w.lv_id == lv && w.org_id == org)
                                                    || (w.lv_id == 3 && deptinplant.Contains(w.org_id))
                                                    || (w.lv_id == 2 && groupinplant.Contains(w.org_id)))).FirstOrDefault();

                    if (get_approve != null)
                    {
                        ViewBag.Transaction = get_approve;
                    }
                }

                if (operation == 1 || operation == 2)//New or Revise
                    ViewBag.OverDue = DateTime.Now.Date > query.eff_date ? 2 : 1;// 1 is have All Button, 2 is have Reject Button Only
                else
                    ViewBag.OverDue = 1;
            }
            else if (appv == 2)
            {
                var groupindept = (from a in dbTNC.tnc_costcentercode
                                   where a.dept_id == org && a.group_id != 0
                                   select a.group_id).Distinct().ToList();
                var doc_gcode = query.TM_GroupCode.group_id;
                if ((lv == 2 && doc_gcode == org) || (lv == 3 && groupindept.Contains(doc_gcode)))
                {
                    TempData["EditDist"] = 1;
                    TempData["ShowFile"] = "show";
                }
            }

            return PartialView(query);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult _ShowDocDetailOnly(string doctype, string groupcode, int runno, byte revno, int reqid)
        {
            List<VM_TranCopy> vobj = new List<VM_TranCopy>();
            VM_TranCopy vm;
            var comment = from t in dbDC.TD_TranCopy
                          where t.req_id == reqid && t.lv_id != 0 //&& t.status_id != 105
                          select t;

            foreach (var item in comment.OrderBy(o => o.act_dt))
            {
                vm = new VM_TranCopy();
                vm.actor = GetUserName(item.actor);
                vm.act_name = item.action_id != null ? item.TM_Action.action_name : "Wait";
                vm.act_dt = item.act_dt;
                vm.status_name = item.TM_Status.status_name;
                vm.lv_name = item.TM_Level.lv_name;
                //vm.comment = item.comment;
                vm.org_name = GetOrgName(item.lv_id, item.org_id);
                vobj.Add(vm);
            }
            ViewBag.CommentList = vobj;

            byte lv = byte.Parse(Session["DC_ULv"].ToString());
            int org = int.Parse(Session["DC_Org"].ToString());
            string user = Session["DC_Auth"].ToString();

            //------------------------------------------------//
            var get_dis = dbDC.TD_DistributionList.Any(a => a.doc_type_short == doctype && a.group_code == groupcode
                                  && a.run_no == runno && a.rev_no == revno && (a.group_id == org || a.group_id == 0));

            //Add Date 2016-02-16 by Monchit
            if (Session["DC_UType"] != null && Session["DC_UType"].ToString() != "0")
            {
                TempData["ShowFile"] = "show";
            }
            else
            {
                if (get_dis)
                    TempData["ShowFile"] = "show";
                else
                    TempData["ShowFile"] = "hide";
            }

            //------------------------------------------------//
            //var comment = from t in dbDC.V_TranCopy
            //              where t.req_id == reqid && t.doc_type_short == doctype && t.group_code == groupcode
            //              && t.run_no == runno && t.rev_no == revno && t.lv_id != 0 && t.status_id != 105
            //              select t;
            if (comment != null)
            {
                if (lv == 1 || lv == 2)
                {
                    var get_approve = comment.Where(w => w.active == true && w.status_id != 0 && (w.lv_id == lv && w.org_id == org)).FirstOrDefault();
                    if (get_approve != null)
                    {
                        if (get_approve.status_id == 3)
                        {
                            var get_dcc = dbDC.TM_User.Any(w => (w.utype_id == 4 || w.utype_id == 3) && w.empcode == user);
                            if (get_dcc)
                            {
                                ViewBag.Transaction = get_approve;
                            }
                        }
                        else
                        {
                            ViewBag.Transaction = get_approve;
                        }
                    }
                }
                else if (lv == 3)
                {
                    var groupindept = (from a in dbTNC.tnc_costcentercode
                                       where a.dept_id == org && a.group_id != 0
                                       select a.group_id).Distinct().ToList();

                    var get_approve = comment.Where(w => w.active == true && w.status_id != 0 && ((w.lv_id == lv && w.org_id == org)
                                                || (w.lv_id == 2 && groupindept.Contains(w.org_id)))).FirstOrDefault();
                    if (get_approve != null)
                    {
                        ViewBag.Transaction = get_approve;
                    }
                }
                else if (lv >= 4)
                {
                    var groupinplant = (from a in dbTNC.tnc_costcentercode
                                        where a.plant_id == org && a.group_id != 0
                                        select a.group_id).Distinct().ToList();
                    var deptinplant = (from b in dbTNC.tnc_costcentercode
                                       where b.plant_id == org && b.dept_id != 0
                                       select b.dept_id).Distinct().ToList();

                    var get_approve = comment.Where(w => w.active == true && w.status_id != 0
                                                    && ((w.lv_id == lv && w.org_id == org)
                                                    || (w.lv_id == 3 && deptinplant.Contains(w.org_id))
                                                    || (w.lv_id == 2 && groupinplant.Contains(w.org_id)))).FirstOrDefault();

                    if (get_approve != null)
                    {
                        ViewBag.Transaction = get_approve;
                    }
                }
            }

            //------------------------------------------------//
            var query = (from a in dbDC.TD_Document
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno
                         select a).FirstOrDefault();

            return PartialView(query);
        }

        private string GetOrgName(byte user_lv, int org)
        {
            string org_name = "";

            if (user_lv < 3)
            {
                org_name = (from o in dbTNC.View_Organization
                            where o.group_id == org
                            select o.group_name).FirstOrDefault();
            }
            else if (user_lv < 4)
            {
                org_name = (from o in dbTNC.View_Organization
                            where o.dept_id == org
                            select o.dept_name).FirstOrDefault();
            }
            else if (user_lv < 5)
            {
                org_name = (from o in dbTNC.View_Organization
                            where o.plant_id == org
                            select o.plant_name).FirstOrDefault();
            }
            return org_name;
        }

        private string GetUserName(string emp_code)
        {
            string name = "";
            var query = (from u in dbTNC.tnc_user
                         where u.emp_code == emp_code
                         select u).FirstOrDefault();
            if (query != null)
            {
                name = query.emp_fname + " " + query.emp_lname.Substring(0, 2) + ".";
            }
            return name;
        }

        //-------------------------------------------//

        [AllowAnonymous]
        public ActionResult Login(string key = null)
        {
            string username = key == null ? Request.Form["Username"].ToString() : "";
            string pass = key == null ? Request.Form["Password"].ToString() : "";

            var chklogin = secure.Login(username, pass, true);//set false to true for Real

            if (key != null)
            {
                username = secure.WebCenterDecode(key);
                chklogin = secure.Login(username, "a", false);
            }

            if (chklogin != null)
            {
                Session["DC_Auth"] = chklogin.emp_code;

                if (chklogin.plant_id == 11)//QC Div.
                {
                    Session["DC_QC"] = 1;
                }
                else
                {
                    Session["DC_QC"] = 0;
                }

                var pos_lv = (from lv in dbDC.TM_Level
                              where lv.position_min <= chklogin.position_level && lv.position_max >= chklogin.position_level
                              select lv).FirstOrDefault();

                if (pos_lv != null)
                {
                    Session["DC_ULv"] = pos_lv.lv_id;
                    if (pos_lv.lv_id <= 2)
                    {
                        Session["DC_Org"] = chklogin.group_id;
                    }
                    else if (pos_lv.lv_id == 3)
                    {
                        Session["DC_Org"] = chklogin.dept_id;
                    }
                    else if (pos_lv.lv_id >= 4)
                    {
                        Session["DC_Org"] = chklogin.plant_id;
                    }
                }

                if (chklogin.emp_lname.Length > 2)
                {
                    Session["DC_Name"] = chklogin.emp_fname + " " + chklogin.emp_lname.Substring(0, 2) + ". (" + pos_lv.lv_name + ")";
                }
                else
                {
                    Session["DC_Name"] = chklogin.emp_fname + " " + chklogin.emp_lname + ". (" + pos_lv.lv_name + ")";
                }

                var chk_sysuser = (from a in dbDC.TM_User
                                   where a.empcode == chklogin.emp_code
                                   select a).FirstOrDefault();

                Session["DC_UType"] = chk_sysuser != null ? chk_sysuser.utype_id : 0;

                //if (pos_lv.lv_id <= 2)
                //{
                //    bool check = dbDC.TD_Questionair.Any(w => w.emp_code == chklogin.emp_code);
                //    if (check)
                //    {
                //        return RedirectToAction("Inprogress", "Home");
                //    }
                //    else
                //    {
                //        return RedirectToAction("Questionair", "Home");
                //    }
                //}

                if (Session["Redirect"] != null)
                {
                    string url = Session["Redirect"].ToString();
                    Session.Remove("Redirect");
                    return Redirect(url);
                }
                else
                {
                    return RedirectToAction("Inprogress", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("DC_Auth");
            Session.Remove("DC_Name");
            Session.Remove("DC_UType");
            Session.Remove("DC_ULv");
            Session.Remove("DC_Org");
            Session.Remove("DC_QC");
            return RedirectToAction("Index", "Home");
        }

        //-------------------------------------------//

        [HttpPost]
        public ActionResult AddQuestionnaire()
        {
            try
            {
                string emp = Session["DC_Auth"].ToString();

                bool check = dbDC.TD_Questionair.Any(w => w.emp_code == emp);

                if (!check)
                {
                    TD_Questionair quiz = new TD_Questionair();

                    quiz.emp_code = emp;
                    quiz.act_dt = DateTime.Now;

                    quiz.u1 = byte.Parse(Request.Form["sex"].ToString());
                    quiz.u2 = byte.Parse(Request.Form["age"].ToString());
                    quiz.u3 = byte.Parse(Request.Form["study"].ToString());
                    quiz.u4 = byte.Parse(Request.Form["position"].ToString());
                    quiz.u5 = byte.Parse(Request.Form["experience"].ToString());
                    quiz.u6 = byte.Parse(Request.Form["section"].ToString());

                    quiz.o1 = byte.Parse(Request.Form["o1"].ToString());
                    quiz.o2 = byte.Parse(Request.Form["o2"].ToString());
                    quiz.o3 = byte.Parse(Request.Form["o3"].ToString());
                    quiz.o4 = byte.Parse(Request.Form["o4"].ToString());
                    quiz.o5 = byte.Parse(Request.Form["o5"].ToString());
                    quiz.o6 = byte.Parse(Request.Form["o6"].ToString());
                    quiz.o7 = byte.Parse(Request.Form["o7"].ToString());
                    quiz.o8 = byte.Parse(Request.Form["o8"].ToString());
                    quiz.o9 = byte.Parse(Request.Form["o9"].ToString());
                    quiz.o10 = byte.Parse(Request.Form["o10"].ToString());
                    quiz.o11 = byte.Parse(Request.Form["o11"].ToString());
                    quiz.o12 = byte.Parse(Request.Form["o12"].ToString());
                    quiz.o13 = byte.Parse(Request.Form["o13"].ToString());
                    quiz.o14 = byte.Parse(Request.Form["o14"].ToString());
                    quiz.o15 = byte.Parse(Request.Form["o15"].ToString());
                    quiz.o16 = byte.Parse(Request.Form["o16"].ToString());
                    quiz.o17 = byte.Parse(Request.Form["o17"].ToString());
                    quiz.o18 = byte.Parse(Request.Form["o18"].ToString());

                    quiz.n1 = byte.Parse(Request.Form["n1"].ToString());
                    quiz.n2 = byte.Parse(Request.Form["n2"].ToString());
                    quiz.n3 = byte.Parse(Request.Form["n3"].ToString());
                    quiz.n4 = byte.Parse(Request.Form["n4"].ToString());
                    quiz.n5 = byte.Parse(Request.Form["n5"].ToString());
                    quiz.n6 = byte.Parse(Request.Form["n6"].ToString());
                    quiz.n7 = byte.Parse(Request.Form["n7"].ToString());
                    quiz.n8 = byte.Parse(Request.Form["n8"].ToString());
                    quiz.n9 = byte.Parse(Request.Form["n9"].ToString());
                    quiz.n10 = byte.Parse(Request.Form["n10"].ToString());
                    quiz.n11 = byte.Parse(Request.Form["n11"].ToString());
                    quiz.n12 = byte.Parse(Request.Form["n12"].ToString());
                    quiz.n13 = byte.Parse(Request.Form["n13"].ToString());
                    quiz.n14 = byte.Parse(Request.Form["n14"].ToString());
                    quiz.n15 = byte.Parse(Request.Form["n15"].ToString());
                    quiz.n16 = byte.Parse(Request.Form["n16"].ToString());
                    quiz.n17 = byte.Parse(Request.Form["n17"].ToString());
                    quiz.n18 = byte.Parse(Request.Form["n18"].ToString());

                    dbDC.TD_Questionair.Add(quiz);
                    dbDC.SaveChanges();
                    TempData["result"] = "Thank you for participating this questionnaire.";
                }
                else
                {
                    TempData["result"] = "You've already done this questionnaire.";
                }

                return RedirectToAction("Inprogress", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult CreateDoc(HttpPostedFileBase flAtt, HttpPostedFileBase flSrc, int[] hcGroup, int[] hcQty, string[] hcNote)
        {
            try
            {
                var docType = Request.Form["selDocType"].ToString();
                var gCode = Request.Form["selGroupCode"].ToString();
                var runno = int.Parse(Request.Form["txtRunno"].ToString());
                var revno = byte.Parse(Request.Form["txtRev"].ToString());
                byte operate = revno != 0 ? (byte)2 : (byte)1;
                byte sub_rev = 0;//GetSubRevCurrent(docType, gCode, runno, revno, operate);
                string actor = Session["DC_Auth"].ToString();
                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    bool check_doc = dbDC.TD_Document
                        .Any(w => w.doc_type_short == docType && w.group_code == gCode && w.run_no == runno && w.rev_no == revno);
                    if (!check_doc && !string.IsNullOrEmpty(actor))
                    {
                        DateTime dt = DateTime.Now;
                        TD_Document doc = new TD_Document();
                        var effdt = Request.Form["dtEff"] != null ? ParseToDate(Request.Form["dtEff"].ToString()) : dt;
                        doc.doc_type_short = docType;
                        doc.group_code = gCode;
                        doc.run_no = runno;
                        doc.rev_no = revno;
                        doc.doc_name = Request.Form["txtDocName"].ToString();
                        doc.eff_date = effdt;
                        doc.check_date = effdt.AddYears(2);
                        doc.reference = Request.Form["txtRemark"].ToString();
                        doc.remark = Request.Form["txaRemark"] != null ? Request.Form["txaRemark"].ToString() : "";

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/";// +docType + "/";// +gCode + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (flAtt != null && flAtt.ContentLength > 0)
                        {
                            if (flAtt.ContentType == "application/pdf")
                            {
                                var extension = Path.GetExtension(flAtt.FileName);
                                var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                    + "-r" + revno.ToString("00") + extension;
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                flAtt.SaveAs(path);
                                doc.attach_file = subPath + fileName;
                            }
                        }

                        if (flSrc != null && flSrc.ContentLength > 0)
                        {
                            var extension = Path.GetExtension(flSrc.FileName);
                            if (extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                            {
                                var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                    + "-r" + revno.ToString("00") + extension;
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                flSrc.SaveAs(path);//Save on Server
                                doc.src_file = subPath + fileName;//Add record to DB
                            }
                        }

                        dbDC.TD_Document.Add(doc);

                        if (Request.Form["sel2List"] != null)//Update Date 2015-01-27 by Monchit W.
                        {
                            AddDistributionList(docType, gCode, runno, revno, Request.Form["sel2List"].ToString());
                        }
                        else
                        {
                            scope.Dispose();
                        }

                        if (CheckHasHardCopy(docType) && hcGroup != null && hcQty != null)
                        {
                            AddHC(hcGroup, hcQty, hcNote, docType, gCode, runno, revno, 0);
                        }
                        GetNextTransaction(docType, gCode, runno, revno, ++sub_rev, 0, lv, org, operate, 1, actor);
                        dbDC.SaveChanges();

                        List<string> document_type = new List<string>() { "EM", "LM" };
                        if (!document_type.Contains(docType))
                        {
                            TNCOrganization tnc_org = new TNCOrganization();
                            tnc_org.GetApprover(actor);
                            SendEmailCenter(docType, gCode, runno, revno, tnc_org.ManagerEMail, operate);
                        }

                        //if (operate == 2)//Revise
                        //{
                        //    UpdateCheckDate(docType, gCode, runno, (byte)(revno - 1), effdt);
                        //}

                        scope.Complete();

                        TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
                    }
                    else
                    {
                        TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action NOT success";
                    }
                }
                return RedirectToAction("Inprogress", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult CreateVDO(HttpPostedFileBase flAtt, HttpPostedFileBase flSrc)
        {
            try
            {
                var docType = Request.Form["txtDocType"].ToString();
                var gCode = Request.Form["selGroupCode"].ToString();
                var runno = int.Parse(Request.Form["txtRunno"].ToString());
                var revno = byte.Parse(Request.Form["txtRev"].ToString());
                byte operate = revno != 0 ? (byte)2 : (byte)1;
                byte sub_rev = 0;//GetSubRevCurrent(docType, gCode, runno, revno, operate);
                string actor = Session["DC_Auth"].ToString();
                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    bool check_doc = dbDC.TD_Document
                        .Any(w => w.doc_type_short == docType && w.group_code == gCode && w.run_no == runno && w.rev_no == revno);
                    if (!check_doc && !string.IsNullOrEmpty(actor))
                    {
                        DateTime dt = DateTime.Now;
                        TD_Document doc = new TD_Document();
                        var effdt = Request.Form["dtEff"] != null ? ParseToDate(Request.Form["dtEff"].ToString()) : dt;
                        doc.doc_type_short = docType;
                        doc.group_code = gCode;
                        doc.run_no = runno;
                        doc.rev_no = revno;
                        doc.doc_name = Request.Form["txtDocName"].ToString();
                        doc.eff_date = effdt;
                        doc.check_date = effdt.AddYears(2);
                        doc.reference = Request.Form["txtRemark"].ToString();
                        doc.remark = Request.Form["txaRemark"] != null ? Request.Form["txaRemark"].ToString() : "";

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/";
                        string AtthName = docType + "-" + gCode + "-" + runno.ToString("0000") + "-r" + revno.ToString("00");
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (flAtt != null && flAtt.ContentLength > 0)
                        {
                            if (flAtt.ContentType == "application/pdf")
                            {
                                var extension = Path.GetExtension(flAtt.FileName);
                                var fileName = AtthName + extension;
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                flAtt.SaveAs(path);
                                doc.attach_file = subPath + fileName;
                            }
                        }

                        if (flSrc != null && flSrc.ContentLength > 0)
                        {
                            var extension = Path.GetExtension(flSrc.FileName).ToLower();
                            if (extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                            {
                                var fileName = AtthName + extension;
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                flSrc.SaveAs(path);//Save on Server
                                doc.src_file = subPath + fileName;//Add record to DB
                            }
                        }

                        string sourcePath = "~/UploadFiles/VDO/" + actor + "/";
                        string targetPath = "~/UploadFiles/VDO/" + dt.Year + "/" + dt.Month + "/";
                        string[] vdoName = GetFileNames(Server.MapPath(sourcePath), "*.mp4");

                        if (vdoName.Length > 0)
                        {
                            string vdofileName = vdoName[0];
                            string vdoNewName = AtthName + ".mp4";

                            if (!Directory.Exists(Server.MapPath(targetPath))) Directory.CreateDirectory(Server.MapPath(targetPath));

                            //Use Path class to manipulate file and directory paths.
                            string sourceFile = Path.Combine(Server.MapPath(sourcePath), vdofileName);
                            string destFile = Path.Combine(Server.MapPath(targetPath), vdoNewName);//Change Name bofore move.

                            // To move a file or folder to a new location:
                            System.IO.File.Move(sourceFile, destFile);
                            doc.vdo_path = targetPath + vdoNewName;//Add record to DB
                        }

                        dbDC.TD_Document.Add(doc);

                        if (Request.Form["sel2List"] != null)//Update Date 2015-01-27 by Monchit W.
                        {
                            AddDistributionList(docType, gCode, runno, revno, Request.Form["sel2List"].ToString());
                        }
                        else
                        {
                            scope.Dispose();
                        }

                        GetNextTransaction(docType, gCode, runno, revno, ++sub_rev, 0, lv, org, operate, 1, actor);
                        dbDC.SaveChanges();

                        scope.Complete();

                        TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
                    }
                    else
                    {
                        TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action NOT success";
                    }
                }
                return RedirectToAction("Inprogress", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult InitialDoc(HttpPostedFileBase flAtt, HttpPostedFileBase flSrc, int[] hcGroup, int[] hcQty, string[] hcNote)
        {
            try
            {
                var docType = Request.Form["selDocType"].ToString();
                var gCode = Request.Form["selGroupCode"].ToString();
                var runno = int.Parse(Request.Form["txtRunno"].ToString());
                var revno = byte.Parse(Request.Form["txtRev"].ToString());
                byte sub_rev = 0;
                string actor = Session["DC_Auth"].ToString();
                byte lv = 1;// byte.Parse(Session["DC_ULv"].ToString());
                int org = dbDC.TM_GroupCode.Where(w => w.group_code == gCode).Select(s => s.group_id).FirstOrDefault();//int.Parse(Session["DC_Org"].ToString());
                byte operate = 1;
                if (revno != 0)
                {
                    operate = 2;
                }

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    bool check_doc = dbDC.TD_Document
                        .Any(w => w.doc_type_short == docType && w.group_code == gCode && w.run_no == runno && w.rev_no == revno);
                    if (!check_doc)
                    {
                        byte review_dt = (from a in dbDC.TM_DocType
                                          where a.doc_type_short == docType
                                          select a.review_year).FirstOrDefault();
                        DateTime dt = DateTime.Now;
                        var effDate = Request.Form["dtEff"] != null ? ParseToDate(Request.Form["dtEff"].ToString()) : dt;
                        TD_Document doc = new TD_Document();
                        doc.doc_type_short = docType;
                        doc.group_code = gCode;
                        doc.run_no = runno;
                        doc.rev_no = revno;
                        doc.doc_name = Request.Form["txtDocName"].ToString();
                        doc.eff_date = effDate;
                        doc.reference = Request.Form["txtRemark"].ToString();
                        //doc.check_date = review_dt != null ? effDate.AddYears(review_dt) : effDate.AddYears(2);
                        doc.check_date = effDate.AddYears(review_dt);

                        // Add data to DB TD_File //
                        string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/";// +docType + "/";// +gCode + "/";
                        if (!Directory.Exists(Server.MapPath(subPath)))
                            Directory.CreateDirectory(Server.MapPath(subPath));

                        if (flAtt != null && flAtt.ContentLength > 0)
                        {
                            if (flAtt.ContentType == "application/pdf")
                            {
                                var extension = Path.GetExtension(flAtt.FileName);
                                var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                    + "-r" + revno.ToString("00") + extension;
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                flAtt.SaveAs(path);
                                doc.attach_file = subPath + fileName;
                            }
                        }

                        if (flSrc != null && flSrc.ContentLength > 0)
                        {
                            var extension = Path.GetExtension(flSrc.FileName);
                            if (extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                            {
                                var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                    + "-r" + revno.ToString("00") + extension;
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                flSrc.SaveAs(path);//Save on Server
                                doc.src_file = subPath + fileName;//Add record to DB
                            }
                        }

                        dbDC.TD_Document.Add(doc);

                        var codeGroup = (from a in dbDC.TM_GroupCode
                                         where a.group_code == gCode
                                         select a.group_id).FirstOrDefault();

                        var distribution_list = Request.Form["sel2List"] != null ? Request.Form["sel2List"].ToString() : null;
                        //var distribution_list = codeGroup.ToString();
                        AddDistributionList(docType, gCode, runno, revno, distribution_list);
                        if (CheckHasHardCopy(docType) && hcGroup != null && hcQty != null)
                        {
                            AddHC(hcGroup, hcQty, hcNote, docType, gCode, runno, revno, 0);
                        }

                        CreateTransaction(docType, gCode, runno, revno, sub_rev, 0, lv, org, operate, false, 1, actor);
                        CreateTransaction(docType, gCode, runno, revno, sub_rev, 100, lv, org, operate, false, 1);// Create Transaction 100 = Controlled
                        //GetNextTransaction(docType, gCode, runno, revno, sub_rev, 0, lv, org, operate, 1, actor);
                        dbDC.SaveChanges();
                        scope.Complete();

                        TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
                    }
                    else
                    {
                        TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action NOT success";
                    }
                }
                return RedirectToAction("DocInitial", "Home");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }

        [HttpPost]
        public ActionResult UpdateDoc(HttpPostedFileBase flAtt, HttpPostedFileBase flSrc, int[] hcGroup, int[] hcQty, string[] hcNote)
        {
            try
            {
                var docType = Request.Form["hdDT"].ToString();
                var gCode = Request.Form["hdGC"].ToString();
                var runno = int.Parse(Request.Form["hdRN"].ToString());
                var revno = byte.Parse(Request.Form["hdRV"].ToString());
                byte operate = revno != 0 ? (byte)2 : (byte)1;
                byte sub_rev = GetSubRevCurrent(docType, gCode, runno, revno, operate);
                string actor = Session["DC_Auth"].ToString();
                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    DateTime dt = DateTime.Now;
                    var effdt = Request.Form["dtEff"] != null ? ParseToDate(Request.Form["dtEff"].ToString()) : dt;
                    var doc = dbDC.TD_Document.Find(docType, gCode, runno, revno);
                    doc.doc_name = Request.Form["txtDocName"].ToString();
                    doc.eff_date = effdt;
                    doc.reference = Request.Form["txtRemark"].ToString();
                    doc.remark = Request.Form["txaRemark"] != null ? Request.Form["txaRemark"].ToString() : "";

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/";// +docType + "/";// +gCode + "/";
                    //string target = "~/UploadFiles/Old/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                        Directory.CreateDirectory(Server.MapPath(subPath));

                    if (flAtt != null && flAtt.ContentLength > 0)
                    {
                        if (flAtt.ContentType == "application/pdf")
                        {
                            var extension = Path.GetExtension(flAtt.FileName);
                            var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                + "-r" + revno.ToString("00") + extension;//+ "_" + sub_rev
                            var path = Path.Combine(Server.MapPath(subPath), fileName);

                            //if (System.IO.File.Exists(path))
                            //{
                            //    var path2 = Path.Combine(Server.MapPath(target), fileName);
                            //    if (System.IO.File.Exists(path2))
                            //    {
                            //        System.IO.File.Delete(path2);
                            //    }

                            //    System.IO.File.Move(path, path2);
                            //}

                            flAtt.SaveAs(path);
                            doc.attach_file = subPath + fileName;
                        }
                    }

                    if (flSrc != null && flSrc.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(flSrc.FileName);
                        if (extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                        {
                            var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                + "-r" + revno.ToString("00") + extension;
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            flSrc.SaveAs(path);//Save on Server
                            doc.src_file = subPath + fileName;//Add record to DB
                        }
                    }

                    var distribution_list = Request.Form["sel2List"] != null ? Request.Form["sel2List"].ToString() : null;

                    if (distribution_list != null)
                    {
                        DelDistributionList(docType, gCode, runno, revno);
                        AddDistributionList(docType, gCode, runno, revno, distribution_list);
                    }

                    if (CheckHasHardCopy(docType))
                    {
                        if (hcGroup != null && hcQty != null)
                        {
                            DelHC(docType, gCode, runno, revno, 0);
                            AddHC(hcGroup, hcQty, hcNote, docType, gCode, runno, revno, 0);
                        }
                        else
                        {
                            DelHC(docType, gCode, runno, revno, 0);
                        }
                    }

                    UpdateTransaction(docType, gCode, runno, revno, sub_rev, 9, lv, org, operate, false, 1, actor, save: true);
                    GetNextTransaction(docType, gCode, runno, revno, sub_rev, 0, lv, org, operate, 4, actor);
                    dbDC.SaveChanges();

                    //if (operate == 2)
                    //{
                    //    UpdateCheckDate(docType, gCode, runno, (byte)(revno - 1), effdt);
                    //}

                    scope.Complete();
                    TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
                }
                return RedirectToAction("Inprogress", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult MgrUpdateDoc(HttpPostedFileBase flAtt, HttpPostedFileBase flSrc, int[] hcGroup, int[] hcQty, string[] hcNote)
        {
            try
            {
                var docType = Request.Form["hdDT"].ToString();
                var gCode = Request.Form["hdGC"].ToString();
                var runno = int.Parse(Request.Form["hdRN"].ToString());
                var revno = byte.Parse(Request.Form["hdRV"].ToString());
                //byte operate = revno != 0 ? (byte)2 : (byte)1;
                //byte sub_rev = GetSubRevCurrent(docType, gCode, runno, revno, operate);
                //string actor = Session["DC_Auth"].ToString();
                //byte lv = byte.Parse(Session["DC_ULv"].ToString());
                //int org = int.Parse(Session["DC_Org"].ToString());

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    DateTime dt = DateTime.Now;
                    var effdt = Request.Form["dtEff"] != null ? ParseToDate(Request.Form["dtEff"].ToString()) : dt;
                    var doc = dbDC.TD_Document.Find(docType, gCode, runno, revno);
                    doc.doc_name = Request.Form["txtDocName"].ToString();
                    doc.eff_date = effdt;
                    doc.reference = Request.Form["txtRemark"].ToString();
                    doc.remark = Request.Form["txaRemark"] != null ? Request.Form["txaRemark"].ToString() : "";

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/";// +docType + "/";// +gCode + "/";
                    //string target = "~/UploadFiles/Old/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                        Directory.CreateDirectory(Server.MapPath(subPath));

                    if (flAtt != null && flAtt.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(flAtt.FileName);
                        if (flAtt.ContentType == "application/pdf")
                        {
                            var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                + "-r" + revno.ToString("00") + extension;
                            var path = Path.Combine(Server.MapPath(subPath), fileName);

                            flAtt.SaveAs(path);
                            doc.attach_file = subPath + fileName;
                        }
                    }

                    if (flSrc != null && flSrc.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(flSrc.FileName);
                        if (extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                        {
                            var fileName = docType + "-" + gCode + "-" + runno.ToString("0000")
                                + "-r" + revno.ToString("00") + extension;
                            var path = Path.Combine(Server.MapPath(subPath), fileName);

                            flSrc.SaveAs(path);//Save on Server
                            doc.src_file = subPath + fileName;//Add record to DB
                        }
                    }

                    var distribution_list = Request.Form["sel2List"] != null ? Request.Form["sel2List"].ToString() : null;

                    if (distribution_list != null)
                    {
                        DelDistributionList(docType, gCode, runno, revno);
                        AddDistributionList(docType, gCode, runno, revno, distribution_list);
                    }

                    if (CheckHasHardCopy(docType))
                    {
                        if (hcGroup != null && hcQty != null)
                        {
                            DelHC(docType, gCode, runno, revno, 0);
                            AddHC(hcGroup, hcQty, hcNote, docType, gCode, runno, revno, 0);
                        }
                        else
                        {
                            DelHC(docType, gCode, runno, revno, 0);
                        }
                    }

                    //UpdateTransaction(docType, gCode, runno, revno, sub_rev, 9, lv, org, operate, false, 1, actor, save: true);
                    //GetNextTransaction(docType, gCode, runno, revno, sub_rev, 0, lv, org, operate, 4, actor);
                    dbDC.SaveChanges();

                    //if (operate == 2)
                    //{
                    //    UpdateCheckDate(docType, gCode, runno, (byte)(revno - 1), effdt);
                    //}

                    scope.Complete();
                    TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
                }
                return RedirectToAction("Inprogress", "Home");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GetNextTransaction(string doc_type_short, string group_code, int run_no, byte rev_no, byte sub_rev, byte status, byte lv, int org, byte operate, byte action, string actor, string comment = "")
        {
            TNCOrganization tnc_org = new TNCOrganization();
            tnc_org.GetApprover(actor);

            if (action == 1) // Issue
            {
                // Issue Transaction
                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, status, lv, org, operate, false, action, actor, comment);

                if (doc_type_short == "EM") // Environmental Manual
                {
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 1, 3, 1, operate, true);//EMR = GA Dept.
                    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(1, 3), operate);
                }
                else if (doc_type_short == "LM") // Labour Manual
                {
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 1, 4, 7, operate, true);//LMR = Admin Div.
                    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(7, 4), operate);
                }
                else if (doc_type_short == "SD" || doc_type_short == "LAW" || doc_type_short == "ทะเบียนกฎหมาย")
                {
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 2, 2, 136, operate, true);//EMR Safety
                    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(136, 2), operate);
                }
                else
                {
                    // Next Transaction
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, ++status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, operate, true); // Upper lv.
                }
            }
            else if (action == 2) // Approve
            {
                if (operate == 6)//Cancel by Issuer
                {
                    var get_cancel = (from a in dbDC.TD_Transaction
                                      where a.status_id == 0 && a.doc_type_short == doc_type_short
                                      && a.group_code == group_code && a.run_no == run_no && a.rev_no == rev_no
                                      select a).FirstOrDefault();
                    if (get_cancel != null)
                    {
                        CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 102, get_cancel.lv_id, get_cancel.org_id, operate, false, 1);//102 is cancel by issuer
                        SetActiveTransaction(doc_type_short, group_code, run_no, rev_no);
                    }
                }
                else if (status == 1)// Status Issuer
                {
                    byte get_doc_lv = (from l in dbDC.TM_DocType
                                       where l.doc_type_short == doc_type_short
                                       select l.doc_lv).FirstOrDefault();

                    if (get_doc_lv > 2) // Level 3, 4
                    {
                        if (doc_type_short == "WI" || doc_type_short == "WP" || doc_type_short == "PC" || doc_type_short == "OC" || doc_type_short == "RP" || doc_type_short == "VDO")
                        {
                            int get_group = (from a in dbDC.TM_GroupCode
                                             where a.group_code == group_code
                                             select a.group_id).FirstOrDefault();

                            var get_reviewer = (from r in dbDC.TM_Reviewer_Prod
                                                where r.group_id == get_group
                                                select r).FirstOrDefault();

                            if (get_reviewer != null)
                            {
                                TNCOrganization review_org = new TNCOrganization();
                                review_org.GetApprover(get_reviewer.qc_group, 1);//1 is Lv. Group Mgr.
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, ++status, (byte)(review_org.OrgLevel + 1), review_org.OrgId, operate, true);
                                SendEmailCenter(doc_type_short, group_code, run_no, rev_no, review_org.ManagerEMail, operate);
                                if (get_reviewer.qc_group != get_reviewer.en_group)
                                {
                                    review_org.GetApprover(get_reviewer.en_group, 1);
                                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, status, (byte)(review_org.OrgLevel + 1), review_org.OrgId, operate, true);
                                    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, review_org.ManagerEMail, operate);
                                }
                            }
                            else if (operate == 4) // Review - Add Condition 2015-05-27
                            {
                                UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                            }
                            else
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 3, 1, 18, operate, true);
                                SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailDCC(group_code), operate);
                            }
                        }
                        else if (operate == 4) // Review // Update Date 2015-05-27 -> Add Condition
                        {
                            UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                        }
                        else if (doc_type_short == "AI" || doc_type_short == "ROE") // Aspect
                        {
                            //Update Date 2015-02-10 by Monchit W. //Change from 5 to 136
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 2, 2, 136, operate, true);//EMR Safety
                            SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(136, 2), operate);
                        }
                        else//Other
                        {
                            var chk_reviewer = dbDC.TM_DocType.Find(doc_type_short);

                            if (chk_reviewer.dcc == "safety")
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 3, 1, 136, operate, true);
                                SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailSafety(), operate);
                            }
                            else
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 3, 1, 18, operate, true);
                                SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailDCC(group_code), operate);
                            }
                        }
                    }
                    else if (operate == 4) // Review - Add Condition 2015-05-27
                    {
                        UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                    }
                    else if (get_doc_lv == 2)//Lv. 2 : QP,EP,LP => Dept.
                    {
                        if (lv <= 2)
                        {
                            if (doc_type_short == "QP")
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, operate, true);
                            }
                            else
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, ++status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, operate, true);
                            }
                            SendEmailCenter(doc_type_short, group_code, run_no, rev_no, tnc_org.ManagerEMail, operate);
                        }
                        //else// Dept. => DCC Staff
                        //{
                        //    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 3, 1, 18, operate, true);
                        //    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailDCC(group_code), operate);
                        //} //Update Date 2015-11-02 by Monchit. //Change from to DCC -> to QMR (K.Duangnapa)
                        else //Add Date 2015-11-02 by Monchit.
                        {
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 2, 3, 49, operate, true);
                            SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(49, 3), operate);
                        }
                    }
                    else if (get_doc_lv == 1)//Lv. 1 : QM,EM,LM => MD
                    {
                        CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 2, 5, 21, operate, true);
                        SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(21, 5), operate);
                    }
                }
                else if (status == 2)// Reviewer
                {
                    var check_all_approve = (from a in dbDC.TD_Transaction
                                             where a.doc_type_short == doc_type_short && a.group_code == group_code
                                             && a.run_no == run_no && a.rev_no == rev_no && a.status_id == status && a.active == true
                                             select a).Count();
                    if (check_all_approve == 0)
                    {
                        var chk_reviewer = dbDC.TM_DocType.Find(doc_type_short);

                        if (operate == 4)// Review Doc
                        {
                            UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                        }
                        else if (chk_reviewer.dcc == "qs")
                        {
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, ++status, 1, 18, operate, true);
                            SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailDCC(group_code), operate);
                        }
                        else if (chk_reviewer.dcc == "safety")
                        {
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, ++status, 1, 136, operate, true);
                            SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailSafety(), operate);
                        }
                    }
                }
                else if (status == 3)// DCC
                {
                    //if (lv == 1)
                    //{
                    //    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, operate, true);
                    //    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, tnc_org.ManagerEMail, operate);
                    //}
                    //else if (lv >= 2) // Complete
                    //{
                    if (operate == 1)// New
                    {
                        var get_issue = (from a in dbDC.TD_Transaction
                                         where a.status_id == 0 && a.doc_type_short == doc_type_short
                                         && a.group_code == group_code && a.run_no == run_no && a.rev_no == rev_no
                                         select a).FirstOrDefault();
                        if (get_issue != null)
                        {
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 100, get_issue.lv_id, get_issue.org_id, operate, false, 1);// 100 = Controlled
                            SetActiveTransaction(doc_type_short, group_code, run_no, rev_no);//Set All Transaction active => false
                            //Check has Copy for print
                            var get_copy = dbDC.TD_Copy.Any(w => w.doc_type_short == doc_type_short && w.group_code == group_code && w.run_no == run_no && w.rev_no == rev_no);
                            if (get_copy)
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, 0, 99, 1, 0, operate, true);
                            }
                        }
                        UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                        SendEmailDistribution(doc_type_short, group_code, run_no, rev_no, operate);
                    }
                    else if (operate == 2)// Revise
                    {
                        var get_issue = (from a in dbDC.TD_Transaction
                                         where a.status_id == 0 && a.doc_type_short == doc_type_short
                                         && a.group_code == group_code && a.run_no == run_no && a.rev_no == rev_no
                                         select a).FirstOrDefault();
                        if (get_issue != null)
                        {
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 100, get_issue.lv_id, get_issue.org_id, operate, false, 1);// Create Transaction 100 = Controlled
                            SetActiveTransaction(doc_type_short, group_code, run_no, rev_no);//Set All Transaction active => false

                            CreateTransaction(doc_type_short, group_code, run_no, (byte)(rev_no - 1), 0, 101, get_issue.lv_id, get_issue.org_id, operate, false, 1);// Create Transaction 101 = Obsolete for old document.

                            //UpdateCancelDate(doc_type_short, group_code, run_no, (byte)(rev_no - 1),
                            //Check has Copy for print
                            var get_copy = dbDC.TD_Copy.Any(w => w.doc_type_short == doc_type_short && w.group_code == group_code && w.run_no == run_no && w.rev_no == rev_no);
                            if (get_copy)
                            {
                                CreateTransaction(doc_type_short, group_code, run_no, rev_no, 0, 99, 1, 0, operate, true);
                            }
                        }
                        UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                        SendEmailDistribution(doc_type_short, group_code, run_no, rev_no, operate);
                    }
                    else if (operate == 3)// Cancel
                    {
                        var get_issue = (from a in dbDC.TD_Transaction
                                         where a.status_id == 0 && a.doc_type_short == doc_type_short
                                         && a.group_code == group_code && a.run_no == run_no && a.rev_no == rev_no
                                         select a).FirstOrDefault();
                        if (get_issue != null)
                        {
                            CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 103, get_issue.lv_id, get_issue.org_id, operate, false, 1);
                        }
                        //SendEmailDistribution(doc_type_short, group_code, run_no, rev_no, operate);
                    }
                    //else if (operate == 4)//Review
                    //{
                    //    UpdateCheckDate(doc_type_short, group_code, run_no, rev_no);
                    //}
                    //Send Email to person in root
                    SendEmailinRoot(doc_type_short, group_code, run_no, rev_no, operate);
                    //}
                }
            }
            else if (action == 3) // Reject
            {
                var get_issue = (from a in dbDC.TD_Transaction
                                 where (a.status_id == 0 || a.status_id == 9) && a.doc_type_short == doc_type_short
                                 && a.group_code == group_code && a.run_no == run_no && a.rev_no == rev_no
                                 orderby a.act_dt descending
                                 select a).FirstOrDefault();
                if (status == 2)// Reviewer
                {
                    SetActiveTransaction(doc_type_short, group_code, run_no, rev_no, operate, status);
                }

                if (operate == 1 || operate == 2)//New or Revise
                {
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, ++sub_rev, 9, get_issue.lv_id, get_issue.org_id, operate, true);
                }
            }
            else if (action == 4) // Edited
            {
                if (doc_type_short == "SD" || doc_type_short == "LAW" || doc_type_short == "ทะเบียนกฎหมาย")
                {
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, 2, 2, 136, operate, true);//EMR Safety
                    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, GetEmailbyOrg(136, 2), operate);
                }
                else
                {
                    CreateTransaction(doc_type_short, group_code, run_no, rev_no, sub_rev, ++status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, operate, true);
                    SendEmailCenter(doc_type_short, group_code, run_no, rev_no, tnc_org.ManagerEMail, operate);
                }
            }
        }

        private string GetEmailSafety()
        {
            string email = "";
            var get_user = from a in dbDC.TM_User
                           where a.utype_id == 6
                           select a;

            foreach (var item in get_user)
            {
                var get_mail = (from a in dbTNC.tnc_user
                                where a.emp_code == item.empcode && a.email != null && a.email != ""
                                select a).FirstOrDefault();
                email += get_mail != null ? "," + get_mail.email : "";
            }
            email = email.Substring(1);
            return email;
        }

        private void SendEmailinRoot(string doc_type_short, string group_code, int run_no, byte rev_no, byte operate)
        {
            var get_root = (from a in dbDC.TD_Transaction
                            where a.doc_type_short == doc_type_short && a.group_code == group_code
                            && a.run_no == run_no && a.rev_no == rev_no && a.operation_id == operate
                            select a.actor).Distinct().ToList();

            var get_mail = from m in dbTNC.tnc_user
                           where get_root.Contains(m.emp_code) && m.email != null && m.email.Trim() != "" && m.emp_status == "A"
                           select m.email;
            if (get_mail.Any())
            {
                var email = "";
                foreach (var item in get_mail)
                {
                    email += "," + item;
                }
                SendEmailCenter(doc_type_short, group_code, run_no, rev_no, email.Substring(1), operate, 3);//3=Complete
            }
        }

        private bool UpdateCheckDate(string doc_type_short, string group_code, int run_no, byte rev_no, DateTime? dt = null)
        {
            using (var localdb = new DocumentControlEntities())
            {
                var query = localdb.TD_Document.Find(doc_type_short, group_code, run_no, rev_no);
                if (query != null)
                {
                    if (dt == null)
                    {
                        byte review_dt = query.TM_DocType.review_year;
                        query.check_date = query.check_date != null ? DateTime.Now.AddYears(review_dt) : query.eff_date.AddYears(review_dt);
                        localdb.SaveChanges();
                    }
                    else
                    {
                        if (query.check_date > dt)
                        {
                            query.check_date = dt;
                            localdb.SaveChanges();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool SetStatusTransaction(string doc_type_short, string group_code, int run_no, byte rev_no, byte current_status, byte new_status)
        {
            var get_old = (from a in dbDC.TD_Transaction
                           where a.status_id == current_status && a.doc_type_short == doc_type_short && a.group_code == group_code
                           && a.run_no == run_no && a.rev_no == (rev_no - 1)
                           select a).FirstOrDefault();
            if (get_old != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SendEmailDistribution(string doc_type_short, string group_code, int run_no, byte rev_no, byte operate)
        {
            var get_dist = from a in dbDC.TD_DistributionList
                           where a.doc_type_short == doc_type_short && a.group_code == group_code
                           && a.run_no == run_no && a.rev_no == rev_no
                           orderby a.group_id ascending
                           select a.group_id;

            foreach (var dgroup in get_dist)
            {
                if (dgroup != 0)
                {
                    var get_mail = from m in dbTNC.tnc_user
                                   where m.emp_group == dgroup && m.email != null && m.email.Trim() != "" && m.emp_status == "A"
                                   select m.email;
                    if (get_mail.Any())
                    {
                        var email = "";
                        foreach (var item in get_mail)
                        {
                            email += "," + item;
                        }
                        SendEmailCenter(doc_type_short, group_code, run_no, rev_no, email.Substring(1), operate, 2);//Controlled
                    }
                }
                else//Select Distribution List All
                {
                    List<byte> send_to_position = new List<byte>() { 1, 2, 3, 5, 7, 10, 13, 21, 22, 23, 24, 25 };
                    //int x = 0;
                    var get_mail = from m in dbTNC.tnc_user
                                   where m.email != null && m.email.Trim() != "" && m.emp_status == "A"
                                   && send_to_position.Contains(m.emp_position.Value) && m.TNCFlage == true
                                   select m.email;//Update Date 2015-03-12 by Monchit W.
                    int i = 0;
                    var email = "";
                    foreach (var item in get_mail)
                    {
                        email += "," + item;
                        if (i < 45)
                        {
                            i++;
                        }
                        else
                        {
                            i = 0;
                            SendEmailCenter(doc_type_short, group_code, run_no, rev_no, email.Substring(1), operate, 2);//Controlled
                            email = "";
                        }
                    }
                    break;
                }
            }
        }

        private string GetEmailDCC(string group_code)
        {
            string email = "";
            var get_response = (from a in dbDC.TM_GroupCode
                                where a.group_code == group_code
                                select a.responsible).FirstOrDefault();
            if (get_response != null)
            {
                var get_mail = (from m in dbTNC.tnc_user
                                where m.emp_code == get_response && m.email != ""
                                select m).FirstOrDefault();
                email = get_mail != null ? get_mail.email : "";
            }
            return email;
        }

        private string GetEmailIssuer(string doc_type_short, string group_code, int run_no, byte rev_no)
        {
            string email = "";
            var get_tran = (from a in dbDC.TD_Transaction
                            where a.doc_type_short == doc_type_short && a.group_code == group_code && a.actor != null
                            && a.run_no == run_no && a.rev_no == rev_no && (a.status_id == 0 || a.status_id == 9)
                            orderby a.sub_rev descending
                            select a).FirstOrDefault();
            if (get_tran != null)
            {
                var get_issuer = dbTNC.tnc_user.Where(w => w.emp_code == get_tran.actor && !string.IsNullOrEmpty(w.email)).Select(s => s.email).FirstOrDefault();
                TNCOrganization tnc_org = new TNCOrganization();
                tnc_org.GetApprover(get_tran.actor);
                email = tnc_org.ManagerEMail + (get_issuer != null ? "," + get_issuer : "");
            }
            return email;
        }

        private string GetEmailbyOrg(int org, byte lv)
        {
            string email = "";
            if (lv == 1) // Eng.
            {
                var get_mail = from a in dbTNC.tnc_user
                               where a.emp_position == 1 && a.emp_group == org && !string.IsNullOrEmpty(a.email)
                               select a.email;

                foreach (var item in get_mail)
                {
                    email += "," + item;
                }
            }
            else if (lv == 2) // Group
            {
                var get_mail = (from a in dbTNC.View_Organization
                                where a.group_id == org
                                select a).FirstOrDefault();

                if (get_mail != null)
                {
                    email = !string.IsNullOrEmpty(get_mail.GroupMgr_email) ? "," + get_mail.GroupMgr_email :
                        !string.IsNullOrEmpty(get_mail.DeptMgr_email) ? "," + get_mail.DeptMgr_email : "," + get_mail.PlantMgr_email;
                }
            }
            else if (lv == 3) // Dept.
            {
                var get_mail = (from a in dbTNC.View_Organization
                                where a.dept_id == org
                                select a).FirstOrDefault();

                if (get_mail != null)
                {
                    email = !string.IsNullOrEmpty(get_mail.DeptMgr_email) ? "," + get_mail.DeptMgr_email : "," + get_mail.PlantMgr_email;
                }
            }
            else if (lv == 4) // Plant/Div
            {
                var get_mail = from a in dbTNC.tnc_user
                               where a.emp_position == 4 && a.emp_plant == org && !string.IsNullOrEmpty(a.email)
                               select a.email;

                foreach (var item in get_mail)
                {
                    email += "," + item;
                }
            }
            else if (lv == 5) // MD
            {
                var get_mail = from a in dbTNC.tnc_user
                               where a.emp_position == 9 && a.emp_plant == org && !string.IsNullOrEmpty(a.email)
                               select a.email;

                foreach (var item in get_mail)
                {
                    email += "," + item;
                }
            }

            return !string.IsNullOrEmpty(email) ? email.Substring(1) : email;
        }

        public bool AddHC(int[] hcGroup, int[] hcQty, string[] hcNote, string hdDocType, string hdGroupCode, int hdRunNo, byte hdRevNo, int cprun)
        {
            try
            {
                if (hcGroup.Length > 0)
                {
                    //int cprun = GetCopyRunNo(hdDocType, hdGroupCode, hdRunNo, hdRevNo);
                    for (int i = 0; i < hcGroup.Length; i++)
                    {
                        TD_Copy cp = new TD_Copy();
                        cp.doc_type_short = hdDocType;
                        cp.group_code = hdGroupCode;
                        cp.note = hcNote[i] != null ? hcNote[i] : string.Empty;
                        cp.run_no = hdRunNo;
                        cp.rev_no = hdRevNo;
                        cp.copy_runno = cprun;
                        cp.group_id = hcGroup[i];
                        cp.qty = hcQty[i];
                        dbDC.TD_Copy.Add(cp);
                    }
                    //dbDC.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            //return RedirectToAction("Inprogress", "Home");
        }

        public void DelHC(string docType, string gCode, int runno, byte revno, int cprun)
        {
            using (var db = new DocumentControlEntities())
            {
                var get_all = from a in db.TD_Copy
                              where a.doc_type_short == docType && a.group_code == gCode && a.run_no == runno
                              && a.rev_no == revno && a.copy_runno == cprun
                              select a.group_id;
                foreach (var item in get_all)
                {
                    TD_Copy copy = db.TD_Copy.Find(docType, gCode, runno, revno, item, cprun);
                    db.TD_Copy.Remove(copy);
                }
                db.SaveChanges();
            }
        }

        public bool AddDistributionList(string docType, string gCode, int runno, byte revno, string list)
        {
            try
            {
                if (!string.IsNullOrEmpty(list))
                {
                    int[] group = list.Split(',').OrderBy(o => o).Select(s => Convert.ToInt32(s)).ToArray();

                    foreach (var item in group)
                    {
                        var check = dbDC.TD_DistributionList.Any(w => w.doc_type_short == docType && w.group_code == gCode
                                                            && w.run_no == runno && w.rev_no == revno && w.group_id == item);
                        if (!check)
                        {
                            var dtb = new TD_DistributionList();
                            dtb.doc_type_short = docType;
                            dtb.group_code = gCode;
                            dtb.run_no = runno;
                            dtb.rev_no = revno;
                            dtb.group_id = item;
                            dbDC.TD_DistributionList.Add(dtb);
                            //dbDC.SaveChanges();
                            if (item == 0) break;//if select group All - add only one
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DelDistributionList(string docType, string gCode, int runno, byte revno)
        {
            var get_all = from a in dbDC.TD_DistributionList
                          where a.doc_type_short == docType && a.group_code == gCode && a.run_no == runno && a.rev_no == revno
                          select a.group_id;
            foreach (var item in get_all)
            {
                TD_DistributionList dist = dbDC.TD_DistributionList.Find(docType, gCode, runno, revno, item);
                dbDC.TD_DistributionList.Remove(dist);
            }
            dbDC.SaveChanges();
        }

        public DateTime ParseToDate(string inputDT)
        {
            char[] delimiters = new char[] { '-', '/', ' ' };
            var temp = inputDT.Split(delimiters);
            DateTime outputDT = DateTime.Parse(temp[2] + "-" + temp[1] + "-" + temp[0]);
            return outputDT;
        }

        public bool CheckHasHardCopy(string docType)
        {
            return dbDC.TM_DocType.Where(w => w.doc_type_short == docType).Select(s => s.copy_flag).FirstOrDefault();
        }

        [HttpGet]
        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public string ShowCopy(string docType)
        {
            var chk = (from a in dbDC.TM_DocType
                       where a.doc_type_short == docType && a.copy_flag == true
                       select a).FirstOrDefault();

            return chk != null ? "show" : "hide";//Update Date 2014-09-05 by Monchit W.
        }

        [HttpGet]
        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult Selecte2AllGroup(string searchTerm)
        {
            var group = dbDC.V_Group
                .Where(w => w.group_name.Contains(searchTerm))
                .OrderBy(o => o.group_name)
                .Select(s => new { id = s.id, text = s.group_name })
                .ToList();
            return Json(group, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult Selecte2TNCGroup(string searchTerm)
        {
            var group = dbTNC.tnc_group_master.Where(w => w.group_name.Contains(searchTerm))
                .OrderBy(o => o.group_name)
                .Select(s => new { id = s.id, text = s.group_name })
                .ToList();
            return Json(group, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult Selecte2ControlDoc(string searchTerm)
        {
            int org = int.Parse(Session["DC_Org"].ToString());
            var control = dbDC.V_Max_Transaction
                .Where(w => w.doc_no.Contains(searchTerm) && w.status_id == 100 && w.org_id == org)
                .OrderBy(o => o.doc_no)
                .Select(a => new { dt = a.doc_type_short, gc = a.group_code, rn = a.run_no, rv = a.rev_no, id = a.doc_no, text = a.doc_name })
                .Take(16).ToList();
            return Json(control, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult CheckDocNo(string docType, string gCode)
        {
            var result = (from a in dbDC.TD_Document
                          where a.doc_type_short == docType && a.group_code == gCode
                          orderby a.run_no descending
                          select a).FirstOrDefault();
            int data = 1;
            if (result != null)
            {
                data = result.run_no + 1;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public int GetCopyRunNo(string doc_type_short, string group_code, int run_no, byte rev_no)
        {
            var query = (from a in dbDC.TD_Copy
                         where a.doc_type_short == doc_type_short && a.group_code == group_code
                         && a.run_no == run_no && a.rev_no == rev_no
                         orderby a.copy_runno descending
                         select a).FirstOrDefault();
            return query != null ? (query.copy_runno + 1) : 0;
        }

        public byte GetSubRevCurrent(string doc_type_short, string group_code, int run_no, byte rev_no, byte operation)
        {
            var query = (from a in dbDC.TD_Transaction
                         where a.doc_type_short == doc_type_short && a.group_code == group_code
                         && a.run_no == run_no && a.rev_no == rev_no && a.operation_id == operation
                         orderby a.sub_rev descending
                         select a).FirstOrDefault();
            return query != null ? query.sub_rev : (byte)0;
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetDistributionList(string doctype, string groupcode, int runno, byte revno, bool lockx = false)
        {
            var get_selected = (from a in dbDC.TD_DistributionList
                                where a.doc_type_short == doctype && a.group_code == groupcode
                                && a.run_no == runno && a.rev_no == revno
                                select a.group_id).ToList();
            //var get_other = dbTNC.tnc_group_master.Where(w => get_selected.Contains(w.id))
            //    .Select(s => new { id = s.id, text = s.group_name });//, locked = true
            var get_other = dbDC.V_Group.Where(w => get_selected.Contains(w.id))
                .Select(s => new { id = s.id, text = s.group_name, locked = lockx });
            if (get_other != null)
                return Json(get_other, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetMyGroup()
        {
            int org = int.Parse(Session["DC_Org"].ToString());
            byte lv = byte.Parse(Session["DC_ULv"].ToString());
            if (lv < 3)
            {
                var get_other = dbDC.V_Group.Where(w => w.id == org)
                    .Select(s => new { id = s.id, text = s.group_name, locked = true });
                if (get_other != null)
                    return Json(get_other, JsonRequestBehavior.AllowGet);
                else
                    return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetGroupByGroupCode(string groupcode)
        {
            var org = (from a in dbDC.TM_GroupCode
                       where a.group_code == groupcode
                       select a.group_id).FirstOrDefault();
            var get_other = dbDC.V_Group.Where(w => w.id == org)
                .Select(s => new { id = s.id, text = s.group_name, locked = true });
            if (get_other != null)
                return Json(get_other, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetCopyList(string doctype, string groupcode, int runno, byte revno, int copy_runno)
        {
            var get_selected = from a in dbDC.TD_Copy.Where(a => a.doc_type_short == doctype && a.group_code == groupcode
                                && a.run_no == runno && a.rev_no == revno && a.copy_runno == copy_runno)
                               join g in dbDC.V_Group on a.group_id equals g.id
                               select new
                               {
                                   g.group_name,
                                   a.group_id,
                                   a.qty,
                                   note = a.note != null ? a.note : ""
                               };

            return Json(get_selected, JsonRequestBehavior.AllowGet);
        }

        public bool CreateTransaction(string doc_type_short, string group_code, int run_no, byte rev_no, byte sub_rev, byte status, byte lv, int org, byte operate, bool active = false, byte action = 0, string actor = null, string comment = null)
        {
            bool exist = dbDC.TD_Transaction.Any(w => w.doc_type_short == doc_type_short && w.group_code == group_code
                && w.run_no == run_no && w.rev_no == rev_no && w.sub_rev == sub_rev && w.status_id == status && w.lv_id == lv
                && w.org_id == org && w.operation_id == operate);

            if (!exist)
            {
                var tdt = new TD_Transaction();
                tdt.doc_type_short = doc_type_short;
                tdt.group_code = group_code;
                tdt.run_no = run_no;
                tdt.rev_no = rev_no;
                tdt.sub_rev = sub_rev;
                tdt.status_id = status;
                tdt.lv_id = lv;
                tdt.org_id = org;
                tdt.operation_id = operate;
                tdt.actor = actor;
                tdt.act_dt = DateTime.Now;
                tdt.comment = comment;
                tdt.active = active;

                if (action != 0) tdt.action_id = action;

                dbDC.TD_Transaction.Add(tdt);
                //dbDC.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        //Must send every Parameters
        public bool UpdateTransaction(string doc_type_short, string group_code, int run_no, byte rev_no, byte sub_rev, byte status, byte lv, int org, byte operate, bool active = false, byte action = 0, string actor = null, string comment = null, bool save = false)
        {
            using (var db = new DocumentControlEntities())
            {
                var tdt = db.TD_Transaction.Find(doc_type_short, group_code, run_no, rev_no, sub_rev, status, lv, org, operate);
                if (tdt != null)
                {
                    tdt.action_id = action;
                    tdt.actor = actor;
                    tdt.comment = comment;
                    tdt.act_dt = DateTime.Now;
                    tdt.active = active;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool SetActiveTransaction(string doc_type_short, string group_code, int run_no, byte rev_no, byte operate = 0, byte status = 0, byte lv = 0, bool active = false)
        {
            var get_tran = from a in dbDC.TD_Transaction
                           where a.doc_type_short == doc_type_short && a.group_code == group_code && a.run_no == run_no
                           && a.rev_no == rev_no && a.active != active
                           select a;

            if (operate != 0)
            {
                get_tran = get_tran.Where(w => w.operation_id == operate);
            }

            if (status != 0)
            {
                get_tran = get_tran.Where(w => w.status_id == status);
            }

            if (lv != 0)
            {
                get_tran = get_tran.Where(w => w.lv_id == lv);
            }

            if (get_tran != null)
            {
                foreach (var item in get_tran)
                {
                    //item.act_dt = DateTime.Now;
                    item.active = active;
                    item.action_id = 6;// 6 is Skip
                }
                dbDC.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult InProgressList(string doc_no, string doc_name, string statusname, byte rev_no = 100, int user = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbDC.V_Transaction
                            where a.active == true && a.status_id != 99 //99 is DCC Confirm
                            select a;

                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());

                if (user == 0)// Default - Wait process
                {
                    query = query.Where(w => w.org_id == org && w.lv_id == lv);
                }
                else if (user == 1)// My Document
                {
                    //var own = Session["DC_Auth"].ToString();
                    if (lv <= 2)
                    {
                        var get_gcode = (from a in dbDC.TM_GroupCode
                                         where a.group_id == org
                                         select a.group_code).ToList();//Add .ToList() faster than no add.
                        query = query.Where(w => get_gcode.Contains(w.group_code));
                        //var owner = (from a in dbDC.TD_Transaction
                        //             where a.status_id == 0 && a.org_id == org && a.lv_id <= 2
                        //             select a).ToList().Select(a => a.doc_type_short + a.group_code + a.run_no.ToString() + a.rev_no.ToString());
                        //query = query.ToList().Where(a => owner.Contains(a.doc_type_short + a.group_code + a.run_no.ToString() + a.rev_no.ToString())).AsQueryable();
                    }
                    //else if (lv == 3)
                    //{
                    //    var groupindept = (from a in dbTNC.tnc_costcentercode
                    //                       where a.dept_id == org && a.group_id != 0
                    //                       select a.group_id).Distinct();
                    //    var owner = (from b in dbDC.TD_Transaction
                    //                 where b.status_id == 0 && groupindept.Contains(b.org_id)
                    //                 select b).ToList().Select(a => a.doc_type_short + a.group_code + a.run_no.ToString() + a.rev_no.ToString());
                    //    query = query.ToList().Where(a => owner.Contains(a.doc_type_short + a.group_code + a.run_no.ToString() + a.rev_no.ToString())).AsQueryable();
                    //}
                    //else if (lv == 4)
                    //{
                    //    var groupinplant = (from a in dbTNC.tnc_costcentercode
                    //                        where a.plant_id == org && a.group_id != 0
                    //                        select a.group_id).Distinct();
                    //    var owner = (from b in dbDC.TD_Transaction
                    //                where b.status_id == 0 && groupinplant.Contains(b.org_id)
                    //                select b).ToList().Select(a => a.doc_type_short + a.group_code + a.run_no.ToString() + a.rev_no.ToString());
                    //    query = query.ToList().Where(a => owner.Contains(a.doc_type_short + a.group_code + a.run_no.ToString() + a.rev_no.ToString())).AsQueryable();
                    //}
                }
                else // Search
                {
                    if (!string.IsNullOrEmpty(doc_no))
                    {
                        query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                    }
                    if (rev_no != 100)
                    {
                        query = query.Where(w => w.rev_no == rev_no);
                    }
                    if (!string.IsNullOrEmpty(doc_name))
                    {
                        query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(statusname))
                    {
                        query = query.Where(w => w.statusname.ToUpper().Contains(statusname.ToUpper()));
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.statusname,
                        s.operation_id,
                        s.operation_name,
                        s.eff_date
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CancelledList(string doc_no, string doc_name, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());
                var query = dbDC.V_Transaction.Where(w => w.status_id == 103);

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }
                if (!string.IsNullOrEmpty(doc_name))
                {
                    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.act_dt
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult IssuerCancelledList(string doc_no, string doc_name, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.V_Transaction.Where(w => w.status_id == 102);

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }
                if (!string.IsNullOrEmpty(doc_name))
                {
                    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.act_dt
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ObsoleteList(string doc_no, string doc_name, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());
                var query = dbDC.V_Transaction.Where(w => w.status_id == 101);

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }
                if (!string.IsNullOrEmpty(doc_name))
                {
                    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ControlledList(string doc_no, string doc_name, byte rev_no = 255, int mode = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dt = DateTime.Now;
                int org = int.Parse(Session["DC_Org"].ToString());
                byte lv = byte.Parse(Session["DC_ULv"].ToString());

                var query = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.eff_date <= dt && w.check_date >= dt)
                            .Select(s => new
                            {
                                s.doc_type_short,
                                s.group_code,
                                s.run_no,
                                s.doc_no,
                                s.rev_no,
                                s.doc_name,
                                s.eff_date,
                                s.org_id,
                                s.check_date
                            });

                if (mode == 0)//Page Load
                {
                    if (lv < 3)
                    {
                        query = query.Where(w => w.org_id == org);
                    }
                    else if (lv == 3)
                    {
                        var groupindept = (from a in dbTNC.tnc_costcentercode
                                           where a.dept_id == org && a.group_id != 0
                                           select a.group_id).Distinct().ToList();
                        query = query.Where(w => groupindept.Contains(w.org_id));
                    }
                    else if (lv >= 4)
                    {
                        var groupinplant = (from a in dbTNC.tnc_costcentercode
                                            where a.plant_id == org && a.group_id != 0
                                            select a.group_id).Distinct().ToList();
                        query = query.Where(w => groupinplant.Contains(w.org_id));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(doc_no))
                    {
                        query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(doc_name))
                    {
                        query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                    }
                    if (rev_no != 255)
                    {
                        query = query.Where(w => w.rev_no == rev_no);
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                //var doctype = from a in dbDC.TM_DocType
                //              where a.doc_lv == 1
                //              select a.doc_type_short;

                // Paging
                var output = query.ToList()
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date,
                        s.check_date,
                        revise = CheckForShowControl(s.doc_type_short, s.group_code, s.run_no, s.rev_no)
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult OverDueList(string doc_no, string doc_name, byte rev_no = 100, int mode = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dt = DateTime.Now;
                int org = int.Parse(Session["DC_Org"].ToString());

                var query = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.eff_date <= dt && w.check_date < dt).ToList()
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date,
                        s.org_id,
                        s.check_date
                    });

                if (mode == 0)//Page Load
                {
                    query = query.Where(w => w.org_id == org);
                }
                else
                {
                    if (!string.IsNullOrEmpty(doc_no))
                    {
                        query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(doc_name))
                    {
                        query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                    }
                    if (rev_no != 100)
                    {
                        query = query.Where(w => w.rev_no == rev_no);
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date,
                        s.check_date,
                        revise = CheckForShowControl(s.doc_type_short, s.group_code, s.run_no, s.rev_no)
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ReqCopyList(string doc_no, string doc_name, int group_id, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dt = DateTime.Now;
                //int org = group_id != -1 ? group_id : int.Parse(Session["DC_Org"].ToString());

                var sub_query = group_id == -1 ? dbDC.TD_DistributionList.ToList()
                                                : dbDC.TD_DistributionList.Where(w => w.group_id == group_id).ToList();

                var query = from a in sub_query // || w.group_id == 0
                            join b in dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && (w.doc_type_short == "WI" || w.doc_type_short == "WP" || w.doc_type_short == "TM" || w.doc_type_short == "RP" || w.doc_type_short == "MSDS" || w.doc_type_short == "OC" || w.doc_type_short == "PC")).ToList()
                            on new { a.doc_type_short, a.group_code, a.run_no, a.rev_no }
                            equals new { b.doc_type_short, b.group_code, b.run_no, b.rev_no }
                            select new
                            {
                                b.doc_type_short,
                                b.group_code,
                                b.run_no,
                                b.doc_no,
                                a.rev_no,
                                b.doc_name,
                                b.eff_date,
                                b.org_id
                            };

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (!string.IsNullOrEmpty(doc_name))
                {
                    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date,
                        yesno = CheckForShowCopy(s.doc_type_short, s.group_code, s.run_no, s.rev_no)
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CompCopyList(string doc_no, string type, int? qty, byte paper = 0, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                //byte lv = byte.Parse(Session["DC_ULv"].ToString());
                //int org = int.Parse(Session["DC_Org"].ToString());
                var query = dbDC.V_TranCopy.Where(w => w.status_id == 104 || w.status_id == 105);

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }
                //if (!string.IsNullOrEmpty(doc_name))
                //{
                //    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                //}
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(w => w.type.ToUpper() == type.ToUpper());
                }
                if (paper != 0)
                {
                    query = query.Where(w => w.paper_id == paper);
                }
                if (qty.HasValue)
                {
                    query = query.Where(w => w.qty == qty.Value);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.req_id,
                        s.type,
                        s.paper_name,
                        s.qty,
                        s.reason,
                        s.status_name,
                        s.act_dt
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private bool CheckForShowCopy(string doctype, string groupcode, int runno, byte revno)
        {
            int org = int.Parse(Session["DC_Org"].ToString());
            var query = (from a in dbDC.TD_DistributionList
                         where a.doc_type_short == doctype && a.group_code == groupcode
                         && a.run_no == runno && a.rev_no == revno && (a.group_id == org || a.group_id == 0)
                         select a).Any();
            return query;
        }

        [HttpPost]
        public JsonResult ControlledShowList(string doc_no, string doc_name, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dt = DateTime.Now;
                var query = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.eff_date <= dt)
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date
                    });

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (!string.IsNullOrEmpty(doc_name))
                {
                    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DCCList(string doc_no, string doc_name, byte rev_no = 100, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.V_Transaction.Where(w => w.status_id == 99 && w.active == true);

                if (!string.IsNullOrEmpty(doc_no))
                {
                    query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                }
                if (rev_no != 100)
                {
                    query = query.Where(w => w.rev_no == rev_no);
                }
                if (!string.IsNullOrEmpty(doc_name))
                {
                    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult FutureUseList(string doc_no, string doc_name, byte rev_no = 100, int mode = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dt = DateTime.Now;
                int org = int.Parse(Session["DC_Org"].ToString());
                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                var query = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.eff_date > dt);

                if (mode == 0)
                {
                    if (lv < 3)
                    {
                        query = query.Where(w => w.org_id == org);
                    }
                    else if (lv == 3)
                    {
                        var groupindept = (from a in dbTNC.tnc_costcentercode
                                           where a.dept_id == org && a.group_id != 0
                                           select a.group_id).Distinct().ToList();
                        query = query.Where(w => groupindept.Contains(w.org_id));
                    }
                    else if (lv >= 4)
                    {
                        var groupinplant = (from a in dbTNC.tnc_costcentercode
                                            where a.plant_id == org && a.group_id != 0
                                            select a.group_id).Distinct().ToList();
                        query = query.Where(w => groupinplant.Contains(w.org_id));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(doc_no))
                    {
                        query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                    }
                    if (rev_no != 100)
                    {
                        query = query.Where(w => w.rev_no == rev_no);
                    }
                    if (!string.IsNullOrEmpty(doc_name))
                    {
                        query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private bool CheckForRevise(string doctype, string groupcode, int runno, byte revno)
        {
            //byte lv = byte.Parse(Session["DC_ULv"].ToString());
            int org = int.Parse(Session["DC_Org"].ToString());

            //1. Check Lastest Revision
            //2. Check Organize of Issuer
            var get_max = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && w.org_id == org).OrderByDescending(o => o.rev_no).FirstOrDefault();

            //3. Check New revision in-progress
            var get_new = dbDC.TD_Transaction.Where(w => w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && w.rev_no == (revno + 1) && w.active == true);

            if (get_max != null && get_max.rev_no == revno && !get_new.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckForCancel(string doctype, string groupcode, int runno, byte revno)
        {
            //byte lv = byte.Parse(Session["DC_ULv"].ToString());
            //int org = int.Parse(Session["DC_Org"].ToString());

            //1. Check Lastest Revision
            //2. Check Organize of Issuer
            //var get_max = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && w.org_id == org).OrderByDescending(o => o.rev_no).FirstOrDefault();

            //3. Check Cancel in-progress
            var inprogress = dbDC.TD_Transaction.Where(w => w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && w.rev_no == revno && w.status_id != 99 && w.active == true);

            if (!inprogress.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckForReview(string doctype, string groupcode, int runno, byte revno)
        {
            //byte lv = byte.Parse(Session["DC_ULv"].ToString());
            //int org = int.Parse(Session["DC_Org"].ToString());

            //1. Check Lastest Revision
            //2. Check Organize of Issuer
            //var get_max = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && w.org_id == org).OrderByDescending(o => o.rev_no).FirstOrDefault();

            //3. Check Cancel in-progress
            var inprogress = dbDC.TD_Transaction.Where(w => w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && w.rev_no == revno && w.status_id != 99 && w.active == true);

            if (!inprogress.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckForShowControl(string doctype, string groupcode, int runno, byte revno)
        {
            int org = int.Parse(Session["DC_Org"].ToString());
            byte lv = byte.Parse(Session["DC_ULv"].ToString());

            if (lv > 3) { return false; }

            var groupindept = (from a in dbTNC.tnc_costcentercode
                               where a.dept_id == org && a.group_id != 0
                               select a.group_id).Distinct().ToList();
            //query = query.Where(w => groupindept.Contains(w.org_id));

            //1. Check Lastest Revision
            //2. Check Organize of Issuer
            var get_max = dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.doc_type_short == doctype
                && w.group_code == groupcode && w.run_no == runno && (w.org_id == org || groupindept.Contains(w.org_id))).OrderByDescending(o => o.rev_no).Select(s => s.rev_no);//.FirstOrDefault()

            //4. Check in-progress
            var get_new = dbDC.TD_Transaction.Where(w => w.doc_type_short == doctype && w.group_code == groupcode
                && w.run_no == runno && w.active == true && w.status_id != 99);//99=DCC Confirm

            if (get_max.Any() && get_max.First() == revno && !get_new.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult ApproveDoc(HttpPostedFileBase pathVDO)
        {
            var actor = Session["DC_Auth"].ToString();
            if (!string.IsNullOrEmpty(actor))
            {
                byte action = Request.Form["selDecision"] == "1" ? (byte)2 : (byte)3;// 2=Approve, 3=Reject

                string docType = Request.Form["hdDT"];
                string gCode = Request.Form["hdGC"];
                int runno = int.Parse(Request.Form["hdRN"].ToString());
                byte revno = byte.Parse(Request.Form["hdRV"].ToString());
                byte subrev = byte.Parse(Request.Form["hdSR"].ToString());
                byte level = byte.Parse(Request.Form["hdLV"].ToString());
                byte status = byte.Parse(Request.Form["hdST"].ToString());
                byte oper = byte.Parse(Request.Form["hdOP"].ToString());
                int org = int.Parse(Request.Form["hdOR"].ToString());
                string comment = Request.Form["txaComment"];

                if (pathVDO != null && pathVDO.ContentLength > 0)
                {
                    UpdateVDOPath(docType, gCode, runno, revno, pathVDO.FileName);
                }

                if ((oper == 3 || oper == 4) && action == 3)
                {
                    UpdateTransaction(docType, gCode, runno, revno, subrev, status, level, org, oper, false, action, actor, comment, true);
                    SetActiveTransaction(docType, gCode, runno, revno, oper);
                }
                else
                {
                    UpdateTransaction(docType, gCode, runno, revno, subrev, status, level, org, oper, false, action, actor, comment, true);
                    GetNextTransaction(docType, gCode, runno, revno, subrev, status, level, org, oper, action, actor);
                }

                if (action == 3)//Reject send to Issuer
                {
                    SendEmailCenter(docType, gCode, runno, revno, GetEmailIssuer(docType, gCode, runno, revno), oper, 1, comment);//1=Reject
                }

                dbDC.SaveChanges();

                TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
                return RedirectToAction("Inprogress", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void UpdateVDOPath(string docType, string gCode, int runno, byte revno, string p)
        {
            using (var localdb = new DocumentControlEntities())
            {
                var query = localdb.TD_Document.Find(docType, gCode, runno, revno);
                if (query != null)
                {
                    //string dir = Path.GetDirectoryName(p).Replace(@"\", "/");
                    //query.vdo_path = "file:" + dir;
                    query.vdo_path = Path.GetDirectoryName(p);
                    localdb.SaveChanges();
                }
            }
        }

        [HttpPost]
        public ActionResult CancelDoc()
        {
            string comment = Request.Form["txaReason"];
            string docType = Request.Form["hdDT"];
            string gCode = Request.Form["hdGC"];
            int runno = int.Parse(Request.Form["hdRN"].ToString());
            byte revno = byte.Parse(Request.Form["hdRV"].ToString());
            var actor = Session["DC_Auth"].ToString();
            byte lv = byte.Parse(Session["DC_ULv"].ToString());
            int org = int.Parse(Session["DC_Org"].ToString());
            byte status = 0;
            byte operate = 3;
            byte sub_rev = GetSubRevCurrent(docType, gCode, runno, revno, operate);
            byte action = 1;
            var cancel_date = Request.Form["dtExp"] != null ? ParseToDate(Request.Form["dtExp"].ToString()) : DateTime.Now;
            UpdateCancelDate(docType, gCode, runno, revno, cancel_date);
            GetNextTransaction(docType, gCode, runno, revno, ++sub_rev, status, lv, org, operate, action, actor, comment);
            dbDC.SaveChanges();
            TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
            return RedirectToAction("Inprogress", "Home");
        }

        [HttpPost]
        public ActionResult ReviewDoc()
        {
            string comment = Request.Form["txaComment"];
            string docType = Request.Form["hdDT"];
            string gCode = Request.Form["hdGC"];
            int runno = int.Parse(Request.Form["hdRN"].ToString());
            byte revno = byte.Parse(Request.Form["hdRV"].ToString());
            var actor = Session["DC_Auth"].ToString();
            byte lv = byte.Parse(Session["DC_ULv"].ToString());
            int org = int.Parse(Session["DC_Org"].ToString());
            byte status = 0;
            byte operate = 4;
            byte sub_rev = GetSubRevCurrent(docType, gCode, runno, revno, operate);
            byte action = 1;
            //var cancel_date = Request.Form["dtExp"] != null ? ParseToDate(Request.Form["dtExp"].ToString()) : DateTime.Now;
            //UpdateCancelDate(docType, gCode, runno, revno, cancel_date);
            GetNextTransaction(docType, gCode, runno, revno, ++sub_rev, status, lv, org, operate, action, actor, comment);
            dbDC.SaveChanges();

            TNCOrganization tnc_org = new TNCOrganization();
            tnc_org.GetApprover(actor);
            SendEmailCenter(docType, gCode, runno, revno, tnc_org.ManagerEMail, operate);

            TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
            return RedirectToAction("OverDueReview", "Home");
        }

        [HttpPost]
        public ActionResult ReqCopyDoc()
        {
            string docType = Request.Form["hdDT"];
            string gCode = Request.Form["hdGC"];
            int runno = int.Parse(Request.Form["hdRN"].ToString());
            byte revno = byte.Parse(Request.Form["hdRV"].ToString());
            int qty = int.Parse(Request.Form["txtQty"].ToString());
            string type = Request.Form["selPaper"];
            byte paper = byte.Parse(Request.Form["selPaper"].ToString());
            string comment = Request.Form["txaReason"];

            TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " request copy success";

            return RedirectToAction("Controlled", "Home");
        }

        private void UpdateCancelDate(string docType, string gCode, int runno, byte revno, DateTime cancel_date)
        {
            TD_Document doc = dbDC.TD_Document.Find(docType, gCode, runno, revno);
            doc.expect_cancel_date = cancel_date;
            //dbDC.SaveChanges();
        }

        public ActionResult ConfirmDoc(string dt, string gc, int rn, byte rv)
        {
            var actor = Session["DC_Auth"].ToString();
            var get_tran = (from a in dbDC.TD_Transaction
                            where a.doc_type_short == dt && a.group_code == gc && a.run_no == rn && a.rev_no == rv
                            && a.status_id == 99 && a.active == true
                            select a).FirstOrDefault();
            if (get_tran != null)
            {
                get_tran.action_id = 5;//Completed
                get_tran.actor = actor;
                get_tran.act_dt = DateTime.Now;
                get_tran.active = false;
                dbDC.SaveChanges();
            }

            var get_group = from a in dbDC.TD_Copy
                            where a.doc_type_short == dt && a.group_code == gc && a.run_no == rn && a.rev_no == rv
                            select a.group_id;

            foreach (var item in get_group)
            {
                SendEmailCenter(dt, gc, rn, rv, "", 0, 4);
            }

            return RedirectToAction("DCCConfirm", "Home");
        }

        [HttpPost]
        public ActionResult IssuerCancelDoc()
        {
            string comment = Request.Form["txaReason"];
            string docType = Request.Form["hdDT"];
            string gCode = Request.Form["hdGC"];
            int runno = int.Parse(Request.Form["hdRN"].ToString());
            byte revno = byte.Parse(Request.Form["hdRV"].ToString());
            var actor = Session["DC_Auth"].ToString();
            byte lv = byte.Parse(Session["DC_ULv"].ToString());
            int org = int.Parse(Session["DC_Org"].ToString());
            byte status = 0;
            byte operate = 6;
            byte sub_rev = GetSubRevCurrent(docType, gCode, runno, revno, operate);
            byte action = 1;
            UpdateTransaction(docType, gCode, runno, revno, sub_rev, 9, lv, org, operate, false, 1, actor);
            GetNextTransaction(docType, gCode, runno, revno, sub_rev, status, lv, org, operate, action, actor, comment);
            //CreateTransaction(doctype, groupcode, runno, revno, 0, 8, lv, org, 6);
            dbDC.SaveChanges();
            TempData["result"] = "Document No. " + docType + "-" + gCode + "-" + runno.ToString("0000") + " rev." + revno.ToString("00") + " action success";
            return RedirectToAction("Inprogress", "Home");
        }

        public void SendEmailCenter(string doc_type_short, string group_code, int run_no, byte rev_no,
                                    string receiver, byte operate, int type = 0, string content = "")
        {
            if (!string.IsNullOrEmpty(receiver))
            {
                string mailto = "";
                char[] delimiter = new char[] { ',' };
                string[] email = receiver.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int max_email = 47;
                if (email.Length > max_email)//Max send email = 50
                {
                    for (int i = 0; i < max_email; i++)
                    {
                        mailto += "," + email[i];
                    }
                    mailto = mailto.Substring(1);
                }
                else
                {
                    mailto = receiver;
                }

                //var get_doc = (from a in dbDC.V_Transaction
                //               where a.doc_type_short == doc_type_short && a.group_code == group_code
                //               && a.run_no == run_no && a.rev_no == rev_no
                //               select a).FirstOrDefault();

                var get_doc = dbDC.TD_Document.Find(doc_type_short, group_code, run_no, rev_no);
                var doc_no = doc_type_short + "-" + group_code + "-" + run_no.ToString("0000") + "-" + rev_no.ToString("00");

                var get_operate = (from o in dbDC.TM_Operation
                                   where o.operation_id == operate
                                   select o.operation_name).FirstOrDefault();

                if (get_doc != null)
                {
                    TNCUtility tnc_util = new TNCUtility();
                    string subject = "";
                    string body = "";//For Real
                    //string body = "Mail :" + mailto + "<br />";//For Test
                    string int_link = "http://webExternal/DocControl";
                    string ext_link = "http://webexternal.nok.co.th/DocControl";
                    string Param = "/Home/ShowDocDetail?dt=" + get_doc.doc_type_short + "&gc=" + get_doc.group_code +
                                    "&rn=" + get_doc.run_no + "&rv=" + get_doc.rev_no;
                    if (type == 0)//Default Approve
                    {
                        subject = get_operate + " Doc. wait for Approve (" + doc_no + ")";
                        body += "Dear. All Concern,<br /><br />" +
                            "<b>Document No. : </b>" + doc_no + "<br />" +
                            "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                            "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "/Home/Inprogress'>Internal</a> | <a href='"
                            + ext_link + "/Home/Inprogress'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    else if (type == 1)//Email Reject for issuer edit
                    {
                        subject = "Reject (" + doc_no + ")";
                        body += "Dear. All Concern,<br /><br />" +
                            "<b>Document No. : </b>" + doc_no + "<br />" +
                            "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                            "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                            "<b>Reason : </b>" + content + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "'>Internal</a> | <a href='" + ext_link + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    else if (type == 2)//Controlled
                    {
                        subject = "Controlled (" + doc_no + ")";
                        body += "Dear. All Concern,<br /><br />" +
                            "<b>Document No. : </b>" + doc_no + "<br />" +
                            "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                            "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />";
                        if (get_doc.attach_file != null && get_doc.attach_file != "")
                        {
                            body += "<b>You can see document at link ---> </b><a href='" + int_link + get_doc.attach_file.Substring(1) + "'>View</a><br />";
                        }

                        body += "<b>Link : </b><a href='" + int_link + Param + "'>Internal</a> | <a href='" + ext_link + Param + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    else if (type == 3)//Complete
                    {
                        subject = "Complete (" + doc_no + ")";
                        body += "Dear. All Concern,<br /><br />" +
                            "<b>Document No. : </b>" + doc_no + "<br />" +
                            "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                            "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                            "<b>Link : </b><a href='" + int_link + Param + "'>Internal</a> | <a href='" + ext_link + Param + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }

                    tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", mailto, subject, body);//For Real
                    //tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", "monchit@nok.co.th", subject, body);//For Test
                }
            }
        }

        public void SendEmailCopy(int req_id, string receiver, int type = 0, string content = "")
        {
            if (!string.IsNullOrEmpty(receiver))
            {
                string mailto = "";
                char[] delimiter = new char[] { ',' };
                string[] email = receiver.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int max_email = 47;
                if (email.Length > max_email)//Max send email = 50
                {
                    for (int i = 0; i < max_email; i++)
                    {
                        mailto += "," + email[i];
                    }
                    mailto = mailto.Substring(1);
                }
                else
                {
                    mailto = receiver;
                }

                var get_doc = (from a in dbDC.V_ReqCopy
                               where a.req_id == req_id
                               select a).FirstOrDefault();

                if (get_doc != null)
                {
                    TNCUtility tnc_util = new TNCUtility();
                    string subject = "";
                    string body = "";//For Real
                    //string body = "Mail :" + mailto + "<br />";//For Test
                    string int_link = "http://webExternal/DocControl";
                    string ext_link = "http://webexternal.nok.co.th/DocControl";
                    //string Param = "/Home/InprogressCopy";
                    if (type == 0)//Default Approve
                    {
                        subject = "Doc. wait for Approve (" + get_doc.doc_no + ")";
                        body += "Dear. All Concern,<br /><br />" +
                            "<b>Document No. : </b>" + get_doc.doc_no + "<br />" +
                            "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                            "<b>Type : </b>" + get_doc.type + "<br />" +
                            "<b>Paper : </b>" + get_doc.paper_name + "<br />" +
                            "<b>Q'ty : </b>" + get_doc.qty + "<br />" +
                            "<b>Reason : </b>" + get_doc.reason + "<br />" +
                            "<b>Link : </b><a href='" + int_link + "/Home/InprogressCopy'>Internal</a> | <a href='"
                            + ext_link + "/Home/InprogressCopy'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    }
                    //else if (type == 1)//Email Reject for issuer edit
                    //{
                    //    subject = "Reject (" + get_doc.doc_no + ")";
                    //    body = "Dear. All Concern,<br /><br />" +
                    //        "<b>Document No. : </b>" + get_doc.doc_no + "<br />" +
                    //        "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                    //        "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                    //        "<b>Reason : </b>" + content + "<br />" +
                    //        "<b>Link : </b><a href='" + int_link + "'>Internal</a> | <a href='" + ext_link + "'>External</a><br />" +
                    //        "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    //}
                    //else if (type == 2)//Controlled
                    //{
                    //    subject = "Controlled (" + get_doc.doc_no + ")";
                    //    body = "Dear. All Concern,<br /><br />" +
                    //        "<b>Document No. : </b>" + get_doc.doc_no + "<br />" +
                    //        "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                    //        "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                    //        "<b>You can see document at link ---> </b><a href='" + int_link + get_doc.attach_file.Substring(1) + "'>View</a><br />" +
                    //        "<b>Link : </b><a href='" + int_link + Param + "'>Internal</a> | <a href='" + ext_link + Param + "'>External</a><br />" +
                    //        "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    //}
                    //else if (type == 3)//Complete
                    //{
                    //    subject = "Complete (" + get_doc.doc_no + ")";
                    //    body = "Dear. All Concern,<br /><br />" +
                    //        "<b>Document No. : </b>" + get_doc.doc_no + "<br />" +
                    //        "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                    //        "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                    //        "<b>Link : </b><a href='" + int_link + Param + "'>Internal</a> | <a href='" + ext_link + Param + "'>External</a><br />" +
                    //        "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";
                    //}

                    tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", mailto, subject, body);//For Real
                    //tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", "monchit@nok.co.th", subject, body);//For Test
                }
            }
        }

        [HttpPost]
        public JsonResult CopyDoc()
        {
            try
            {
                string docType = Request.Form["doctype"];
                string gCode = Request.Form["groupcode"];
                var runno = int.Parse(Request.Form["runno"].ToString());
                var revno = byte.Parse(Request.Form["revno"].ToString());
                var qty = int.Parse(Request.Form["qty"].ToString());
                var type = Request.Form["type"].ToString();
                var size = byte.Parse(Request.Form["size"].ToString());
                string reason = Request.Form["reason"];

                TD_ReqCopy copy = new TD_ReqCopy();
                copy.doc_type_short = docType;
                copy.group_code = gCode;
                copy.run_no = runno;
                copy.rev_no = revno;
                copy.qty = qty;
                copy.type = type;
                copy.paper_id = size;
                copy.reason = reason;

                dbDC.TD_ReqCopy.Add(copy);
                dbDC.SaveChanges();

                var actor = Session["DC_Auth"].ToString();

                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());
                //TNCOrganization tnc_org = new TNCOrganization();
                //tnc_org.GetApprover(actor);
                byte action = 1;//1=Issue
                GetNextTranCopy(copy.req_id, 0, lv, org, action, actor);

                return Json("Request Copy Complete");
            }
            catch (Exception ex)
            {
                return Json("Error:" + ex);
            }
        }

        [HttpPost]
        public ActionResult ApproveCopy()
        {
            var actor = Session["DC_Auth"].ToString();
            if (!string.IsNullOrEmpty(actor))
            {
                byte action = byte.Parse(Request.Form["selDecision"].ToString());

                int req_id = int.Parse(Request.Form["hdREQ"].ToString());
                byte level = byte.Parse(Request.Form["hdLV"].ToString());
                byte status = byte.Parse(Request.Form["hdST"].ToString());
                int org = int.Parse(Request.Form["hdOR"].ToString());
                string comment = Request.Form["txaComment"];

                GetNextTranCopy(req_id, status, level, org, action, actor);

                dbDC.SaveChanges();
            }
            return RedirectToAction("InprogressCopy", "Home");
        }

        public ActionResult InprogressCopy()
        {
            //ViewBag.Menu = 12;
            ViewBag.ULv = byte.Parse(Session["DC_ULv"].ToString());
            var paper = from a in dbDC.TM_Paper
                        select a;
            ViewBag.Paper = paper;
            return View();
        }

        [HttpPost]
        public ActionResult InProgressCopyList(string doc_no, string statusname, string type, int? qty, byte paper = 0, byte rev_no = 100, int user = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbDC.V_TranCopy
                            where a.active == true
                            select a;

                byte lv = byte.Parse(Session["DC_ULv"].ToString());
                int org = int.Parse(Session["DC_Org"].ToString());

                bool show_print = Session["DC_UType"].ToString() != "0" ? true : false;

                if (user == 0)// Default - Wait process
                {
                    query = query.Where(w => w.org_id == org && w.lv_id == lv);
                }
                else if (user == 1)// My Document
                {
                    var own = Session["DC_Auth"].ToString();
                    if (lv <= 2)
                    {
                        var owner = (from a in dbDC.TD_TranCopy
                                     where a.status_id == 0 && a.org_id == org && a.lv_id <= 2
                                     select a).ToList().Select(a => a.req_id);
                        query = query.ToList().Where(a => owner.Contains(a.req_id)).AsQueryable();
                    }
                }
                else // Search
                {
                    if (!string.IsNullOrEmpty(doc_no))
                    {
                        query = query.Where(w => w.doc_no.ToUpper().Contains(doc_no.ToUpper()));
                    }
                    if (rev_no != 100)
                    {
                        query = query.Where(w => w.rev_no == rev_no);
                    }
                    //if (!string.IsNullOrEmpty(doc_name))
                    //{
                    //    query = query.Where(w => w.doc_name.ToUpper().Contains(doc_name.ToUpper()));
                    //}
                    if (!string.IsNullOrEmpty(statusname))
                    {
                        query = query.Where(w => w.statusname.ToUpper().Contains(statusname.ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(type))
                    {
                        query = query.Where(w => w.type.ToUpper() == type.ToUpper());
                    }
                    if (paper != 0)
                    {
                        query = query.Where(w => w.paper_id == paper);
                    }
                    if (qty.HasValue)
                    {
                        query = query.Where(w => w.qty == qty.Value);
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.group_code,
                        s.run_no,
                        s.doc_no,
                        s.rev_no,
                        s.req_id,
                        s.type,
                        s.paper_name,
                        s.qty,
                        s.reason,
                        //s.doc_name,
                        s.statusname,
                        show_print
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private void InsertUpdateTranCopy(int req_id, byte status, byte lv, int org, bool active = false, byte? action = null, string actor = null)
        {
            var dtnow = DateTime.Now;
            var exist = dbDC.TD_TranCopy.Find(req_id, status, lv, org);
            if (exist == null)//Insert //no row -> insert
            {
                TD_TranCopy tcp = new TD_TranCopy();
                tcp.req_id = req_id;
                tcp.status_id = status;
                tcp.lv_id = lv;
                tcp.org_id = org;
                tcp.active = active;
                tcp.action_id = action;
                tcp.actor = actor;
                tcp.act_dt = dtnow;

                dbDC.TD_TranCopy.Add(tcp);
            }
            else//Update //has row and active is true -> update
            {
                if (dbDC.TD_TranCopy.Any(w => w.req_id == req_id && w.status_id == status
                    && w.lv_id == lv && w.org_id == org && w.active == true))
                {
                    exist.active = active;
                    exist.action_id = action;
                    exist.actor = actor;
                    exist.act_dt = dtnow;
                }
            }
            dbDC.SaveChanges();
        }

        private void GetNextTranCopy(int req_id, byte status, byte lv, int org, byte action, string actor)
        {
            TNCOrganization tnc_org = new TNCOrganization();
            tnc_org.GetApprover(actor);

            if (action == 1) // Issue
            {
                // Issue TranCopy
                InsertUpdateTranCopy(req_id, status, lv, org, false, action, actor);
                InsertUpdateTranCopy(req_id, ++status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true);
                SendEmailCopy(req_id, tnc_org.ManagerEMail);
            }
            else if (action == 2) // Approve
            {
                InsertUpdateTranCopy(req_id, status, lv, org, false, action, actor);
                if (status == 1)//Issuer
                {
                    //InsertUpdateTranCopy(req_id, 3, 2, 18, true);//3=DCC, 18=QS Group
                    InsertUpdateTranCopy(req_id, 99, 1, 18, true);//99=DCC Confirm, 18=QS Group
                }
                //else if (status == 3)//DCC Manager
                //{
                //    InsertUpdateTranCopy(req_id, 99, 1, 18, true);//99=DCC Confirm, 18=QS Group
                //}
                else if (status == 99)//DCC Staff
                {
                    var get_issue_tran = (from a in dbDC.TD_TranCopy
                                          where a.req_id == req_id && a.status_id == 0
                                          select a).FirstOrDefault();
                    InsertUpdateTranCopy(req_id, 105, get_issue_tran.lv_id, get_issue_tran.org_id, false, 1);
                }
                //else if (status == 106)//Issuer receive //Add 2017-01-10 by Monchit
                //{
                //    InsertUpdateTranCopy(req_id, 105, get_issue_tran.lv_id, get_issue_tran.org_id, false, 1);
                //}
            }
            else if (action == 3) // Reject
            {
                InsertUpdateTranCopy(req_id, status, lv, org, false, action, actor);
                InsertUpdateTranCopy(req_id, 104, lv, org, false, 1);//104=Rejected
            }
            //    scope.Complete();
            //}
        }

        [HttpPost]
        public ActionResult UpdateDist(string doctype, string groupcode, int runno, byte revno, string dist)
        {
            try
            {
                if (!string.IsNullOrEmpty(dist))
                {
                    if (AddDistributionList(doctype, groupcode, runno, revno, dist))
                    {
                        dbDC.SaveChanges();
                    }
                }
                return Json("Update Distribution List completed");
            }
            catch (Exception)
            {
                return Json("Update Distribution List not success");
            }
        }

        private static string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileName(files[i]);
            return files;
        }

        protected override void Dispose(bool disposing)
        {
            dbDC.Dispose();
            base.Dispose(disposing);
        }

        //public ActionResult RejectDoc()
        //{
        //    var actor = Session["DC_Auth"].ToString();
        //    string docType = Request.Form["hdDT"];
        //    string gCode = Request.Form["hdGC"];
        //    int runno = int.Parse(Request.Form["hdRN"].ToString());
        //    byte revno = byte.Parse(Request.Form["hdRV"].ToString());
        //    byte subrev = byte.Parse(Request.Form["hdSR"].ToString());
        //    byte level = byte.Parse(Request.Form["hdLV"].ToString());
        //    byte status = byte.Parse(Request.Form["hdST"].ToString());
        //    byte oper = byte.Parse(Request.Form["hdOP"].ToString());
        //    int org = int.Parse(Request.Form["hdOR"].ToString());
        //    GetNextTransaction(docType, gCode, runno, revno, subrev, status, level, org, oper, 3, actor);
        //    return RedirectToAction("Inprogress", "Home");
        //}

        //private string DisplayDocNo(string doc_type_short, string group_code, int run_no)
        //{
        //    return doc_type_short.Trim() + "-" + group_code.Trim() + "-" + run_no.ToString("0000");
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult Login(string id)
        //{
        //    getUserInfo user = new getUserInfo();
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        string empcode = TNCCrypto.Decode(id);
        //        if (user.setUser(empcode))
        //        {
        //            Session["DC_Auth"] = empcode;
        //            Session["DC_Name"] = user.Emp_fname + " " + user.Emp_lname;
        //            //Session["DC_Org"] = user.GroupID;
        //            var sa_user = (from u in dbDC.TM_Admin
        //                           where u.emp_code == empcode
        //                           select u).FirstOrDefault();
        //            Session["DC_UType"] = sa_user != null ? sa_user.utype_id : 0;

        //            var position_lv = (from a in dbTNCAdmin.tnc_position_master
        //                               where a.id == user.Emp_position
        //                               select a.position_level).FirstOrDefault();
        //            if (position_lv != null)
        //            {
        //                var sa_lv = (from b in dbDC.TM_Level
        //                             where b.position_min <= position_lv && b.position_max >= position_lv
        //                             select b.lv_id).FirstOrDefault();
        //                Session["DC_UserLv"] = sa_lv;
        //                if (sa_lv <= 2)
        //                {
        //                    Session["DC_Org"] = user.GroupID;
        //                }
        //                else if (sa_lv == 3)
        //                {
        //                    Session["DC_Org"] = user.DeptID;
        //                }
        //                else if (sa_lv == 4)
        //                {
        //                    Session["DC_Org"] = user.PlantID;
        //                }
        //            }

        //            if (Session["Redirect"] != null)
        //            {
        //                string url = Session["Redirect"].ToString();
        //                Session.Remove("Redirect");
        //                return Redirect(url);
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "DCProcess");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
    }
}