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

        public async Task<int> AddParameterAsync(Parameter entity)
        {
            context.Parameters.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;    
        }

        public Task<Parameter> FindParameterByIdAsync(int id)
        {
            return context.Parameters.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<Parameter> FindParameterByNameAsync(string name)
        {
            return context.Parameters.FirstOrDefaultAsync(p => p.Name == name);
        }

        public Task<List<Parameter>> GetParametersAsync()
        {
            return context.Parameters.ToListAsync();
        }

        public async Task RemoveParameterByIdAsync(int id)
        {
            var parameter = await context.Parameters.FirstOrDefaultAsync(p => p.Id == id);
            if (parameter != null)
            {
                context.Parameters.Remove(parameter);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateParameterAsync(Parameter entity)
        {
            context.Parameters.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
