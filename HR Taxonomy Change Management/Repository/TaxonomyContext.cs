using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HR_Taxonomy_Change_Management.Repository
{
    public class TaxonomyContext : DbContext
    { 
        public TaxonomyContext(DbContextOptions<TaxonomyContext> options) 
            :base(options)
        { 
            
        }

        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<ChangeDetail> ChangeDetail { get; set; }
        public virtual DbSet<RequestStatus> RequestStatus { get; set; }
        public virtual DbSet<ChangeStatus> ChangeStatus { get; set; }
        public virtual DbSet<RequestType> RequestType { get; set; }
        public virtual DbSet<RequestStatusType> RequestStatusType { get; set; }
        public virtual DbSet<ChangeStatusType> ChangeStatusType { get; set; }
        public virtual DbSet<Taxonomy> Taxonomy { get; set; }
        public virtual DbSet<ChangePeriod> ChangePeriod { get; set; }
        public virtual DbSet<TaxonomyOwner> TaxonomyOwner { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // this is a 1:N - Requests and RequestType
            modelBuilder.Entity<Request>()
                .HasOne(x => x.RequestType);

            // this is a 1:N - Requests and ChangeDetail
            modelBuilder.Entity<Request>()
                .HasMany(x => x.ChangeDetail)
                .WithOne(x => x.Request);

            // this is a N:N - Requests and Status
            modelBuilder.Entity<Request>()
                .HasMany(x => x.RequestStatuses)
                .WithMany(x => x.Requests);

            modelBuilder.Entity<ChangePeriod>()
                .HasMany(x => x.Requests)
                .WithOne(x => x.ChangePeriod)
                .OnDelete(DeleteBehavior.NoAction);

            //this is a N:N - ChangeDetail and Status
            modelBuilder.Entity<ChangeDetail>()
                .HasMany(x => x.ChangeStatuses)
                .WithMany(x => x.Changes);

            // this is a 1:N - Status and StatusType
            modelBuilder.Entity<RequestStatus>()
                .HasOne(x => x.StatusTypes);

            modelBuilder.Entity<ChangeStatus>()
            .HasOne(x => x.StatusTypes);

            // this is a 1:1 - self join parent and child
            modelBuilder.Entity<Taxonomy>()
                .HasOne(x => x.ParentTaxonomy)
                .WithMany(c => c.ChildTaxonomy)
                .HasForeignKey(c => c.ParentId);

            modelBuilder.Entity<TaxonomyOwner>()
                .HasMany(x => x.Taxonomies);
        }
    }
}
