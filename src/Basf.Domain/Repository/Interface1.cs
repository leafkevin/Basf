using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basf.Domain.Repository
{
    public interface IStorage
    {
        TEntity Get(object objKey);
        int Create(TEntity entity);
    }
}
