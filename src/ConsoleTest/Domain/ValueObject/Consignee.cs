namespace ConsoleTest.Domain.ValueObject
{
    public class Consignee
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public Consignee(int userId, string userName, string address)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.Address = address;
        }
    }
}
