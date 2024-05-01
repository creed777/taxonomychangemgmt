using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Domain.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.FeatureManagement;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;


namespace HR_Taxonomy_Change_Management.Pages
{

    partial class Taxonomy : BasePage
    {
        [Inject] private ITaxonomyDomain? TaxonomyDomain { get; set; }

        private List<TaxonomyDTO> TaxonomyList { get; set; } = new();
        private record TaxonomyRecord(int Id, string Name, int? ParentId, int? OwnerId, string? OwnerEmail, string? OwnerName);
        private List<TaxonomyRecord>? _taxonomies = new List<TaxonomyRecord>();
        private bool LoaderVisible { get; set; } = false;
        int[][]? indexSelectedIds = new int[6][];
        private List<TaxonomyGridDTO> TaxonomyOwners { get; set; } = new();
        private List<TaxonomyDTO> L1List { get; set; } = new();
        private List<TaxonomyDTO> L2List { get; set; } = new();
        private List<TaxonomyDTO> L3List { get; set; } = new();
        private List<TaxonomyDTO> L4List { get; set; } = new();
        private List<TaxonomyDTO> L5List { get; set; } = new();

        private List<int> SelectedL1Ids { get; set; } = new();
        private List<int> SelectedL2Ids { get; set; } = new();
        private List<int> SelectedL3Ids { get; set; } = new();
        private List<int> SelectedL4Ids { get; set; } = new();
        private List<int> SelectedL5Ids { get; set; } = new();  

        private List<TaxonomyGridDTO> _grid = new();
        private List<TaxonomyGridDTO> filtered_grid = new();

        private TelerikGrid<TaxonomyGridDTO>? OwnerGrid { get; set; }
        private bool txOwnerPopupIsEnabled { get; set; }

        public Taxonomy()
        {
        }

        protected async override Task OnInitializedAsync()
        {
            LoaderVisible = true;
            TaxonomyList = await TaxonomyDomain.GetAllTaxonomyAsync();
            L1List = TaxonomyList.Where(x => x.ParentId == null).ToList();

            foreach (var x in TaxonomyList)
            {
                var temp = new TaxonomyRecord(x.TaxonomyId, x.Name, x.ParentId, x.OwnerId, x.OwnerEmail, x.OwnerName);
                _taxonomies.Add(temp);
            }

            LoadOwnerGrid();
            GetFilterLists();
            LoaderVisible = false;
        }

        private void GetFilterLists()
        {
            for (int i = 0; i < _grid.Count; i++)
            {
                _grid[i].GridId = i + 1;
            }

            L1List = TaxonomyList.Where(x => x.ParentId == null).ToList();

            var l2 = _grid.DistinctBy(x => x.L2).Select(x => x.L2Id).ToList();
            L2List = TaxonomyList.Where(x => l2.Contains(x.TaxonomyId)).ToList();
            var l3 = _grid.DistinctBy(x => x.L3).Select(x => x.L3Id).ToList();
            L3List = TaxonomyList.Where(x => l3.Contains(x.TaxonomyId)).ToList();
            var l4 = _grid.DistinctBy(x => x.L4).Select(x => x.L4Id).ToList();
            L4List = TaxonomyList.Where(x => l4.Contains(x.TaxonomyId)).ToList();
            var l5 = _grid.DistinctBy(x => x.L5).Select(x => x.L5Id).ToList();
            L5List = TaxonomyList.Where(x => l5.Contains(x.TaxonomyId)).ToList();
        }

