using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Domain.Enums
{
    public enum StatusId
    {
        done = 1,
        inserted = 2,
        sendtovira = 3,
        rejected = 4,
        sendtotaz = 5,
        awaitingConfirmation = 6,
        inLine = 7,
        inProgress = 8,
        awaitingRejecting = 9,
        all = 0
    }
}
