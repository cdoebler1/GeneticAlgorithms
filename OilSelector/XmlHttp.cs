using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OilSelector
{
    class XmlHttp
    {
        private string m_aspSessID;
        private string m_basketSessID;


        public XmlHttp(string url)
        {
            string header = GetHeaderFromWeb(url);
        }

        public string AspSessID
        {
            get { return m_aspSessID; }
        }

        public string BasketSessID
        {
            get { return m_basketSessID; }
        }

        // "Product.Favorite=False&RemoveFavoriteOnToggle=False&Product.StockCode=9727-C10.016&Product.ProductType=Product&Quantity=1"
        // "e4y5zxhpyvftnnzrk3d41lzw"
        // "e5007c90-f8ad-4394-bb58-4c7f48222756"
        public bool PostMessage(string vars, string aspSessID, string basketSessID)
        {
            try
            {
                byte[] postBytes = Encoding.ASCII.GetBytes(vars);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://cannoninstrument.com/Basket/AddToBasket");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";    //"application/x-www-form-urlencoded";
                webRequest.ContentLength = postBytes.Length;

                CookieContainer cc = new CookieContainer();
                Uri u = new Uri(@"http://cannoninstrument.com");
                cc.Add(u, new Cookie("ASP.NET_SessionId", aspSessID));
                cc.Add(u, new Cookie("BasketSessionId", basketSessID));

                webRequest.CookieContainer = cc;

                Stream postStream = webRequest.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Close();

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                if (webResponse.StatusCode == HttpStatusCode.OK)
                    return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public string GetHeaderFromWeb(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK)
                return webResponse.Headers.ToString();
            else
                return "";
        }
    }
}
