using Microsoft.EntityFrameworkCore;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Guild> Guilds { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<SubReddit> SubReddits { get; set; }

        public DbSet<SubRedditHistory> SubRedditHistories { get; set; }

        public DbSet<GuildSettings> GuildSettings { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<ChannelSubscriptionMapper> ChannelSubscriptionMapper { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChannelSubscriptionMapper>()
                .HasKey(x => new {x.ChannelId, x.SubscriptionId});

            base.OnModelCreating(modelBuilder);
        }
    }
}
