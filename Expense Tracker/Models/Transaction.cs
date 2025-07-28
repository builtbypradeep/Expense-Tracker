using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage ="Please select a category.")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; } = string.Empty;

        public DateTime Date {  get; set; } = DateTime.Now;

        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0.")]
        public int Amount { get; set; } /*= new CultureInfo("en-IN");*/


        [NotMapped]
        public string CategoryTitleWithIcon
        {
            get
            {
                return Category == null ? string.Empty : Category.Icon + " " + Category.Title;
            }
        }


        [NotMapped]
        public string FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Expense") ?  "- " : "+ ") + Amount.ToString("C0", new CultureInfo("hi-IN"));
            }
        }

    }
}
