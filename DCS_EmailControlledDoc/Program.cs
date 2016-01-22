using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonFunction;

namespace DCS_EmailControlledDoc
{
    public class Program
    {
        //Program send email Document that Controlled on effective date.
        static void Main(string[] args)
        {
            try
            {
                DocumentControlEntities dbDoc = new DocumentControlEntities();
                TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();
                SendEmail sm = new SendEmail();

                DateTime dtNow = DateTime.Now.Date;
                //DateTime dtTest = new DateTime(2015, 6, 6);//for Test

                var query = from a in dbDoc.V_Max_Transaction
                            where a.eff_date == dtNow && a.status_id == 100//100 = Controlled
                            select a;

                foreach (var q in query)
                {
                    sm.SendEmailDistribution(q.doc_type_short, q.group_code, q.run_no, q.rev_no);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DCS_EmailControlledDoc Error occured: " + ex.GetType().ToString());
                Console.WriteLine("Error Message: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                Console.WriteLine("No Error");
                Environment.Exit(0);
            }
        }
    }

    public class SendEmail
    {
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();
        DocumentControlEntities dbDC = new DocumentControlEntities();
        public void SendEmailDistribution(string doc_type_short, string group_code, int run_no, byte rev_no)
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
                        SendEmailControlled(doc_type_short, group_code, run_no, rev_no, email.Substring(1));//Controlled
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
                            SendEmailControlled(doc_type_short, group_code, run_no, rev_no, email.Substring(1));//Controlled
                            email = "";
                        }
                    }
                    break;
                }
            }
        }

        public void SendEmailControlled(string doc_type_short, string group_code, int run_no, byte rev_no,
                                    string receiver)
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

                var get_doc = (from a in dbDC.V_Transaction
                               where a.doc_type_short == doc_type_short && a.group_code == group_code
                               && a.run_no == run_no && a.rev_no == rev_no
                               select a).FirstOrDefault();

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

                    subject = "Controlled (" + get_doc.doc_no + ")";
                    body = "Dear. All Concern,<br /><br />" +
                        "<b>Document No. : </b>" + get_doc.doc_no + "<br />" +
                        "<b>Document Name : </b>" + get_doc.doc_name + "<br />" +
                        "<b>Effective Date : </b>" + get_doc.eff_date.Date.ToString("dd-MM-yyyy") + "<br />" +
                        "<b>You can see document at link ---> </b><a href='" + int_link + get_doc.attach_file.Substring(1) + "'>View</a><br />" +
                        "<b>Link : </b><a href='" + int_link + "/Home/Controlled'>Internal</a> | <a href='" + ext_link + "/Home/Controlled'>External</a><br />" +
                        "<br /><b>*This message is automatic sending from Document Control, please do not reply*</b>";

                    //tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", mailto, subject, body);//For Real
                    tnc_util.SendMail(16, "TNCAutoMail-DCS@nok.co.th", "monchit@nok.co.th", subject, body);//For Test
                }
            }
        }
    }
}
