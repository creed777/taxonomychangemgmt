using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository;

namespace HR_Taxonomy_Change_Management.Domain
{
    public class TaxonomyDomain : ITaxonomyDomain
    {
        public ITaxonomyRepository Repository { get; set; }

        public TaxonomyDomain(ITaxonomyRepository taxonomyRepository) 
        {
            Repository = taxonomyRepository;
        }

        public async Task<List<TaxonomyDTO>> GetAllTaxonomyAsync()
        {
            List<TaxonomyDTO> OwnedTaxonomies = new();
            List<TaxonomyDTO> taxonomyList = new();
            var result = await Repository.GetAllTaxonomyAsync().ConfigureAwait(false);

            foreach (var item in result) 
            {
                var t = new TaxonomyDTO()
                {
                    TaxonomyId = item.TaxonomyId,
                    Name = item.Name,
                    ParentId = item.ParentId,
                    OwnerId = item.Owner!=null?item.Owner.OwnerId:null,
                    OwnerEmail = item.Owner != null?item.Owner.OwnerEmail:string.Empty,
                    OwnerName = item.Owner != null ? item.Owner.OwnerName: string.Empty,
                };

                t.ChildTaxonomy = new();
                foreach(var child in item.ChildTaxonomy)
                {
                    var ct = new TaxonomyDTO()
                    {
                        TaxonomyId = child.TaxonomyId,
                        Name = child.Name,
                        ParentId = child.ParentId,
                        OwnerId = item.Owner != null ? item.Owner.OwnerId: null,
                        OwnerEmail = item.Owner != null ? item.Owner.OwnerEmail : string.Empty,
                        OwnerName = item.Owner != null ? item.Owner.OwnerName: string.Empty,
                    };

                    if(t.OwnerId != null)
                        OwnedTaxonomies.Add(ct);

                    t.ChildTaxonomy.Add(ct);
                }

                taxonomyList.Add(t);
            }

            return taxonomyList;
        }

        public async Task<List<TaxonomyDTO>> GetAllTaxonomyOwnersAsync()
        {
            List<TaxonomyDTO> OwnedTaxonomies = new();
            var result = await Repository.GetAllTaxonomyAsync().ConfigureAwait(false);

            foreach (var item in result)
            {
                var t = new TaxonomyDTO()
                {
                    TaxonomyId = item.TaxonomyId,
                    Name = item.Name,
                    ParentId = item.ParentId,
                    OwnerId = item.Owner != null ? item.Owner.OwnerId : null,
                    OwnerEmail = item.Owner != null ? item.Owner.OwnerEmail : string.Empty,
                    OwnerName = item.Owner != null ? item.Owner.OwnerName : string.Empty,
                };

                t.ChildTaxonomy = new();
                foreach (var child in item.ChildTaxonomy)
                {
                    var ct = new TaxonomyDTO()
                    {
                        TaxonomyId = child.TaxonomyId,
                        Name = child.Name,
                        ParentId = child.ParentId,
                        OwnerId = item.Owner != null ? item.Owner.OwnerId : null,
                        OwnerEmail = item.Owner != null ? item.Owner.OwnerEmail : string.Empty,
                        OwnerName = item.Owner != null ? item.Owner.OwnerName : string.Empty,
                    };

                    if (t.OwnerId != null)
                        OwnedTaxonomies.Add(ct);
                }
            }

            return OwnedTaxonomies;
        }

    }
}
 