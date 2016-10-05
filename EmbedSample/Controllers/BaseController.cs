using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using FBIUCRDemo.BusinessLogic;
using Microsoft.PowerBI.Api.Beta;
using Microsoft.Rest;
using Microsoft.PowerBI.Security;
using FBIUCRDemo.Models;
using System.Threading.Tasks;

namespace FBIUCRDemo.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string workspaceCollection;
        protected readonly string workspaceId;
        protected readonly string accessKey;
        protected readonly string apiUrl;

        public BaseController()
        {
            this.workspaceCollection = ConfigurationManager.AppSettings["powerbiWorkspaceCollection"];
            this.accessKey = ConfigurationManager.AppSettings["powerbiAccessKey"];
            this.apiUrl = ConfigurationManager.AppSettings["powerbiApiUrl"];
            System.Web.HttpContext.Current.Application["WorkspaceID"] = ConfigurationManager.AppSettings["powerbiWorkspaceId"];
            //if (System.Web.HttpContext.Current.Application["WorkspaceID"] == null)
            //{
            //    PowerBIHelper.UploadReports();
            //}
            this.workspaceId = System.Web.HttpContext.Current.Application["WorkspaceID"].ToString();
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

        protected IPowerBIClient CreatePowerBIClient(PowerBIToken token)
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