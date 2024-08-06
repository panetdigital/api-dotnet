﻿using System;

namespace FinancialPlannerAPI.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Source { get; set; }
        public DateTime Date { get; set; }
    }
}

