using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using ISODocument.Models;
using WebCommonFunction;

namespace ISODocument.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        DocumentControlEntities dbDC = new DocumentControlEntities();
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

        public ActionResult DocumentList()
        {
            var get_group = from a in dbTNC.tnc_group_master
                            orderby a.group_name
                            select a;
            ViewBag.AllGroup = get_group;
            return View();
        }

        public ActionResult PrintCopyList(string dt, string gc, int rn, byte rv)
        {
            return new ActionAsPdf("CopyList", new { doc_type_short = dt, group_code = gc, run_no = rn, rev_no = rv });
            //{ CustomSwitches = "--print-media-type --footer-font-size 8 --footer-left \"FM-QS-0050-00, EFF.DATE 22-OCT-15,P.1/1\"" };
            // { FileName = "CopyList.pdf" };
        }

        public ActionResult PrintReqCopyList(string dt, string gc, int rn, byte rv, int req)
        {
            return new ActionAsPdf("ReqCopyList", new { doc_type_short = dt, group_code = gc, run_no = rn, rev_no = rv, req = req });
            //{ CustomSwitches = "--print-media-type --footer-font-size 8 --footer-left \"FM-QS-0050-00, EFF.DATE 22-OCT-15,P.1/1\"" };
            // { FileName = "CopyList.pdf" };
        }

        public ActionResult CopyList(string doc_type_short, string group_code, int run_no, byte rev_no)
        {
            var document = (from a in dbDC.TD_Document
                            where a.doc_type_short == doc_type_short && a.group_code == group_code
                            && a.run_no == run_no && a.rev_no == rev_no
                            select a).FirstOrDefault();

            //var copy = from a in dbDC.TD_Copy
            //           where a.doc_type_short == doc_type_short && a.group_code == group_code
            //           && a.run_no == run_no && a.rev_no == rev_no && a.copy_runno == 0
            //           select a;

            //ViewBag.CopyList = copy;

            return View(document);
        }

        public ActionResult ReqCopyList(string doc_type_short, string group_code, int run_no, byte rev_no, int req)
        {
            var document = (from a in dbDC.TD_Document
                            where a.doc_type_short == doc_type_short && a.group_code == group_code
                            && a.run_no == run_no && a.rev_no == rev_no
                            select a).FirstOrDefault();

            ViewBag.ReqId = req;

            //var copy = from a in dbDC.TD_ReqCopy
            //           where a.req_id == req
            //           select a;

            //ViewBag.ReqCopyList = copy;

            return View(document);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetReqCopyList(string doctype, string groupcode, int runno, byte revno, int req)
        {
            var get_selected = from a in dbDC.TD_TranCopy.Where(a => a.req_id == req && a.status_id == 0)
                               join g in dbDC.V_Group on a.org_id equals g.id
                               select new
                               {
                                   g.group_name,
                                   a.org_id,
                                   a.TD_ReqCopy.qty,
                                   note = a.TD_ReqCopy.TM_Paper.paper_name
                                   //note = a.TD_ReqCopy.reason != null ? a.TD_ReqCopy.reason : ""
                               };

            return Json(get_selected, JsonRequestBehavior.AllowGet);
        }

        //-------------------------------------------//
        
        public ActionResult Monthly()
        {
            return View();
        }

        [HttpPost]
        public JsonResult MonthlyList(DateTime date_from, DateTime date_to, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dateto = date_to.AddDays(1);

                var query = from a in dbDC.V_Transaction
                            where a.status_id >= 100 && (a.act_dt >= date_from && a.act_dt <= dateto)
                            select a;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_lv,
                        s.doc_type_full,
                        s.operation_name,
                        s.status_name,
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.group_name,
                        s.act_dt,
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

        public void ExportMonthly(DateTime date_from, DateTime date_to)
        {
            TNCUtility util = new TNCUtility();

            var dateto = date_to.AddDays(1);

            var query = (from a in dbDC.V_Transaction
                         where a.status_id >= 100 && (a.act_dt >= date_from && a.act_dt <= dateto)
                         orderby a.doc_lv descending
                         select new
                        {
                            a.doc_lv,
                            a.doc_type_full,
                            a.operation_name,
                            a.status_name,
                            a.doc_no,
                            a.rev_no,
                            a.doc_name,
                            a.group_name,
                            a.eff_date,
                            a.act_dt
                        }).AsEnumerable() // <<== This forces the following Select to operate in memory
                        .Select(t => new
                        {
                            Level = t.doc_lv,
                            DocType = t.doc_type_full,
                            Operation = t.operation_name,
                            Status = t.status_name,
                            DocNo = t.doc_no.Substring(0, t.doc_no.Length - 3),
                            Rev = t.rev_no,
                            DocName = t.doc_name,
                            Group = t.group_name,
                            EffDate = t.eff_date.ToString("dd-MM-yyyy"),
                            CompleteDate = t.act_dt.Value.ToString("dd-MM-yyyy")
                        }).ToList();

            //var query = (from a in dbDC.V_Transaction.Where(w => w.status_id >= 100
            //                    && (w.act_dt >= date_from && w.act_dt <= dateto)).ToList()
            //            join b in dbTNC.tnc_group_master.ToList() on a.org_id equals b.id
            //            orderby a.doc_lv descending
            //            select new
            //            {
            //                Level = a.doc_lv,
            //                DocType = a.doc_type_full,
            //                Operation = a.operation_name,
            //                Status = a.status_name,
            //                DocNo = a.doc_no.Substring(0, a.doc_no.Length - 3),
            //                Rev = a.rev_no,
            //                DocName = a.doc_name,
            //                Group = b.group_name,
            //                EffDate = a.eff_date.ToString("dd-MM-yyyy"),
            //                CompleteDate = a.act_dt.Value.ToString("dd-MM-yyyy")
            //            }).ToList();

            util.CreateExcel(query, "Monthly_" + date_from.ToString("dd-MM-yyyy") + "_" + date_to.ToString("dd-MM-yyyy"));
        }

        //-------------------------------------------//

        public ActionResult MonthlyCancel()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CancelList(DateTime date_from, DateTime date_to, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbDC.V_Transaction.Where(w => w.status_id == 103 && (w.act_dt >= date_from && w.act_dt <= date_to));

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_lv,
                        s.doc_type_full,
                        //s.operation_name,
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

        public void ExportCancel(DateTime date_from, DateTime date_to)
        {
            TNCUtility util = new TNCUtility();

            var query = (from a in dbDC.V_Transaction.ToList()
                         where a.status_id == 103 && (a.act_dt >= date_from && a.act_dt <= date_to)
                         select new
                         {
                             Level = a.doc_lv,
                             DocType = a.doc_type_full,
                             //Operation = a.operation_name,
                             DocNo = a.doc_no.Substring(0, a.doc_no.Length - 3),
                             Rev = a.rev_no,
                             DocName = a.doc_name,
                             CancelDate = a.act_dt.Value.ToString("dd-MM-yyyy")
                         }).ToList();

            util.CreateExcel(query, "Cancel_" + date_from.ToString("dd-MM-yyyy") + "_" + date_to.ToString("dd-MM-yyyy"));
        }

        //-------------------------------------------//

        public ActionResult DocListbyEmail()
        {
            var get_group = from a in dbTNC.tnc_group_master
                            orderby a.group_name
                            select a;
            ViewBag.AllGroup = get_group;
            return View();
        }

        [HttpPost]
        public JsonResult DocEmailList(int group_id, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbDC.TD_DistributionList.Where(w => w.group_id == group_id || w.group_id == 0).ToList()
                            join b in dbDC.V_Max_Transaction.Where(w => w.status_id == 100).ToList()
                            on new { a.doc_type_short, a.group_code, a.run_no, a.rev_no }
                            equals new { b.doc_type_short, b.group_code, b.run_no, b.rev_no }
                            join c in dbTNC.tnc_group_master.ToList()
                            on b.org_id equals c.id
                            select new
                            {
                                b.doc_type_short,
                                b.group_code,
                                b.run_no,
                                b.doc_no,
                                a.rev_no,
                                b.doc_name,
                                b.eff_date,
                                c.group_name
                            };

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
                        s.group_name
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public void ExportEmailList(int selGroup)
        {
            TNCUtility util = new TNCUtility();

            var query = (from a in dbDC.TD_DistributionList.Where(w => w.group_id == selGroup || w.group_id == 0).ToList()
                         join b in dbDC.V_Max_Transaction.Where(w => w.status_id == 100).ToList()
                         on new { a.doc_type_short, a.group_code, a.run_no, a.rev_no }
                         equals new { b.doc_type_short, b.group_code, b.run_no, b.rev_no }
                         join c in dbTNC.tnc_group_master.ToList()
                         on b.org_id equals c.id
                         select new
                         {
                             DocNo = b.doc_no.Substring(0, b.doc_no.Length - 3),
                             Rev = a.rev_no,
                             DocName = b.doc_name,
                             EffDate = b.eff_date.ToString("dd-MM-yyyy"),
                             Originator = c.group_name
                         }).ToList();

            util.CreateExcel(query, "EmailList");
        }

        //-------------------------------------------//

        public ActionResult DocListbyCopy()
        {
            var get_group = from a in dbTNC.tnc_group_master
                            orderby a.group_name
                            select a;
            ViewBag.AllGroup = get_group;
            return View();
        }

        [HttpPost]
        public JsonResult DocCopyList(int group_id, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbDC.V_Sum_CopyDoc.Where(w => w.group_id == group_id).ToList()
                            join b in dbDC.V_Max_Transaction.Where(w => w.status_id == 100).ToList()
                            on new { a.doc_type_short, a.group_code, a.run_no, a.rev_no }
                            equals new { b.doc_type_short, b.group_code, b.run_no, b.rev_no }
                            join c in dbTNC.tnc_group_master.ToList()
                            on b.org_id equals c.id
                            select new
                            {
                                a.doc_no,
                                a.rev_no,
                                b.doc_name,
                                b.eff_date,
                                c.group_name,
                                a.qty
                            };

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_no,
                        s.rev_no,
                        s.doc_name,
                        s.eff_date,
                        s.group_name,
                        s.qty
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public void ExportCopyList(int selGroup)
        {
            TNCUtility util = new TNCUtility();

            var query = (from a in dbDC.V_Sum_CopyDoc.Where(w => w.group_id == selGroup).ToList()
                         join b in dbDC.V_Max_Transaction.Where(w => w.status_id == 100).ToList()
                         on new { a.doc_type_short, a.group_code, a.run_no, a.rev_no }
                         equals new { b.doc_type_short, b.group_code, b.run_no, b.rev_no }
                         join c in dbTNC.tnc_group_master.ToList()
                         on b.org_id equals c.id
                         select new
                         {
                             DocNo = a.doc_no,
                             Rev = a.rev_no,
                             DocName = b.doc_name,
                             EffDate = b.eff_date.ToString("dd-MM-yyyy"),
                             Originator = c.group_name,
                             Qty = a.qty
                         }).ToList();

            util.CreateExcel(query, "CopyList");
        }

        //-------------------------------------------//

        public ActionResult PDFGroupCopyList()
        {
            int group_id = int.Parse(Request.Form["selGroup"].ToString());

            var query = from a in dbDC.V_Sum_CopyDoc.Where(w => w.group_id == group_id).ToList()
                        join b in dbDC.V_Max_Transaction.Where(w => w.status_id == 100).ToList()
                        on new { a.doc_type_short, a.group_code, a.run_no, a.rev_no }
                        equals new { b.doc_type_short, b.group_code, b.run_no, b.rev_no }
                        join c in dbTNC.tnc_group_master.ToList()
                        on b.org_id equals c.id
                        select new
                        {
                            a.doc_no,
                            a.rev_no,
                            b.doc_name,
                            b.eff_date,
                            c.group_name,
                            a.qty
                        };

            //ViewBag.CopyList = query;
            return new ViewAsPdf(query);
        }

        public ActionResult PDFGroupEmailList()
        {
            int group_id = int.Parse(Request.Form["selGroup"].ToString());
            //var get_doc = from c in dbDC.TD_Copy
            //              where c.group_id == group_id
            //              group c by new { c.doc_type_short, c.group_code, c.run_no, c.rev_no, c.group_id } into g
            //              select new { g.Key, sum_qty = g.Sum(c => c.qty) };
            //return new ViewAsPdf(get_doc);
            return new ActionAsPdf("GroupEmailList", new { group_id = group_id });// { FileName = "CopyList.pdf" };
        }

        public ActionResult GroupEmailList(int group_id)
        {
            return View();
        }

        //-------------------------------------------//

        public ActionResult ReportCopy()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CopyReportList(DateTime date_from, DateTime date_to, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var dateto = date_to.AddDays(1);
                var query = from a in dbDC.V_Report_Copy
                            where (a.act_dt >= date_from && a.act_dt <= dateto)
                            select a;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.doc_no,
                        s.doc_name,
                        s.qty,
                        s.type,
                        s.paper_name,
                        s.group_name,
                        s.act_dt,
                        s.reason
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public void ExportCopy(DateTime date_from, DateTime date_to)
        {
            TNCUtility util = new TNCUtility();
            var dateto = date_to.AddDays(1);
            var query = (from a in dbDC.V_Report_Copy.Where(w => (w.act_dt >= date_from && w.act_dt <= dateto)).ToList()
                         orderby a.doc_no ascending
                         select new
                         {
                             DocNo = a.doc_no,
                             DocName = a.doc_name,
                             Qty = a.qty,
                             DocType = a.type,
                             Paper = a.paper_name,
                             Group = a.group_name,
                             CompleteDate = a.act_dt.ToString("dd-MM-yyyy"),
                             Reason = a.reason
                         });

            util.CreateExcel(query.ToList(), "Copy_" + date_from.ToString("dd-MM-yyyy") + "_" + date_to.ToString("dd-MM-yyyy"));
        }

        //-------------------------------------------//

        public ActionResult ReportOverdue()
        {
            var get_group = from a in dbTNC.tnc_group_master
                            orderby a.group_name
                            select a;
            ViewBag.AllGroup = get_group;
            return View();
        }

        [HttpPost]
        public JsonResult OverdueReportList(int org_id, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {//DateTime date_from, DateTime date_to, 
            try
            {
                var dt = DateTime.Now;
                //var dateto = date_to.AddDays(1);
                var query = from a in dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.eff_date <= dt && w.check_date < dt).ToList()
                            join b in dbTNC.tnc_group_master.ToList()
                            on a.org_id equals b.id
                            select new
                            {
                                a.doc_type_short,
                                a.group_code,
                                a.run_no,
                                a.rev_no,
                                a.doc_no,
                                a.doc_name,
                                a.eff_date,
                                a.check_date,
                                a.org_id,
                                b.group_name
                            };

                if (org_id != 0)//Page Load
                {
                    query = query.Where(w => w.org_id == org_id);
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query.AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize)
                    .Select(s => new
                    {
                        s.doc_no,
                        s.doc_name,
                        s.eff_date,
                        s.check_date,
                        s.group_name,
                        process = CheckActiveTran(s.doc_type_short, s.group_code, s.run_no, s.rev_no)
                    });

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public void ExportOverdue(int selGroup)
        {
            TNCUtility util = new TNCUtility();
            var dt = DateTime.Now;
            var query = from a in dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && w.eff_date <= dt && w.check_date < dt).ToList()
                        join b in dbTNC.tnc_group_master.ToList()
                        on a.org_id equals b.id
                        select new
                        {
                            a.doc_type_short,
                            a.group_code,
                            a.run_no,
                            a.rev_no,
                            a.doc_no,
                            a.doc_name,
                            a.eff_date,
                            a.check_date,
                            a.org_id,
                            b.group_name
                        };

            if (selGroup != 0)//Page Load
            {
                query = query.Where(w => w.org_id == selGroup);
            }

            var output = query
                    .Select(s => new
                    {
                        DocNo = s.doc_no,
                        DocName = s.doc_name,
                        EffDate = s.eff_date.ToString("dd-MM-yyyy"),
                        CheckDate = s.check_date != null ? s.check_date.Value.ToString("dd-MM-yyyy") : "",
                        Group = s.group_name,
                        InProcess = CheckActiveTran(s.doc_type_short, s.group_code, s.run_no, s.rev_no)
                    });

            util.CreateExcel(output.ToList(), "OverDue");
        }

        private bool CheckActiveTran(string doctype, string groupcode, int runno, byte revno)
        {
            var get_tran = dbDC.TD_Transaction.Where(w => w.doc_type_short == doctype && w.group_code == groupcode && w.run_no == runno && (w.rev_no == revno || w.rev_no == (revno+1)) && w.active == true).Count();

            if (get_tran > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult ReportComplete2Year()
        {
            var chk_date = new DateTime(2014,1,11);
            var query = from a in dbDC.TD_Transaction
                        where a.status_id == 100 && a.TD_Document.eff_date <= chk_date
                        select a;
            return View(query);
        }

        //-------------------------------------------//

        public ActionResult OverdueByDate()
        {
            return View();
        }

        public void ExportOverdueByDate(DateTime date_from, DateTime date_to)
        {
            TNCUtility util = new TNCUtility();
            var query = from a in dbDC.V_Max_Transaction.Where(w => w.status_id == 100 && (w.check_date >= date_from && w.check_date <= date_to)).ToList()
                        join b in dbTNC.tnc_group_master.ToList()
                        on a.org_id equals b.id
                        select new
                        {
                            a.doc_type_short,
                            a.group_code,
                            a.run_no,
                            a.rev_no,
                            a.doc_no,
                            a.doc_name,
                            a.eff_date,
                            a.check_date,
                            a.org_id,
                            b.group_name
                        };

            var output = query
                    .Select(s => new
                    {
                        DocNo = s.doc_no,
                        DocName = s.doc_name,
                        EffDate = s.eff_date.ToString("dd-MM-yyyy"),
                        CheckDate = s.check_date.Value.ToString("dd-MM-yyyy"),//s.check_date != null ? s.check_date.Value.ToString("dd-MM-yyyy") : "",
                        Group = s.group_name,
                    });

            util.CreateExcel(output.ToList(), "OverDue_" + date_from.ToString("dd-MM-yyyy") + "_" + date_to.ToString("dd-MM-yyyy"));
        }

        //[HttpPost]
        //public JsonResult GetTNCGroupList()
        //{
        //    try
        //    {
        //        var result = dbTNC.tnc_group_master.OrderBy(o => o.group_name)
        //            .Select(r => new { DisplayText = r.group_name, Value = r.id });
        //        return Json(new { Result = "OK", Options = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}
    }
}
