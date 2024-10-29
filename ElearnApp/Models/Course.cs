namespace ElearnApp.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public Category Category { get; set; }
        public ICollection<CourseImage> Images { get; set; }
    }
}
