using Newtonsoft.Json;
using FBIUCRDemo.BusinessLogic;
using FBIUCRDemo.Models;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.PowerBI.Security;
using Microsoft.PowerBI.Api.Beta;
using Microsoft.Rest;
using System.Threading.Tasks;

namespace FBIUCRDemo.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        private readonly string workspaceCollection;
        private readonly string workspaceId;
        private readonly string accessKey;
        private readonly string apiUrl;

        public DownloadController()
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
        public ActionResult NIBRS()
        {
            return View();
        }

        public ActionResult Arson()
        {
            return View();
        }

        public JsonResult FilterData(string filterJson)
        {
            try
            {
                var filterData = JsonConvert.DeserializeObject<FilterModel>(filterJson);

                DataAccess access = new DataAccess();
                DataSet dsResult = access.FilterData(filterData);
                Task<object>[] tasks = new Task<object>[3];

                tasks[0] = Task.Factory.StartNew(() => new DataProcessor().GetCsvLink(dsResult));
                tasks[1] = Task.Factory.StartNew(() => new DataProcessor().GetJsonLink(dsResult));
                tasks[2] = Task.Factory.StartNew(() => new DataProcessor().GetResultData(dsResult));

                Task.WaitAll(tasks);

                ResultModel result = tasks[2].Result != null ? (ResultModel)tasks[2].Result : null;
                result.CsvLink = tasks[0].Result != null ? tasks[0].Result.ToString() : null;
                result.JsonLink = tasks[1].Result != null ? tasks[1].Result.ToString() : null;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                StorageClient.LogError(ex);
            }
            return null;
        }

        public JsonResult FilterArsonData(string filterJson)
        {
            try
            {
                var filterData = JsonConvert.DeserializeObject<ArsonFilterModel>(filterJson);

                DataAccess access = new DataAccess();
                DataSet dsResult = access.FilterData(filterData);
                Task<object>[] tasks = new Task<object>[3];

                tasks[0] = Task.Factory.StartNew(() => new DataProcessor().GetCsvLink(dsResult));
                tasks[1] = Task.Factory.StartNew(() => new DataProcessor().GetJsonLink(dsResult));
                tasks[2] = Task.Factory.StartNew(() => new DataProcessor().GetResultData(dsResult, true));

                Task.WaitAll(tasks);

                ResultModel result = tasks[2].Result != null ? (ResultModel)tasks[2].Result : null;
                result.CsvLink = tasks[0].Result != null ? tasks[0].Result.ToString() : null;
                result.JsonLink = tasks[1].Result != null ? tasks[1].Result.ToString() : null;

                return Json(result, JsonRequestBehavior.AllowGet);
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
            var devToken = PowerBIToken.CreateDevToken(this.workspaceCollection, this.workspaceId);
            using (var client = this.CreatePowerBIClient(devToken))
            {
                var reportsResponse = client.Reports.GetReports(this.workspaceCollection, this.workspaceId);

                var viewModel = new ReportsViewModel
                {
                    Reports = reportsResponse.Value.ToList()
                };

                return PartialView(viewModel);
            }
        }

        public async Task<ActionResult> Report(string reportId)
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