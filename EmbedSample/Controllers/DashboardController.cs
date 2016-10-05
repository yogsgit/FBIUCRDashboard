using Microsoft.PowerBI.Api.Beta;
using Microsoft.PowerBI.Security;
using FBIUCRDemo.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FBIUCRDemo.BusinessLogic;
using Microsoft.PowerBI.Api.Beta.Models;
using System.Collections.Generic;

namespace FBIUCRDemo.Controllers
{
    public class DashboardController : BaseController
    {
        public DashboardController() : base()
        {

        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var devToken = PowerBIToken.CreateDevToken(this.workspaceCollection, this.workspaceId);
                using (var client = CreatePowerBIClient(devToken))
                {
                    var viewModel = new List<ReportViewModel>();

                    var reportsResponse = await client.Reports.GetReportsAsync(this.workspaceCollection, this.workspaceId);
                    var dashboardReports = reportsResponse.Value.Where(m => m.Name.ToLower().Contains("dashboard"));

                    foreach(Report report in dashboardReports)
                    {
                        var embedToken = PowerBIToken.CreateReportEmbedToken(this.workspaceCollection, this.workspaceId, report.Id);
                        viewModel.Add(new ReportViewModel() { Report = report, AccessToken = embedToken.Generate(this.accessKey) });
                    }

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                StorageClient.LogError(ex);
            }
            return null;
        }

        
    }
}