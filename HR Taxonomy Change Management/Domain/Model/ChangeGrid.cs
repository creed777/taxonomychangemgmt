namespace HR_Taxonomy_Change_Management.Domain.Model
{
    public class ChangeGrid
    {
        public DateTime SubmitDate { get; set; }
        public string SubmitUser { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RequestTypeName { get; set; } = string.Empty;
        public string ChangeDescription { get; set; } = string.Empty;
    }
}
