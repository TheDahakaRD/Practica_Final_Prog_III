namespace Let_s_Do_It.Models
{
    public class TaskModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
    }
}
