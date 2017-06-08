using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;
using LIMS.Util;

namespace LIMS.Services
{
    public class GoodsExtraService
    {
        public GoodsExtraEntity Get(string id)
        {
            return GoodsExtraRepsitory.Get(id);
        }

        public IList<QueryBarcodeEntity> QueryBarcode(string barcode)
        {
            return GoodsStateRepository.QueryBarcodeDetail(barcode);
        }

        //public IList<GoodsExtraEntity> GetProductsByUnit(string unitId)
        //{
        //    return GoodsExtraRepsitory.GetProductsByUnit(unitId);
        //}
    }
}
