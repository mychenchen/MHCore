using AutoMapper;
using Crm.WebApp.AuthorizeHelper;
using Currency.Common;
using Currency.Common.Caching;
using Currency.Common.DIRegister;
using Currency.Common.Redis;
using Currency.Models.DB_Entity;
using Currency.Models.Mapper_Entity;
using Currency.Service.IService;
using Currency.Weixin;
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
        public readonly BasicApi _basicApi;
        public readonly RedisManager _redisManager;
        public readonly CoreMemoryCache _cache;
        public SystemUserController(
            IMapper map,
            ISystemUserDAL systemUserDAL,
            BasicApi basicApi,
            RedisManager redisManager,
            CoreMemoryCache cache
            ) : base(map)
        {
            _user = systemUserDAL;
            _basicApi = basicApi;
            redisManager.DbNum = 2;
            _redisManager = redisManager;
            _cache = cache;
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


        /// <summary>
        /// 设置redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet, NoSign]
        public ResultObject SaveRedisValue(string key, string value)
        {
            //_basicApi.GetAccessToken();
            var model = new user
            {
                name = value,
                sex = 17,
                time = DateTime.Now
            };
            _redisManager.DbNum = 1;
            var flag = _redisManager.StringSet(key, model);

            return Success("保存成功:" + flag);
        }

        /// <summary>
        /// 获取redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet, NoSign]
        public ResultObject PullRedisValue(string key)
        {
            var flag = _redisManager.StringGet<user>(key);

            return Success(flag);
        }

        /// <summary>
        /// 设置cache缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet, NoSign]
        public ResultObject SaveCache(string key, string value)
        {
            //_basicApi.GetAccessToken();
            var model = new user
            {
                name = value,
                sex = 17,
                time = DateTime.Now
            };
            var flag = _cache.Set(key, model);

            return Success(flag);
        }

        /// <summary>
        /// 获取ceche缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet, NoSign]
        public ResultObject PullCache(string key)
        {
            var flag = _cache.Get(key);
            if (!string.IsNullOrWhiteSpace(flag))
            {
                return Success(flag.ToObject<user>());
            }
            return Success();
        }

        public class user
        {
            public string name { get; set; }
            public int sex { get; set; }
            public DateTime time { get; set; }
        }
    }
}
