using HR_Taxonomy_Change_Management_Tests.RepositoryMocks;
using HR_Taxonomy_Change_Management.Domain;
using Xunit;
using HR_Taxonomy_Change_Management.Domain.Model;

namespace HR_Taxonomy_Change_Management_Tests.DomainTests
{
    public class RequestDomainTests
    {

        [Fact]
        public async Task GetRequestAsync_ReturnsRequestDTO()
        {
            var RequestMock = new RequestMockRepository();
            var requestMock = RequestMock.GetMockRepository().Object;
            var changeMock = ChangeMockRepository.GetMockRepository();
            var domain = new RequestDomain(requestMock, changeMock.Object );

            var result = await domain.GetRequestAsync(1).ConfigureAwait(false);

            Assert.NotNull( result );
            Assert.IsAssignableFrom<RequestDTO>( result );
        }
    }
}

