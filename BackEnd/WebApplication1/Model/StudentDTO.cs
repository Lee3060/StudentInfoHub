namespace WebApplication1.Model
{
    public class StudentDTO
    {
        public string? StudentName { get; set; }

        public string? Gender { get; set; }

        public int Age { get; set; }

        public List<SubjectDTO> Subjects { get; set; }
    }
}
