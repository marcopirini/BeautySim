namespace BeautySim2023.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Results
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public decimal Score { get; set; }
        public int IdEvent { get; set; }
        public int IdTeacher { get; set; }
        public int IdCase { get; set; }
        //public long Deleted { get; set; }
        public int IdStudent { get; set; }
        public string FilePath { get; set; }
        public string CaseName { get; set; }
    }
}
