using AutoMapper;
using Crm.WebApp.AuthorizeHelper;
using Currency.Common.LogManange;
using MH.WebApp.Controllers;
using MH.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Crm.WebApp.API
{
    /// <summary>
    /// 常用函数接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class CommonController : BaseController
    {
        public CommonController(
            IMapper mapper
            ) : base(mapper)
        {
        }

        #region 上传图片

        /// <summary>
        /// 上传 文件,并返回相对url(不包含 host port wwwroot)
        /// 上传旧图地址,则会删除旧图
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost, NoSign]
        public async Task<ResultObject> UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return Error("请先选择文件");
            }
            var wuliPath = Directory.GetCurrentDirectory();
            var fileName = Request.Query["fileName"];
            var oldPath = Request.Query["oldPath"];
            string uploadPath = "uploads" + "/" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fileExt = Path.GetExtension(file.FileName).Trim('.'); //文件扩展名，不含“.”
            string newFileName = Guid.NewGuid().ToString().Replace("-", "") + "." + fileExt; //随机生成新的文件名
            //是否存在自定义名称
            if (!string.IsNullOrEmpty(fileName))
            {
                newFileName = fileName + "." + fileExt;
            }
            var filePath = Path.Combine(uploadPath, newFileName);
            var url = $@"/{uploadPath}/{newFileName}";
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                if (!string.IsNullOrEmpty(oldPath))
                {
                    DeleteImage(wuliPath + oldPath);
                }
                return Success(url);
            }
            catch (Exception ex)
            {
                DeleteImage(wuliPath + url);
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 富文本编辑框,图片上传
        /// </summary>
        /// <returns></returns>
        [HttpPost, NoSign]
        public async Task<IActionResult> WangEditorUpload()
        {
            try
            {
                List<string> list = new List<string>();
                var files = Request.Form.Files;
                var wuliPath = Directory.GetCurrentDirectory();
                string uploadPath = "uploads" + "/" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileExt = Path.GetExtension(formFile.FileName).Trim('.'); //文件扩展名，不含“.”
                        string newFileName = Guid.NewGuid().ToString().Replace("-", "") + "." + fileExt; //随机生成新的文件名

                        var filePath = Path.Combine(uploadPath, newFileName);

                        using (var stream = new FileStream(filePath, FileMode.CreateNew))
                        {
                            await formFile.CopyToAsync(stream);
                            var url = $@"/{uploadPath}/{newFileName}";
                            list.Add(url);
                        }
                    }
                }
                return new JsonResult(list);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return new JsonResult(ex.Message);
            }

        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="url"></param>
        private void DeleteImage(string url)
        {
            try
            {
                System.IO.File.Delete(url);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        #endregion

    }
}
