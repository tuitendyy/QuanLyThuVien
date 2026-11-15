using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Repositories
{
    /// <summary>Repository tổng quát cho các bảng danh mục đơn giản (Thể loại, Tác giả, NXB...).</summary>
    public class CrudRepository<T> where T : class
    {
        public List<T> GetAll()
        {
            using (var db = new LibraryContext()) return db.Set<T>().ToList();
        }

        public List<T> Find(Expression<Func<T, bool>> predicate)
        {
            using (var db = new LibraryContext()) return db.Set<T>().Where(predicate).ToList();
        }

        public void Add(T entity)
        {
            using (var db = new LibraryContext())
            {
                db.Set<T>().Add(entity);
                db.SaveChanges();
            }
        }

        public void Update(T entity)
        {
            using (var db = new LibraryContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(params object[] key)
        {
            using (var db = new LibraryContext())
            {
                var e = db.Set<T>().Find(key);
                if (e == null) return;
                db.Set<T>().Remove(e);
                db.SaveChanges();
            }
        }
    }
}
