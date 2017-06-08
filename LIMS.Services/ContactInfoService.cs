using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Entities;
using LIMS.Models;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class ContactInfoService
    {
        public IList<ContactInfoModel> GetByVesteeId(string vesteeId)
        {
            var entities = ContactInfoRepository.GetByVesteeId(vesteeId);

            var list = new List<ContactInfoModel>();
            foreach(var entity in entities)
            {
                list.Add(new ContactInfoModel
                {
                    Id = entity.Id,
                    VesteeId = entity.VesteeId,
                    Address = entity.Address,
                    ContactPerson = entity.ContactPerson,
                    ContactWay1 = entity.ContactWay1,
                    ContactWay2 = entity.ContactWay2,
                    ContactWay3 = entity.ContactWay3,
                    ContactWay4 = entity.ContactWay4
                });
            }

            return list;
        }

        public ContactInfoModel Get(string id)
        {
            var entity = ContactInfoRepository.Get(id);

            if(entity != null)
            {
                return new ContactInfoModel
                {
                    Id = entity.Id,
                    VesteeId = entity.VesteeId,
                    Address = entity.Address,
                    ContactPerson = entity.ContactPerson,
                    ContactWay1 = entity.ContactWay1,
                    ContactWay2 = entity.ContactWay2,
                    ContactWay3 = entity.ContactWay3,
                    ContactWay4 = entity.ContactWay4
                };
            }
            else
            {
                return null;
            }
        }
    }
}
