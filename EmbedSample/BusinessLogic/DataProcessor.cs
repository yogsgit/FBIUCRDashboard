using Newtonsoft.Json;
using FBIUCRDemo.Models;
using System.Data;
using System.Linq;
using System.Text;

namespace FBIUCRDemo.BusinessLogic
{
    public class DataProcessor
    {
        public object GetCsvLink(DataSet dsresult)
        {
            StringBuilder sbData = new StringBuilder();
            if (dsresult != null && dsresult.Tables.Count == 1 && dsresult.Tables[0].Rows.Count > 0)
            {
                bool isFirst = true;
                foreach (DataColumn col in dsresult.Tables[0].Columns)
                {
                    if (isFirst)
                    {
                        sbData.Append(col.ColumnName);
                        isFirst = false;
                    }
                    else
                        sbData.Append(string.Format(",{0}", col.ColumnName));
                }
                int colCnt = dsresult.Tables[0].Columns.Count;
                foreach (DataRow row in dsresult.Tables[0].Rows)
                {
                    sbData.AppendLine();
                    isFirst = true;
                    for (int i = 0; i < colCnt; i++)
                    {
                        if (isFirst)
                        {
                            sbData.Append(row[i]);
                            isFirst = false;
                        }
                        else
                            sbData.Append(string.Format(",{0}", row[i]));
                    }
                }

                StorageClient storageClient = new StorageClient();
                string csvLink = storageClient.GetBlobLink(sbData);

                return csvLink;
            }
            return null;
        }

        public object GetJsonLink(DataSet dsresult)
        {
            if (dsresult != null && dsresult.Tables.Count == 1 && dsresult.Tables[0].Rows.Count > 0)
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(dsresult.Tables[0]);

                StorageClient storageClient = new StorageClient();
                return storageClient.GetBlobLink(JSONString);
            }
            return null;
        }

        public object GetResultData(DataSet dsresult, bool isArson = false)
        {
            ResultModel result = new ResultModel();
            if (dsresult != null && dsresult.Tables.Count == 1 && dsresult.Tables[0].Rows.Count > 0)
            {
                result.TotalRecords = dsresult.Tables[0].Rows.Count;

                if (!isArson)
                {
                    var cntInc = from row in dsresult.Tables[0].AsEnumerable()
                                 select row.Field<string>("IncidentNumber");

                    result.UniqueIncidents = cntInc.Distinct().Count();
                    result.SampleTable = GenerateTable(dsresult);
                }
                else
                {
                    result.SampleTable = GenerateArsonTable(dsresult);
                }
            }
            return result;
        }

        private string GenerateTable(DataSet dsResult)
        {
            var THEAD = "<thead><tr role=\"row\"><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">IncidentNumber</th><th class=\"sorting_disabled\"  rowspan=\"1\" colspan=\"1\">IncidentDate</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Offense</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Location</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Weapon</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">VictimGender</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">VictimAge</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">OffenderGender</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">OffenderAge</th></tr></thead>";
            var TBODY = "<tbody>";
            var TBODYEND = "</tbody>";
            var TRODD = "<tr class=\"odd\" role=\"row\">";
            var TREVEN = "<tr class=\"even\" role=\"row\">";
            var TREND = "</tr>";
            var TD = "<td>";
            var TDEND = "</td>";

            int cnt = 50;
            if (dsResult.Tables[0].Rows.Count < 50)
                cnt = dsResult.Tables[0].Rows.Count;
            StringBuilder sbTable = new StringBuilder(THEAD);
            sbTable.Append(TBODY);
            for (int i = 0; i < cnt; i++)
            {
                if (i == 0 || i % 2 == 0)
                    sbTable.Append(TRODD);
                else
                    sbTable.Append(TREVEN);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["IncidentNumber"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["IncidentDate"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["OffenseName"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["LocationName"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["WeaponName"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["VictimGender"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["VictimAge"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["OffenderGender"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[i]["OffenderAge"].ToString()).Append(TDEND);
                sbTable.Append(TREND);
            }
            sbTable.Append(TBODYEND);

            return sbTable.ToString();
        }

        private string GenerateArsonTable(DataSet dsResult)
        {
            var THEAD = "<thead><tr role=\"row\"><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Year</th><th class=\"sorting_disabled\"  rowspan=\"1\" colspan=\"1\">Month</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Offense</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Reported</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Unfounded</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Actual</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Cleared</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Juvenile Cleared</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Uninhabited</th><th class=\"sorting_disabled\" rowspan=\"1\" colspan=\"1\">Est Damage</th></tr></thead>";
            var TBODY = "<tbody>";
            var TBODYEND = "</tbody>";
            var TRODD = "<tr class=\"odd\" role=\"row\">";
            var TREVEN = "<tr class=\"even\" role=\"row\">";
            var TREND = "</tr>";
            var TD = "<td>";
            var TDEND = "</td>";

            int cnt = 50;
            if (dsResult.Tables[0].Rows.Count < 50)
                cnt = dsResult.Tables[0].Rows.Count;
            StringBuilder sbTable = new StringBuilder(THEAD);
            sbTable.Append(TBODY);
            for (int i = 0; i < cnt; i++)
            {
                if (i == 0 || i % 2 == 0)
                    sbTable.Append(TRODD);
                else
                    sbTable.Append(TREVEN);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Year"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Month"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Offense"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Reported"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Unfounded"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Actual"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Cleared"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Juvenile Cleared"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Uninhabited"].ToString()).Append(TDEND);
                sbTable.Append(TD).Append(dsResult.Tables[0].Rows[0]["Est Damage"].ToString()).Append(TDEND);
                sbTable.Append(TREND);
            }
            sbTable.Append(TBODYEND);

            return sbTable.ToString();
        }
    }
}