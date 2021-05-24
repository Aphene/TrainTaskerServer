using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace AphAsyncHandler
{
    public class Coins
    {
        static string key = "XXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        static string server = "http://rest.coinapi.io/";
        static string period = "1DAY";
        static string lastPrice = "";
        static string predictedPrice = "";

        public static string GetCoins() 
        {
            string[] coins = { "BTC", "ETH", "LTC" };
            string dir = "C:\\Inetpub\\wwwroot\\Templates\\";
            string template=File.ReadAllText(dir+"ListTemplate.html");
            string item = "<button class=\"ui-btn\" onclick= \"loadGraph('{CoinSymbol}')\" >{CoinSymbol}</button>";
            string list = "";
            for (int i = 0; i < coins.Length; ++i)
            {
                string itemi = item.Replace("{CoinSymbol}", coins[i])+"\r\n";
                list += itemi;
            }
            template = template.Replace("{{LIST}}", list);

            return template;
        }

        public static string LoadGraph(string coin)
        {
            int days = 30;
            string dir = "C:\\Inetpub\\wwwroot\\Templates\\";
            string template = File.ReadAllText(dir + "GraphTemplate.html");
            string data = getHistory(coin, days);
            template = template.Replace("<data>", data);

            string xaxislabels = "";
            for (int i = days; i > 0; --i)
            {
                if (xaxislabels != "") xaxislabels += ",";
                xaxislabels += "\""+i.ToString()+"\"";
            }
            xaxislabels += ",\"p\"";
            template = template.Replace("<xaxislabels>", xaxislabels);
            template = template.Replace("<CoinSymbol>", coin);
            template = template.Replace("{LastPrice}", lastPrice);
            template = template.Replace("{PredictedPrice}", predictedPrice);

            return template;
        }

        public static string getHistory(string symbol, int days)
        {
            string start = getDatePrev(days);
            string end = getDateNow();
            string request = "v1/ohlcv/" + symbol+"/USD" + "/history?period_id=" + period + "&time_start=" + start + "&time_end=" + end + "";
            string reply = get(server + request);
            //string reply = File.ReadAllText("BTC_USD.txt");
            JSONObject json = JSONObject.Create(reply);
            List<double> list = new List<double>();
            for (int i = 0; i < json.list.Count; ++i)
            {
                var item = json.list[i];
                var value = item.list[7];
                double v = value.n;
                list.Add(v);
            }
            double prediction = FitPrices.poly3degree(list);
            string data = "";
            for (int i = 0; i < list.Count; ++i)
            {
                if (i > 0) data += ",";
                data += list[i].ToString("00.0000");
            }
            data += "," + prediction.ToString("00.0000");
            predictedPrice = prediction.ToString("00.0000");
            lastPrice = list[list.Count - 1].ToString("00.0000");
            return data;
        }

        public static string getDateNow()
        {
            //return "2020-02-01T00:00:00.0000000Z";
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static string getDatePrev(int days)
        {
            // return "2020-01-01T00:00:00.0000000Z";
            return DateTime.UtcNow.AddDays(-days).ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static string get(string url)
        {
            string reply = "";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("X-CoinAPI-Key: " + key);
            url = url.Replace(" ", "%20");

            reply = webClient.DownloadString(url);
            return reply;
        }
    }
}
