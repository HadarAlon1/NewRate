using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rate.Microservice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using static Rate.Microservice.ResponseRate;

namespace ConsoleToWebAPI.Controllers
{
    [ApiController]
    [Route("Rate/{action}")]
    public class RateController : ControllerBase
    {
        Root DataResponse = ResponseRate.GetRate();
        List<rate> arrlCurrencies = ResponseRate.GetRateXml();

        public string Get( )
        {
            //max
            Double max = arrlCurrencies.Max(a => a.value);
            string sMax = "";
            string NameMax = "";
            foreach (rate rate in arrlCurrencies)
            {
                if (rate.value == max)
                {
                    sMax = sMax + rate.name + ",";
                    NameMax = rate.name;
                }
            }

            sMax = " the value of Max rate:" + sMax + " is:" + max.ToString();

            //*1000
            string mul = " Multiple all rates by 1000";
            foreach (rate rate in arrlCurrencies)
            {
                mul = mul + rate.name + " is:" + (rate.value * 1000).ToString() + "\r\n";
            }
            //order by type
            var oType = arrlCurrencies.OfType<rate>().OrderBy(i => i.type);
            string type = "The cur order by type:\r\n";
            foreach (rate rate in oType)
            {
                type = type + "the type of " + rate.name + " is:" + rate.type + "\r\n";
            }
            //order by name
            var oName = arrlCurrencies.OfType<rate>().OrderBy(i => i.name);
            string name = "The cur order by name:\r\n";
            foreach (rate rate in oName)
            {
                name = name + "the cur is:" + rate.name + "\r\n";
            }
            // Save the max rate in DB(SQL) if it changed from previous run
            string sLog = "";
            SqlConnection m_Connection = new SqlConnection(
              @"Data Source=DESKTOP-TAJ7C3U\SQLEXPRESS;" +
              "Initial Catalog=Hadar_DB;" +
              "Integrated Security=SSPI;");
            //m_MutexStatement.WaitOne();
            string sSQL = "SELECT Top 1 * FROM [RatesHistory] order by Code desc";
            DbCommand cmd;
            cmd = new SqlCommand(sSQL, m_Connection);
            m_Connection.Open();
            cmd.ExecuteNonQuery();
            SqlDataAdapter Adapter = new SqlDataAdapter(sSQL, m_Connection);
            DataTable table = new DataTable();
            Adapter.Fill(table);
            List<RatesHistory> RatesHistories = new List<RatesHistory>();
            RatesHistory RatesHistory;
            if (table.Rows.Count == 0)
            {
                sSQL = @"INSERT INTO [dbo].[RatesHistory]([UpdateTime],[CurrencyName],[max_rate])VALUES
            (GETDATE() ,N'" + NameMax + "'," + max.ToString() + ")";
                cmd = new SqlCommand(sSQL, m_Connection);
                cmd.ExecuteNonQuery();
            }
            else
            {

                if (max > Convert.ToInt32(table.Rows[0]["max_rate"]))
                {
                    sSQL = @"INSERT INTO [dbo].[RatesHistory]([UpdateTime],[CurrencyName],[max_rate])VALUES
            (GETDATE() ,N'" + NameMax + "'," + max.ToString() + ")";
                    cmd = new SqlCommand(sSQL, m_Connection);
                    cmd.ExecuteNonQuery();
                    RatesHistory = new RatesHistory();
                    RatesHistory.CurrencyName = NameMax;
                    RatesHistory.max_rate = max;
                    RatesHistory.UpdateTime = DateTime.Now;
                    RatesHistories.Add(RatesHistory);
                    RatesHistory = new RatesHistory();
                    RatesHistory.CurrencyName = table.Rows[0]["CurrencyName"].ToString();
                    RatesHistory.max_rate = Convert.ToDouble(table.Rows[0]["max_rate"]);
                    RatesHistory.UpdateTime = Convert.ToDateTime(table.Rows[0]["UpdateTime"]);
                    RatesHistories.Add(RatesHistory);
                }
                //json log
                var oJsonSettings = new JsonSerializerSettings();
                var data = JsonConvert.SerializeObject(RatesHistories, oJsonSettings);
                if (data != "[]")
                {
                    System.Text.StringBuilder sData = new System.Text.StringBuilder(1048576);

                    sData.Append(data);
                    System.IO.StreamWriter sr = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/log/" + DateTime.Now.ToString("yyyy’_‘MM’_‘dd’T’HH’_’mm’_’ss") + @".json", false, System.Text.Encoding.UTF8);
                    sr.Write(sData);
                    sr.Close();
                     sLog = "The log file create in:" + System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/log/" + DateTime.Now.ToString("yyyy’_‘MM’_‘dd’T’HH’_’mm’_’ss") + @".json";

                }

            }
                return sMax + "\r\n" + mul + "\r\n" + type + "\r\n" + name + "\r\n" + sLog;

        }

    }
}
