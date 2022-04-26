﻿using DataAnnotationsExtensions;
using ExpensesControlApp.Helpers;
using ExpensesControlApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExpensesControlApp.ViewModels
{
    public class RegularExpenseVM
    {
        public int RegularExpenseId { get; set; }
        public int? ExpenseId { get; set; }
        public Expense? Expense { get; set; }

        [Required(ErrorMessage = "The Name of the expense is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Name should only consist of letters")]
        [MaxLength(15), MinLength(3, ErrorMessage = "The Name should be at least 3 characters long")]
        public string ExpenseName { get; set; }

        [Required(ErrorMessage = "The Amount of the expense is required")]
        [RegularExpression(@"^[0-9]+(,[0-9]{1,2})?$", ErrorMessage = "The Amount has invalid format")]
        [Min(0.01, ErrorMessage = "The Amount should be greater than zero")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Time span of the Expense is required")]
        public TimeSpanOption TimeSpan { get; set; }

        
    }
}
