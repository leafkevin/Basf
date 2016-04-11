using System.Collections.Generic;

namespace Basf.Domain.Event
{
    public interface IEventSourcing<TAggRootId>
    {
        /// <summary>
        /// 从指定版本创建快照
        /// </summary>
        /// <param name="version">为空，表示从最新的当前版本创建快照</param>
        void CreateSnapshot(int? version);
        /// <summary>
        /// 从快照中加载
        /// </summary>
        void LoadFromSnapshot();
        /// <summary>
        /// 从当前版本进行重演事件
        /// </summary>
        /// <param name="domainEvents"></param>
        void ReplayFrom(IEnumerable<IDomainEvent<TAggRootId>> domainEvents);
    }
}
