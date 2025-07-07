using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LoanBack.Models.Requests;

public class LoanRequest
{

    [JsonConverter(typeof(NullableIntJsonConverter))]
    public int? Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "LoanTypeId is required.")]
    public int LoanTypeId { get; set; }

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CurrencyId is required.")]
    public int CurrencyId { get; set; }

    [Required]
    [Range(1, 120, ErrorMessage = "Loan term must be between 1 and 120 months.")]
    public int MonthsTerm { get; set; }

}

