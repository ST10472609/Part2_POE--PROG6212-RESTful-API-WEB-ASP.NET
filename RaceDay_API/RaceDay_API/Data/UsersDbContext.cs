using Microsoft.EntityFrameworkCore;
using RaceDay_API.Models;

namespace RaceDay_API.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }
        public DbSet<Participants> Participants { get; set; }

        public DbSet<ParticipantsLogin> ParticipantsLogin { get; set; }

        public DbSet<Organizer> Organizers { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventEnrollments> EventEnrollments { get; set; }

        public DbSet<Resultss> Resultss { get; set; }

    }
}