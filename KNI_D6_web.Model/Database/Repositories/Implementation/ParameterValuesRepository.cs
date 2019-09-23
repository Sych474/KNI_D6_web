using System.Linq;
using System.Threading.Tasks;
using KNI_D6_web.Model.Database.Repositories.Exceptions;
using KNI_D6_web.Model.Parameters;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class ParameterValuesRepository : BaseRepository, IParameterValuesRepository
    {
        public ParameterValuesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddAllParameterValuesForUserAsync(string userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new EntityNotFoundException($"user with id {userId} not found");

            var parameterValues = context.Parameters.Select(p => new ParameterValue()
            {
                UserId = userId,
                ParameterId = p.Id,
                Value = ParameterValue.DefaultValue
            });

            await context.ParameterValues.AddRangeAsync(parameterValues);
            await context.SaveChangesAsync();
        }

        public async Task AddParameterValuesForParameterAsync(int parameterId)
        {
            var parameter = await context.Parameters.FirstOrDefaultAsync(u => u.Id == parameterId);

            if (parameter == null)
                throw new EntityNotFoundException($"parameter with id {parameterId} not found");

            var parameterValues = context.Users.Select(u => new ParameterValue()
            {
                UserId = u.Id,
                ParameterId = parameterId,
                Value = ParameterValue.DefaultValue
            });

            await context.ParameterValues.AddRangeAsync(parameterValues);
            await context.SaveChangesAsync();
        }

        public async Task DecrementParamenterValueForUserAsync(int parameterId, string userId)
        {
            var parameterValue = await context.ParameterValues.FirstOrDefaultAsync(pv => pv.ParameterId == parameterId && pv.UserId == userId);
            if (parameterValue == null)
                throw new EntityNotFoundException($"parameterValue for user with id {userId} and parameter with id {parameterValue} not found");

            parameterValue.Value--;
            context.ParameterValues.Update(parameterValue);

            await context.SaveChangesAsync();
        }

        public async Task IncrementParamenterValueForUserAsync(int parameterId, string userId)
        {
            var parameterValue = await context.ParameterValues.FirstOrDefaultAsync(pv => pv.ParameterId == parameterId && pv.UserId == userId);
            if (parameterValue == null)
                throw new EntityNotFoundException($"parameterValue for user with id {userId} and parameter with id {parameterValue} not found");
            parameterValue.Value++;
            context.ParameterValues.Update(parameterValue);

            await context.SaveChangesAsync();
        }

        public async Task ResetParameterValuesByIdAsync(int parameterId, int newValue = ParameterValue.DefaultValue)
        {
            var parameterValues = await context.ParameterValues.Where(pv => pv.ParameterId == parameterId).ToListAsync();

            foreach (var item in parameterValues)
            {
                item.Value = newValue;
            }
            context.ParameterValues.UpdateRange(parameterValues);
            await context.SaveChangesAsync();
        }
    }
}
