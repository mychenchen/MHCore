using System;

namespace Currency.Common
{
    /// <summary>
    /// 时间戳帮助类
    /// </summary>
    public class TimeStampHelper
    {
        protected int m_timestamp;

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static int GettimeStamp()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtNow = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return Convert.ToInt32(timeStamp);
        }

        /// <summary>  
        /// 将时间格式YYYY-MM-DD转换成时间戳Timestamp    
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static int GetTimeStamp(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
        }

        public static int GetTimeStamp(string dt)
        {
            if (string.IsNullOrEmpty(dt))
            {
                return 0;
            }
            DateTime dtTime = DateTime.Parse(dt);
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dtTime - dateStart).TotalSeconds);
            return timeStamp;
        }

        /// <summary>  
        /// 时间戳Timestamp转换成日期  
        /// </summary>  
        /// <param name="timeStamp"></param>  
        /// <returns></returns>  
        public static DateTime GetDateTime(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        /// <summary>  
        /// 时间戳Timestamp转换成日期  
        /// </summary>  
        /// <param name="timeStamp"></param>  
        /// <returns></returns>  
        public static DateTime GetDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            int lTime = int.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return dtStart.Add(toNow);
        }
    }
}
