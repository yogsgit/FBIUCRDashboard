using Newtonsoft.Json;
using FBIUCRDemo.BusinessLogic;
using FBIUCRDemo.Models;
using System;
using System.Data;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace FBIUCRDemo.Controllers
{
    public class DownloadController : Controller
    {
        public DownloadController():base()
        {

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
    }
}