namespace HR_Taxonomy_Change_Management.Domain.Model
{
    public record TaxonomyGridDTO
    {
        public int GridId { get; set; }
        public int? L1Id { get; init; }
        public string? L1 { get; init; }
        public int? L1OwnerId { get; init; }
        public string? L1OwnerName { get; init; }
        public string? L1OwnerEmail { get; init; }
        public int? L2Id { get; init; }
        public string? L2 { get; init; }
        public int? L2OwnerId { get; init; }
        public string? L2OwnerName { get; init; }
        public string? L2OwnerEmail { get; init; }
        public int? L3Id { get; init; }
        public string? L3 { get; init; }
        public int? L3OwnerId { get; init; }
        public string? L3OwnerName { get; init; }
        public string? L3OwnerEmail { get; init; }
        public int? L4Id { get; init; }
        public string? L4 { get; init; }
        public int? L4OwnerId { get; init; }
        public string? L4OwnerName { get; init; }
        public string? L4OwnerEmail { get; init; }
        public int? L5Id { get; init; }
        public string? L5 { get; init; }
        public int? L5OwnerId { get; init; }
        public string? L5OwnerName { get; init; }
        public string? L5OwnerEmail { get; init; }
        public int? OwnerId { get; init; }
        public string? OwnerName { get; init; }
        public string? OwnerEmail { get; init; }
    }
}
