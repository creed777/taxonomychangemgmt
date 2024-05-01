using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;

internal class Program
{
    /// <summary>
    /// This purpose was to get new data for the unit tests.  The request data is stored in a JSON file in the project resources.
    /// This program should only be run on a local machine.  It downloads a number of request records and saves them to user "Documents" folder in JSON format.  
    /// I tried to write the files directly to the test project resources but apparantly I can't add a test project as a reference.
    /// The app is not bullet-proof, none of the entries are validated because I figured if you can't get it right, there's no hope.  ;-)
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {

        int recordCount = 0;
        int period = 0;

        Console.WriteLine("What data do you want to generate?");
        Console.WriteLine("1. All");
        Console.WriteLine("2. Requests w/Changes");
        Console.WriteLine("3. Change Status Types");
        Console.WriteLine("4. Request Types");
        Console.WriteLine("5. Request Status");
        Console.WriteLine("6. Request Status Types");
        Console.WriteLine("7. Change Periods");
        Console.WriteLine("8. Taxonomy");
        Console.WriteLine("9. Taxonomy Owner");
        int.TryParse(Console.ReadLine(), out int scope);

        if (scope == 0)
            return;

        if (scope < 3)
        {
            Console.WriteLine("How many records?");
            int.TryParse(Console.ReadLine(), out recordCount);
            Console.WriteLine("Which change period?  (0 will get the same # of records from every period)");
            int.TryParse(Console.ReadLine(), out period);

            if (recordCount == 0)
                return;
        }

        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        IConfiguration configuration = builder.Build();
        var connString = configuration.GetConnectionString("Dev_Request_Extract");

        DataManagement dm = new();
        using (SqlConnection conn = new SqlConnection(connString))
        {
            switch (scope)
            {
                case 1:
                    dm.GenerateRequestData(conn, recordCount, period);
                    dm.GenerateChangeStatusTypeData(conn);
                    dm.GenerateRequestTypeData(conn);
                    dm.GenerateRequestStatusTypeData(conn);
                    dm.GenerateRequestStatusData(conn);
                    dm.GenerateChangePeriodData(conn);
                    dm.GenerateTaxonomyData(conn);
                    break;

                case 2:
                    dm.GenerateRequestData(conn, recordCount, period);
                    break;

                case 3:
                    dm.GenerateChangeStatusTypeData(conn);
                    break;
                case 4:
                    dm.GenerateRequestTypeData(conn);
                    break;
                case 5:
                    dm.GenerateRequestStatusData(conn);
                    break;
                case 6:
                    dm.GenerateRequestStatusTypeData(conn);
                    break;
                case 7:
                    dm.GenerateChangePeriodData(conn);
                    break;
                case 8:
                    dm.GenerateTaxonomyData(conn);
                    break;
                case 9:
                    dm.GenerateTaxonomyOwnerData(conn);
                    break;
            }
        }
    }
}
    public class DataManagement 
{
    private string DestinationFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public static T ConvertRows<T>(DataRow row, T objectType) where T : new()
    {
        T item = new T();

        foreach(DataColumn c in row.Table.Columns)
        {
            PropertyInfo p = objectType.GetType().GetProperty(c.ColumnName);
            if (p != null && row[c] != DBNull.Value)
            {
                p.SetValue(objectType, row[c], null);
            }
        }

        return objectType;
    }

    public void GenerateRequestData(SqlConnection conn, int recordCount, int period)
    {
        List<Request> requestList = new();

        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();
            if (period == 0)
            {
                var changePeriodIds = context.ChangePeriod
                    .Where(x => x.IsDeleted == false)
                    .Select(x => x.ChangePeriodId)
                    .ToListAsync()
                    .Result;

                foreach (var periodId in changePeriodIds)
                {

                    var result = context.Request
                       .Include(request => request.ChangeDetail)
                           .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                           .ThenInclude(statuses => statuses.StatusTypes)
                       .Include(request => request.RequestType)
                       .Include(request => request.RequestStatuses)
                           .ThenInclude(statuses => statuses.StatusTypes)
                       .OrderByDescending(x => x.RequestId)
                       .Where(x => x.ChangePeriodId == periodId)
                       .AsNoTracking()
                       .ToListAsync()
                       .Result;

                    requestList.AddRange(result.Take(recordCount));
                    result.Clear();
                }
            }
            else
            {
                requestList.AddRange(context.Request
                    .Include(request => request.ChangeDetail)
                        .ThenInclude(changeDetail => changeDetail.ChangeStatuses)
                        .ThenInclude(statuses => statuses.StatusTypes)
                    .Include(request => request.RequestType)
                    .Include(request => request.RequestStatuses)
                        .ThenInclude(statuses => statuses.StatusTypes)
                    .Take(recordCount)
                    .Where(x => x.ChangePeriodId == period)
                    .AsNoTracking()
                    .ToListAsync()
                    .Result);
            }
        }

