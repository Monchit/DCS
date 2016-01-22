using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using ISODocument.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISODocument.Controllers
{
    public class ChartController : Controller
    {
        DocumentControlEntities dbDC = new DocumentControlEntities();

        public ActionResult ChartCopy()
        {
            return View();
        }

        public ActionResult _GetChartCopy(int yy)
        {
            Highcharts chart = new Highcharts("chart")
            .InitChart(new Chart
            {
                DefaultSeriesType = ChartTypes.Column,
                Height = 600
            })
            .SetTitle(new Title { Text = "Copy Statistics" })
            .SetSubtitle(new Subtitle { Text = "Document Control" })
            .SetXAxis(new XAxis
            {
                Categories = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                Title = new XAxisTitle { Text = "Year 2013" }
            })
            .SetYAxis(new YAxis
            {
                Min = 0,//Start at
                Title = new YAxisTitle { Text = "Requests" }
            })
            .SetLegend(new Legend
            {
                Layout = Layouts.Vertical,
                Align = HorizontalAligns.Left,
                VerticalAlign = VerticalAligns.Top,
                X = 100,
                Y = 70,
                Floating = true,
                BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                Shadow = true
            })
            .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y +' requests'; }" })
            .SetPlotOptions(new PlotOptions
            {
                Column = new PlotOptionsColumn
                {
                    PointPadding = 0.2,
                    BorderWidth = 0
                }
            })
            .SetCredits(new Credits { Enabled = false });

            var get_copydata = from a in dbDC.V_Report_Copy
                               where a.act_dt.Year == yy
                               select a;
            object[] issue = new object[12];
            object[] comp = new object[12];
            object[] rej = new object[12];
            for (int i = 0; i < 12; i++)
            {
                issue[i] = get_copydata.Where(w => w.status_id == 0 && w.act_dt.Month == (i + 1)).Count();
                comp[i] = get_copydata.Where(w => w.status_id == 105 && w.act_dt.Month == (i + 1)).Count();
                rej[i] = get_copydata.Where(w => w.status_id == 104 && w.act_dt.Month == (i + 1)).Count();
            }

            chart.SetSeries(new[]
            {
                new Series { Name = "Issue", Data = new Data(issue) },
                new Series { Name = "Completed", Data = new Data(comp) },
                new Series { Name = "Rejected", Data = new Data(rej) },
            });

            return PartialView(chart);
        }
    }
}
