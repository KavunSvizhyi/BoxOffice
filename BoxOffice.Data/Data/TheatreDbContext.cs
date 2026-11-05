using BoxOffice.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BoxOffice.Data.Data
{
    public class TheatreDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketBooking> TicketBookings { get; set; }

        public TheatreDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().HasIndex(x => x.Email);

            modelBuilder.Entity<TicketBooking>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.TicketBookings)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TicketBooking>()
                .HasOne(x => x.Ticket)
                .WithOne(x => x.TicketBooking)
                .HasForeignKey<TicketBooking>(tb => tb.TicketId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
