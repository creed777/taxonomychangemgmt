using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management_Tests.Utilities;
using Microsoft.Graph.Models;
using Moq;

namespace HR_Taxonomy_Change_Management_Tests.RepositoryMocks
{
    internal class RequestMockRepository
    {
        public TaxonomyContext mockContext { get; set; }

        public RequestMockRepository()
        {
            var x = new TestDbContext();
            mockContext = x.CreateContext();
        }

        public Mock<IRequestRepository> GetMockRepository()
        {
            var mock = new Mock<IRequestRepository>();
            var dataGen = new ReqestDataInMemory();
            var request = dataGen.GenerateRequestAsync(mockContext);

            mock.Setup(m => m.GetRequestAsync(It.IsAny<int>()))
            .Returns(() => request);
            return mock;
        }
    }
}
