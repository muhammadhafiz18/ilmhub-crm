using System.Text.RegularExpressions;
using Ilmhub.Spaces.Client.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ilmhub.Spaces.Client.Pages;
public partial class LeadDetailsDialog
{
    [Parameter] public required Lead Lead { get; set; }

    [CascadingParameter]
    private MudDialogInstance? MudDialog { get; set; }
    private void Submit()
    {
        if (MudDialog != null)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
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
}
