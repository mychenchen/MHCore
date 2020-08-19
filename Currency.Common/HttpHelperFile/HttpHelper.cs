using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Currency.Common.HttpHelperFile
{
    /// <summary>
    /// Http 帮助类
    /// </summary>
    public class HttpHelper
    {
        public static CookieCollection cookies;
        #region  Post
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpPost(string postUrl, string paramData, HttpWebRequestContentType contentType)
        {

            string ret = string.Empty;
            HttpWebResponse response = null;
            StreamReader sr = null;
            Stream newStream = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化成二进制流
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Timeout = 15000;                //响应时间
                if (cookies!=null)
                {
                    webReq.CookieContainer = new CookieContainer();
                    webReq.CookieContainer.Add(cookies);
                }
                webReq.ContentLength = byteArray.Length;

                newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                response = (HttpWebResponse)webReq.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();

            }
            catch (System.Exception ex)
            {
                var strt = ex.Message;
                //throw ex;
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return ret;
        }

        /// <summary>
        /// Post 处理状态码不是200返回的数据
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpPostNot200(string postUrl, string paramData, HttpWebRequestContentType contentType)
        {

            string ret = string.Empty;
            HttpWebResponse response = null;
            StreamReader sr = null;
            Stream newStream = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化成二进制流
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Timeout = 15000;                //响应时间
                webReq.ContentLength = byteArray.Length;

                newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                response = (HttpWebResponse)webReq.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();

            }
            catch (WebException e)
            {
                using (WebResponse response_ = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response_;
                    //Console.WriteLine(e);
                    if (response_ == null)
                    {
                        return e.ToString();
                    }
                    using (Stream data = response_.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        // Console.WriteLine(text);
                        return text;
                    }
                }
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return ret;
        }


        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <param name="contenType"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string Param, Hashtable headht = null, string contenType = "application/json;charset=utf-8")
        {
            HttpWebRequest request;


            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = contenType;
            request.Accept = "*/*";
            request.Timeout = 15000;
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            request.AllowAutoRedirect = false;
            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;
            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }
            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(Param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

        public static string PostJson(string url, string postData)
        {
            string result = "";
            System.Net.Http.HttpContent httpContent = new System.Net.Http.StringContent(postData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            //string postUrl = "http://test.***.gov.cn:81/***/**/request";
            string postUrl = url;
            if (postUrl.StartsWith("https"))
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
            }
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            try
            {
                System.Net.Http.HttpResponseMessage response = httpClient.PostAsync(postUrl, httpContent).Result;
                //result = response.IsSuccessStatusCode?string.Empty:response.StatusCode.ToString();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion

        #region  PostAsync
        /// <summary>
        /// 异步带返回值Post
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string postUrl, string paramData, HttpWebRequestContentType contentType)
        {
            string ret = string.Empty;
            HttpWebResponse response = null;
            StreamReader sr = null;
            Stream newStream = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = EnumHelper.GetExtendValue(contentType);
                webReq.Timeout = 4000;
                webReq.ContentLength = byteArray.Length;

                newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                Stream tempStream = await webReq.GetRequestStreamAsync();
                sr = new StreamReader(tempStream, Encoding.UTF8);
                ret = sr.ReadToEnd();

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return ret;
        }
        #endregion

        #region 异步带返回值Post
        /// <summary>
        /// 异步带返回值Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name=""></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostUsingAsync(string url, string postData, HttpWebRequestContentType contentType)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = EnumHelper.GetExtendValue(contentType);
            request.ContentLength = byteArray.Length;
            request.Timeout = 3000;
            using (Stream myRequestStream = await request.GetRequestStreamAsync())
            {
                myRequestStream.Write(byteArray, 0, byteArray.Length);
                WebResponse response = request.GetResponse();
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                return myStreamReader.ReadToEnd();
            }

        }
        #endregion

        #region Get
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpGet(string getUrl, HttpWebRequestContentType contentType)
        {
            Stream myResponseStream = null;
            StreamReader myStreamReader = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "GET";
            request.ContentType = EnumHelper.GetExtendValue(contentType);
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
            catch (Exception ex)
            {
                //throw ex;远程服务器返回错误: (404) 未找到。
                return ex.Message;
                //return "";
            }
            finally
            {
                if (myStreamReader != null)
                {
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
            }
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url, Hashtable headht = null, string ContentType = "application/json;charset=utf-8")
        {
            HttpWebRequest request;

            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = ContentType;
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string Http401Get(string getUrl, HttpWebRequestContentType contentType)
        {
            Stream myResponseStream = null;
            StreamReader myStreamReader = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "GET";
            request.ContentType = EnumHelper.GetExtendValue(contentType);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
            catch (WebException ex)
            {
                //throw ex;远程服务器返回错误: (404) 未找到。
                var response = (HttpWebResponse)ex.Response;
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();

                return retString;
                //return "";
            }
            finally
            {
                if (myStreamReader != null)
                {
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
            }
        }

        #endregion

        #region HttpGetAsync
        /// <summary>
        /// HttpGetAsync
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string getUrl, HttpWebRequestContentType contentType)
        {
            Stream myResponseStream = null;
            StreamReader myStreamReader = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "GET";
            request.ContentType = EnumHelper.GetExtendValue(contentType);
            try
            {
                var response = await request.GetResponseAsync();
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                myStreamReader.Close();
                myResponseStream.Close();
            }
        }
        #endregion

        #region Put
        /// <summary>
        /// put
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpPut(string postUrl, string paramData, HttpWebRequestContentType contentType)
        {

            string ret = string.Empty;
            HttpWebResponse response = null;
            StreamReader sr = null;
            Stream newStream = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化成二进制流
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "PUT";
                webReq.ContentType = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Timeout = 5000;                //响应时间
                webReq.ContentLength = byteArray.Length;
                if (cookies != null)
                {
                    webReq.CookieContainer = new CookieContainer();
                    webReq.CookieContainer.Add(cookies);
                }
                newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                response = (HttpWebResponse)webReq.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();

            }
            catch (System.Exception ex)
            {
                var strt = ex.Message;
                //throw ex;
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return ret;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpDelete(string getUrl, HttpWebRequestContentType contentType)
        {
            Stream myResponseStream = null;
            StreamReader myStreamReader = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "DELETE";
            request.ContentType = EnumHelper.GetExtendValue(contentType);
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                myResponseStream = response.GetResponseStream();
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
            catch (Exception ex)
            {
                //throw ex;远程服务器返回错误: (404) 未找到。
                return ex.Message;
                //return "";
            }
            finally
            {
                if (myStreamReader != null)
                {
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
            }
        }
        /// <summary>
        /// put
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpDelete(string postUrl, string paramData, HttpWebRequestContentType contentType)
        {

            string ret = string.Empty;
            HttpWebResponse response = null;
            StreamReader sr = null;
            Stream newStream = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化成二进制流
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "DELETE";
                webReq.ContentType = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Accept = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Timeout = 5000;                //响应时间
                webReq.ContentLength = byteArray.Length;
                if (cookies != null)
                {
                    webReq.CookieContainer = new CookieContainer();
                    webReq.CookieContainer.Add(cookies);
                }
                newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                response = (HttpWebResponse)webReq.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();

            }
            catch (System.Exception ex)
            {
                var strt = ex.Message;
                //throw ex;
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return ret;
        }
        /// <summary>
        /// put
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpDelete(string postUrl, string paramData, HttpWebRequestContentType contentType, string token = "", string domain = "")
        {

            string ret = string.Empty;
            HttpWebResponse response = null;
            StreamReader sr = null;
            Stream newStream = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化成二进制流
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "DELETE";
                webReq.ContentType = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Accept = EnumHelper.GetExtendValue(contentType);//ApplicationJSON类型
                webReq.Timeout = 5000;                //响应时间
                webReq.ContentLength = byteArray.Length;
                if (token != "" && domain != "")
                {
                    webReq.CookieContainer = new CookieContainer();
                    webReq.CookieContainer.Add(new Cookie("token", token, "/", domain));
                }
                newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                response = (HttpWebResponse)webReq.GetResponse();
                sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();

            }
            catch (System.Exception ex)
            {
                var strt = ex.Message;
                //throw ex;
            }
            finally
            {
                if (newStream != null)
                {
                    newStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            return ret;
        }
        #endregion


        #region PATCH
        /// <summary>
        /// 发送Patch请求
        /// </summary>
        /// <param name="getUrl">请求的URL</param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string HttpPatch(string getUrl, string Param, Hashtable headht = null, string contenType = "application/json;charset=utf-8")
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            StreamWriter requestStream = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                // httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = contenType;
                //httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpWebRequest.Referer = getUrl;
                httpWebRequest.Accept = "*/*";           
                httpWebRequest.Method = "PATCH";

                if (headht != null)
                {
                    foreach (DictionaryEntry item in headht)
                    {
                        httpWebRequest.Headers.Add(item.Key.ToString(), item.Value.ToString());
                    }
                }
                requestStream = new StreamWriter(httpWebRequest.GetRequestStream());
                requestStream.Write(Param);
                requestStream.Close();
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                return string.Empty;
            }
        }   
        #endregion

    }
}
