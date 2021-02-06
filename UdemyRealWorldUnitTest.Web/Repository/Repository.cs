using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Models;

namespace UdemyRealWorldUnitTest.Web.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        //veritabanı ile igili işlemleri burada gerçekleştiriyoruz

        private readonly UdemyUnitTestDBContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(UdemyUnitTestDBContext context)
        {

            _context = context;//new UdemyUnitTestDBContext() denilenebilir.

            //tabloyu tutuyor
            _dbSet = _context.Set < TEntity >();
        }

        public async Task Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            //sadece primary key üzerinden ilgili datayı bulur
            return await _dbSet.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            //entity değişti diyoruz
            _context.Entry(entity).State = EntityState.Modified;

           // _dbSet.Update(entity);
            _context.SaveChanges();

        }
    }
}
