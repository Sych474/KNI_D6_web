using System.Collections.Generic;
using System.Threading.Tasks;
using KNI_D6_web.Model.Parameters;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class ParametersRepository : BaseRepository, IParametersRepository
    {
        public ParametersRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> AddParameterAsync(Parameter entity)
        {
            bool result = true;
            try
            {
                context.Parameters.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                result = false;
            }

            return result;
        }

        public Task<List<Parameter>> GetParametersAsync()
        {
            return context.Parameters.ToListAsync();
        }

        public async Task<bool> RemoveParameterByIdAsync(int id)
        {
            bool result = true;
            try
            {
                var parameter = await context.Parameters.FirstOrDefaultAsync(p => p.Id == id);
                if (parameter != null)
                {
                    context.Parameters.Remove(parameter);
                    await context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException)
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> UpdateParameterAsync(Parameter entity)
        {
            bool result = true;
            try
            {
                context.Parameters.Update(entity);
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
