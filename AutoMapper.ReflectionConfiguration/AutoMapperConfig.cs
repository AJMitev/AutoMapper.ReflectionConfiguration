namespace AutoMapper.ReflectionConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoMapper;
    using AutoMapper.Configuration;
    using AutoMapper.ReflectionConfiguration.Abstracts;

    public static class AutoMapperConfig
    {
        private const string ProfileNameTemplate = "ReflectionProfile";

        private static bool isInitialized;

        public static IMapper MapperInstance { get; set; }


        public static void RegisterMappings(params Assembly[] assemblies)
            => RegisterMappings(ProfileNameTemplate, assemblies);

        public static void RegisterMappings(string profileName = ProfileNameTemplate, params Assembly[] assemblies)
        {
            if (isInitialized)
            {
                return;
            }


            isInitialized = true;

            var types = assemblies.SelectMany(a => a.GetExportedTypes()).ToList();

            var config = new MapperConfigurationExpression();
            config.CreateProfile(profileName,
                configuration =>
                {
                    CreateFromMappings(configuration, types);
                    CreateToMappings(configuration, types);
                    CreateCustomMappings(configuration, types);
                    CreateGenericCustomMappings(configuration, types);
                });

            MapperInstance = new Mapper(new MapperConfiguration(config));
        }

        private static void CreateFromMappings(IProfileExpression configuration, List<Type> types)
            => GetFromMaps(types).ToList().ForEach(map => configuration.CreateMap(map.Source, map.Destination));


        private static void CreateToMappings(IProfileExpression configuration, List<Type> types)
            => GetToMaps(types).ToList().ForEach(map => configuration.CreateMap(map.Source, map.Destination));

        private static void CreateCustomMappings(IProfileExpression configuration, List<Type> types)
            => GetCustomMappings(types).ToList().ForEach(map => map.CreateMappings(configuration));

        private static void CreateGenericCustomMappings(IProfileExpression configuration, List<Type> types)
        => GetGenericCustomMappings(types).ToList().ForEach(map => map.CreateMappings(configuration));

        private static IEnumerable<TypesMap> GetFromMaps(IEnumerable<Type> types)
            => from t in types
               from i in t.GetTypeInfo().GetInterfaces()
               where i.GetTypeInfo().IsGenericType &&
                     i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                     !t.GetTypeInfo().IsAbstract &&
                     !t.GetTypeInfo().IsInterface
               select new TypesMap
               {
                   Source = i.GetTypeInfo().GetGenericArguments()[0],
                   Destination = t,
               };

        private static IEnumerable<TypesMap> GetToMaps(IEnumerable<Type> types)
            => from t in types
               from i in t.GetTypeInfo().GetInterfaces()
               where i.GetTypeInfo().IsGenericType &&
                     i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                     !t.GetTypeInfo().IsAbstract &&
                     !t.GetTypeInfo().IsInterface
               select new TypesMap
               {
                   Source = t,
                   Destination = i.GetTypeInfo().GetGenericArguments()[0],
               };

        private static IEnumerable<IHaveCustomMappings> GetCustomMappings(IEnumerable<Type> types)
            => from t in types
               from i in t.GetTypeInfo().GetInterfaces()
               where typeof(IHaveCustomMappings).GetTypeInfo().IsAssignableFrom(t) &&
                     !t.GetTypeInfo().IsAbstract &&
                     !t.GetTypeInfo().IsInterface &&
                     !t.GetTypeInfo().ContainsGenericParameters
               select (IHaveCustomMappings)Activator.CreateInstance(t);

        private static IEnumerable<IHaveCustomMappings> GetGenericCustomMappings(IEnumerable<Type> types)
            => from t in types
               from i in t.GetTypeInfo().GetInterfaces()
               where typeof(IHaveCustomMappings).GetTypeInfo().IsAssignableFrom(t) &&
                     !t.GetTypeInfo().IsAbstract &&
                     !t.GetTypeInfo().IsInterface &&
                     t.GetTypeInfo().ContainsGenericParameters
               select (IHaveCustomMappings)Activator.CreateInstance(t.MakeGenericType(typeof(object)));
    }
}
