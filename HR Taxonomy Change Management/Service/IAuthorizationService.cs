namespace HR_Taxonomy_Change_Management.Misc
{
    public interface IAuthorizationService
    {
        bool IsApprover { get; }
        bool IsOwner { get; }
        string UserEmail { get; }
        string UserName { get; }
    }
}
