using FBIUCRDemo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace FBIUCRDemo.BusinessLogic
{
    public class DataAccess
    {
        public DataSet FilterData(FilterModel filterData)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["UCR_DataEntities"];
            DataSet dsResult = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString.ConnectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = GenerateORQuery(filterData);
                    command.CommandType = System.Data.CommandType.Text;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;
                    
                    conn.Open();
                    adapter.Fill(dsResult);
                    conn.Close();
                    adapter.Dispose();
                    command.Dispose();
                }
                catch(Exception ex)
                {
                    StorageClient.LogError(ex);
                }
            }
            return dsResult;
        }

        public DataSet FilterData(ArsonFilterModel filterData)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["UCR_DataEntities"];
            DataSet dsResult = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString.ConnectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = conn;
                    command.CommandText = GenerateArsonORQuery(filterData);
                    command.CommandType = System.Data.CommandType.Text;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = command;

                    conn.Open();
                    adapter.Fill(dsResult);
                    conn.Close();
                    adapter.Dispose();
                    command.Dispose();
                }
                catch (Exception ex)
                {
                    StorageClient.LogError(ex);
                }
            }
            return dsResult;
        }

        private void GetAndData(DataSet dsResult)
        {
            var incNos = from row in dsResult.Tables[0].AsEnumerable()
                         select row.Field<string>("IncidentNumber");
            var uniqueInc = incNos.Distinct();
        }

        private string GenerateArsonORQuery(ArsonFilterModel filterData)
        {
            bool useAnd = false;
            StringBuilder sbQuery = new StringBuilder("select * from ARSON_View");
            sbQuery.Append(GetNumValOR(filterData.Year, ref useAnd, "Year"));
            sbQuery.Append(GetBetValOR(filterData.MonthFrom, filterData.MonthTo, ref useAnd, "Month"));
            sbQuery.Append(GetListValOR(filterData.OffenseName, ref useAnd, "Offense"));

            return sbQuery.ToString();
        }

        private string GenerateORQuery(FilterModel filterData)
        {
            bool useAnd = false;
            StringBuilder sbQuery = new StringBuilder("select * from NIBRSData_View");

            sbQuery.Append(GetNumValOR(filterData.Year, ref useAnd, "Year"));
            sbQuery.Append(GetBetValOR(filterData.MonthFrom, filterData.MonthTo, ref useAnd, "Month"));
            sbQuery.Append(GetBetValOR(filterData.HourFrom, filterData.HourTo, ref useAnd, "IncidentHour"));

            sbQuery.Append(GetStrValOR(filterData.OffenseName, ref useAnd, "OffenseName"));
            sbQuery.Append(GetStrValOR(filterData.OffenseLocation, ref useAnd, "LocationName"));
            sbQuery.Append(GetListValOR(filterData.OffenseWeapon, ref useAnd, "WeaponName"));
            sbQuery.Append(GetListValOR(filterData.OffensePropertyLoss, ref useAnd, "PropertyLoss"));
            sbQuery.Append(GetListValOR(filterData.OffensePropertyDesc, ref useAnd, "PropertyDesc"));
            sbQuery.Append(GetListValOR(filterData.OffenseDrug, ref useAnd, "DrugName"));

            sbQuery.Append(GetListValOR(filterData.VictimType, ref useAnd, "VictimType"));
            sbQuery.Append(GetListValOR(filterData.VictimGender, ref useAnd, "VictimGender"));
            sbQuery.Append(GetListValOR(filterData.VictimRace, ref useAnd, "VictimRace"));
            sbQuery.Append(GetListValOR(filterData.VictimEthnicity, ref useAnd, "VictimEthnicity"));
            sbQuery.Append(GetListValOR(filterData.VictimInjury, ref useAnd, "VictimInjury"));
            sbQuery.Append(GetCompValOR(filterData.VictimAge, filterData.VictimAgeComp, ref useAnd, "VictimAge"));

            sbQuery.Append(GetListValOR(filterData.OffenderGender, ref useAnd, "OffenderGender"));
            sbQuery.Append(GetListValOR(filterData.OffenderRace, ref useAnd, "OffenderRace"));
            sbQuery.Append(GetListValOR(filterData.OffenderEthnicity, ref useAnd, "OffenderEthnicity"));
            sbQuery.Append(GetListValOR(filterData.OffenderRelation, ref useAnd, "Relationship"));
            sbQuery.Append(GetCompValOR(filterData.OffenderAge, filterData.OffenderAgeComp, ref useAnd, "OffenderAge"));

            sbQuery.Append(GetListValOR(filterData.ArresteeGender, ref useAnd, "ArrestGender"));
            sbQuery.Append(GetListValOR(filterData.ArresteeRace, ref useAnd, "ArresteeRace"));
            sbQuery.Append(GetListValOR(filterData.ArresteeEthnicity, ref useAnd, "ArresteeEthnicity"));
            sbQuery.Append(GetListValOR(filterData.ArresteeOffense, ref useAnd, "ArrestOffenseName"));
            sbQuery.Append(GetListValOR(filterData.ArresteeType, ref useAnd, "ArrestType"));
            sbQuery.Append(GetListValOR(filterData.ArresteeWeapon, ref useAnd, "ArrestWeapon"));
            sbQuery.Append(GetCompValOR(filterData.ArresteeAge, filterData.ArresteeAgeComp, ref useAnd, "ArresteeAge"));

            return sbQuery.ToString();
        }

        private string GetNumValOR(int? val, ref bool useAnd, string field)
        {
            if (val != null && val != 0)
            {
                if (useAnd)
                    string.Format(" and {0} = {1}", field, val);
                else
                {
                    useAnd = true;
                    return string.Format(" where {0} = {1}", field, val);
                }
            }
            return null;
        }

        private string GetStrValOR(string val, ref bool useAnd, string field)
        {
            if (!string.IsNullOrEmpty(val))
            {
                if (useAnd)
                    return string.Format(" and {0} = '{1}'", field, val);
                else
                {
                    useAnd = true;
                    return string.Format(" where {0} = '{1}'", field, val);
                }
            }
            return null;
        }

        private string GetListValOR(List<string> vals, ref bool useAnd, string field)
        {
            if (vals != null && vals.Count > 0)
            {
                if (vals.Count == 1 && !string.IsNullOrEmpty(vals[0]))
                {
                    if (useAnd)
                        return string.Format(" and {0} = '{1}'", field, vals[0]);
                    else
                    {
                        useAnd = true;
                        return string.Format(" where {0} = '{1}'", field, vals[0]);
                    }
                }
                else if (vals.Count > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < vals.Count; i++)
                    {
                        if (i == 0)
                            sb.Append(string.Format("'{0}'", vals[i]));
                        else
                            sb.Append(string.Format(",'{0}'", vals[i]));
                    }
                    if (useAnd)
                        return string.Format(" and {0} in ({1})", field, sb.ToString());
                    else
                    {
                        useAnd = true;
                        return string.Format(" where {0} in ({1})", field, sb.ToString());
                    }
                }
            }
            return null;
        }

        private string GetCompValOR(int? val, string comp, ref bool useAnd, string field)
        {
            if (val != null && val != 0 && !string.IsNullOrEmpty(comp))
            {
                if (useAnd)
                    return string.Format(" and {0} {1} {2}", field, comp, val.ToString());
                else
                {
                    useAnd = true;
                    return string.Format(" where {0} {1} {2}", field, comp, val.ToString());
                }
            }
            return null;
        }

        private string GetBetValOR(int? from, int? to, ref bool useAnd, string field)
        {
            if (from != null && from != 0 && to != null && to != 0 && (from <= to))
            {
                if (useAnd)
                    return string.Format(" and {0} between {1} and {2}", field, from, to);
                else
                {
                    useAnd = true;
                    return string.Format(" where {0} between {1} and {2}", field, from, to);
                }
            }
            return null;
        }
    }
}