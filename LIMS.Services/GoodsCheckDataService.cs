using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class GoodsCheckDataService
    {
        public void Scan(GoodsCheckDataEntity data)
        {
            GoodsCheckDataRepository.Scan(data);
        }
    }
}
