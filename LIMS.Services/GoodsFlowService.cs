using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Models;
using LIMS.Entities;
using LIMS.Repositories;
using LIMS.Util;

namespace LIMS.Services
{
    public class GoodsFlowService
    {
        public string GetBarcode()
        {
            return GoodsFlowRepository.GetBarcode();
        }

        public IList<GoodsFlowEntity> QueryWaitingValid(GoodsFlowValidType validType, bool isValid, string hospitalId)
        {
            return this.QueryWaitingValid(validType, isValid, "", hospitalId);
        }

        public IList<GoodsFlowEntity> QueryWaitingValid(GoodsFlowValidType validType, bool isValid, string vendorId, string hospitalId)
        {
            return GoodsFlowRepository.QueryWaitingValid(validType, isValid, vendorId, hospitalId);
        }

        public IList<GoodsFlowEntity> QueryWaitingValid(GoodsFlowValidType validType, int formNo, bool isValid, string hospitalId)
        {
            return this.QueryWaitingValid(validType, formNo, isValid, "", hospitalId);
        }

        public IList<GoodsFlowEntity> QueryWaitingValid(GoodsFlowValidType validType, int formNo, bool isValid, string vendorId, string hospitalId)
        {
            return GoodsFlowRepository.QueryWaitingValid(validType, formNo, isValid, vendorId, hospitalId);
        }

        public void CancelValid(GoodsFlowValidType validType, string barcode, string userId, string hospitalId)
        {
            this.CancelValid(validType, barcode, userId, null, hospitalId);
        }

        public void CancelValid(GoodsFlowValidType validType, string barcode, string userId, string vendorId, string hospitalId)
        {
            GoodsFlowRepository.CancelValid(validType, barcode, userId, vendorId, hospitalId);
        }

        public void Valid(GoodsFlowValidType validType, string barcode, string userId, string hospitalId)
        {
            GoodsFlowRepository.Valid(validType, barcode, userId, null, hospitalId);
        }

        public void Valid(GoodsFlowValidType validType, string barcode, string userId, string vendorId, string hospitalId)
        {
            GoodsFlowRepository.Valid(validType, barcode, userId, vendorId, hospitalId);
        }

        public int SumValid(GoodsFlowValidType validType, string formId)
        {
            return GoodsFlowRepository.SumValid(validType, formId);
        }

        public GoodsFlowEntity GetByBarcode(GoodsFlowValidType validType, string barcode, string hospitalId)
        {
            return this.GetByBarcode(validType, barcode, null, hospitalId);
        }

        public GoodsFlowEntity GetByBarcode(GoodsFlowValidType validType, string barcode, string vendorId, string hospitalId)
        {
            return GoodsFlowRepository.GetByBarcode(validType, barcode, vendorId, hospitalId);
        }

        public GoodsFlowEntity GetByBarcode(string barcode)
        {
            return GoodsFlowRepository.GetByBarcode(barcode);
        }

        public void BatchCancelValid(GoodsFlowValidType validType, string id, string userId)
        {
            GoodsFlowRepository.BatchCancelValid(validType, id, userId);
        }




        public bool ValidInspection(string barcode, string userId, string hospitalId)
        {
            return GoodsFlowRepository.ValidInspection(barcode, userId, hospitalId);
        }

        public bool CancelInspection(string barcode, string userId, string hospitalId)
        {
            return GoodsFlowRepository.CancelInspection(barcode, userId, hospitalId);
        }

        public bool ValidIncoming(string barcode, string userId, string hospitalId)
        {
            return GoodsFlowRepository.ValidIncoming(barcode, userId, hospitalId);
        }

        public bool CancelIncoming(string barcode, string userId, string hospitalId)
        {
            return GoodsFlowRepository.CancelIncoming(barcode, userId, hospitalId);
        }


        public bool ValidReturn(string barcode, string returnForm, string userId)
        {
            return GoodsFlowRepository.ValidReturn(barcode, returnForm, userId);
        }

        public bool CancelReturn(string barcode, string formId, string userId)
        {
            return GoodsFlowRepository.CancelReturn(barcode, formId, userId);
        }
    }
}
