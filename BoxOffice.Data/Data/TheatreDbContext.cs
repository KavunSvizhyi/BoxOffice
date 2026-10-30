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

            modelBuilder.Entity<TicketBooking>()
                .HasOne(tb => tb.Customer)
                .WithMany(c => c.TicketBookings)
                .HasForeignKey(tb => tb.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // C:\Users\maksr\source\repos\BoxOffice\BoxOffice.Data\Data\Dummy Data\Person.json
            DirectoryInfo? projectDir = new DirectoryInfo(AppContext.BaseDirectory).Parent?.Parent?.Parent;

            string path = Path.Combine($"{projectDir!.FullName}.Data", "Data", "Dummy Data");

            modelBuilder.Entity<Person>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<Person>>(File.ReadAllText($@"{path}\Person.json"))!);

            modelBuilder.Entity<Genre>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<Genre>>(File.ReadAllText($@"{path}\Genre.json"))!);

            modelBuilder.Entity<Author>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<Author>>(File.ReadAllText($@"{path}\Author.json"))!);

            modelBuilder.Entity<Customer>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<Customer>>(File.ReadAllText($@"{path}\Customer.json"))!);

            modelBuilder.Entity<Performance>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<Performance>>(File.ReadAllText($@"{path}\Performance.json"))!);

            modelBuilder.Entity<Ticket>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<Ticket>>(File.ReadAllText($@"{path}\Ticket.json"))!);

            modelBuilder.Entity<TicketBooking>()
                .HasData(JsonSerializer.Deserialize<IEnumerable<TicketBooking>>(File.ReadAllText($@"{path}\TicketBooking.json"))!);
        }
    }
}
