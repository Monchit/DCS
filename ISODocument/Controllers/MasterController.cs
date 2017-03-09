using ISODocument.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace ISODocument.Controllers
{
    public class MasterController : Controller
    {
        DocumentControlEntities dbDC = new DocumentControlEntities();
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

        [Check_Authen]
        [Check_Authorize_SysAdmin]
        public ActionResult Users()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_SysAdmin]
        public ActionResult UserType()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult Status()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult GroupCode()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult DocType()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult Reviewer()
        {
            //ViewBag.Menu = 9;
            var get_group = from a in dbTNC.tnc_group_master
                            orderby a.group_name
                            select a;
            ViewBag.AllGroup = get_group;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult Action()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult Operation()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult Level()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult Paper()
        {
            //ViewBag.Menu = 9;
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult ChangeRoute()
        {
            return View();
        }

        [Check_Authen]
        [Check_Authorize_Admin]
        public ActionResult EditDoc()
        {
            ViewBag.DocType = dbDC.TM_DocType;
            ViewBag.GroupCode = dbDC.TM_GroupCode;
            return View();
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult UserList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_User;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.empcode,
                        s.utype_id
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
        public JsonResult CreateUser()
        {
            try
            {
                var formData = Request.Form["empcode"].ToString();
                var dbData = dbDC.TM_User.Where(w => w.empcode == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_User data = new TM_User();
                    data.empcode = formData;
                    data.utype_id = byte.Parse(Request.Form["utype_id"]);
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_User.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_User.OrderByDescending(o=> o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateUser()
        {
            try
            {
                var data = dbDC.TM_User.Find(Request.Form["empcode"].ToString());
                data.utype_id = byte.Parse(Request.Form["utype_id"]);
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteUser()
        {
            try
            {
                var data = dbDC.TM_User.Find(Request.Form["empcode"].ToString());
                dbDC.TM_User.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult UTypeList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_UserType;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.utype_id,
                        s.utype_name
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
        public JsonResult CreateUType()
        {
            try
            {
                var formData = byte.Parse(Request.Form["utype_id"].ToString());
                var dbData = dbDC.TM_UserType.Where(w => w.utype_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_UserType data = new TM_UserType();
                    data.utype_id = formData;
                    data.utype_name = Request.Form["utype_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_UserType.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_UserType.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateUType()
        {
            try
            {
                var id = byte.Parse(Request.Form["utype_id"].ToString());
                var data = dbDC.TM_UserType.Find(id);
                data.utype_name = Request.Form["utype_name"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteUType()
        {
            try
            {
                var id = byte.Parse(Request.Form["utype_id"].ToString());
                var data = dbDC.TM_UserType.Find(id);
                dbDC.TM_UserType.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult StatusList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_Status;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.status_id,
                        s.status_name
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
        public JsonResult CreateStatus()
        {
            try
            {
                var formData = byte.Parse(Request.Form["status_id"].ToString());
                var dbData = dbDC.TM_Status.Where(w => w.status_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Status data = new TM_Status();
                    data.status_id = formData;
                    data.status_name = Request.Form["status_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_Status.Add(data);
                }

                dbDC.SaveChanges();
                //Creating method must return the new created object as Record property.
                return Json(new { Result = "OK", Record = dbDC.TM_Status.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateStatus()
        {
            try
            {
                var id = byte.Parse(Request.Form["status_id"].ToString());
                var data = dbDC.TM_Status.Find(id);
                data.status_name = Request.Form["status_name"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteStatus()
        {
            try
            {
                var id = byte.Parse(Request.Form["status_id"].ToString());
                var data = dbDC.TM_Status.Find(id);
                dbDC.TM_Status.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult GCodeList(string code, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbDC.TM_GroupCode select a;

                if (!string.IsNullOrEmpty(code))
                {
                    query = query.Where(w => w.group_code.Contains(code));
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query.OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize)
                    .Select(s => new
                    {
                        s.group_code,
                        s.group_id,
                        s.responsible
                    });

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateGCode()
        {
            try
            {
                var formData = Request.Form["group_code"].ToString();
                var dbData = dbDC.TM_GroupCode.Where(w => w.group_code == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_GroupCode data = new TM_GroupCode();
                    data.group_code = formData;
                    data.group_id = int.Parse(Request.Form["group_id"].ToString());
                    data.responsible = Request.Form["responsible"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_GroupCode.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_GroupCode.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGCode()
        {
            try
            {
                var id = Request.Form["group_code"].ToString();
                var data = dbDC.TM_GroupCode.Find(id);
                data.group_id = int.Parse(Request.Form["group_id"].ToString());
                data.responsible = Request.Form["responsible"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGCode()
        {
            try
            {
                var id = Request.Form["group_code"].ToString();
                var data = dbDC.TM_GroupCode.Find(id);
                dbDC.TM_GroupCode.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult DocTypeList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_DocType;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_type_short,
                        s.doc_type_full,
                        s.doc_lv,
                        s.dcc,
                        s.copy_flag,
                        s.review_year
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
        public JsonResult CreateDocType()
        {
            try
            {
                var formData = Request.Form["doc_type_short"].ToString();
                var dbData = dbDC.TM_DocType.Where(w => w.doc_type_short == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_DocType data = new TM_DocType();
                    data.doc_type_short = formData;
                    data.doc_type_full = Request.Form["doc_type_full"];
                    data.doc_lv = byte.Parse(Request.Form["doc_lv"].ToString());
                    var x = Request.Form["copy_flag"] != null ? Request.Form["copy_flag"].ToString() : "false";
                    data.dcc = Request.Form["dcc"];
                    data.copy_flag = bool.Parse(x);
                    data.review_year = byte.Parse(Request.Form["review_year"].ToString());
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_DocType.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_DocType.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateDocType()
        {
            try
            {
                var id = Request.Form["doc_type_short"].ToString();
                var data = dbDC.TM_DocType.Find(id);
                data.doc_type_full = Request.Form["doc_type_full"];
                data.doc_lv = byte.Parse(Request.Form["doc_lv"].ToString());
                var x = Request.Form["copy_flag"] != null ? Request.Form["copy_flag"].ToString() : "false";
                data.copy_flag = bool.Parse(x);
                data.dcc = Request.Form["dcc"];
                data.review_year = byte.Parse(Request.Form["review_year"].ToString());
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteDocType()
        {
            try
            {
                var id = Request.Form["doc_type_short"].ToString();
                var data = dbDC.TM_DocType.Find(id);
                dbDC.TM_DocType.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult ReviewerList(int group_id = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbDC.TM_Reviewer_Prod select a;

                if (group_id != 0)
                {
                    query = query.Where(w => w.group_id == group_id);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.group_id,
                        s.qc_group,
                        s.en_group
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
        public JsonResult CreateReviewer()
        {
            try
            {
                var formData = int.Parse(Request.Form["group_id"].ToString());
                var dbData = dbDC.TM_Reviewer_Prod.Where(w => w.group_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Reviewer_Prod data = new TM_Reviewer_Prod();
                    data.group_id = formData;
                    data.qc_group = int.Parse(Request.Form["qc_group"].ToString());
                    data.en_group = int.Parse(Request.Form["en_group"].ToString());
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_Reviewer_Prod.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_Reviewer_Prod.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateReviewer()
        {
            try
            {
                var id = int.Parse(Request.Form["group_id"].ToString());
                var data = dbDC.TM_Reviewer_Prod.Find(id);
                data.qc_group = int.Parse(Request.Form["qc_group"].ToString());
                data.en_group = int.Parse(Request.Form["en_group"].ToString());
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteReviewer()
        {
            try
            {
                var id = int.Parse(Request.Form["group_id"].ToString());
                var data = dbDC.TM_Reviewer_Prod.Find(id);
                dbDC.TM_Reviewer_Prod.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult ActionList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_Action;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.action_id,
                        s.action_name
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
        public JsonResult CreateAction()
        {
            try
            {
                var formData = byte.Parse(Request.Form["action_id"].ToString());
                var dbData = dbDC.TM_Action.Where(w => w.action_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Action data = new TM_Action();
                    data.action_id = formData;
                    data.action_name = Request.Form["action_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_Action.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_Action.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateAction()
        {
            try
            {
                var id = byte.Parse(Request.Form["action_id"].ToString());
                var data = dbDC.TM_Action.Find(id);
                data.action_name = Request.Form["action_name"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteAction()
        {
            try
            {
                var id = byte.Parse(Request.Form["action_id"].ToString());
                var data = dbDC.TM_Action.Find(id);
                dbDC.TM_Action.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult OperationList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_Operation;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.operation_id,
                        s.operation_name
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
        public JsonResult CreateOperation()
        {
            try
            {
                var formData = byte.Parse(Request.Form["operation_id"].ToString());
                var dbData = dbDC.TM_Operation.Where(w => w.operation_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Operation data = new TM_Operation();
                    data.operation_id = formData;
                    data.operation_name = Request.Form["operation_name"].ToString();
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_Operation.Add(data);
                }

                dbDC.SaveChanges();

                return Json(new { Result = "OK", Record = dbDC.TM_Operation.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateOperation()
        {
            try
            {
                var id = byte.Parse(Request.Form["operation_id"].ToString());
                var data = dbDC.TM_Operation.Find(id);
                data.operation_name = Request.Form["operation_name"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteOperation()
        {
            try
            {
                var id = byte.Parse(Request.Form["operation_id"].ToString());
                var data = dbDC.TM_Operation.Find(id);
                dbDC.TM_Operation.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult LevelList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_Level;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.lv_id,
                        s.lv_name,
                        s.position_min,
                        s.position_max
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
        public JsonResult CreateLevel()
        {
            try
            {
                var formData = byte.Parse(Request.Form["lv_id"].ToString());
                var dbData = dbDC.TM_Level.Where(w => w.lv_id == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Level data = new TM_Level();
                    data.lv_id = formData;
                    data.lv_name = Request.Form["lv_name"].ToString();
                    data.position_min = byte.Parse(Request.Form["position_min"].ToString());
                    data.position_max = byte.Parse(Request.Form["position_max"].ToString());
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_Level.Add(data);
                }

                dbDC.SaveChanges();
                //Creating method must return the new created object as Record property.
                return Json(new { Result = "OK", Record = dbDC.TM_Level.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateLevel()
        {
            try
            {
                var id = byte.Parse(Request.Form["lv_id"].ToString());
                var data = dbDC.TM_Level.Find(id);
                data.lv_name = Request.Form["lv_name"].ToString();
                data.position_min = byte.Parse(Request.Form["position_min"].ToString());
                data.position_max = byte.Parse(Request.Form["position_max"].ToString());
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteLevel()
        {
            try
            {
                var id = byte.Parse(Request.Form["lv_id"].ToString());
                var data = dbDC.TM_Level.Find(id);
                dbDC.TM_Level.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult PaperList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.TM_Paper;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.paper_id,
                        s.paper_name
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
        public JsonResult CreatePaper()
        {
            try
            {
                var formData = Request.Form["paper_name"].ToString();
                var dbData = dbDC.TM_Paper.Where(w => w.paper_name == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_Paper data = new TM_Paper();
                    data.paper_name = formData;
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["DC_Auth"].ToString();

                    dbDC.TM_Paper.Add(data);
                }

                dbDC.SaveChanges();
                //Creating method must return the new created object as Record property.
                return Json(new { Result = "OK", Record = dbDC.TM_Paper.OrderByDescending(o => o.update_dt).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdatePaper()
        {
            try
            {
                var id = byte.Parse(Request.Form["paper_id"].ToString());
                var data = dbDC.TM_Paper.Find(id);
                data.paper_name = Request.Form["paper_name"].ToString();
                data.update_dt = DateTime.Now;
                data.update_by = Session["DC_Auth"].ToString();
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePaper()
        {
            try
            {
                var id = byte.Parse(Request.Form["paper_id"].ToString());
                var data = dbDC.TM_Paper.Find(id);
                dbDC.TM_Paper.Remove(data);
                dbDC.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //------------------------------------------------//

        [HttpPost]
        public JsonResult GetTNCUserList()
        {
            try
            {
                var result = dbTNC.tnc_user.OrderBy(o => o.emp_fname).ThenBy(o => o.emp_lname).Select(r => new { DisplayText = r.emp_fname + " " + r.emp_lname, Value = r.emp_code });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetUTypeList()
        {
            try
            {
                var result = dbDC.TM_UserType
                    .Select(r => new { DisplayText = r.utype_name, Value = r.utype_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetTNCGroupList()
        {
            try
            {
                var result = dbTNC.tnc_group_master.OrderBy(o => o.group_name)
                    .Select(r => new { DisplayText = r.group_name, Value = r.id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDCCList()
        {
            try
            {
                var result = from a in dbDC.TM_User.Where(w => w.utype_id == 4).ToList()
                             join b in dbTNC.tnc_user.ToList() on a.empcode equals b.emp_code
                             select new
                             {
                                 DisplayText = b.emp_fname + " " + b.emp_lname,
                                 Value = a.empcode
                             };

                //var result = dbDC.TM_User.Where(w => w.utype_id == 4)
                //    .Select(r => new { DisplayText = r.empcode, Value = r.empcode });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    
    }
}
