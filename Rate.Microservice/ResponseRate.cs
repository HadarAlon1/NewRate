using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace Rate.Microservice
{
    public class ResponseRate
    {
        
    public class rate
    {
        public string name { get; set; }
        public string unit { get; set; }
        public double value { get; set; }
        public string type { get; set; }
    }

    public class Rates
    {
        public rate btc { get; set; }
        public rate eth { get; set; }
        public rate ltc { get; set; }
        public rate bch { get; set; }
        public rate bnb { get; set; }
        public rate eos { get; set; }
        public rate xrp { get; set; }
        public rate xlm { get; set; }
        public rate link { get; set; }
        public rate dot { get; set; }
        public rate yfi { get; set; }
        public rate usd { get; set; }
        public rate aed { get; set; }
        public rate ars { get; set; }
        public rate aud { get; set; }
        public rate bdt { get; set; }
        public rate bhd { get; set; }
        public rate bmd { get; set; }
        public rate brl { get; set; }
        public rate cad { get; set; }
        public rate chf { get; set; }
        public rate clp { get; set; }
        public rate cny { get; set; }
        public rate czk { get; set; }
        public rate dkk { get; set; }
        public rate eur { get; set; }
        public rate gbp { get; set; }
        public rate hkd { get; set; }
        public rate huf { get; set; }
        public rate idr { get; set; }
        public rate ils { get; set; }
        public rate inr { get; set; }
        public rate jpy { get; set; }
        public rate krw { get; set; }
        public rate kwd { get; set; }
        public rate lkr { get; set; }
        public rate mmk { get; set; }
        public rate mxn { get; set; }
        public rate myr { get; set; }
        public rate ngn { get; set; }
        public rate nok { get; set; }
        public rate nzd { get; set; }
        public rate php { get; set; }
        public rate pkr { get; set; }
        public rate pln { get; set; }
        public rate rub { get; set; }
        public rate sar { get; set; }
        public rate sek { get; set; }
        public rate sgd { get; set; }
        public rate thb { get; set; }
        public rate @try { get; set; }
        public rate twd { get; set; }
        public rate uah { get; set; }
        public rate vef { get; set; }
        public rate vnd { get; set; }
        public rate zar { get; set; }
        public rate xdr { get; set; }
        public rate xag { get; set; }
        public rate xau { get; set; }
        public rate bits { get; set; }
        public rate sats { get; set; }
    }

    public class Root
    {
        public Rates rates { get; set; }
    }


        public class RatesHistory
        {
            public DateTime UpdateTime { get; set; }
            public string CurrencyName { get; set; }
            public double max_rate { get; set; }
        }
        public static Root GetRate()
    {
        Root DataResponse;
        WebRequest webRequest = WebRequest.Create("https://api.coingecko.com/api/v3/exchange_rates");
        WebResponse webResponse = webRequest.GetResponse();
        Stream webStream = webResponse.GetResponseStream();
        //// convert stream to string
        StreamReader reader = new StreamReader(webStream);
        string responseFromServer = reader.ReadToEnd();
        webStream.Close();
        DataResponse = JsonConvert.DeserializeObject<Root>(responseFromServer.ToString());
        return DataResponse;
    }  
        public static List<rate> GetRateXml()
    {
        Root DataResponse;
        WebRequest webRequest = WebRequest.Create("https://api.coingecko.com/api/v3/exchange_rates");
        WebResponse webResponse = webRequest.GetResponse();
        Stream webStream = webResponse.GetResponseStream();
        //// convert stream to string
        StreamReader reader = new StreamReader(webStream);
        string responseFromServer = reader.ReadToEnd();
        webStream.Close();
            webStream.Close(); 
            XmlDocument doc = new XmlDocument();
            // To convert JSON text contained in string json into an XML node,
            doc = JsonConvert.DeserializeXmlNode(responseFromServer);
            var node = doc.SelectSingleNode("rates");
            var xnodes = node.ChildNodes;
            rate oCurrency;
            List<rate> arrlCurrencies = new List<rate>();
            foreach (XmlNode anode in xnodes)
            {
                try
                {

                    XmlNode nameNode = anode.SelectSingleNode("name");
                    XmlNode unitNode = anode.SelectSingleNode("unit");
                    XmlNode valueNode = anode.SelectSingleNode("value");
                    XmlNode typeNode = anode.SelectSingleNode("type");
                    oCurrency = new rate();
                    oCurrency.name = nameNode.InnerText;
                    oCurrency.unit = unitNode.InnerText;
                    oCurrency.value = double.Parse(valueNode.InnerText);
                    oCurrency.type = typeNode.InnerText;
                    arrlCurrencies.Add(oCurrency);
                }
                catch //(Exception ex)
                {
                }
            }
            return arrlCurrencies;
    }

    }
}



