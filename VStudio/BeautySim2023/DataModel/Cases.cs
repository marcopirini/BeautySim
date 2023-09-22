namespace BeautySim2023.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Cases
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocationFile { get; set; }
        public long Deleted { get; set; }
    }
}
