﻿// <auto-generated />
using System;
using HR_Taxonomy_Change_Management.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    [DbContext(typeof(TaxonomyContext))]
    [Migration("20231003152324_changeperiod")]
    partial class changeperiod
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChangeDetailChangeStatus", b =>
                {
                    b.Property<int>("ChangeDetailId")
                        .HasColumnType("int");

                    b.Property<int>("ChangeStatusesChangeStatusId")
                        .HasColumnType("int");

                    b.HasKey("ChangeDetailId", "ChangeStatusesChangeStatusId");

                    b.HasIndex("ChangeStatusesChangeStatusId");

                    b.ToTable("ChangeDetailChangeStatus");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangeDetail", b =>
                {
                    b.Property<int>("ChangeDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChangeDetailId"));

                    b.Property<string>("ChangeText")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("CurrentL1")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("CurrentL2")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("CurrentL3")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("CurrentL4")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("CurrentL5")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("LegacyId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifyUser")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NewL1")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NewL2")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NewL3")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NewL4")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NewL5")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubmitUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("ChangeDetailId");

                    b.HasIndex("RequestId");

                    b.ToTable("ChangeDetail");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangePeriod", b =>
                {
                    b.Property<int>("ChangePeriodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChangePeriodId"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreateUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifyUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ChangePeriodId");

                    b.ToTable("ChangePeriod");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangeStatus", b =>
                {
                    b.Property<int?>("ChangeStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("ChangeStatusId"));

                    b.Property<int?>("ChangeDetailId")
                        .HasColumnType("int");

                    b.Property<int>("ChangeStatusTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ReviewText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StatusDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubmitUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("ChangeStatusId");

                    b.HasIndex("ChangeStatusTypeId");

                    b.ToTable("ChangeStatus");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangeStatusType", b =>
                {
                    b.Property<int>("ChangeStatusTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChangeStatusTypeId"));

                    b.Property<string>("StatusTypeName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ChangeStatusTypeId");

                    b.ToTable("ChangeStatusType");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.Request", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestId"));

                    b.Property<string>("Change")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<int?>("ChangePeriodId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Justification")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LegacyId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifyUser")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("RequestTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubmitUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("RequestId");

                    b.HasIndex("ChangePeriodId");

                    b.HasIndex("RequestTypeId");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.RequestStatus", b =>
                {
                    b.Property<int?>("RequestStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("RequestStatusId"));

                    b.Property<int?>("RequestId")
                        .HasColumnType("int");

                    b.Property<int>("RequestStatusTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StatusDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubmitUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("RequestStatusId");

                    b.HasIndex("RequestStatusTypeId");

                    b.ToTable("RequestStatus");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.RequestStatusType", b =>
                {
                    b.Property<int>("RequestStatusTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestStatusTypeId"));

                    b.Property<string>("StatusTypeName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RequestStatusTypeId");

                    b.ToTable("RequestStatusType");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.RequestType", b =>
                {
                    b.Property<int>("RequestTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestTypeId"));

                    b.Property<string>("RequestTypeName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RequestTypeId");

                    b.ToTable("RequestType");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.Taxonomy", b =>
                {
                    b.Property<int>("TaxonomyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaxonomyId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("TaxonomyId");

                    b.HasIndex("ParentId");

                    b.ToTable("Taxonomy");
                });

            modelBuilder.Entity("RequestRequestStatus", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<int>("RequestStatusesRequestStatusId")
                        .HasColumnType("int");

                    b.HasKey("RequestId", "RequestStatusesRequestStatusId");

                    b.HasIndex("RequestStatusesRequestStatusId");

                    b.ToTable("RequestRequestStatus");
                });

            modelBuilder.Entity("ChangeDetailChangeStatus", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.ChangeDetail", null)
                        .WithMany()
                        .HasForeignKey("ChangeDetailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.ChangeStatus", null)
                        .WithMany()
                        .HasForeignKey("ChangeStatusesChangeStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangeDetail", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.Request", "Request")
                        .WithMany("ChangeDetail")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangeStatus", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.ChangeStatusType", "StatusTypes")
                        .WithMany()
                        .HasForeignKey("ChangeStatusTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StatusTypes");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.Request", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.ChangePeriod", "ChangePeriod")
                        .WithMany("Requests")
                        .HasForeignKey("ChangePeriodId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.RequestType", "RequestType")
                        .WithMany("Requests")
                        .HasForeignKey("RequestTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChangePeriod");

                    b.Navigation("RequestType");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.RequestStatus", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.RequestStatusType", "StatusTypes")
                        .WithMany()
                        .HasForeignKey("RequestStatusTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StatusTypes");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.Taxonomy", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.Taxonomy", "ParentTaxonomy")
                        .WithMany("ChildTaxonomy")
                        .HasForeignKey("ParentId");

                    b.Navigation("ParentTaxonomy");
                });

            modelBuilder.Entity("RequestRequestStatus", b =>
                {
                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.Request", null)
                        .WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HR_Taxonomy_Change_Management.Repository.Model.RequestStatus", null)
                        .WithMany()
                        .HasForeignKey("RequestStatusesRequestStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.ChangePeriod", b =>
                {
                    b.Navigation("Requests");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.Request", b =>
                {
                    b.Navigation("ChangeDetail");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.RequestType", b =>
                {
                    b.Navigation("Requests");
                });

            modelBuilder.Entity("HR_Taxonomy_Change_Management.Repository.Model.Taxonomy", b =>
                {
                    b.Navigation("ChildTaxonomy");
                });
#pragma warning restore 612, 618
        }
    }
}
