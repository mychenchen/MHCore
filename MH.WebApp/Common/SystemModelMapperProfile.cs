using AutoMapper;
using Currency.Common.DI;
using Currency.Models;
using Currency.Models.DB_Entity;
using Currency.Models.Mapper_Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MH.WebApp.Common
{
    public class SystemModelMapperProfile : Profile
    {
        public SystemModelMapperProfile()
        {
            // 添加尽可能多的这些行的，因为你需要映射你的对象
            //CreateMap<SystemUserEntity, SystemUserDto>().ReverseMap();

            var assembly = RuntimeHelper.GetAssemblyByName("Currency.Models");

            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                if (!type.IsDefined(typeof(AutoMappersAttribute))) continue;
                var autoMapper = type.GetCustomAttribute<AutoMappersAttribute>();

                foreach (var source in autoMapper.ToSource)
                {
                    CreateMap(type, source).ReverseMap();
                }
                //service.AddScoped(inter, type);
            }

        }
        //public static void Init()
        //{

        //    var assembly = RuntimeHelper.GetAssemblyByName("Currency.Models");

        //    var types = assembly.GetExportedTypes();
        //    foreach (var type in types)
        //    {
        //        if (!type.IsDefined(typeof(AutoMappersAttribute))) continue;
        //        var autoMapper = type.GetCustomAttribute<AutoMappersAttribute>();

        //        foreach (var source in autoMapper.ToSource)
        //        {
        //            new Profile().CreateMap(source, type).ReverseMap();
        //        }
        //        //service.AddScoped(inter, type);
        //    }

        //}

    }
}
