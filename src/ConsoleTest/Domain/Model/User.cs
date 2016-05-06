using System.Threading.Tasks;
using Basf.Domain;
using Basf.Domain.Event;
using ConsoleTest.Domain.Event;
using System;

namespace ConsoleTest.Domain.Model
{
    public class User : AggRoot<int>
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public bool IsLocked { get; set; } = false;
        public string Address { get; set; }
        public User(int userId, string userName, int age)
            : base(userId)
        {
            this.UserName = userName;
            this.Age = age;
        }
        public async Task ChangeName(string commandId, string userName)
        {
            await this.ApplyChange(new UserNameChanged(commandId, userName));
        }
        public async Task Lock(string commandId)
        {
            await this.ApplyChange(new UserLocked(commandId, this.UniqueId));
        }
        public void Handle(UserNameChanged domainEvent)
        {
            this.UserName = domainEvent.UserName;
        }
        public void Handle(UserLocked domainEvent)
        {
            this.IsLocked = true;
        }
        public override string ToString()
        {
            return String.Format("{{UserId:{0},UserName:{1},Age:{2},Version:{3}}}", this.UniqueId, this.UserName, this.Age, this.Version);
        }
    }
}
