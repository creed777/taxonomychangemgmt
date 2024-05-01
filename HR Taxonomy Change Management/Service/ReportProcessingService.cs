using HR_Taxonomy_Change_Management.Domain.Model;
using HR_Taxonomy_Change_Management.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Groups.Item.Team.Channels.Item.DoesUserHaveAccessuserIdUserIdTenantIdTenantIdUserPrincipalNameUserPrincipalName;
using Telerik.Blazor;
using Telerik.Blazor.Components.Editor;
using Telerik.Windows.Documents.Flow.Model.Watermarks;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.Model.Printing;
using Telerik.Windows.Documents.Spreadsheet.PropertySystem;
using Telerik.Windows.Documents.Spreadsheet.Utilities;

namespace HR_Taxonomy_Change_Management.Misc
{
    public class ReportProcessingService
    {
        readonly IChangeRepository ChangeRepository;
        readonly IRequestRepository RequestRepository;
        readonly ITaxonomyRepository TaxRepository;

        private string ReportDirectory = ".\\Misc\\";
        private string ReportName = "Report_Template.xlsx";

        [CascadingParameter] public DialogFactory? Dialogs { get; set; }

        public ReportProcessingService(int? changePeriodId, IChangeRepository changeRepository, IRequestRepository requestRepository, ITaxonomyRepository taxRepository)
        {
            ChangeRepository = changeRepository;
            RequestRepository = requestRepository;
            TaxRepository = taxRepository;
        }

        public async Task<Workbook> PopulateReportAsync(int changePeriodId)
        {

            using (var fs = this.GetFileStream(ReportName))
            {
                if (changePeriodId == 0)
                    throw new Exception("oops");

                var requests = await RequestRepository.GetRequestsAsyncByChangePeriod((int)changePeriodId).ConfigureAwait(false);
                var activationCount = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Add).Sum(x => x.ChangeDetail.Count);
                var deactivationCount = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Remove).Sum(x => x.ChangeDetail.Count);

                XlsxFormatProvider formatProvider = new();
                var workbook = formatProvider.Import(fs);
                var worksheet = workbook.ActiveWorksheet;
                worksheet.Name = "ChangePeriod"+changePeriodId;
                HeaderFooterSettings settings = worksheet.WorksheetPageSetup.HeaderFooterSettings;
                settings.Header.CenterSection.Text = "Change Period Open";

                //Get Activation and Deactivation counts by L1
                var taxList = await TaxRepository.GetAllTaxonomyAsync().ConfigureAwait(false);

                Dictionary<string, int> l1ActivationTotals = new();
                Dictionary<string, int> l1DeactivationTotals = new();

                var addChanges = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Add).SelectMany(x => x.ChangeDetail);
                var addGroups = addChanges.GroupBy(x => x.NewL1).ToList();
                addGroups.ForEach(x => l1ActivationTotals.Add(x.Key!, x.Count()));

                var deleteChanges = requests.Where(x => x.RequestTypeId == (int)RequestTypeEnum.Remove).SelectMany(x => x.ChangeDetail);
                var deleteGroups = deleteChanges.GroupBy(x => x.NewL1).ToList();
                deleteGroups.ForEach(x => l1DeactivationTotals.Add(x.Key!, x.Count()));

                CellStyle cellStyle = workbook.Styles.Add("NewLevel", CellStyleCategory.Custom);
                cellStyle.BeginUpdate();
                cellStyle.ForeColor = new Telerik.Documents.Common.Model.ThemableColor(Telerik.Documents.Media.Color.FromRgb(255, 0, 0));
                cellStyle.EndUpdate();

                cellStyle = workbook.Styles.Add("NoStyle", CellStyleCategory.Custom);
                cellStyle.BeginUpdate();
                cellStyle.ForeColor = new Telerik.Documents.Common.Model.ThemableColor(Telerik.Documents.Media.Color.FromRgb(0, 0, 0));
                cellStyle.EndUpdate();

