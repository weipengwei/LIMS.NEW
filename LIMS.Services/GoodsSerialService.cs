using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class GoodsSerialService
    {
        public GoodsSerialEntity Get(string id)
        {
            return GoodsSerialRepository.Get(id);
        }

        public GoodsSerialEntity GetByBarcode(string barcode)
        {
            return GoodsSerialRepository.GetByBarcode(barcode);
        }

        public IList<GoodsSerialBarcodeEntity> GetBarcodes(string serialId)
        {
            return GoodsSerialRepository.GetBarcodes(serialId);
        }

        public IList<GoodsSerialBarcodeEntity> GetBarcodesByRoot(string serialId)
        {
            return GoodsSerialRepository.GetBarcodesByRoot(serialId);
        }

        public void UpdatePrint(string serialId, IList<string> barcodes)
        {
            GoodsSerialRepository.UpdatePrint(serialId, barcodes);
        }
    }
}
