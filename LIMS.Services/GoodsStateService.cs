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
    public class GoodsStateService
    {
        public string GetBarcode()
        {
            return GoodsStateRepository.GetBarcode();
        }
        
        public IList<string> GetBarcodes(int count)
        {
            return GoodsStateRepository.GetBarcodes(count);
        }

        public IList<GoodsStateEntity> QueryInvalid(FormType formType, string vendorId, string hospitalId)
        {
            return GoodsStateRepository.QueryInvalid(formType, vendorId, hospitalId);
        }

        public IList<GoodsStateEntity> QueryInvalid(int orderFormNo, FormType formType, string vendorId, string hospitalId)
        {
            return GoodsStateRepository.QueryInvalid(orderFormNo, formType, vendorId, hospitalId);
        }

        public GoodsStateEntity GetByBarcode(string barcode)
        {
            return GoodsStateRepository.GetByBarcode(barcode);
        }


        #region Valid or Invalid
        public bool Cancel(string formId, FormType formType, string userId)
        {
            return GoodsStateRepository.SetValid(formId, formType, false, userId);
        }

        public bool Cancel(string barcode, string futureFormId, FormType futureFormType, string userId, string hospitalId, out GoodsBarModel model, out string errorCode)
        {
            return this.SetValid(barcode, futureFormId, futureFormType, hospitalId, false, userId, out model, out errorCode);
        }

        public bool Cancel(string barcode, FormType formType, string hospitalId, string vendorId, string userId, out GoodsBarModel model, out string errorCode)
        {
            return this.SetValid(barcode, formType, hospitalId, vendorId, userId, false, out model, out errorCode);
        }

        public bool Cancel(string barcode, FormType formType, string hospitalId, string userId, out string errorCode)
        {
            return this.SetValid(barcode, formType, hospitalId, userId, false, out errorCode);
        }

        public bool Cancel(string barcode, FormType formType, string hospitalId, string userId,
            out GoodsBarModel model, out string errorCode)
        {
            return this.SetValid(barcode, formType, hospitalId, userId, false, out model, out errorCode);
        }

        public bool Validate(string barcode, string futureFormId, FormType futureFormType, string userId, string hospitalId,
            out GoodsBarModel model, out string errorCode)
        {
            return this.SetValid(barcode, futureFormId, futureFormType, hospitalId, true, userId, out model, out errorCode);
        }

        public bool Validate(string barcode, FormType formType, string hospitalId, string vendorId, string userId, out string errorCode)
        {
            return this.SetValid(barcode, formType, hospitalId, vendorId, userId, true, out errorCode);
        }

        public bool Validate(string barcode, FormType formType, string hospitalId, string userId, out string errorCode)
        {
            return this.SetValid(barcode, formType, hospitalId, userId, true, out errorCode);
        }

        public bool Validate(string barcode, FormType formType, string hospitalId, string userId, 
            out GoodsBarModel model, out string errorCode)
        {
            return this.SetValid(barcode, formType, hospitalId, userId, true, out model, out errorCode);
        }


        private bool SetValid(string barcode, FormType formType, string hospitalId, string vendorId, string userId, bool valid, out string errorCode)
        {
            if (!this.CanValidate(barcode, formType, hospitalId, vendorId, out errorCode))
            {
                return false;
            }

            if (!GoodsStateRepository.SetValid(barcode, valid, userId))
            {
                errorCode = GoodsStateValidateCodes.BarcodeNotExist;
                return false;
            }

            return true;
        }

        private bool SetValid(string barcode, FormType formType, string hospitalId, string userId, bool valid, out string errorCode)
        {
            if (!this.CanValidate(barcode, formType, hospitalId, out errorCode))
            {
                return false;
            }

            if (!GoodsStateRepository.SetValid(barcode, valid, userId))
            {
                errorCode = GoodsStateValidateCodes.BarcodeNotExist;
                return false;
            }

            return true;
        }

        private bool SetValid(string barcode, FormType formType, string hospitalId, string userId, bool valid,
            out GoodsBarModel model, out string errorCode)
        {
            model = null;
            if(!this.SetValid(barcode, formType, hospitalId, userId, valid, out errorCode))
            {
                return false;
            }

            var entity = this.GetByBarcode(barcode);

            var product = new ProductService().Get(entity.ProductId);
            if (product == null)
            {
                throw new Exception(string.Format("The product({0}) does not exist", entity.ProductId));
            }

            model = new GoodsBarModel
            {
                Id = entity.Id,
                FormNo = entity.OrderFormNo,
                ProductName = product.Name,
                Barcode = entity.Barcode,
                IsValid = entity.FutureValid
            };

            return true;
        }

        private bool SetValid(string barcode, FormType formType, string hospitalId, string vendorId, string userId, bool valid,
            out GoodsBarModel model, out string errorCode)
        {
            model = null;
            if (!this.SetValid(barcode, formType, hospitalId, vendorId, userId, valid, out errorCode))
            {
                return false;
            }

            var entity = this.GetByBarcode(barcode);

            var product = new ProductService().Get(entity.ProductId);
            if (product == null)
            {
                throw new Exception(string.Format("The product({0}) does not exist", entity.ProductId));
            }

            model = new GoodsBarModel
            {
                Id = entity.Id,
                FormNo = entity.OrderFormNo,
                ProductName = product.Name,
                Barcode = entity.Barcode,
                IsValid = entity.FutureValid
            };

            return true;
        }

        public bool SetValid(string barcode, string formId, FormType formType, string hospitalId, bool valid, string userId, out GoodsBarModel model, out string errorCode)
        {
            model = null;
            if (formType == FormType.Return)
            {
                var returnForm = ReturnFormRepository.Get(formId);
                if(!GoodsStateRepository.CanValidate(barcode, formType, hospitalId, returnForm.VendorId, out errorCode))
                {
                    return false;
                }
            }
            else
            {
                if (!GoodsStateRepository.CanValidate(barcode, formType, hospitalId, out errorCode))
                {
                    return false;
                }
            }

            if (!GoodsStateRepository.SetValid(barcode, formId, formType, valid, userId))
            {
                errorCode = GoodsStateValidateCodes.BarcodeNotExist;
                return false;
            }

            var entity = this.GetByBarcode(barcode);

            var product = new ProductService().Get(entity.ProductId);
            if (product == null)
            {
                throw new Exception(string.Format("The product({0}) does not exist", entity.ProductId));
            }

            model = new GoodsBarModel
            {
                Id = entity.Id,
                FormNo = entity.OrderFormNo,
                ProductName = product.Name,
                Barcode = entity.Barcode,
                IsValid = entity.FutureValid
            };

            return true;
        }
        #endregion


        #region Validate State
        public bool CanFormValidate(string formId, FormType formType, string hospitalId, out string errorCode)
        {
            return GoodsStateRepository.CanFormValidate(formId, formType, hospitalId, out errorCode);
        }

        public bool CanValidate(string barcode, FormType formType, string hospitalId, out string errorCode)
        {
            return GoodsStateRepository.CanValidate(barcode, formType, hospitalId, out errorCode);
        }

        public bool CanValidate(string barcode, FormType formType, string hospitalId, string vendorId, out string errorCode)
        {
            return GoodsStateRepository.CanValidate(barcode, formType, hospitalId, vendorId, out errorCode);
        }
        #endregion


        public int CountValid(string futureFormId, FormType futureFormType)
        {
            return GoodsStateRepository.CountValid(futureFormId, futureFormType);
        }
    }
}
