using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using LIMS.Repositories;
using LIMS.Entities;
using LIMS.Models;
using LIMS.Util;

namespace LIMS.Services
{
    public class UnitService
    {
        private static object ms_lock = new object();
        private static IDictionary<string, UnitModel> ms_UnitCache = new Dictionary<string, UnitModel>();

        public IList<UnitEntity> QueryRoots(UnitType unitType)
        {
            return UnitRepository.Query(Constant.DEFAULT_UNIT_ROOT_ID, unitType);
        }

        public IList<UnitEntity> Query(string name, string rootId)
        {
            return UnitRepository.Query(name, rootId);
        }

        public IList<UnitEntity> QueryRoots(string condition, UnitType unitType, PagerInfo pager)
        {
            return UnitRepository.Query(condition, Constant.DEFAULT_UNIT_ROOT_ID, unitType, pager);
        }

        public IList<UnitEntity> QueryUnits(string condition, string parentId, PagerInfo pager)
        {
            return UnitRepository.Query(condition, parentId, UnitType.None, pager);
        }

        public IList<UnitEntity> GetByRootId(string rootId)
        {
            return UnitRepository.GetByRootId(rootId);
        }

        public IList<UnitEntity> GetAllById(string id)
        {
            return UnitRepository.GetAllById(id);
        }

        public UnitModel Get(string id)
        {
            UnitModel mode;
            if(ms_UnitCache.TryGetValue(id, out mode))
            {
                return mode;
            }

            lock (ms_lock)
            {
                if (ms_UnitCache.TryGetValue(id, out mode))
                {
                    return mode;
                }
                else
                {
                    var entity = UnitRepository.Get(id);
                    if (entity == null)
                    {
                        return null;
                    }

                    var unit = new UnitModel
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        Type = entity.Type,
                        Description = entity.Description,
                        ShortCode = entity.ShortCode,
                        ContactId = entity.ContactId,
                        ReceiptId = entity.DefaultReceiptId,
                        BusinessType = entity.BusinessType,
                        RootId = entity.RootId
                    };

                    if (!string.IsNullOrEmpty(entity.ContactId))
                    {
                        var contact = new ContactInfoService().Get(entity.ContactId);

                        if (contact != null)
                        {
                            unit.ContactInfo = new ContactInfoModel
                            {
                                Id = contact.Id,
                                VesteeId = contact.VesteeId,
                                Address = contact.Address,
                                ContactPerson = contact.ContactPerson,
                                ContactWay1 = contact.ContactWay1,
                                ContactWay2 = contact.ContactWay2,
                                ContactWay3 = contact.ContactWay3,
                                ContactWay4 = contact.ContactWay4
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(entity.DefaultReceiptId))
                    {
                        unit.Receipt = new ReceiptInfoService().Get(entity.DefaultReceiptId);
                    }
                    ms_UnitCache[unit.Id] = unit;

                    return unit;
                }
            }
            
        }

        public IList<UnitEntity> GetStorerooms(string rootId)
        {
            return UnitRepository.GetByBusinessType(rootId, UnitBusinessType.Storeroom);
        }

        public IList<UnitEntity> GetStorerooms(string rootId, string userId)
        {
            return UnitRepository.GetByBusinessType(rootId, userId, UnitBusinessType.Storeroom);
        }

        public void Save(UnitEntity unit, ContactInfoEntity contactInfo)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var conn = db.CreateConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {

                        if (string.IsNullOrEmpty(contactInfo.Id))
                        {
                            contactInfo.Id = Guid.NewGuid().ToString();
                            ContactInfoRepository.Add(contactInfo, db, trans);
                        }
                        else
                        {
                            ContactInfoRepository.Update(contactInfo, db, trans);
                        }

                        unit.ContactId = contactInfo.Id;
                        if (string.IsNullOrEmpty(unit.Id))
                        {
                            unit.Id = Guid.NewGuid().ToString();
                            if (unit.Type == UnitType.Hospital || unit.Type == UnitType.Vendor)
                            {
                                unit.RootId = Constant.DEFAULT_UNIT_ROOT_ID;
                                unit.ParentId = unit.Id;
                            }

                            UnitRepository.Add(unit, db, trans);
                        }
                        else
                        {
                            UnitRepository.Update(unit, db, trans);
                            ClearCache(unit.Id);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

        }

        private void ClearCache(string id)
        {
            lock (ms_lock)
            {
                if(ms_UnitCache.Keys.Contains(id))
                {
                    ms_UnitCache.Remove(id);
                }
            }
        }

        public IList<UnitEntity> GetHospitalsByUserId(string userId)
        {
            return UnitRepository.GetHospitalsByUserId(userId);
        }

        public IList<UnitEntity> GetApplyUnits(string userId, string hospitalId)
        {
            return UnitRepository.GetApplyUnits(userId, hospitalId);
        }

        public IList<UnitEntity> GetVendorsByHospitalUnit(string unitId)
        {
            return UnitRepository.GetVendorsByHospitalUnit(unitId);
        }

        public IList<UnitEntity> GetVendorsByHospital(string hospitalId)
        {
            return UnitRepository.GetVendorsByHospital(hospitalId);
        }

        public IList<UnitEntity> GetHospitalsByVendor(string vendorId)
        {
            return UnitRepository.GetHospitalsByVendor(vendorId);
        }
    }
}
