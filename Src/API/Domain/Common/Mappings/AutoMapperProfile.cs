using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domain.Common.Mappings
{
    public static class AutoMapperProfile
    {
        public static IServiceCollection AddMapProfiles(this IServiceCollection container, string name)
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> assList = ass.ToList<Assembly>();
            List<Assembly> medAssList = assList.Where(x => x.FullName.Contains(name)).ToList();
            List<System.Reflection.Assembly> rtn = new();

            foreach (var lass in medAssList)
                rtn.AddRange( GetAssemblies(lass));
            container.AddAutoMapper(rtn);

            return container;
        }

        private static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
        {
            yield return typeof(Profile).GetTypeInfo().Assembly;
            if (assembly != null)
            {
                yield return assembly;
            }
        }
    }
}
