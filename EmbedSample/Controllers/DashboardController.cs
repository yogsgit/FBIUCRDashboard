using Microsoft.PowerBI.Api.Beta;
using Microsoft.PowerBI.Security;
using Microsoft.Rest;
using FBIUCRDemo.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FBIUCRDemo.BusinessLogic;

namespace FBIUCRDemo.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string workspaceCollection;
        private readonly string workspaceId;
        private readonly string accessKey;
        private readonly string apiUrl;

        public DashboardController()
        {
            this.workspaceCollection = ConfigurationManager.AppSettings["powerbiWorkspaceCollection"];
            this.accessKey = ConfigurationManager.AppSettings["powerbiAccessKey"];
            this.apiUrl = ConfigurationManager.AppSettings["powerbiApiUrl"];
            if (System.Web.HttpContext.Current.Application["WorkspaceID"] == null)
            {
                PowerBIHelper.UploadReports();
            }
            this.workspaceId = System.Web.HttpContext.Current.Application["WorkspaceID"].ToString();
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var devToken = PowerBIToken.CreateDevToken(this.workspaceCollection, this.workspaceId);
                using (var client = this.CreatePowerBIClient(devToken))
                {
                    var reportsResponse = await client.Reports.GetReportsAsync(this.workspaceCollection, this.workspaceId);
                    var report = reportsResponse.Value.FirstOrDefault(r => r.Name == "UCRDashboard");
                    var embedToken = PowerBIToken.CreateReportEmbedToken(this.workspaceCollection, this.workspaceId, report.Id);

                    var viewModel = new ReportViewModel
                    {
                        Report = report,
                        AccessToken = embedToken.Generate(this.accessKey)
                    };

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                StorageClient.LogError(ex);
            }
            return null;
        }

        [ChildActionOnly]
        public ActionResult Reports()
        {
            try
            {
                var devToken = PowerBIToken.CreateDevToken(this.workspaceCollection, this.workspaceId);
                using (var client = this.CreatePowerBIClient(devToken))
                {
                    var reportsResponse = client.Reports.GetReports(this.workspaceCollection, this.workspaceId);

                    var viewModel = new ReportsViewModel
                    {
                        Reports = reportsResponse.Value.OrderBy(m => m.Name).ToList()
                    };

                    return PartialView(viewModel);
                }
            }
            catch (Exception ex)
            {
                StorageClient.LogError(ex);
            }
            return null;
        }

        public async Task<ActionResult> Report(string reportId)
        {
            try
            {
                var devToken = PowerBIToken.CreateDevToken(this.workspaceCollection, this.workspaceId);
                using (var client = this.CreatePowerBIClient(devToken))
                {
                    var reportsResponse = await client.Reports.GetReportsAsync(this.workspaceCollection, this.workspaceId);
                    var report = reportsResponse.Value.FirstOrDefault(r => r.Id == reportId);
                    var embedToken = PowerBIToken.CreateReportEmbedToken(this.workspaceCollection, this.workspaceId, report.Id);

                    var viewModel = new ReportViewModel
                    {
                        Report = report,
                        AccessToken = embedToken.Generate(this.accessKey)
                    };

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                StorageClient.LogError(ex);
            }
            return null;
        }

        private IPowerBIClient CreatePowerBIClient(PowerBIToken token)
        {
            var jwt = token.Generate(accessKey);
            var credentials = new TokenCredentials(jwt, "AppToken");
            var client = new PowerBIClient(credentials)
            {
                BaseUri = new Uri(apiUrl)
            };

            return client;
        }
    }
}