using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RoyalCode.UnitOfWork.EntityFramework.Diagnostics.Internal
{
    /// <summary>
    /// <para>
    ///     Internal class for extends the <see cref="DbContext"/>.
    /// </para>
    /// </summary>
    public class UnitOfWorkDbDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        /// <summary>
        /// Creates a new info.
        /// </summary>
        /// <param name="extension"></param>
        public UnitOfWorkDbDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension) : base(extension) { }

#if NET5_0
    
        /// <inheritdoc />
        public override long GetServiceProviderHashCode() => 0;   
    
#else

    /// <inheritdoc />
    public override int GetServiceProviderHashCode() => 0;

    /// <inheritdoc />
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => true;

#endif

        /// <inheritdoc />
        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) { }

        /// <inheritdoc />
        public override bool IsDatabaseProvider => false;

        /// <inheritdoc />
        public override string LogFragment => string.Empty;
    }
}