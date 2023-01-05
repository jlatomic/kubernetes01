using Microsoft.EntityFrameworkCore;

namespace kubernetes1
{
    public class MiModelDb : DbContext
    {
        public MiModelDb(DbContextOptions<MiModelDb> options) : base(options)
        { }

        public DbSet<MiModel> MiModelProp => Set<MiModel>();

    }
}
