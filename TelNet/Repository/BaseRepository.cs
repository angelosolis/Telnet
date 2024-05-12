using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TelNet.Contracts;

namespace TelNet.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
         where T : class
    {
        private readonly DbContext _db;
        private readonly DbSet<T> _table;

        public class CheckoutItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public DbSet<T> Table
        {
            get { return _table; }
        }

        public BaseRepository()
        {
            _db = new TelnetDBEntities();
            _table = _db.Set<T>();
        }

        public T Get(object id)
        {
            return _table.Find(id);
        }


        public List<T> GetAll()
        {
            return _table.ToList();
        }

        public ErrorCode Create(T t)
        {
            try
            {
                _table.Add(t);
                _db.SaveChanges();

                return ErrorCode.Success;
            }
            catch (Exception)
            {
                return ErrorCode.Error;
            }
        }

        public ErrorCode Delete(object id)
        {
            try
            {
                var obj = Get(id);
                _table.Remove(obj);
                _db.SaveChanges();

                return ErrorCode.Success;
            }
            catch (Exception)
            {
                return ErrorCode.Error;
            }
        }

        public ErrorCode Update(object id, T t)
        {
            try
            {
                var Oldobj = Get(id);
                _db.Entry(Oldobj).CurrentValues.SetValues(t);
                _db.SaveChanges();

                return ErrorCode.Success;
            }
            catch (Exception)
            {
                return ErrorCode.Error;
            }
        }

        public ErrorCode UpdateStatus(object id, int status)
        {
            throw new NotImplementedException();
        }
    }
}
