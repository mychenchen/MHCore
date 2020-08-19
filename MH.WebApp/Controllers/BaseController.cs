using AutoMapper;
using Currency.Common;
using MH.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MH.WebApp.Controllers
{
    /// <summary>
    /// 通用
    /// </summary>
    public class BaseController : ControllerBase
    {
        protected readonly IMapper _map;
        public BaseController(IMapper map)
        {
            _map = map;
        }
        #region Success

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ResultObject Success()
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Success,
                message = "成功",
                resultTime = DateTime.Now,
                data = ""
            };
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResultObject SuccessNoObj(string msg)
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Success,
                message = msg,
                resultTime = DateTime.Now,
                data = null
            };
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ResultObject Success(object obj)
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Success,
                message = "成功",
                resultTime = DateTime.Now,
                data = obj
            };
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ResultObject Success(string msg, object obj)
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Success,
                message = msg,
                resultTime = DateTime.Now,
                data = obj
            };
            return res;
        }

        /// <summary>
        /// 分页列表返回
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        protected ResultObject SuccessPage(int page, int rows, int count, object list)
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Success,
                message = "查询成功",
                resultTime = DateTime.Now,
                data = new
                {
                    page,
                    rows,
                    count,
                    list
                }
            };
            return res;
        }
        #endregion

        #region Error

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ResultObject Error()
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Error,
                message = "失败",
                resultTime = DateTime.Now,
                data = ""
            };
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ResultObject Error(string msg, object obj = null)
        {
            ResultObject res = new ResultObject()
            {
                code = (int)ErrorCode.Error,
                message = msg,
                resultTime = DateTime.Now,
                data = obj
            };
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ResultObject Error(ErrorCode code, string msg, object obj)
        {
            ResultObject res = new ResultObject()
            {
                code = code.ToInt(),
                message = msg,
                resultTime = DateTime.Now,
                data = obj
            };
            return res;
        }
        #endregion
    }
}
