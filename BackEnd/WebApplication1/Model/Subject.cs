using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Model
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string? SubjectName { get; set; }


        // Foreign key for Student
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [JsonIgnore]  // To avoid circular reference in the JSON serialization
        public Student? Student { get; set; }
    }
}
