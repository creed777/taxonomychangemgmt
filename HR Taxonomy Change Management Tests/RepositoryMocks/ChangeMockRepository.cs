using HR_Taxonomy_Change_Management.Repository;
using Moq;

namespace HR_Taxonomy_Change_Management_Tests.RepositoryMocks
{
    internal class ChangeMockRepository
    {
        public static Mock<IChangeRepository> GetMockRepository()
        {
            var mock = new Mock<IChangeRepository>();

            return mock;
        }
    }
}
