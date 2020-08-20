using System.Text.Json;
using System.Json;
using System.Collections.Generic;
using System.Data;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json.Serialization;
using System;
using System.ComponentModel;
using static Currency.Common.SystemTextJsonConvert;

namespace Currency.Common
{
    /// <summary>
    /// Json操作
    /// </summary>
    public static class Json
    {
        public static string ToJson(this object Json)
        {
            var encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowRanges(UnicodeRanges.All);
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(encoderSettings);

            //此处不用去除T,如果去除T后,json解析会出错,system.text.json 只识别带有T的时间格式
            //options.Converters.Add(new DateTimeTxtConverter()); 

            //options.WriteIndented = true;
            return Json == null ? null : JsonSerializer.Serialize(Json, options);
        }
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonSerializer.Deserialize<T>(Json);
        }
        public static List<T> ToList<T>(this string Json)
        {
            return Json == null ? null : JsonSerializer.Deserialize<List<T>>(Json);
        }
        public static DataTable ToTable(this string Json)
        {
            return Json == null ? null : JsonSerializer.Deserialize<DataTable>(Json);
        }
    }
}
