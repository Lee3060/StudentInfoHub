using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Model;



namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext context;

      
        public StudentController(StudentDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetStudent()
        {
            var data = await context.Students.Where(s => !s.IsDeleted).Include(s => s.Subjects).ToListAsync();
            return Ok(data);
        }

        ////GET: api/student/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Student>> GetStudent(int id)
        //{
        //    //var student = await context.Students.FindAsync(id);
        //    //var student = await context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == id);
        //    var student = await context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == id); // Include the related subjects

        //    if (student == null)
        //    {
        //        return NotFound();
        //    }
        //    return student;
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            //var student = await context.Students.FindAsync(id);
            //var student = await context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == id);
            var student = await context.Students.Where(s => !s.IsDeleted).Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == id); // Include the related subjects

            if (student == null)
            {
                return NotFound();
            }

            // Map to DTO
            var studentDTO = new StudentDTO()
            {
                StudentName = student.StudentName,
                Age = student.Age,
                Gender = student.Gender,
                Subjects = student.Subjects.Select(sub => new SubjectDTO()
                {
                    SubjectName = sub.SubjectName
                }).ToList()

            };

            return studentDTO;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student std)
        {
            await context.Students.AddAsync(std);
            await context.SaveChangesAsync();
            return Ok(std);
            //return CreatedAtAction(nameof(GetStudent), new { id = std.Id }, std);
        }

        [HttpPost("{id}/subjects")]
        public async Task<ActionResult<Subject>> AddSubjectToStudent(int id, Subject subject)
        {
            var student = await context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            subject.StudentId = id; // Set the foreign key
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, subject);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Student>> UpdateStudent(int id, Student std)
        {
            if(id != std.Id)
            {
                return BadRequest();
            }
            context.Entry(std).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(std);
        }

        //[HttpPatch("{id:int}")]
        //public async Task<IActionResult> PatchStudent([FromRoute] int id, [FromBody] JsonPatchDocument<StudentDTO> patchDoc)
        //{
        //    if (patchDoc == null)
        //    {
        //        BadRequest();
        //    }
        //    var data = await context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == id);
        //    if(data == null)
        //    {
        //        return NotFound(data);
        //    }
        //    var studentDTO = new StudentDTO()
        //    {
        //        StudentName = data.StudentName,
        //        Age = data.Age,
        //        Gender = data.Gender,
        //        Subjects = data.Subjects.Select(sub => new SubjectDTO()
        //        {
        //            SubjectName = sub.SubjectName
        //        }).ToList()

        //    };
        //    patchDoc.ApplyTo(studentDTO,ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    data.StudentName = studentDTO.StudentName;
        //    data.Gender = studentDTO.Gender;

        //    await context.SaveChangesAsync();

        //    return Ok(data);

        //}

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchStudent(int id, [FromBody] JsonPatchDocument<Student> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            // Find the student by id
            var student = await context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Apply the patch to the student object
            patchDoc.ApplyTo(student, ModelState);

            // Check for model validation errors after applying the patch
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Save changes
            await context.SaveChangesAsync();

            return Ok(student);
        }

        [HttpPatch("{id}/soft-delete")]
        public async Task<ActionResult> DeletePatchStudent(int id)
        {
            var data = await context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if(data == null)
            {
                return NotFound();
            }

            // Mark the student as soft-deleted
            data.IsDeleted = true;
            await context.SaveChangesAsync();
            return Ok(data);

        }

        [HttpPatch("{id}/restore")]
        public async Task<ActionResult> RestoreStudent(int id)
        {
            var data = await context.Students.FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted);
            if(data == null)
            {
                return NotFound("Student not found or not deleted.");
            }

            data.IsDeleted = false;
            await context.SaveChangesAsync();
            return Ok(data);
        }

        [HttpPut("{studentId}/subjects/{subjectId}")]
        public async Task<IActionResult> UpdateSubject(int studentId, int subjectId, Subject updatedSubject)
        {
            // Check if the student exists
            var student = await context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Find the existing subject for this student
            var subject = await context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId && s.StudentId == studentId);
            if (subject == null)
            {
                return NotFound("Subject not found for this student.");
            }

            // Check if the student already has the same subject (but not the same subjectId)
            var duplicateSubjectExists = await context.Subjects
                .AnyAsync(s => s.SubjectName == updatedSubject.SubjectName && s.StudentId == studentId && s.Id != subjectId);

            if (duplicateSubjectExists)
            {
                return Conflict("The student already has this subject.");
            }

            // Update the subject name
            subject.SubjectName = updatedSubject.SubjectName;
            await context.SaveChangesAsync();

            return Ok(subject);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> StudentDelete(int id)
        {
            var std = await context.Students.FindAsync(id);
            if (std == null)
            {
                return NotFound();
            }
            context.Students.Remove(std);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{studentId}/subjects/{subjectId}")]
        public async Task<IActionResult> DeleteSubject(int studentId, int subjectId)
        {
            // Check if the student exists
            var student = await context.Students.Include(s => s.Subjects).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Find the subject for this student
            var subject = student.Subjects.FirstOrDefault(s => s.Id == subjectId);
            if (subject == null)
            {
                return NotFound("Subject not found for this student.");
            }

            // Remove the subject
            context.Subjects.Remove(subject);
            await context.SaveChangesAsync();

            return Ok("Subject deleted successfully.");
        }

    }
}
