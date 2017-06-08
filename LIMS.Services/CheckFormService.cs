using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Util;
using LIMS.Models;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class CheckFormService
    {
        public CheckFormEntity Get(string id)
        {
            return CheckFormRepository.Get(id);
        }

        public IList<CheckFormCategoryEntity> GetCategories(string id)
        {
            return CheckFormRepository.GetCategories(id);
        }

        public IList<CheckFormProductEntity> GetProducts(string id)
        {
            return CheckFormRepository.GetProducts(id);
        }

        public CheckFormProductEntity GetCheckProduct(string formId, string productId)
        {
            return CheckFormRepository.GetCheckProduct(formId, productId);
        }

        public void Save(CheckFormEntity form, IList<CheckFormCategoryEntity> categories)
        {
            CheckFormRepository.Save(form, categories);
        }

        public IList<CheckFormEntity> Query(CheckCondition condition, PagerInfo pager)
        {
            return CheckFormRepository.Query(condition, pager);
        }


        public void BeginCheck(string id, string userId)
        {
            CheckFormRepository.UpdateStatus(id, CheckFormStatus.Checking, userId);
        }


        public void Complete(string id, string userId)
        {
            CheckFormRepository.UpdateStatus(id, CheckFormStatus.Complete, userId);
        }

        //public void AuditProducts(string formId, IList<string> noMatchIds, string operatorId)
        //{
        //    CheckFormRepository.AduitProducts(formId, noMatchIds, operatorId);
        //}

        public int SaveCheckProducts(string id, IList<CheckFormProductEntity> checkProducts, string userId)
        {
            if(checkProducts != null && checkProducts.Count > 0)
            {
                foreach (var item in checkProducts)
                {
                    item.CheckId = id;
                    item.OperatorId = userId;
                    item.OperatedTime = DateTime.Now;
                }
            }

            return CheckFormRepository.UpdateCheckProducts(id, checkProducts, userId);
        }

        public bool IsAllMatch(string id, string userId)
        {
            return CheckFormRepository.IsAllMatch(id, userId);
        }

        public void HandleCheckProducts(string id, string userId)
        {
            CheckFormRepository.HandleCheckProducts(id, userId);
        }

        public void OverCheckProducts(string id, string userId)
        {
            CheckFormRepository.OverCheckProducts(id, userId);
        }

        public void RedoCheckProducts(string id, string userId)
        {
            CheckFormRepository.RedoCheckProducts(id, userId);
        }

        //public IList<CheckFormProductEntity> GetCheckProducts(string id, string userId)
        //{
        //    return CheckFormRepository.GetCheckProducts(id, userId);
        //}

        public CheckFormProductValidationEntity GetProductValidation(string id)
        {
            return CheckFormRepository.GetProductValidation(id);
        }

        public CheckFormProductValidationEntity GetProductValidation(string formId, string barcode)
        {
            return CheckFormRepository.GetProductValidation(formId, barcode);
        }

        public void SaveProductValidation(CheckFormProductValidationEntity entity)
        {
            CheckFormRepository.SaveProductValidation(entity);
        }



        public IList<CheckFormProductEntity> GetHandledCheckProducts(string formId)
        {
            return CheckFormRepository.GetHandledCheckProducts(formId);
        }

        public IList<CheckFormProductValidationEntity> GetProductsValidation(string formId)
        {
            return CheckFormRepository.GetProductsValidation(formId);
        }


        public IList<CheckFormProductEntity> GetHandleCheckProducts(string formId, string userId)
        {
            return CheckFormRepository.GetHandleCheckProducts(formId, userId);
        }

        public IList<CheckFormProductValidationEntity> GetProductsValidation(string formId, string userId)
        {
            return CheckFormRepository.GetProductsValidation(formId, userId);
        }


        public void Compute(string id, string userId)
        {
            CheckFormRepository.Compute(id, userId);
        }

        public void ReturnCheckProducts(string id, string userId)
        {
            CheckFormRepository.ReturnCheckProducts(id, userId);
        }




        public int CountUnhandle(string checkId)
        {
            return CheckFormRepository.CountUnhandle(checkId);
        }

        public int CountUnadjust(string checkId)
        {
            return CheckFormRepository.CountUnadjust(checkId);
        }

        public void Ignore(string checkId, string id)
        {
            CheckFormRepository.UpdateAdjustType(checkId, id, CheckAdjustType.Ignore);
        }

        public void Override(string checkId, string id)
        {
            CheckFormRepository.UpdateAdjustType(checkId, id, CheckAdjustType.Override);
        }

        public void AdjustInventory(string id, string userId)
        {
            CheckFormRepository.AdjustInventory(id, userId);
        }
    }
}
