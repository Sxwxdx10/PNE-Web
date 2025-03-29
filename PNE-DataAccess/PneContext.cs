using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PNE_core.Enums;
using PNE_core.Models;
using PNE_core.Services.Interfaces;
using System.ComponentModel;
using System.Linq;
using NetTopologySuite.IO;

namespace PNE_DataAccess;

public partial class PneContext : DbContext, IPneDbContext
{
    public PneContext()
    {
    }

    public PneContext(DbContextOptions<PneContext> options)
        :  base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=PNE;Username=pne_owner;Password=mJz7Re5jZVdl",
                x => x.UseNetTopologySuite()
            );
        }
    }

    #region tables
    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<Embarcation> Embarcations { get; set; }

    public virtual DbSet<Lavage> Lavages { get; set; }

    public virtual DbSet<Miseaeau> Miseaeaus { get; set; }

    public virtual DbSet<Notedossier> Notedossiers { get; set; }

    public virtual DbSet<Planeau> Planeaus { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    public virtual DbSet<StationLavage> StationLavages { get; set; }

    public virtual DbSet<EEE> EEEs { get; set; }

    #endregion
    #region tables de liaison

    public virtual DbSet<Embarcationutilisateur> Embarcationutilisateurs { get; set; }

    public virtual DbSet<RolesUtilisateurs> RolesUtilisateurs { get; set; }

    public virtual DbSet<CertificationUtilisateur> CertificationUtilisateurs { get; set; }

    public virtual DbSet<EmployePlaneau> EmployePlaneaus { get; set; }

    public virtual DbSet<EEEPlanEau> EEEPlanEaus { get; set; }

    #endregion

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }

    public void Add(object entity)
    {
        base.Add(entity);
    }

    public void Update(object entity)
    {
        base.Update(entity);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum<Niveau>("niveau")
            .HasPostgresEnum<TypeLavage>("type_lavage")
            .HasPostgresEnum<TypePneId>("type_pne_id")
            .HasPostgresEnum<StationPersonnelStatus>("station_personnel_status")
            .HasPostgresExtension("postgis");

        #region tables

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.HasKey(e => e.CodeCertification).HasName("certification_pkey");

            entity.ToTable("certification");

            entity.Property(e => e.CodeCertification)
                .HasColumnType("character varying")
                .HasColumnName("code_certification");
            entity.Property(e => e.NomFormation)
                .HasColumnType("character varying")
                .HasColumnName("nom_formation");
        });

        modelBuilder.Entity<Embarcation>(entity =>
        {
            entity.HasKey(e => e.IdEmbarcation).HasName("embarcation_pkey");

            entity.ToTable("embarcation");

            entity.Property(e => e.IdEmbarcation)
                .HasColumnType("character varying")
                .HasColumnName("id_embarcation");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Longueur).HasColumnName("longueur");
            entity.Property(e => e.Marque)
                .HasMaxLength(255)
                .HasColumnName("marque");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
        });

        modelBuilder.Entity<Lavage>(entity =>
        {
            entity.HasKey(e => e.IdLavage).HasName("lavage_pkey");

            entity.ToTable("lavage");

            entity.Property(e => e.IdLavage)
                .HasMaxLength(10)
                .HasColumnName("id_lavage");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.IdEmbarcation)
                .HasColumnType("character varying")
                .HasColumnName("id_embarcation");
            entity.Property(e => e.SelfServe)
                .HasColumnName("self_serve");
            entity.Property(e => e.TypeLavage)
                .HasColumnType("character varying")
                .HasColumnName("type_lavage")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<TypeLavage>(v, true)
                );

            entity.HasOne(d => d.EmbarcationNavigation).WithMany(p => p.Lavages)
                .HasForeignKey(d => d.IdEmbarcation)
                .HasConstraintName("lavage_id_embarcation_fkey");

        });

        modelBuilder.Entity<Miseaeau>(entity =>
        {
            entity.HasKey(e => e.IdMiseEau).HasName("miseaeau_pkey");

            entity.ToTable("miseaeau");

            entity.Property(e => e.IdMiseEau)
                .HasColumnName("id_mise_eau");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.DureeSejourEnJours)
                .HasColumnName("duree_en_jours")
                .HasColumnType("integer");
            entity.Property(e => e.IdEmbarcation)
                .HasColumnName("id_embarcation");
            entity.Property(e => e.IdPlanEau)
                .HasColumnName("id_plan_eau");
        });

        modelBuilder.Entity<Notedossier>(entity =>
        {
            entity.HasKey(e => e.Idnote).HasName("notedossier_pkey");

            entity.ToTable("notedossier");

            entity.Property(e => e.Idnote)
                .HasColumnName("idnote");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.IdEmbarcation)
                .HasColumnName("id_embarcation");
            entity.Property(e => e.IdPlanEau)
                .HasColumnName("id_plan_eau");
            entity.Property(e => e.Note)
                .HasColumnName("note");

            entity.HasOne(d => d.IdEmbarcationNavigation)
                .WithMany(p => p.Notedossiers)
                .HasForeignKey(d => d.IdEmbarcation)
                .HasConstraintName("notedossier_id_embarcation_utilisateur_fkey");

            entity.HasOne(d => d.IdPlanEauNavigation).WithMany(p => p.Notedossiers)
                .HasForeignKey(d => d.IdPlanEau)
                .HasConstraintName("notedossier_id_plan_eau_fkey");
        });

        modelBuilder.Entity<Planeau>(entity =>
        {
            entity.HasKey(e => e.IdPlanEau).HasName("planeau_pkey");
            entity.ToTable("planeau");
            entity.Property(e => e.IdPlanEau)
                .HasMaxLength(10)
                .HasColumnName("id_plan_eau");
            entity.Property(e => e.EmplacementString)
                .HasColumnType("character varying")
                .HasColumnName("emplacement");
            entity.Property(e => e.Nom)
                .HasColumnType("character varying")
                .HasColumnName("nom");
            entity.Property(e => e.NiveauCouleur)
                .HasColumnName("niveau_couleur")
                .HasColumnType("character varying")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<Niveau>(v, true)
                );
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.NomRole).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.NomRole)
                .HasColumnType("character varying")
                .HasColumnName("nom_role");
            entity.Property(e => e.Description)
                .HasColumnType("character varying");
        });

        modelBuilder.Entity<Role>().HasData(
            new Role { NomRole = "admin", Description = "Administarteur, employe de la CREE" },
            new Role { NomRole = "gerant", Description = "le gerant d'un plan d'eau" },
            new Role { NomRole = "employe", Description = "personne qui travail a un plan d'eau" },
            new Role { NomRole = "chercheur", Description = "personne qui fait des recherche sur la propagation des EEE" },
            new Role { NomRole = "patrouilleur", Description = "Membre des forces de l'ordre" },
            new Role { NomRole = "plaisancier", Description = "personne qui aime bien les bateaux" },
            new Role { NomRole = "superadmin", Description = "Administrateur global du système avec tous les droits" },
            new Role { NomRole = "supergerant", Description = "Gérant responsable de plusieurs plans d'eau" }
        );

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnType("character varying");
            entity.Property(e => e.DateCreation)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_creation");
            entity.Property(e => e.DisplayName)
                .HasColumnType("character varying")
                .HasColumnName("display_name");
            entity.Property(e => e.Email)
                .HasColumnType("character varying");
        });

        modelBuilder.Entity<StationLavage>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnType("character varying");
            entity.Property(e => e.Nom)
                .HasColumnType("character varying")
                .HasColumnName("nom");
            entity.Property(e => e.PositionString)
                .HasColumnType("character varying")
                .HasColumnName("Position");
            entity.Property(e => e.StationPersonnelStatus)
                .HasColumnType("character varying")
                .HasColumnName("station_personnel_status")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<StationPersonnelStatus>(v, true)
                );
        });

        modelBuilder.Entity<EEE>(entity =>
        {

        });

        #endregion

        #region tables liaison

        modelBuilder.Entity<Embarcationutilisateur>(entity =>
        {
            entity.Property(e => e.IdEmbarcation)
                .HasColumnName("id_embarcation");
            entity.Property(e => e.IdUtilisateur)
                .HasColumnName("id_utilisateur");

            entity.HasOne(d => d.IdEmbarcationNavigation).WithMany(p => p.Embarcationutilisateurs)
                .HasForeignKey(d => d.IdEmbarcation)
                .HasConstraintName("embarcationutilisateur_id_embarcation_fkey");

            entity.HasOne(d => d.UtilisateurNavigation).WithMany(p => p.Embarcationutilisateurs)
                .HasForeignKey(d => d.IdUtilisateur)
                .HasConstraintName("embarcationutilisateur_sub_fkey");
        });

        modelBuilder.Entity<RolesUtilisateurs>(entity =>
        {
            entity.HasOne(d => d.Role)
                .WithMany(p => p.RolesUtilisateurs)
                .HasForeignKey(d => d.nom_role).HasConstraintName("FK_RolesUtilisateurs_NomRole");
            entity.HasOne(d => d.Utilisateur)
                .WithMany(p => p.RolesUtilisateurs)
                .HasForeignKey(d => d.IdUtilisateur).HasConstraintName("FK_RolesUtilisateurs_IdUtilisateur");
        });

        modelBuilder.Entity<CertificationUtilisateur>(entity =>
        {
            entity.HasOne(d => d.certification)
                .WithMany(p => p.CertificationUtilisateur)
                .HasForeignKey(d => d.CodeCertification).HasConstraintName("FK_CertificationUtilisateur_CodeCertification");
            entity.HasOne(d => d.Utilisateur)
                .WithMany(p => p.CertificationUtilisateur)
                .HasForeignKey(d => d.IdUtilisateur).HasConstraintName("FK_CertificationUtilisateur_IdUtilisateur");
        });

        modelBuilder.Entity<EmployePlaneau>(entity =>
        {
            entity.HasOne(d => d.Utilisateur)
                .WithMany(p => p.EmployePlaneau)
                .HasForeignKey(d => d.IdUtilisateur).HasConstraintName("FK_EmployePlaneau_IdUtilisateur");
            entity.HasOne(d => d.Planeau)
                .WithMany(p => p.EmployePlaneau)
                .HasForeignKey(d => d.IdPlaneau).HasConstraintName("FK_EmployePlaneau_IdPlaneau");
        });

        modelBuilder.Entity<EEEPlanEau>(entity => 
        {
            entity.HasOne(d => d.EEE)
                .WithMany(p => p.EEEPlanEau)
                .HasForeignKey(d => d.IdEEE).HasConstraintName("FK_EEEPlanEau_IdEEE");
            entity.HasOne(d => d.PlanEauNavigation)
                .WithMany(p => p.EEEPlanEau)
                .HasForeignKey(d => d.IdPlanEau).HasConstraintName("FK_EEEPlanEau_IdPlanEau");
        });

        #endregion

        modelBuilder.HasSequence("serial_embarcation");
        modelBuilder.HasSequence("serial_embarcation_utilisateur");
        modelBuilder.HasSequence("serial_lavage");
        modelBuilder.HasSequence("serial_mise_eau");
        modelBuilder.HasSequence("serial_note");
        modelBuilder.HasSequence("serial_plan_eau");


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
