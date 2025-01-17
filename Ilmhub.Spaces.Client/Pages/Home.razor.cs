using System.Text.RegularExpressions;
using Ilmhub.Spaces.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ilmhub.Spaces.Client.Pages;
public partial class Home
{
    private bool resetValueOnEmptyText;
    private bool coerceText;
    private bool coerceValue;
    private string value1, value2;
    private MudDropContainer<Lead> dropContainer = default!;
    private List<Lead> leads = new();
    private string searchQuery = "";
    private string currentStatus = "Barchasi";
    private List<Lead> filteredLeads = new();
    [CascadingParameter]
    private MudDialogInstance? MudDialog { get; set; }
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

    private async Task<IEnumerable<string>> Search1(string value, CancellationToken token)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5, token);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return courses;
        return courses.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task<IEnumerable<string>> Search2(string value, CancellationToken token)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5, token);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return sources;
        return sources.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private IEnumerable<Lead> SortLeads(IEnumerable<Lead> leads, string sortLabel, SortDirection direction)
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
    private Dictionary<int, bool> leadDetailsVisibility = new Dictionary<int, bool>();
    private Dictionary<string, SortDirection> columnSortDirections = new();
    private Dictionary<string, List<Lead>> columnLeads = [];

    protected override async Task OnInitializedAsync()
    {
        leads = (await LeadService.GetLeadsAsync()).OrderByDescending(l => l.ModifiedAt).ToList();
        filteredLeads = new List<Lead>(leads);
        
        // Initialize columnLeads for each column
        var columns = new[] { "Yangi Lidlar", "Bog'lanilgan", "Kuzatuvda", "Yakuniy Holat" };
        foreach (var column in columns)
        {
            columnLeads[column] = leads.Where(l => GetColumnForStatus(l.Status) == column).ToList();
        }
    }

    private void ShowLeadDetails(Lead lead)
    {
        DialogService.Show<LeadDetailsDialog>("Lead Details", new DialogParameters { ["Lead"] = lead });
    }

    private void OnSearch(string text)
    {
        searchQuery = text;

        if (string.IsNullOrEmpty(text))
        {
            filteredLeads = new List<Lead>(leads);
        }
        else
        {
            filteredLeads = leads.Where(l =>
                (string.IsNullOrEmpty(currentStatus) || currentStatus == "Barchasi" || GetColumnForStatus(l.Status) == currentStatus) &&
                (l.Name?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) == true ||
                    l.Phone?.Contains(searchQuery) == true))
                .ToList();
        }

        StateHasChanged();
    }

    private void SetCurrentStatus(string status)
    {
        currentStatus = status;
        if (!string.IsNullOrEmpty(searchQuery))
        {
            filteredLeads = leads.Where(l =>
                (currentStatus == "Barchasi" || GetColumnForStatus(l.Status) == currentStatus) &&
                (l.Name?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) == true ||
                    l.Phone?.Contains(searchQuery) == true))
                .ToList();
        }
        else
        {
            filteredLeads = new List<Lead>(leads);
        }

        StateHasChanged();
    }

    private async Task LeadUpdated(MudItemDropInfo<Lead> dropInfo)
    {
        if (dropInfo.Item != null)
        {
            dropInfo.Item.Status = GetStatusForColumn(dropInfo.DropzoneIdentifier);
            dropInfo.Item.ModifiedAt = DateTime.Now;

            await LeadService.UpdateLeadAsync(dropInfo.Item);
            leads = leads.OrderByDescending(l => l.ModifiedAt).ToList();
            StateHasChanged();
        }
    }

    private async Task UpdateLeadStatus(Lead lead, LeadStatus newStatus)
    {
        lead.Status = newStatus;
        lead.ModifiedAt = DateTime.Now;
        await LeadService.UpdateLeadAsync(lead);
        leads = leads.OrderByDescending(l => l.ModifiedAt).ToList();
        StateHasChanged();
    }

    private string GetColumnForStatus(LeadStatus status)
    {
        return status switch
        {
            LeadStatus.New => "Yangi Lidlar",
            LeadStatus.Phone or LeadStatus.Contacted => "Bog'lanilgan",
            LeadStatus.Recontact or LeadStatus.Incomplete or LeadStatus.Registered or LeadStatus.AttendedTrialLesson => "Kuzatuvda",
            LeadStatus.Acquired or LeadStatus.NotAcquired or LeadStatus.Lost => "Yakuniy Holat",
            _ => "Yangi Lidlar"
        };
    }

    private LeadStatus GetStatusForColumn(string column)
    {
        return column switch
        {
            "Yangi Lidlar" => LeadStatus.New,
            "Bog'lanilgan" => LeadStatus.Contacted,
            "Kuzatuvda" => LeadStatus.Recontact,
            "Yakuniy Holat" => LeadStatus.Acquired,
            _ => LeadStatus.New
        };
    }

    private string GetColorForColumn(string column)
    {
        return column switch
        {
            "Barchasi" => "default",
            "Yangi Lidlar" => "dodgerblue",
            "Bog'lanilgan" => "orange",
            "Kuzatuvda" => "mediumvioletred",
            "Yakuniy Holat" => "seagreen",
            _ => "Barchasi"
        };
    }

    private string GetIconForColumn(string column)
    {
        return column switch
        {
            "Barchasi" => Icons.Material.Filled.SelectAll,
            "Yangi Lidlar" => Icons.Material.Filled.NewLabel,
            "Bog'lanilgan" => Icons.Material.Filled.Phone,
            "Kuzatuvda" => Icons.Material.Filled.Visibility,
            "Yakuniy Holat" => Icons.Material.Filled.Done,
            _ => Icons.Material.Filled.NewLabel
        };
    }

    private Color GetColorForStatus(LeadStatus status)
    {
        return status switch
        {
            LeadStatus.New => Color.Info,
            LeadStatus.Phone => Color.Warning,
            LeadStatus.Contacted => Color.Primary,
            LeadStatus.Recontact => Color.Secondary,
            LeadStatus.Incomplete => Color.Error,
            LeadStatus.Registered => Color.Success,
            LeadStatus.AttendedTrialLesson => Color.Tertiary,
            LeadStatus.Acquired => Color.Success,
            LeadStatus.NotAcquired => Color.Error,
            LeadStatus.Lost => Color.Dark,
            _ => Color.Default
        };
    }

    public static string FormatPhoneNumber(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        // Regular expression to match and capture digits
        var regex = new Regex(@"(\d{2})[\s\-]?(\d{3})[\s\-]?(\d{4})");

        // Replace the matched digits with the desired format
        var result = regex.Replace(input, "$1 $2 $3");

        // Handle cases where input doesn't match the pattern
        if (result == input)
        {
            return input; // Return original input if it doesn't match the pattern
        }

        return result;
    }

    private void ToggleSort(string column)
    {
        // Initialize or toggle sort direction for this column
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

        // Sort only the leads in this column
        var columnLeads = leads.Where(l => GetColumnForStatus(l.Status) == column).ToList();
        var sortedColumnLeads = SortLeads(columnLeads, "ModifiedAt", columnSortDirections[column]);
        
        // Update the main leads list with the sorted column while preserving other columns
        var otherLeads = leads.Where(l => GetColumnForStatus(l.Status) != column).ToList();
        leads = [.. otherLeads, .. sortedColumnLeads];
        
        // Update filtered leads
        filteredLeads = [.. leads];
        
        StateHasChanged();
        dropContainer.Refresh();
    }
}