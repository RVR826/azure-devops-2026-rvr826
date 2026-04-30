using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Votex.DataAccess.Models;

namespace Votex.DataAccess
{
    public class VotexDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Vote> Votes { get; set; } = null!;
        public DbSet<Voting> Votings { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;
        
        public VotexDbContext(DbContextOptions<VotexDbContext> options) : base(options)
        { }

        public VotexDbContext() : base()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Voting>()
                .HasMany(e => e.Options)
                .WithOne(e => e.Voting)
                .HasForeignKey(e => e.VotingId)
                .HasPrincipalKey(e => e.Id);

            modelBuilder.Entity<Voting>()
                .HasMany(e => e.Votes)
                .WithOne(e => e.Voting)
                .HasForeignKey(e => e.VotingId)
                .HasPrincipalKey(e => e.Id);

            modelBuilder.Entity<Voting>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Votings)
                .UsingEntity(j => j.ToTable("UserVoting"));
            
            modelBuilder.Entity<Voting>()
                .HasMany(e => e.AlreadyVoted)
                .WithMany(e => e.AlreadyVotedFor)
                .UsingEntity(j => j.ToTable("AlreadyVoted"));

            base.OnModelCreating(modelBuilder);

        }
    }
}
