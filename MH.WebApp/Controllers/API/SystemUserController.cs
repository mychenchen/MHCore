using AutoMapper;
using Crm.WebApp.AuthorizeHelper;
using Currency.Common;
using Currency.Common.DIRegister;
using Currency.Models.DB_Entity;
using Currency.Models.Mapper_Entity;
using Currency.Service.IService;
using MH.WebApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MH.WebApp.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("allow_all")]
    public class SystemUserController : BaseController
    {
        protected readonly ISystemUserDAL _user;
        public SystemUserController(
            IMapper map
            ) : base(map)
        {
            this._user = DI.GetService<ISystemUserDAL>();
        }

        [NoSign]
        [HttpGet]
        public async Task<ResultObject> Demo1()
        {
            try
            {
                SystemUserEntity model = new SystemUserEntity
                {
                    Id = Guid.NewGuid(),
                    Salt = RandomCode.Number(6, true),
                    IsDelete = 0,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    LoginName = "123123",
                    LoginPwd = "123123",
                    NickName = "123123",
                    UpdateTime = DateTime.Now
                };
                var ss = await _user.Insert(model);
                return Success(ss);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [NoSign]
        [HttpGet]
        public async Task<ResultObject> Demo2()
        {
            var model = await _user.Select(10, 1, a => a.IsDelete == 0, a => a.CreateTime, true);
            var data = _map.Map<List<SystemUserDto>>(model.List);

            return SuccessPage(model.PageIndex, model.PageSize, model.TotalSize, data);
        }
    }
}
