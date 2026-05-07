using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Interfaces
{
    public interface ISwitchService
    {
        Task AdviceAsync(Guid txId);
        Task ReverseAsync(Guid txId);
    }
}
