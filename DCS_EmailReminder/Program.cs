using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonFunction;

namespace DCS_EmailReminder
{
    public class Program
    {
        //Program Email Reminder for Review Document
        static void Main(string[] args)
        {
            //Program pgm = new Program();
            
            try
            {
                MailCenterEntities dbMail = new MailCenterEntities();
                DocumentControlEntities dbDoc = new DocumentControlEntities();
                TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();
                
                DateTime dtNow = DateTime.Now.Date;
                DateTime dtCheck = DateTime.Now.AddMonths(1).Date;//Before one month
                //DateTime dtTest = new DateTime(2015, 6, 6);//for Test

                //Review every 2 years and send email before due date 1 month.
                var query = from a in dbDoc.V_Max_Transaction
                            where a.check_date <= dtNow && a.status_id == 100//100 = Controlled
                            select a;

                //var count_query = 0;
                //var count_send = 0;
                foreach (var q in query)
                {
                    GetEmail gm = new GetEmail();

                    string email = gm.GetEmailbyOrg(q.org_id, 0);//Get email Mgr. down

                    var get_mail = (from a in dbTNC.View_Organization
                                    where a.group_id == q.org_id
                                    select a).FirstOrDefault();

                    string cc = null;
                    if (get_mail != null)
                    {
                        cc += !string.IsNullOrEmpty(get_mail.DeptMgr_email) ? "," + get_mail.DeptMgr_email : "";
                        cc += !string.IsNullOrEmpty(get_mail.PlantMgr_email) ? "," + get_mail.PlantMgr_email : "";
                    }

                    cc = !string.IsNullOrEmpty(cc) ? cc.Substring(1) : null;
                    
                    //Console.WriteLine("doc no:" + q.doc_no);

                    if (!string.IsNullOrEmpty(email))
                    {
                        TNCUtility tnc_util = new TNCUtility();

                        string int_link = "http://webExternal/DocControl";
                        string ext_link = "http://webexternal.nok.co.th/DocControl";
                        string subject = "Pls. review document no. " + q.doc_no;
                        string body = "Dear. Document Owner,<br /><br />" +
                            "<b>This document / format need to review period 2 years</b><br />" +
                            "<b>Document No. : </b>" + q.doc_no + "<br />" +
                            "<b>Document Name : </b>" + q.doc_name + "<br />" +
                            "<b>You can see document at link ---> </b><a href='" + int_link + q.attach_file.Substring(1) + "'>View</a><br />" +
                            "<b>Link : </b><a href='" + int_link + "/Home/OverDueReview'>Internal</a> | <a href='" + ext_link + "/Home/OverDueReview'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";

                        tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", email, subject, body, cc);//For Real
                        //tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", "monchit@nok.co.th", subject, body);//For Test
                        //count_send++;
                    }
                    //else
                    //{
                    //    Console.WriteLine("NO EMAIL:" + q.doc_no);
                    //}
                    //count_query++;
                }
                //Console.WriteLine("total : " + count_query + ", count send : " + count_send);
            }
            catch (Exception)
            {
                Console.WriteLine("Error E-mail Remind");
                Console.ReadLine();
            }
        }
    }

    public class GetEmail
    {
        public string GetEmailbyOrg(int org, byte lv)
        {
            TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

            string email = "";

            if (lv == 0) // All email in group
            {
                var get_mail = from a in dbTNC.tnc_user
                               where a.emp_group == org && !string.IsNullOrEmpty(a.email) && a.emp_status == "A"
                               select a.email;

                foreach (var item in get_mail)
                {
                    email += "," + item;
                }
            }
            else if (lv == 1) // Eng.
            {
                var get_mail = from a in dbTNC.tnc_user
                               where a.emp_position == 1 && a.emp_group == org && !string.IsNullOrEmpty(a.email) && a.emp_status == "A"
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
                var get_mail = (from a in dbTNC.View_Organization
                                where a.plant_id == org
                                select a).FirstOrDefault();

                if (get_mail != null)
                {
                    email = !string.IsNullOrEmpty(get_mail.PlantMgr_email) ? "," + get_mail.PlantMgr_email : "";
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

        public string GetEmailDCC(string group_code)
        {
            TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();
            DocumentControlEntities dbDoc = new DocumentControlEntities();

            string email = "";
            var get_response = (from a in dbDoc.TM_GroupCode
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

        public string GetEmailIssuer(string doc_type_short, string group_code, int run_no, byte rev_no)
        {
            TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();
            DocumentControlEntities dbDoc = new DocumentControlEntities();
            string email = "";
            var get_tran = (from a in dbDoc.TD_Transaction
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
    }
}
