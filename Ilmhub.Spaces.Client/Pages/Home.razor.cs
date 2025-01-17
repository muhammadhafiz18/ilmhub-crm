using System.Text.RegularExpressions;
using Ilmhub.Spaces.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ClosedXML.Excel;
using Microsoft.JSInterop;

namespace Ilmhub.Spaces.Client.Pages;
public partial class Home
{
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;    
    private readonly bool resetValueOnEmptyText = false;
    private readonly bool coerceText = false;
    private readonly bool coerceValue = false;
    private string? value1;
    private string? Value1
    {
        get => value1;
        set
        {
            if (value1 == value) return;
            value1 = value;
            selectedCourse = value;
            ApplyFilters();
        }
    }

    private string? value2;
    private string? Value2
    {
        get => value2;
        set
        {
            if (value2 == value) return;
            value2 = value;
            selectedSource = value;
            ApplyFilters();
        }
    }

    private MudDropContainer<Lead> dropContainer = default!;
    private List<Lead> leads = [];
    private string searchQuery = "";
    private List<Lead> filteredLeads = [];
    private string? selectedCourse;
    private MudDateRangePicker? _picker;
    private DateRange _dateRange = new(DateTime.Today.AddDays(-7), DateTime.Today);
    private string? selectedSource;
    private bool IsCleared { get; set; }
    private string[] courses =
    {
        "English Phonics 1", "English Phonics 2", "English Phonics 3", "English Phonics 4",
        "English Starters", "English The Spire 1", "English The Spire 2", "English The Spire 3",
        "English The Spire 4", "English The Spire 5", "English The Spire 6", "English Level 7",
        "English Level 8", "IT Scratch", "IT Lego", "IT Extended Scratch", "Lego (Spike Prime)",
        "IT Savodxonlik", "IT App inventor", "IT Robotics (extended)", "IT Extended Python", "IT AutoCAD",
        "IT C++", "IT Frontend", "IT Backend"  
    };

