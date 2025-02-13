﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Models;
using Prema.ShuleOne.Web.Server.BulkSms;
using Prema.ShuleOne.Web.Server.Services;
namespace Prema.ShuleOne.Web.Server.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Student").WithTags(nameof(Student));

        group.MapGet("/All", async (ShuleOneDatabaseContext db) =>
        {
            return await db.Student
                .Where(s => s.admission_status != AdmissionStatus.Inactive)
                .OrderByDescending(s => s.date_of_admission)
                .ToListAsync();
        })
        .WithName("GetAllStudents")
        .WithOpenApi();


        group.MapGet("/Admissions", async (ShuleOneDatabaseContext db) =>
        {
            return await db.Student
                .Where(s => s.admission_status != AdmissionStatus.Inactive && s.date_of_admission > DateTime.UtcNow.AddDays(-5))
                .OrderByDescending(s => s.date_of_admission)
                .ToListAsync();
        })
        .WithName("GetAdmissions")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Student>, NotFound>> (int id, ShuleOneDatabaseContext db) =>
        {
            return await db.Student.AsNoTracking()
                .FirstOrDefaultAsync(model => model.id == id)
                is Student model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetStudentById")
        .WithOpenApi();

        group.MapGet("/Contact/{id}", async Task<Results<Ok<StudentContact>, NotFound>> (int id, ShuleOneDatabaseContext db) =>
        {
            var model = await db.StudentContact
                .FirstOrDefaultAsync(sc => sc.fk_student_id == id && sc.contact_priority == 1);

            return model != null
                ? TypedResults.Ok(model)
                : TypedResults.NotFound();
        })
        .WithName("GetStudentContactById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Student student, ShuleOneDatabaseContext db) =>
        {
            var affected = await db.Student
                .Where(model => model.id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.current_grade, student.current_grade)
                    .SetProperty(m => m.date_of_admission, student.date_of_admission)
                    .SetProperty(m => m.upi, student.upi)
                    .SetProperty(m => m.assessment_no, student.assessment_no)
                    .SetProperty(m => m.birth_cert_entry_no, student.birth_cert_entry_no)
                    .SetProperty(m => m.medical_needs, student.medical_needs)
                    .SetProperty(m => m.date_of_birth, student.date_of_birth)
                    .SetProperty(m => m.id, student.id)
                    .SetProperty(m => m.surname, student.surname)
                    .SetProperty(m => m.other_names, student.other_names)
                    .SetProperty(m => m.date_created, student.date_created)
                    .SetProperty(m => m.date_updated, student.date_updated)
                    .SetProperty(m => m.fk_created_by, student.fk_created_by)
                    .SetProperty(m => m.gender, student.gender)
                    .SetProperty(m => m.village_or_estate, student.village_or_estate)
                    .SetProperty(m => m.fk_residence_ward_id, student.fk_residence_ward_id)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateStudent")
        .WithOpenApi();

        group.MapPost("/", async (Student student, ShuleOneDatabaseContext db) =>
        {
            db.Student.Add(student);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Student/{student.id}",student);
        })
        .WithName("CreateStudent")
        .WithOpenApi();

        group.MapPost("/Admit/", async (StudentDto studentDto, ShuleOneDatabaseContext db, IBulkSms mobileSasa, FileGeneratorService fileGeneratorService) =>
        {
            DateTime currentDateTime = DateTime.UtcNow;
            Student student = new Student()
            {
                id = studentDto.id,
                surname = studentDto.surname,
                other_names = studentDto.other_names,
                date_created = currentDateTime,
                date_updated = currentDateTime,
                fk_created_by = studentDto.fk_created_by,
                gender = studentDto.gender,
                village_or_estate = studentDto.village_or_estate,
                fk_residence_ward_id = studentDto.fk_residence_ward_id,
                current_grade = studentDto.current_grade,
                date_of_admission = studentDto.date_of_admission,
                upi = studentDto.upi,
                assessment_no = studentDto.assessment_no,
                birth_cert_entry_no = studentDto.birth_cert_entry_no,
                medical_needs = studentDto.medical_needs,
                date_of_birth = studentDto.date_of_birth,
                admission_status = AdmissionStatus.Pending
            };
            db.Student.Add(student);
            await db.SaveChangesAsync();

            StudentContact primaryContact = new StudentContact()
            {
                id = studentDto.primary_contact.id,
                surname = studentDto.primary_contact.surname,
                other_names = studentDto.primary_contact.other_names,
                date_created = currentDateTime,
                date_updated = currentDateTime,
                fk_created_by = studentDto.primary_contact.fk_created_by,
                gender = studentDto.primary_contact.gender,
                village_or_estate = studentDto.primary_contact.village_or_estate,
                fk_residence_ward_id = studentDto.primary_contact.fk_residence_ward_id,
                contact_priority = 1,
                phone_number = studentDto.primary_contact.phone_number,
                email = studentDto.primary_contact.email,
                occupation = studentDto.primary_contact.occupation,
                relationship = studentDto.primary_contact.relationship,
                fk_student_id = student.id
            };
            db.StudentContact.Add(primaryContact);

            if(studentDto.secondary_contact != null)
            {
                StudentContact secondaryContact = new StudentContact()
                {
                    id = studentDto.secondary_contact.id,
                    surname = studentDto.secondary_contact.surname,
                    other_names = studentDto.secondary_contact.other_names,
                    date_created = currentDateTime,
                    date_updated = currentDateTime,
                    fk_created_by = studentDto.secondary_contact.fk_created_by,
                    gender = studentDto.secondary_contact.gender,
                    village_or_estate = studentDto.secondary_contact.village_or_estate,
                    fk_residence_ward_id = studentDto.secondary_contact.fk_residence_ward_id,
                    contact_priority = 2,
                    phone_number = studentDto.secondary_contact.phone_number,
                    email = studentDto.secondary_contact.email,
                    occupation = studentDto.secondary_contact.occupation,
                    relationship = studentDto.secondary_contact.relationship,
                    fk_student_id = student.id
                };
                db.StudentContact.Add(secondaryContact);
            }

            await db.SaveChangesAsync();

            //generate file
            await fileGeneratorService.GenerateFile(new Reports.AdmissionLetterDetails()
            {
                ParentName = primaryContact.other_names + " " + primaryContact.surname,
                StudentOtherNames = student.other_names,
                StudentFirstName = student.surname,
                Grade = student.current_grade.ToString(),
                AdmissionNumber = student.id.ToString(),
                SchoolContactNumber = "0712290257",
                EmailAddress = "info@lifway.co.ke",
                HeadteacherName = "Mr. Bett",
                HeadteacherContact = "0712290257",
                HeadteacherSign = "https://files.prema.co.ke/enock_signature.jpg"
            });

            //send message to parent/guardian
            //string parentName = $"{primaryContact.other_names} {primaryContact.surname}";
            //string childName = $"{student.other_names} {student.surname}";
            //string message = $"Dear {parentName}, your child, {childName}, has been admitted to Lifeway Christian School. Admission No: {student.id}. Welcome to an exciting learning journey with us!";


            //await mobileSasa.SendSms(primaryContact.phone_number, parentName, message);

            return TypedResults.Created($"/api/Student/{student.id}", student);
        })
        .WithName("AdmitStudent")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ShuleOneDatabaseContext db) =>
        {
            var affected = await db.Student
                .Where(model => model.id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteStudent")
        .WithOpenApi();
    }
}
