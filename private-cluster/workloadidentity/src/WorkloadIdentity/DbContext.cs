using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using WorkloadIdentity.Models;

namespace WorkloadIdentity.Data
{
    
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            
            modelBuilder.Entity<Customer>().HasData(
                new Customer() { Id = "991e0c2f-a768-40b9-9eaa-b7c31eb3fcc4", Company = "Microsoft", Level = "100", SourceId = 1001, Created = DateTime.Now.AddDays(-3), Updated =  DateTime.Now },
                new Customer() { Id = "b84178a0-b0ff-4721-96cc-5d271d93f6b9", Company = "Google", Level = "100", SourceId = 1002, Created = DateTime.Now.AddDays(-5), Updated = DateTime.Now },
                new Customer() { Id = "fbf6dc01-93f9-4772-891f-46e5a79d6e2a", Company = "Apple", Level = "100", SourceId = 1003, Created = DateTime.Now.AddDays(-7), Updated = DateTime.Now },
                new Customer() { Id = "5c3ac12f-ec83-449e-a37e-de7442cde7da", Company = "Amazon", Level = "100", SourceId = 1004, Created = DateTime.Now.AddDays(-9), Updated = DateTime.Now }
                );

            base.OnModelCreating(modelBuilder);

        }
    }
}