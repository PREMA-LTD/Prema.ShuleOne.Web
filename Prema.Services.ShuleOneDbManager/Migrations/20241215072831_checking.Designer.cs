﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Prema.ShuleOne.Web.Server.Database;

#nullable disable

namespace Prema.Services.ShuleOneDbManager.Migrations
{
    [DbContext(typeof(ShuleOneDatabaseContext))]
    [Migration("20241215072831_checking")]
    partial class checking
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.2.24128.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Document", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("date_updated")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("document_type")
                        .HasColumnType("int");

                    b.Property<string>("file_name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("fk_created_by")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("fk_student_id")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("fk_student_id");

                    b.ToTable("document");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Employee", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("date_updated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("email")
                        .HasColumnType("longtext");

                    b.Property<string>("fk_created_by")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("fk_employee_bank_details")
                        .HasColumnType("int");

                    b.Property<int>("fk_residence_ward_id")
                        .HasColumnType("int");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<string>("kra")
                        .HasColumnType("longtext");

                    b.Property<string>("nssf")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("other_names")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("phone_number")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("shif")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("village_or_estate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("fk_employee_bank_details");

                    b.HasIndex("fk_residence_ward_id");

                    b.ToTable("employee");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.EmployeeBankDetails", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("account_no")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("bank_branch")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("bank_name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("employee_bank_details");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.FeeType", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("fee_type");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Grade", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("int");

                    b.Property<int?>("Teacherid")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("Teacherid");

                    b.ToTable("grade");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.County", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("county");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.Subcounty", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("fk_county_id")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("fk_county_id");

                    b.ToTable("subcounty");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.Ward", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("fk_subcounty_id")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("fk_subcounty_id");

                    b.ToTable("ward");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.SMSFailure", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("date_time")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("error")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("fk_sms_record_id")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("fk_sms_record_id");

                    b.ToTable("sms_failure");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.SMSRecord", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTime>("date_time_sent")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("failure_count")
                        .HasColumnType("int");

                    b.Property<string>("message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("recipient_contact")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("recipient_name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("sms_record");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Student", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("admission_status")
                        .HasColumnType("int");

                    b.Property<string>("assessment_no")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("birth_cert_entry_no")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("current_grade")
                        .HasColumnType("int");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("date_of_admission")
                        .HasColumnType("datetime(6)");

                    b.Property<DateOnly>("date_of_birth")
                        .HasColumnType("date");

                    b.Property<DateTime>("date_updated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("fk_created_by")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("fk_residence_ward_id")
                        .HasColumnType("int");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<string>("medical_needs")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("other_names")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("upi")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("village_or_estate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("fk_residence_ward_id");

                    b.ToTable("student");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.StudentContact", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<byte>("contact_priority")
                        .HasColumnType("tinyint unsigned");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("date_updated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("email")
                        .HasColumnType("longtext");

                    b.Property<string>("fk_created_by")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("fk_residence_ward_id")
                        .HasColumnType("int");

                    b.Property<int>("fk_student_id")
                        .HasColumnType("int");

                    b.Property<int>("gender")
                        .HasColumnType("int");

                    b.Property<string>("occupation")
                        .HasColumnType("longtext");

                    b.Property<string>("other_names")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("phone_number")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("relationship")
                        .HasColumnType("int");

                    b.Property<string>("surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("village_or_estate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("fk_residence_ward_id");

                    b.HasIndex("fk_student_id");

                    b.ToTable("student_contact");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Subject", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("subject");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.TutionFeesStructure", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("id"));

                    b.Property<decimal>("amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("date_updated")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("fk_grade_id")
                        .HasColumnType("int");

                    b.Property<int>("term")
                        .HasColumnType("int");

                    b.Property<string>("updated_by")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("fk_grade_id");

                    b.ToTable("tution_fees_structure");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Teacher", b =>
                {
                    b.HasBaseType("Prema.ShuleOne.Web.Server.Models.Employee");

                    b.Property<string>("highest_qualification")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("start_date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("subjects")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("tsa_no")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.ToTable("teacher");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Document", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Student", "Student")
                        .WithMany("Documents")
                        .HasForeignKey("fk_student_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Employee", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.EmployeeBankDetails", "EmployeeBankDetails")
                        .WithMany()
                        .HasForeignKey("fk_employee_bank_details")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Location.Ward", "Ward")
                        .WithMany("Employee")
                        .HasForeignKey("fk_residence_ward_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeBankDetails");

                    b.Navigation("Ward");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Grade", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Teacher", null)
                        .WithMany("grades")
                        .HasForeignKey("Teacherid");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.Subcounty", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Location.County", "County")
                        .WithMany("Subcounties")
                        .HasForeignKey("fk_county_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("County");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.Ward", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Location.Subcounty", "Subcounty")
                        .WithMany("Wards")
                        .HasForeignKey("fk_subcounty_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subcounty");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.SMSFailure", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.SMSRecord", "SMSRecord")
                        .WithMany("SMSFailures")
                        .HasForeignKey("fk_sms_record_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SMSRecord");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Student", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Location.Ward", "Ward")
                        .WithMany("Student")
                        .HasForeignKey("fk_residence_ward_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ward");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.StudentContact", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Location.Ward", "Ward")
                        .WithMany("StudentContact")
                        .HasForeignKey("fk_residence_ward_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("fk_student_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");

                    b.Navigation("Ward");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.TutionFeesStructure", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Grade", "grade")
                        .WithMany()
                        .HasForeignKey("fk_grade_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("grade");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Teacher", b =>
                {
                    b.HasOne("Prema.ShuleOne.Web.Server.Models.Employee", null)
                        .WithOne()
                        .HasForeignKey("Prema.ShuleOne.Web.Server.Models.Teacher", "id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.County", b =>
                {
                    b.Navigation("Subcounties");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.Subcounty", b =>
                {
                    b.Navigation("Wards");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Location.Ward", b =>
                {
                    b.Navigation("Employee");

                    b.Navigation("Student");

                    b.Navigation("StudentContact");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.SMSRecord", b =>
                {
                    b.Navigation("SMSFailures");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Student", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("Prema.ShuleOne.Web.Server.Models.Teacher", b =>
                {
                    b.Navigation("grades");
                });
#pragma warning restore 612, 618
        }
    }
}
