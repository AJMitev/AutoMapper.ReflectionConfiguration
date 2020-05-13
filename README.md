# AutoMapper.ReflectionConfiguration

![NuGet Badge](https://buildstats.info/nuget/AutoMapperConfiguration.Reflection)

Automatically creates an AutoMapper profile by Reflection and provides an easy way to register your mappings.

## How it works?

By installing this package you will receive three interfaces to work with - IMapTo, IMapFrom and IHaveCustomMappings. They will give you all you need to register your mappings. During application start up all classes that implements this three interfaces will be collected and registered in new AutoMapper profile.

## How to install?

You can install this library using NuGet into your project.

```nuget
Install-Package AutoMapper.ReflectionConfiguration
```

or by using dotnet CLI

```
dotnet add package AutoMapper.ReflectionConfiguration
```

## How to use?

- First you need to register AutoMapper and provide all assemblies that contains mapping classes. This extension method will register your assemblies and will register AutoMapper into ASP.NET IOC Container that means you can inject IMapper interface whatever its needed.

```c#
public void ConfigureServices(IServiceCollection services)
{
    // Rest of registrations here.

    services.AddAutoMapper(typeof(ErrorViewModel).GetTypeInfo().Assembly, typeof(UserServiceModel).GetTypeInfo().Assembly);
}
```

After this all you need to do is to write your C# classes and specify how they should be mapped. This will be easy with IMapTo and IMapFrom interfaces. All you need to do is to implement them and specify destination/source class. In case you have some difference in property naming or want to settup some thing custom mapping you can do this by implementing IHaveCustomMappings interface. This will enforce you to implement a method in witch you can do wathever its needed. Here is some examples:

- IMapFrom

```C#
public class UserDetailsViewModel : IMapFrom<User>
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
}
```

- IMapTo

```C#
public class AddUserInputModel : IMapTo<User>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
}
```

- IHaveCustomMappings

```C#
public class UserDetailsViewModel : IMapFrom<User>, IHaveCustomMappings
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }

    public void CreateMappings(IProfileExpression configuration)
    {
        configuration.CreateMap<User, UserDetailsViewModel>()
            .ForMember(x => x.FullName, cfg => cfg.MapFrom(y => y.FirstName + y.LastName));
    }
}
```

## Credits

This repo is inspired from [Nikolay Kostov](https://github.com/NikolayIT) and the original code can be found in his project [PressCenter](https://github.com/NikolayIT/PressCenters.com/tree/master/src/Services/PressCenters.Services.Mapping). I extract mapping logic from there with a goal to make it easy for reuse in other projects.