    private string[] sources = 
    {
        "Telegram", "Instagram", "Referral"
    };

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private async Task<IEnumerable<string>> Search1(string value, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (string.IsNullOrEmpty(value))
            return courses;
        return courses.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task<IEnumerable<string>> Search2(string value, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (string.IsNullOrEmpty(value))
            return sources;
        return sources.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private static IEnumerable<Lead> SortLeads(IEnumerable<Lead> leads, string sortLabel, SortDirection direction)
    {
        if (string.IsNullOrEmpty(sortLabel))
            return leads;

        return sortLabel switch
        {
            "ModifiedAt" => direction == SortDirection.Ascending
                ? leads.OrderBy(l => l.ModifiedAt ?? l.CreatedAt)
                : leads.OrderByDescending(l => l.ModifiedAt ?? l.CreatedAt),
            _ => leads
        };
    }
    private readonly Dictionary<string, SortDirection> columnSortDirections = [];
    private readonly Dictionary<string, List<Lead>> columnLeads = [];

    protected override async Task OnInitializedAsync()
    {
        leads = (await LeadService.GetLeadsAsync()).OrderByDescending(l => l.ModifiedAt).ToList();
        
        filteredLeads = [.. leads.Where(l =>
            !_dateRange.Start.HasValue || !_dateRange.End.HasValue || 
                (l.ModifiedAt.HasValue && 
                 l.ModifiedAt.Value.Date >= _dateRange.Start.Value.Date && 
                 l.ModifiedAt.Value.Date <= _dateRange.End.Value.Date)
        )];
        
        var columns = new[] { "Yangi Lidlar", "Bog'lanilgan", "Kuzatuvda", "Yakuniy Holat" };
        foreach (var column in columns)
        {
            columnLeads[column] = [.. leads.Where(l => GetColumnForStatus(l.Status) == column)];
        }
        
    }

    private void ShowLeadDetails(Lead lead)
    {
        DialogService.Show<LeadDetailsDialog>("Lead Details", new DialogParameters { ["Lead"] = lead });
    }

    private void ApplyFilters()
    {
        filteredLeads = [.. leads.Where(l =>
            (string.IsNullOrEmpty(searchQuery) || 
                l.Name?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) == true ||
                l.Phone?.Contains(searchQuery) == true) &&
            (string.IsNullOrEmpty(selectedCourse) || 
                l.InterestedCourse?.Name == selectedCourse) &&
            (string.IsNullOrEmpty(selectedSource) || 
                l.Source.ToString() == selectedSource) &&
            (!_dateRange.Start.HasValue || !_dateRange.End.HasValue || 
                (l.ModifiedAt.HasValue && 
                 l.ModifiedAt.Value.Date >= _dateRange.Start.Value.Date && 
                 l.ModifiedAt.Value.Date <= _dateRange.End.Value.Date))
        )];

        StateHasChanged();
        dropContainer.Refresh();

        if (IsCleared is true)
        {
            _dateRange = new DateRange(DateTime.Today.AddDays(-7), DateTime.Today);
        }

        IsCleared = false;
    }

    private void Search3()
    {
        if (_dateRange.Start.HasValue && _dateRange.End.HasValue)
        {
            if (_dateRange.End.Value.Date > DateTime.Now.Date)
            {
                _dateRange = new DateRange(DateTime.Today.AddDays(-7), DateTime.Today);
                Snackbar.Add("Iltimos hozirgi kundan avvalni tanlang", Severity.Error);
                return;
            }

            var daysDifference = (_dateRange.End.Value - _dateRange.Start.Value).TotalDays;
            if (daysDifference > 31)
            {
                _dateRange = new DateRange(DateTime.Today.AddDays(-7), DateTime.Today);
                Snackbar.Add("Bir oydan ko'p tanlash mumkin emas", Severity.Error);
                return;
            }
        }

        ApplyFilters();
    }

    private void OnSearch(string text)
    {
        searchQuery = text;
        ApplyFilters();
    }

    private async Task LeadUpdated(MudItemDropInfo<Lead> dropInfo)
    {
        if (dropInfo.Item != null)
        {
            dropInfo.Item.Status = GetStatusForColumn(dropInfo.DropzoneIdentifier);
            dropInfo.Item.ModifiedAt = DateTime.Now;

            await LeadService.UpdateLeadAsync(dropInfo.Item);
            UpdateLeadsOrder();
        }
    }

    private void UpdateLeadsOrder()
    {
        leads = [.. leads.OrderByDescending(l => l.ModifiedAt)];
        
        var currentFiltered = filteredLeads.ToList();
        filteredLeads = currentFiltered.OrderByDescending(l => l.ModifiedAt).ToList();
        
        StateHasChanged();
        dropContainer.Refresh();
    }

    private async Task UpdateLeadStatus(Lead lead, LeadStatus newStatus)
    {
        lead.Status = newStatus;
        lead.ModifiedAt = DateTime.Now;
        await LeadService.UpdateLeadAsync(lead);
        leads = [.. leads.OrderByDescending(l => l.ModifiedAt)];
        StateHasChanged();
    }

    private static string GetColumnForStatus(LeadStatus status)
    {
        return status switch
        {
            LeadStatus.Yangi => "Yangi Lidlar",
            LeadStatus.Aloqa or LeadStatus.Boglanildi => "Bog'lanilgan",
            LeadStatus.QaytaBoglanish or LeadStatus.Tugallanmagan or LeadStatus.RegistratsiyaBolgan or LeadStatus.SinovDarsda => "Kuzatuvda",
            LeadStatus.Kelishilindi or LeadStatus.Kelishilinmadi or LeadStatus.Yoqotildi => "Yakuniy Holat",
            _ => "Yangi Lidlar"
        };
    }

    private static LeadStatus GetStatusForColumn(string column)
    {
        return column switch
        {
            "Yangi Lidlar" => LeadStatus.Yangi,
            "Bog'lanilgan" => LeadStatus.Boglanildi,
            "Kuzatuvda" => LeadStatus.QaytaBoglanish,
            "Yakuniy Holat" => LeadStatus.Kelishilindi,
            _ => LeadStatus.Yangi
        };
    }

    private static Color GetColorForStatus(LeadStatus status)
    {
        return status switch
        {
            LeadStatus.Yangi => Color.Info,
            LeadStatus.Aloqa => Color.Warning,
            LeadStatus.Boglanildi => Color.Primary,
            LeadStatus.QaytaBoglanish => Color.Secondary,
            LeadStatus.Tugallanmagan => Color.Error,
            LeadStatus.RegistratsiyaBolgan => Color.Success,
            LeadStatus.SinovDarsda => Color.Tertiary,
            LeadStatus.Kelishilindi => Color.Success,
            LeadStatus.Kelishilinmadi => Color.Error,
            LeadStatus.Yoqotildi => Color.Dark,
            _ => Color.Default
        };
    }

    public static string FormatPhoneNumber(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var regex = new Regex(@"(\d{2})[\s\-]?(\d{3})[\s\-]?(\d{4})");

        var result = regex.Replace(input, "$1 $2 $3");

        if (result == input)
        {
            return input; 
        }

        return result;
    }

    private void ToggleSort(string column)
    {
        if (!columnSortDirections.ContainsKey(column))
        {
            columnSortDirections[column] = SortDirection.Ascending;
        }
        else
        {
            columnSortDirections[column] = columnSortDirections[column] == SortDirection.Ascending 
                ? SortDirection.Descending 
                : SortDirection.Ascending;
        }

        var columnLeads = leads.Where(l => GetColumnForStatus(l.Status) == column).ToList();
        var sortedColumnLeads = SortLeads(columnLeads, "ModifiedAt", columnSortDirections[column]);
        
        var otherLeads = leads.Where(l => GetColumnForStatus(l.Status) != column).ToList();
        leads = [.. otherLeads, .. sortedColumnLeads];
        
        filteredLeads = [.. leads];
        
        StateHasChanged();
        dropContainer.Refresh();
    }

    private void ClearFilters()
    {
        searchQuery = "";
        Value1 = null;
        Value2 = null;
        selectedCourse = null;
        selectedSource = null;
        _dateRange = new DateRange(null, null); 
        IsCleared = true;
        ApplyFilters();
    }

    private async Task ExportToExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Leads");

        worksheet.Cell(1, 1).Value = "Name";
        worksheet.Cell(1, 2).Value = "Phone";
        worksheet.Cell(1, 3).Value = "Status";
        worksheet.Cell(1, 4).Value = "Source";
        worksheet.Cell(1, 5).Value = "Created At";
        worksheet.Cell(1, 6).Value = "Modified At";
        worksheet.Cell(1, 7).Value = "Interested Course";
        worksheet.Cell(1, 8).Value = "Notes";

        for (int i = 0; i < filteredLeads.Count; i++)
        {
            var lead = filteredLeads[i];
            int row = i + 2;

            worksheet.Cell(row, 1).Value = lead.Name;
            worksheet.Cell(row, 2).Value = lead.Phone;
            worksheet.Cell(row, 3).Value = lead.Status.ToString();
            worksheet.Cell(row, 4).Value = lead.Source.ToString();
            worksheet.Cell(row, 5).Value = lead.CreatedAt;
            worksheet.Cell(row, 6).Value = lead.ModifiedAt;
            worksheet.Cell(row, 7).Value = lead.InterestedCourse?.Name;
            worksheet.Cell(row, 8).Value = lead.Notes;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        await JSRuntime.InvokeVoidAsync("downloadFileFromStream", 
            "Ilmhub Leads.xlsx", 
            Convert.ToBase64String(content));
    }
}