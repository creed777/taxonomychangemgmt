using HR_Taxonomy_Change_Management.Domain.Model;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace HR_Taxonomy_Change_Management.Component
{
    partial class TaxonomyDDLComponent
    {
        [Parameter]
        public List<TaxonomyDdlDTO> TaxonomyList { get; set; }
        [Parameter]
        public EventCallback<List<TaxonomyDdlDTO>> CurrentLevelSelectedEvent { get; set; }
        [Parameter]
        public EventCallback<List<TaxonomyDdlDTO>> NewLevelSelectedEvent { get; set; }
        [Parameter]
        public List<TaxonomyDdlDTO>? CurrentLevels { get; set; }
        [Parameter]
        public List<TaxonomyDdlDTO>? NewLevels { get; set; }
        [Parameter]
        public bool AllowCustom { get; set; }
        [Parameter]
        public string? DDLGroup { get; set; }

        public List<TaxonomyDdlDTO> L1 { get; set; } = new();
        public List<TaxonomyDdlDTO> L2 { get; set; } = new();
        public List<TaxonomyDdlDTO> L3 { get; set; } = new();
        public List<TaxonomyDdlDTO> L4 { get; set; } = new();
        public List<TaxonomyDdlDTO> L5 { get; set; } = new();

        public string? L1Value { get; set; }
        public string? L2Value { get; set; }
        public string? L3Value { get; set; }
        public string? L4Value { get; set; }
        public string? L5Value { get; set; }

        int? previousSelected = null;

        /// <summary>
        /// The goal is to have one DDL component for all Request types: new, move, remove, rename.
        /// </summary>
        public TaxonomyDDLComponent() { }

        protected override void OnParametersSet()
        {
            if (CurrentLevels == null) { CurrentLevels = new(); }
            if (NewLevels == null) { NewLevels = new(); }

            if (CurrentLevels.Count == 0)
            {
                List<TaxonomyDdlDTO> list = new List<TaxonomyDdlDTO>();
                list.Add(new TaxonomyDdlDTO() {Name="", TaxonomyId=string.Empty});
                list.AddRange(TaxonomyList.Where(x => x.ParentId == null).ToList());
                GetType().GetProperty("L1").SetValue(this, list);
            }

            if (DDLGroup == "New")
            {
                var y = 1;
                foreach (TaxonomyDdlDTO level in NewLevels)
                {
                    if (!string.IsNullOrEmpty(level.TaxonomyId))
                        {
                        GetType().GetProperty(string.Concat("L", y, "Value")).SetValue(this, level.TaxonomyId == "0" ? level.Name : level.TaxonomyId);
                        GetType().GetProperty(string.Concat("L", y)).SetValue(this, TaxonomyList.Where(x => x.ParentId == NewLevels[y - 1].ParentId).ToList(), null);
                        y++;
                    }
                }
            }
            else
            {
                var y = 1;
                foreach (TaxonomyDdlDTO level in CurrentLevels)
                {
                    if (!string.IsNullOrEmpty(level.TaxonomyId))
                    {
                        GetType().GetProperty(string.Concat("L", y, "Value")).SetValue(this, level.TaxonomyId=="0"?level.Name:level.TaxonomyId);
                        GetType().GetProperty(string.Concat("L", y)).SetValue(this, TaxonomyList.Where(x => x.ParentId == CurrentLevels[y - 1].ParentId).ToList(), null);
                        y++;
                    }
                }
            }
        }

        /// <summary>
        /// Populates a change ddl based on the parent id of the previous ddl and adds the selection to a ChangeDetailDTO object.
        /// </summary>
        /// <param name="selectedValue">This has to be a string because of the ability to edit the selected value</param>
        /// <param name="level"></param>
        public async Task GetNextLevel(string selectedValue, int level)
        {
            //Get the appropriate Taxonomy List object
            var SelectedLevels = GetType().GetProperty(DDLGroup+"Levels"??"").GetValue(this,null) as List<TaxonomyDdlDTO>;

            //if this is replacing a level that is last in the index, remove the old value. 
            //further down I'm clearing a range if a new dd value is selected to remove all the child values
            if (string.IsNullOrEmpty(selectedValue))
                return;

            //if the selected value has not changed, do nothing.
            if (level <= SelectedLevels!.Count())
            {
                if (selectedValue == SelectedLevels[level-1].TaxonomyId || selectedValue == SelectedLevels[level - 1].Name)
                return;
            }

            int.TryParse(selectedValue, out int selectedInt);

            if (selectedInt != 0)
            {
                var currentLevel = TaxonomyList.Where(x => x.TaxonomyId == selectedValue).SingleOrDefault();
                if (currentLevel != null && SelectedLevels.Count >= level && SelectedLevels[level-1].TaxonomyId != null)
                {
                    SelectedLevels.RemoveRange(level - 1, SelectedLevels.Count - (level- 1));
                };

                SelectedLevels.Add(currentLevel);
            }
            else
            {
                int parentId=0;
                if (previousSelected == null && level > 1)
                {
                    int.TryParse(GetType().GetProperty(string.Concat("L", level - 1, "Value")).GetValue(this).ToString(), out int value);
                    parentId = value;
                }

                if (SelectedLevels.Count() >= level && SelectedLevels[level - 1] != null)
                    SelectedLevels.RemoveRange(level - 1, SelectedLevels.Count - (level-1));

                SelectedLevels.Add(new TaxonomyDdlDTO()
                {
                    Name = selectedValue,
                    ParentId = parentId != 0? parentId : null
                });
            }

            previousSelected = selectedInt;

            //Event passes the new selection object to the parent page
            if (DDLGroup == "Current")
                await CurrentLevelSelectedEvent.InvokeAsync(SelectedLevels);
            else
                 await NewLevelSelectedEvent.InvokeAsync(SelectedLevels);

            //populates the next dropdown list based on the selection justmade
            List<TaxonomyDdlDTO> nextLevelTaxonomy = new();
            nextLevelTaxonomy.Add(new TaxonomyDdlDTO (){ Name = "", TaxonomyId = string.Empty });
            nextLevelTaxonomy.AddRange(TaxonomyList.Where(x => x.ParentId == selectedInt).ToList());

            if (level <= 4)
                GetType().GetProperty(string.Concat("L", level + 1)).SetValue(this, nextLevelTaxonomy);

        }

        public void ClearForm()
        {
            CurrentLevels = new();
            NewLevels = new();
            L1Value = L2Value = L3Value = L4Value = L5Value = string.Empty;

            //reset all the dropdown boxes
//            OnParametersSet();
        }

    }
}
