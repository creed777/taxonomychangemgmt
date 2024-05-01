using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace HR_Taxonomy_Change_Management.Domain
{
    public class AdminDomain : IAdminDomain
    {
        private readonly IAdminRepository? AdminRepository;
        private readonly IRequestRepository? RequestRepository;
        private readonly ITaxonomyRepository? TaxonomyRepository;
        private readonly IChangeRepository? ChangeRepository;

        public AdminDomain(IAdminRepository? adminRepository, IRequestRepository? requestRepository, ITaxonomyRepository taxononmyRepository, IChangeRepository? changeRepository)
        {
            AdminRepository = adminRepository;
            RequestRepository = requestRepository;
            TaxonomyRepository = taxononmyRepository;
            ChangeRepository = changeRepository;
        }

        public async Task<ChangePeriodDTO> GetCurrentChangePeriodAsync()
        {
            var result = await AdminRepository.GetCurrentChangePeriodAsync().ConfigureAwait(false);
            return MapChangePeriodToChangePeriodDTO(result);
        }

        public async Task<List<ChangePeriodDTO>> GetAllChangePeriodsAsync()
        {
            var periods = await AdminRepository.GetAllChangePeriodsAsync().ConfigureAwait(false);
            List<ChangePeriodDTO> mappedPeriods = new();

            foreach (ChangePeriod period in periods)
            {
                var p = new ChangePeriodDTO()
                {
                    ChangePeriodId = period.ChangePeriodId,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    CreateDate = period.CreateDate,
                    CreateUser = period.CreateUser,
                    ModifyDate = period.ModifyDate,
                    ModifyUser = period.ModifyUser,
                    IsDeleted = period.IsDeleted
                };

                mappedPeriods.Add(p);
            }

            return mappedPeriods;
        }

        /// <inheritdoc />
        public async Task<List<ChangePeriodDTO>> GetChangePeriodsAsync()
        {
            var periods = await AdminRepository.GetChangePeriodsAsync().ConfigureAwait(false);
            List<ChangePeriodDTO> mappedPeriods = new();

            foreach (ChangePeriod period in periods)
            {
                var p = new ChangePeriodDTO()
                {
                    ChangePeriodId = period.ChangePeriodId,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    CreateDate = period.CreateDate,
                    CreateUser = period.CreateUser,
                    ModifyDate = period.ModifyDate,
                    ModifyUser = period.ModifyUser,
                    IsDeleted = period.IsDeleted
                };

                mappedPeriods.Add(p);
            }

            return mappedPeriods;
        }

        ///<inheritdoc/>
        public async Task<ChangePeriodDTO?> GetChangePeriodAsync(int changePeriodId)
        {
            var result = await AdminRepository.GetChangePeriodAsync(changePeriodId).ConfigureAwait(false);
            var mappedChangePeriod = MapChangePeriodToChangePeriodDTO(result!);
            return mappedChangePeriod;
        }

        public async Task<ChangePeriodDTO?> GetChangePeriodAsync(DateTime changePeriod)
        {
            var result = await AdminRepository.GetChangePeriodAsync(changePeriod).ConfigureAwait(false);
            var mappedChangePeriod = MapChangePeriodToChangePeriodDTO(result!);
            return mappedChangePeriod;
        }

        public async Task<int> UpdateChangePeriodAsync(ChangePeriodDTO changePeriodDTO)
        {
            var mappedChangePeriod = MapChangePeriodDTOToChangePeriod(changePeriodDTO);
            return await AdminRepository.UpdateChangePeriodAsync(mappedChangePeriod).ConfigureAwait(false);
        }

        public async Task<int> DeleteChangePeriodAsync(int changePeriodId, string UserEmail)
        {
            var period = await AdminRepository.GetChangePeriodAsync(changePeriodId).ConfigureAwait(false);
            period.IsDeleted = true;
            period.ModifyDate = DateTime.UtcNow;
            period.ModifyUser = UserEmail;
            return await AdminRepository.UpdateChangePeriodAsync(period).ConfigureAwait(false);
        }

        public async Task<int> AddChangePeriodAsync(ChangePeriodDTO changePeriod)
        {
            var mappedChangePeriod = MapChangePeriodDTOToChangePeriod(changePeriod);
            return await AdminRepository.AddChangePeriodAsync(mappedChangePeriod).ConfigureAwait(false);
        }
        public ChangePeriodDTO MapChangePeriodToChangePeriodDTO(ChangePeriod changePeriod)
        {
            var changePeriodDTO = new ChangePeriodDTO()
            {
                ChangePeriodId = changePeriod.ChangePeriodId,
                CreateDate = changePeriod.CreateDate,
                CreateUser = changePeriod.CreateUser,
                EndDate = changePeriod.EndDate,
                StartDate = changePeriod.StartDate,
                ModifyDate = changePeriod.ModifyDate,
                ModifyUser = changePeriod.ModifyUser,
                IsClosed = changePeriod.IsClosed,
                IsDeleted = changePeriod.IsDeleted
            };

            return changePeriodDTO;
        }

        public ChangePeriod MapChangePeriodDTOToChangePeriod(ChangePeriodDTO changePeriodDTO)
        {
            var changePeriod = new ChangePeriod()
            {
                ChangePeriodId = changePeriodDTO.ChangePeriodId,
                CreateDate = changePeriodDTO.CreateDate,
                CreateUser = changePeriodDTO.CreateUser,
                EndDate = changePeriodDTO.EndDate,
                StartDate = changePeriodDTO.StartDate,
                ModifyDate = changePeriodDTO.ModifyDate,
                ModifyUser = changePeriodDTO.ModifyUser,
                IsClosed = changePeriodDTO.IsClosed,
                IsDeleted = changePeriodDTO.IsDeleted
            };

            return changePeriod;
        }

        public async Task<int> CountOfOpenRequestsbyPeriod(int changePeriodId)
        {
            return await RequestRepository.CountOfOpenRequestsbyPeriod(changePeriodId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> ClosePeriod(int changePeriodId)
        {
            int addResult = 0;
            int renameResult = 0;
            int moveResult = 0;

            Dictionary<string, string> results = new();
            TaxonomyRepository.BackupTaxonomyDbForClose();

            var requests = await RequestRepository.GetAllRequestsAsync(changePeriodId).ConfigureAwait(false);
            var addRequests = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Add).ToList();

            foreach (Request request in addRequests) 
            {
                var taxonomies = CreateAddTaxonomyModel(request);
                await TaxonomyRepository.ApplyAddChangesAsync(taxonomies).ConfigureAwait(false);
                addResult += 1;
            }

            var renameRequests = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Rename).ToList();
            foreach (Request request in renameRequests)
            {
                var taxonomies = CreateRenameTaxonomyModel(request);
                await TaxonomyRepository.ApplyRenameChangesAsync(taxonomies, request.RequestId).ConfigureAwait(false);
                renameResult += 1;
            }

            var moveRequests = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Move).ToList();
            foreach (Request request in moveRequests)
            {
                var taxonomies = CreateMoveTaxonomyModel(request);
                await TaxonomyRepository.ApplyMoveChangesAsync(taxonomies, request.RequestId).ConfigureAwait(false);
                moveResult += 1;
            }

            results.Add("add", string.Concat(addResult, " of ", addRequests.Count));
            results.Add("rename", string.Concat(renameResult, " of ", renameRequests.Count));
            results.Add("move", string.Concat(moveResult, " of ", moveRequests.Count));

            if (addRequests.Count() == addResult && renameRequests.Count() == renameResult && moveRequests.Count() == moveResult)
            {
                var result = await AdminRepository.CloseChangePeriodAsync(changePeriodId).ConfigureAwait(false);
                results.Add("isClosed", result?"Success":"Error Closing");
            }
            else
            {
                results.Add("isClosed", "Error closing");
            }

            return results;
        }

        private List<Taxonomy> CreateAddTaxonomyModel(Request request)
        {
            List<Taxonomy> taxonomies = new();

            foreach (ChangeDetail change in request.ChangeDetail)
            {
                int? id = null;
                for (int i = 1; i <= 5; i++)
                {
                    id = change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null) as int?;
                    if(id ==0 )
                    {

                    }
                }
            }

             foreach (ChangeDetail change in request.ChangeDetail)
            {
                int? id = null;
                for (int i = 1; i <= 5; i++)
                {
                    id = change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null) as int?;
                    var name = change.GetType().GetProperty(string.Concat("CurrentL", i)).GetValue(change, null) as string;

                    if (id != null && id == 0 &&  !taxonomies.Where(x => x.Name == name).Any())
                    {
                        int? parentId = null;
                        if (change.GetType().GetProperty(string.Concat("CurrentL", i - 1, "Id")) != null)
                        {
                           parentId = change.GetType().GetProperty(string.Concat("CurrentL", i - 1, "Id")).GetValue(change, null) as int?;
                        }

                        var taxonomy = new Taxonomy
                        {
                            ParentId = parentId,
                            Name = change.GetType().GetProperty(string.Concat("CurrentL", i)).GetValue(change, null) as string ?? string.Empty,
                            TaxonomyId = (int)id
                        };

                        //if the parentId == 0 then its a child of a new level
                        if (taxonomy.ParentId == 0)
                        {
                            Taxonomy parent = new();
                            if (change.GetType().GetProperty(string.Concat("CurrentL", i - 1, "Id")) != null)
                            {
                                var parentName = change.GetType().GetProperty(string.Concat("CurrentL", i - 1)).GetValue(change, null) as string ?? string.Empty;
                                parent = taxonomies.Where(x => x.Name == parentName).Single();
                            }
                            
                            parent.ChildTaxonomy.Add(taxonomy);
                        }
                        else
                            taxonomies.Add(taxonomy);
                    }
                }
            }

            return taxonomies;
        }

        private List<Taxonomy[,]> CreateRenameTaxonomyModel(Request request)
        {
            List<Taxonomy[,]> taxonomy = new List<Taxonomy[,]> {};

            Dictionary<Taxonomy, Taxonomy> rename = new();
            foreach(ChangeDetail change in request.ChangeDetail)
            {
                for (int i = 1; i <= 5; i++)
                {
                    var newT = new Taxonomy();
                    var currentT = new Taxonomy();

                    if (change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change,null) != null)
                    {
                        int.TryParse(change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null).ToString(), out int currentId);
                        int.TryParse(change.GetType().GetProperty(string.Concat("NewL", i, "Id")).GetValue(change, null).ToString(), out int newId);

                        if (newId == 0)
                        {
                            currentT = new Taxonomy
                            {
                                Name = change.GetType().GetProperty(string.Concat("CurrentL", i)).GetValue(change, null) as string ?? string.Empty,
                                TaxonomyId = currentId
                            };

                            newT = new Taxonomy
                            {
                                Name = change.GetType().GetProperty(string.Concat("NewL", i)).GetValue(change, null) as string ?? string.Empty,
                                TaxonomyId = newId
                            };

                            Taxonomy[,] taxonomies = new Taxonomy[1, 2] { { currentT, newT } };
                            taxonomy.Add(taxonomies);
                        }
                    }
                }
            }

            return taxonomy;
        }

        private List<Taxonomy[,]> CreateMoveTaxonomyModel(Request request)
        {
            List<Taxonomy[,]> taxonomy = new List<Taxonomy[,]> { };
            var newT = new Taxonomy();
            var currentT = new Taxonomy();
            Dictionary<Taxonomy, Taxonomy> rename = new();
            bool quit = false;

            foreach (ChangeDetail change in request.ChangeDetail)
            {
                for (int i = 1; i <= 5; i++)
                {

                    if (quit == false && change.GetType().GetProperty(string.Concat("CurrentL", i, "Id")).GetValue(change, null) == null)
                    {
                        int.TryParse(change.GetType().GetProperty(string.Concat("CurrentL", i-1, "Id")).GetValue(change, null).ToString(), out int currentId);

                        currentT = new Taxonomy
                        {
                            Name = change.GetType().GetProperty(string.Concat("CurrentL", i-1)).GetValue(change, null) as string ?? string.Empty,
                            TaxonomyId = currentId
                        };

                        quit = true;
                    }
                }

                quit = false;
                for (int i = 1; i <= 5; i++)
                {
                    if (quit == false && change.GetType().GetProperty(string.Concat("NewL", i, "Id")).GetValue(change, null) == null)
                    {
                        int.TryParse(change.GetType().GetProperty(string.Concat("NewL", i - 1, "Id")).GetValue(change, null).ToString(), out int newId);
                        newT = new Taxonomy
                        {
                            Name = change.GetType().GetProperty(string.Concat("NewL", i - 1)).GetValue(change, null) as string ?? string.Empty,
                            TaxonomyId = newId
                        };
                        
                        quit = true;
                    }
                }

                Taxonomy[,] taxonomies = new Taxonomy[1, 2] { { currentT, newT } };
                taxonomy.Add(taxonomies);

            }

            return taxonomy;
        }

        public async Task<Workbook> GetReportAsync(int changePeriodId)
        {
            var rps = new ReportProcessingService(changePeriodId, ChangeRepository, RequestRepository, TaxonomyRepository);
            return await rps.PopulateReportAsync(1).ConfigureAwait(false);            
        }
    }
}
