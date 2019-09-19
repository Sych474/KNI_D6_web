using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class SemestersRepository : ISemestersRepository
    {
        private readonly ApplicationDbContext context;

        public SemestersRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddSemesterAsync(Semester entity)
        {
            bool result = true;
            try
            {
                context.Semesters.Add(entity);

                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                result = false;
            }
            return result;
        }

        public Task<Semester> FindCurrentSemesterAsync()
        {
            return context.Semesters.FirstOrDefaultAsync(s => s.IsCurrent);
        }

        public Task<Semester> FindSemesterByIdAsync(int id)
        {
            return context.Semesters.FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<List<Semester>> GetSemestersAsync()
        {
            return context.Semesters.ToListAsync();
        }

        public async Task<bool> RemoveSemesterByIdAsync(int id)
        {
            bool result = true;
            try
            {
                var entity = await context.Semesters.FirstOrDefaultAsync(s => s.Id == id);
                context.Semesters.Remove(entity);

                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> UpdateSemesterAsync(Semester entity)
        {
            bool result = true;
            try
            {
                context.Semesters.Update(entity);

                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                result = false;
            }
            return result;
        }
    }
}
