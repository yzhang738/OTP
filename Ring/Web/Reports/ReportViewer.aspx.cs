using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using OTP.Ring.Business;
using System.IO;
using System.Xml;
using OTP.Ring.Common;
using System.Diagnostics;

namespace OTP.Ring.Web.Reports
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeReportViewer();
            }
        }

        private void InitializeReportViewer()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var reportId = Request.QueryString["r"];
            var parameters = Request.QueryString["p"];
            var securityUserId = Request.QueryString["u"];

            var repo = new ReportBL(ExtensionMethods.GetConfigurationSetting(Constant.DataConnectionString), 
                ExtensionMethods.GetConfigurationSetting(Constant.ReportConnectionString));

            var reportData = repo.GetReportDocumentData(int.Parse(reportId), parameters, securityUserId);

            StringReader stream = new StringReader(reportData.ReportDefinition);
            this.ReportViewer1.LocalReport.LoadReportDefinition(stream);
            this.ReportViewer1.LocalReport.DisplayName = reportData.DisplayName;

            foreach (var dataSource in reportData.DataSources)
            {
                this.ReportViewer1.LocalReport.DataSources.Add(dataSource);
            }

            this.ReportViewer1.LocalReport.Refresh();

            watch.Stop();
            Logger.LogDiagnostic(string.Format("Method call took {0} milliseconds", watch.ElapsedMilliseconds));
        }
    }
}