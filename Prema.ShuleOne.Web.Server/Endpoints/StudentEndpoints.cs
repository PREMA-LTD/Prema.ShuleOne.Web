using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Prema.ShuleOne.Web.Backend.Database;
using Prema.ShuleOne.Web.Server.Models;
namespace Prema.ShuleOne.Web.Server.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Student").WithTags(nameof(Student));

        group.MapGet("/", async (ShuleOneDatabaseContext db) =>
        {
            return await db.Student.ToListAsync();
        })
        .WithName("GetAllStudents")
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
