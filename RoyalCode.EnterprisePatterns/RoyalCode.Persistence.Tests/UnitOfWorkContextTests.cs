using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics;
using RoyalCode.UnitOfWork.Abstractions;
using Xunit;

namespace RoyalCode.Persistence.Tests;

public class UnitOfWorkContextTests
{
    [Fact]
    public void Create_From_DI()
    {
        ServiceCollection services = new();
        services.AddTransient<ICreateFromDIService, CreateFromDIService>();
        
        services.AddUnitOfWork<CreateFromDIDbContext>()
            .ConfigureDbContext(builder =>
            {
                builder.UseInMemoryDatabase(nameof(Create_From_DI));
                builder.AddInterceptors((IUnitOfWorkInitializeInterceptor)new CreateFromDIInitializerInterceptor());
                builder.UseUnitOfWork();
            });

        var sp = services.BuildServiceProvider();

        var uow = sp.GetService<IUnitOfWorkContext>();
        Assert.NotNull(uow);
        Assert.True(CreateFromDIInitializerInterceptor.Intercepted);
        Assert.True(CreateFromDIInitializerInterceptor.ServiceFounded);
    }
}

#region Create_From_DI classes 

public class CreateFromDIDbContext : DbContext
{
    public CreateFromDIDbContext(DbContextOptions<CreateFromDIDbContext> options) : base(options) { }
}

public interface ICreateFromDIService { }

public class CreateFromDIService : ICreateFromDIService { }

public class CreateFromDIInitializerInterceptor : IUnitOfWorkInitializeInterceptor
{
    public static bool Intercepted = false;
    public static bool ServiceFounded = false;
    
    public void Initializing(DbContext context)
    {
        var service = context.GetService<ICreateFromDIService>();
        
        Intercepted = true;
        ServiceFounded = service != null;
    }
}

#endregion