                //there are named ranges in the worksheet for Activation, Deactivation and Approved Changes.  This way, the only row/cell coodinates that are hard-coded are the Row Count totals.
                //this was kind of "hokey" to me.  But as it works right now, you have to get the named range, then the workbook and the rows/cells.  So we have to repeat this for every named range to update.
                //there's an open item in their backlog to refine this process.
                //https://docs.telerik.com/devtools/document-processing/knowledge-base/set-value-using-named-range
                foreach (var name in workbook.Names)
                {
                    if(name.Name == "TypeTotal")
                    {
                        string[] refersToElements = name.RefersTo.Split("!".ToCharArray());

                        foreach (Worksheet sheet in workbook.Sheets)
                        {
                            var extractedName = refersToElements[0].Replace("=", String.Empty).ToUpper();
                            extractedName = extractedName.Replace("'", String.Empty).ToUpper();

                            if (sheet.Name.ToUpper() == extractedName)
                            {
                                string rangeName = refersToElements[1];
                                string fromIndexName = rangeName.Split(":".ToCharArray())[0];
                                string toIndexName = rangeName.Split(":".ToCharArray())[1];

                                bool nameRefersToIndexFrom = NameConverter.TryConvertCellNameToIndex(fromIndexName, out bool isRowFromAbsolute, out int fromRowIndex, out bool isColumnFromAbsolute, out int fromColumnIndex);
                                bool nameRefersToIndexTo = NameConverter.TryConvertCellNameToIndex(toIndexName, out bool isRowToAbsolute, out int toRowIndex, out bool isColumnToAbsolute, out int toColumnIndex);

                                if (nameRefersToIndexFrom && nameRefersToIndexTo)
                                {
                                    CellRange cellRange = new CellRange(fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex);
                                    worksheet.Cells[fromRowIndex, fromColumnIndex].SetValue(l1ActivationTotals.Sum(x => x.Value));
                                    worksheet.Cells[fromRowIndex+1, fromColumnIndex].SetValue(l1DeactivationTotals.Sum(x => x.Value));
                                    worksheet.Cells[fromRowIndex+2, fromColumnIndex].SetValue(l1DeactivationTotals.Sum(x => x.Value) + l1ActivationTotals.Sum(x => x.Value));
                                }
                            }
                        }

                    }

                    if (name.Name == "Activation")
                    {
                        string[] refersToElements = name.RefersTo.Split("!".ToCharArray());

                        foreach (Worksheet sheet in workbook.Sheets)
                        {
                            var extractedName = refersToElements[0].Replace("=", String.Empty).ToUpper();
                            extractedName = extractedName.Replace("'", String.Empty).ToUpper();

                            if (sheet.Name.ToUpper() == extractedName)
                            {
                                string rangeName = refersToElements[1];
                                string fromIndexName = rangeName.Split(":".ToCharArray())[0];
                                string toIndexName = rangeName.Split(":".ToCharArray())[1];

                                bool nameRefersToIndexFrom = NameConverter.TryConvertCellNameToIndex(fromIndexName, out bool isRowFromAbsolute, out int fromRowIndex, out bool isColumnFromAbsolute, out int fromColumnIndex);
                                bool nameRefersToIndexTo = NameConverter.TryConvertCellNameToIndex(toIndexName, out bool isRowToAbsolute, out int toRowIndex, out bool isColumnToAbsolute, out int toColumnIndex);

                                if (nameRefersToIndexFrom && nameRefersToIndexTo)
                                {
                                    CellRange cellRange = new CellRange(fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex);

                                    var i = 0;
                                    foreach (var item in l1ActivationTotals)
                                    {
                                        if(i>0)
                                            worksheet.Rows.Insert(fromRowIndex+i);

                                        worksheet.Cells[fromRowIndex+i, fromColumnIndex].SetValue(item.Key);
                                        worksheet.Cells[fromRowIndex+i, toColumnIndex].SetValue(item.Value);
                                        i++;
                                    }
                                }
                            }
                        }
                    }

                    if (name.Name == "Deactivation")
                    {
                        string[] refersToElements = name.RefersTo.Split("!".ToCharArray());

                        foreach (Worksheet sheet in workbook.Sheets)
                        {
                            var extractedName = refersToElements[0].Replace("=", String.Empty).ToUpper();
                            extractedName = extractedName.Replace("'", String.Empty).ToUpper();

                            if (sheet.Name.ToUpper() == extractedName)
                            {
                                string rangeName = refersToElements[1];
                                string fromIndexName = rangeName.Split(":".ToCharArray())[0];
                                string toIndexName = rangeName.Split(":".ToCharArray())[1];

                                bool nameRefersToIndexFrom = NameConverter.TryConvertCellNameToIndex(fromIndexName, out bool isRowFromAbsolute, out int fromRowIndex, out bool isColumnFromAbsolute, out int fromColumnIndex);
                                bool nameRefersToIndexTo = NameConverter.TryConvertCellNameToIndex(toIndexName, out bool isRowToAbsolute, out int toRowIndex, out bool isColumnToAbsolute, out int toColumnIndex);

                                if (nameRefersToIndexFrom && nameRefersToIndexTo)
                                {
                                    CellRange cellRange = new CellRange(fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex);

                                    var i = 0;
                                    foreach (var item in l1DeactivationTotals)
                                    {
                                        if(i>0)
                                            worksheet.Rows.Insert(fromRowIndex+i);

                                        worksheet.Cells[fromRowIndex + i, fromColumnIndex].SetValue(item.Key);
                                        worksheet.Cells[fromRowIndex + i, toColumnIndex].SetValue(item.Value);
                                        i++;
                                    }
                                }
                            }
                        }
                    }

                    if (name.Name == "Changes")
                    {
                        string[] refersToElements = name.RefersTo.Split("!".ToCharArray());

                        foreach (Worksheet sheet in workbook.Sheets)
                        {
                            var extractedName = refersToElements[0].Replace("=", String.Empty).ToUpper();
                            extractedName = extractedName.Replace("'", String.Empty).ToUpper();

                            if (sheet.Name.ToUpper() == extractedName)
                            {
                                string rangeName = refersToElements[1];
                                string fromIndexName = rangeName.Split(":".ToCharArray())[0];
                                string toIndexName = rangeName.Split(":".ToCharArray())[1];

                                bool nameRefersToIndexFrom = NameConverter.TryConvertCellNameToIndex(fromIndexName, out bool isRowFromAbsolute, out int fromRowIndex, out bool isColumnFromAbsolute, out int fromColumnIndex);
                                bool nameRefersToIndexTo = NameConverter.TryConvertCellNameToIndex(toIndexName, out bool isRowToAbsolute, out int toRowIndex, out bool isColumnToAbsolute, out int toColumnIndex);

                                if (nameRefersToIndexFrom && nameRefersToIndexTo)
                                {
                                    CellRange cellRange = new CellRange(fromRowIndex, fromColumnIndex, toRowIndex, toColumnIndex);

                                    var i = 1;
                                    foreach(var request in requests)
                                    {
                                        foreach (var item in request.ChangeDetail)
                                        {
                                            if (i > 1)
                                                worksheet.Rows.Insert(fromRowIndex + i);

                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 0].SetValue(request.RequestType.RequestTypeName);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 1].SetValue(item.CurrentL1);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 2].SetValue(item.CurrentL2);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 3].SetValue(item.CurrentL3);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 4].SetValue(item.CurrentL4);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 5].SetValue(item.CurrentL5);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 6].SetValue(item.NewL1);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 1].SetStyleName(item.NewL1Id == 0 || item.CurrentL1 != item.NewL1 ? "NewLevel" : "NoStyle");
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 7].SetValue(item.NewL2);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 1].SetStyleName(item.NewL2Id == 0 || item.CurrentL1 != item.NewL1 ? "NewLevel" : "NoStyle");
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 8].SetValue(item.NewL3);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 1].SetStyleName(item.NewL3Id == 0 || item.CurrentL1 != item.NewL1 ? "NewLevel" : "NoStyle");
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 9].SetValue(item.NewL4);
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 1].SetStyleName(item.NewL4Id == 0 || item.CurrentL1 != item.NewL1 ? "NewLevel" : "NoStyle");
                                            worksheet.Cells[fromRowIndex + i, fromColumnIndex + 10].SetValue(item.NewL5);
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return workbook;
            }
        }

        private Stream GetFileStream(string fileName)
        {
            string filePath = string.Format("{0}\\{1}", this.ReportDirectory, fileName);

            return new FileStream(filePath, FileMode.OpenOrCreate);
        }
    }
}
