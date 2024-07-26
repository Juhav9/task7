namespace BlazorPeople.Data
{
    public class Address
    {
        public int id { get; set; }
        public string StreetAddress { get; set; } = null!;
        public string PostalAddress { get; set; } = null!;
        public int PostalNumber { get; set; }
    }
}
