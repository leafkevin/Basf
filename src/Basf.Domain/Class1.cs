using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basf.Domain
{
    public interface IEventStream:IEntity<string>
    {
        string Bucket { get; }
        int  Version { get; }
        IEnumerable<IWritableEvent> Events { get; }
        IEnumerable<IWritableEvent> Uncommitted { get; }

        void Add(Object @event, IDictionary<String, String> headers);
        void AddSnapshot(Object memento, IDictionary<String, String> headers);
        void Commit(Guid commitId, IDictionary<String, String> commitHeaders);

        void AddChild(IEventStream stream);

        void ClearChanges();
    }
}
