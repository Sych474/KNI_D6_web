using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model.Parameters;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class ParameterValuesRepository : BaseRepository, IParameterValuesRepository
    {
        public ParameterValuesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> AddParameterValues(IEnumerable<ParameterValue> entities)
        {
            bool result = true;
            try
            {
                context.ParameterValues.AddRange(entities);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                result = false;
            }
            
            return result;
        }

        public async Task<bool> DecrementParamenterValueForUser(int parameterId, string userId)
        {
            bool result = true;
            var parameterValue = await context.ParameterValues.FirstOrDefaultAsync(pv => pv.ParameterId == parameterId && pv.UserId == userId);
            if (parameterValue != null)
            {
                try
                {
                    parameterValue.Value--;
                    context.ParameterValues.Update(parameterValue);

                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> IncrementParamenterValueForUser(int parameterId, string userId)
        {
            bool result = true;
            var parameterValue = await context.ParameterValues.FirstOrDefaultAsync(pv => pv.ParameterId == parameterId && pv.UserId == userId);
            if (parameterValue != null)
            {
                try
                {
                    parameterValue.Value++;
                    context.ParameterValues.Update(parameterValue);

                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> ResetParameterValuesById(int parameterId, int newValue = ParameterValue.DefaultValue)
        {
            bool result = true;
            try
            {
                var parameterValues = await context.ParameterValues.Where(pv => pv.ParameterId == parameterId).ToListAsync();

                foreach (var item in parameterValues)
                {
                    item.Value = newValue;
                }
                context.ParameterValues.UpdateRange(parameterValues);
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
