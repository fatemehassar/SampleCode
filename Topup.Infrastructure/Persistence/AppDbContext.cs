using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Application.Interfaces;

namespace Topup.Infrastructure.Persistence
{
    public class AppDbContext
     : DbContext,
       IApplicationDbContext
    {
    }
}
