namespace BeautySim2023.DataModel
{
    using System;

    public partial class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public long Role { get; set; }
        public string Title { get; set; }
        public string Organization { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<int> IdParentUser { get; set; }
    }
}