        //var results = GetRequestTestData(context, period, recordCount);

        var requestListJson = JsonConvert.SerializeObject(requestList, Formatting.Indented,
             new JsonSerializerSettings()
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             });

        var filename = Path.Combine(DestinationFolder, "RequestJson.txt");
        File.WriteAllText(filename, requestListJson);
    }

    public void GenerateChangeStatusTypeData(SqlConnection conn)
    {
        Console.WriteLine("Generating Change Status Type report...");
        List<ChangeStatusType> changeStatusTypes = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            changeStatusTypes = context.ChangeStatusType
            .ToListAsync()
            .Result;
        }

        var statusTypeListJson = JsonConvert.SerializeObject(changeStatusTypes, Formatting.Indented);
        var filename = Path.Combine(DestinationFolder, "ChangeStatusTypeJson.txt");
        File.WriteAllText(filename, statusTypeListJson);
        Console.WriteLine("Report complete");
    }

    public void GenerateRequestTypeData(SqlConnection conn)
    {
        Console.WriteLine("Generating Request Type report...");
        List<RequestType> requestTypes = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            requestTypes = context.RequestType
            .ToListAsync()
            .Result;
        }

        var requestTypeListJson = JsonConvert.SerializeObject(requestTypes, Formatting.Indented);
        var filename = Path.Combine(DestinationFolder, "RequestTypeJson.txt");
        File.WriteAllText(filename, requestTypeListJson);
        Console.WriteLine("Report complete");
    }

    public void GenerateRequestStatusData(SqlConnection conn)
    {
        Console.WriteLine("Generating Request Status report...");
        List<RequestStatus> requestStatus = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            requestStatus = context.RequestStatus
            .ToListAsync()
            .Result;
        }

        var requestStatusListJson = JsonConvert.SerializeObject(requestStatus, Formatting.Indented);
        var filename = Path.Combine(DestinationFolder, "RequestTypeJson.txt");
        File.WriteAllText(filename, requestStatusListJson);
        Console.WriteLine("Report complete");
    }

    public void GenerateRequestStatusTypeData(SqlConnection conn)
    {
        Console.WriteLine("Generating Request Status Type report...");
        List<RequestStatusType> requestStatusType = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            requestStatusType = context.RequestStatusType
            .ToListAsync()
            .Result;
        }

        var requestStatusTypeJson = JsonConvert.SerializeObject(requestStatusType, Formatting.Indented);
        var filename = Path.Combine(DestinationFolder, "RequestStatusTypeJson.txt");
        File.WriteAllText(filename, requestStatusTypeJson);
        Console.WriteLine("Report complete");
    }

    public void GenerateChangePeriodData(SqlConnection conn)
    {
        Console.WriteLine("Generating Change Period report...");
        List<ChangePeriod> changePeriod = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            changePeriod = context.ChangePeriod
            .ToListAsync()
            .Result;
        }

        var changePeriodJson = JsonConvert.SerializeObject(changePeriod, Formatting.Indented);
        var filename = Path.Combine(DestinationFolder, "ChangePeriodJson.txt");
        File.WriteAllText(filename, changePeriodJson);
        Console.WriteLine("Report Complete");
    }

    public void GenerateTaxonomyData(SqlConnection conn)
    {
        Console.WriteLine("Generating Taxonomy report...");
        List<Taxonomy> taxonomy = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            taxonomy = context.Taxonomy
            .ToListAsync()
            .Result;
        }

        var taxonomyJson = JsonConvert.SerializeObject(taxonomy, Formatting.Indented,
             new JsonSerializerSettings()
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             });

        var filename = Path.Combine(DestinationFolder, "TaxonomyJson.txt");
        File.WriteAllText(filename, taxonomyJson);
        Console.WriteLine("Report Complete");
    }

    public void GenerateTaxonomyOwnerData(SqlConnection conn)
    {
        Console.WriteLine("Generating Taxonomy Owner report...");
        List<TaxonomyOwner> owner = new();
        DbContextOptions<TaxonomyContext>? _contextOptions;
        _contextOptions = new DbContextOptionsBuilder<TaxonomyContext>()
            .UseSqlServer(conn)
            .Options;

        using (var context = new TaxonomyContext(_contextOptions))
        {
            context.Database.EnsureCreated();

            owner = context.TaxonomyOwner
            .ToListAsync()
            .Result;
        }

        var ownerJson = JsonConvert.SerializeObject(owner, Formatting.Indented,
             new JsonSerializerSettings()
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             });

        var filename = Path.Combine(DestinationFolder, "TaxonomyOwnerJson.txt");
        File.WriteAllText(filename, ownerJson);
        Console.WriteLine("Report Complete");
    }

}