        public List<TaxonomyGridDTO> LoadOwnerGrid()
        {
            _grid = new List<TaxonomyGridDTO>();

            GetLevel(null, 1, new());
            
            //this is deleting the "extra" levels created by GetLevel().  For example when building the node Benefits > Benefits Management > Benefits Enrollment Administration, 
            // it will also crate the nodes "Benefits" and "Benefits > Benefits Management".  These aren't needed so I'm looking for the lowest level node and deleteing anything higher in that grouping.
            var groupL4 = _grid.GroupBy(x => new {x.L1, x.L2, x.L3, x.L4});
            foreach (var group in groupL4)
            {
                if (group.Count() > 1)
                {
                    var item = group.Where(x => x.L5Id == null).First();
                    var index = _grid.IndexOf(_grid.Find(x => x == item) ?? new TaxonomyGridDTO());
                    if (index > -1)
                        _grid.RemoveAt(index);
                }
            }

            var groupL3 = _grid.GroupBy(x => new { x.L1, x.L2, x.L3});
            foreach (var group in groupL3)
            {
                if (group.Count() > 1)
                {
                    var item = group.Where(x => x.L4Id == null).First();
                    var index = _grid.IndexOf(_grid.Find(x => x == item) ?? new TaxonomyGridDTO());
                    if (index > -1)
                        _grid.RemoveAt(index);
                }
            }

            var groupL2 = _grid.GroupBy(x => new { x.L1, x.L2 });
            foreach (var group in groupL2)
            {
                if (group.Count() > 1)
                {
                    var item = group.Where(x => x.L3Id == null).First();
                    var index = _grid.IndexOf(_grid.Find(x => x == item) ?? new TaxonomyGridDTO());
                    if (index > -1)
                        _grid.RemoveAt(index);
                }
            }

            var groupL1 = _grid.GroupBy(x => new { x.L1 });
            foreach (var group in groupL1)
            {
                if (group.Count() > 1)
                {
                    var item = group.Where(x => x.L2Id == null).FirstOrDefault();
                    var index = _grid.IndexOf(_grid.Find(x => x == item) ?? new TaxonomyGridDTO());
                    if(index > -1)
                        _grid.RemoveAt(index);
                }
            }

            return filtered_grid = _grid;

        }

        private void GetLevel(int? id, int level, TaxonomyGridDTO gridItem)
        {
            var items = _taxonomies.Where(item => item.ParentId == (id==0?null:id));

            foreach (var item in items)
            {
                var nextLevel = level + 1;
                var newGridItem = GetGridItem(item, level, gridItem);
                if (newGridItem is not null)
                    GetLevel(item.Id, nextLevel, newGridItem);
            }


            if (gridItem != new TaxonomyGridDTO())
                _grid.Add(gridItem with { });
        }

        private TaxonomyGridDTO? GetGridItem(TaxonomyRecord tax, int level, TaxonomyGridDTO gridItem)
        {
            var grid = level switch
            {
                1 => gridItem with { L1 = tax.Name, L1Id = tax.Id, L1OwnerId = tax.OwnerId, L1OwnerEmail = tax.OwnerEmail, L1OwnerName = tax.OwnerName },
                2 => gridItem with { L2 = tax.Name, L2Id = tax.Id, L2OwnerId = tax.OwnerId, L2OwnerEmail = tax.OwnerEmail, L2OwnerName = tax.OwnerName },
                3 => gridItem with { L3 = tax.Name, L3Id = tax.Id, L3OwnerId = tax.OwnerId, L3OwnerEmail = tax.OwnerEmail, L3OwnerName = tax.OwnerName },
                4 => gridItem with { L4 = tax.Name, L4Id = tax.Id, L4OwnerId = tax.OwnerId, L4OwnerEmail = tax.OwnerEmail, L4OwnerName = tax.OwnerName },
                5 => gridItem with { L5 = tax.Name, L5Id = tax.Id, L5OwnerId = tax.OwnerId, L5OwnerEmail = tax.OwnerEmail, L5OwnerName = tax.OwnerName },
                _ => null
            };

            return grid;
        }

        private void FilterGrid(int level, List<int> list)
        {
            indexSelectedIds[level] = list.ToArray();
            filtered_grid = _grid;

            for(int i=1; i < 5; i++)
            {
                if(indexSelectedIds[i] != null && indexSelectedIds[i].ToList().Count() > 0)
                {

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
                    filtered_grid = i switch
                    {
                        1 => _grid.Where(x => indexSelectedIds[i].ToList().Contains((int)x.L1Id)).ToList(),
                        2 => _grid.Where(x => indexSelectedIds[i].ToList().Contains((int)x.L2Id)).ToList(),
                        3 => _grid.Where(x => indexSelectedIds[i].ToList().Contains((int)x.L3Id)).ToList(),
                        4 => _grid.Where(x => indexSelectedIds[i].ToList().Contains((int)x.L4Id)).ToList(),
                        5 => _grid.Where(x => indexSelectedIds[i].ToList().Contains((int)x.L5Id)).ToList(),
                    };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

                };
            }
        }

        public void Openpopup()
        {
            PopupRef.AnchorAlign.HorizontalAlign = PopupHorizontalAlign.Right;
            PopupRef.AnchorAlign.VerticalAlign = PopupVerticalAlign.Top;
            PopupRef.Popup_PositionSet();
            PopupRef.Popup_OpenAsync();
        }
    }
}
