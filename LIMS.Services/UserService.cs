using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIMS.Models;
using LIMS.Entities;
using LIMS.Repositories;

namespace LIMS.Services
{
    public class UserService
    {
        private static object ms_Lock = new object();
        private static IDictionary<string, UserEntity> ms_Users = new Dictionary<string, UserEntity>();

        public static bool TryGetUserByAccount(string account, out UserEntity user)
        {
            if(UserRepository.TryGetByAccount(account, out user))
            {
                Set(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void Set(UserEntity user)
        {
            if(!ms_Users.ContainsKey(user.Id))
            {
                lock(ms_Lock)
                {
                    if(!ms_Users.ContainsKey(user.Id))
                    {
                        ms_Users[user.Id] = user;
                    }
                }
            }
        }

        public static bool TryGet(string id, out UserEntity user)
        {
            if(!ms_Users.TryGetValue(id, out user))
            {
                user = UserRepository.Get(id);
                if(user == null)
                {
                    return false;
                }

                Set(user);
            }

            return true;
        }

        public UserEntity GetByAccount(string account, string userId)
        {
            return UserRepository.GetByAccount(account, userId);
        }

        public IList<UserEntity> Query(string name, string rootId)
        {
            return UserRepository.Query(name, rootId);
        }

        public IList<UserModel> Query(string condition, string rootId, string unitId, PagerInfo pager)
        {
            var unitService = new UnitService();

            var entities = UserRepository.Query(condition, rootId, unitId, pager);
            
            var list = new List<UserModel>();
            foreach(var entity in entities)
            {
                var mode = new UserModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Account = entity.Account,
                    UnitId = entity.UnitId,
                    Title = entity.Title
                };

                var unit = unitService.Get(entity.UnitId);
                if(unit != null)
                {
                    mode.UnitName = unit.Name;
                }

                list.Add(mode);
            }

            return list;
        }

        public UserModel Get(string id)
        {
            var entity = UserRepository.Get(id);

            if(entity != null)
            {
                return new UserModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Account = entity.Account,
                    UnitId = entity.UnitId,
                    Title = entity.Title
                };
            }
            else
            {
                return null;
            }
        }

        public void Save(UserEntity user)
        {
            if(string.IsNullOrEmpty(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
                UserRepository.Add(user);
            }
            else
            {
                UserRepository.Update(user);
                ClearCache(user.Id);
            }
        }

        private void ClearCache(string id)
        {
            lock(ms_Lock)
            {
                if(ms_Users.Keys.Contains(id))
                {
                    ms_Users.Remove(id);
                }
            }
        }
    }
}
