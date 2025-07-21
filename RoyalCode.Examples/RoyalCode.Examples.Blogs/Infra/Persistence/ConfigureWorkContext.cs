using RoyalCode.UnitOfWork.EntityFramework;

namespace RoyalCode.Examples.Blogs.Infra.Persistence;

public static class ConfigureWorkContext
{

    public static IUnitOfWorkBuilder ConfigureBlogs(this IUnitOfWorkBuilder builder)
    {
        
        return builder;
    }
}
