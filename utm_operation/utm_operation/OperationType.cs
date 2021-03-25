using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_operation
{
   public enum OperationType
    {
       VLOS,
       BVLOS,
       EVLOS,
       VLOSorEVLOS,
       VLOSorBVLOS,
       EVLOSorBVLOS,
       VLOSorBVLOSorEVLOS,
       Delivery,
    }
}
