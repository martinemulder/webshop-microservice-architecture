using HalfWerk.AuthenticationService.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfWerk.AuthenticationService.DAL
{
    public class AuthenticationContext : DbContext
    {
        public DbSet<CredentialEntity> CredentialEntities { get; set; }
        public DbSet<RoleEntity> RoleEntities { get; set; }
        public DbSet<CredentialRoleEntity> CredentialRoleEntities { get; set; }

        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CredentialEntity>(credential =>
            {
                credential.ToTable("Credentials");

                credential.HasKey(k => k.Id);

                credential.HasIndex(k => k.Email)
                    .IsUnique()
                    .HasFilter(null);

                credential.Property(k => k.Email).IsRequired().HasMaxLength(255);
                credential.Property(k => k.Password).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<RoleEntity>(role =>
            {
                role.ToTable("Roles");

                role.HasKey(x => x.Id);
                role.HasIndex(x => x.Id);

                role.Property(x => x.Name).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<CredentialRoleEntity>(credrole =>
            {
                credrole.ToTable("CredentialRole");

                credrole.HasKey(x => new { x.CredentialId, x.RoleId });

                credrole.HasOne(x => x.Credential)
                    .WithMany(x => x.CredentialRoles)
                    .HasForeignKey(x => x.CredentialId);

                credrole.HasOne(x => x.Role)
                    .WithMany(x => x.CredentialRoles)
                    .HasForeignKey(x => x.RoleId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
