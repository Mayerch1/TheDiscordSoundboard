

using Microsoft.EntityFrameworkCore;

namespace TheDiscordSoundboard.Models
{

    public class SqliteContext : DbContext
    {
        public SqliteContext(DbContextOptions<SqliteContext> options) : base(options) { }

        public DbSet<Buttons> ButtonItems { get; set; }

        public DbSet<TrackData> TrackDataItems { get; set; }

        public DbSet<Config> ConfigItem { get; set; }

    }

}
