using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebApplication1.Model
{
    
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? StudentName { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }

        public bool IsDeleted { get; set; } // Soft delete flag

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
