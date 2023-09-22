namespace BeautySim2023.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Events
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string CompleteName { get; set; }
        public string Location { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string OrganizedBy { get; set; }
        public string SponsoredBy { get; set; }
        public long Deleted { get; set; }
    }
}
