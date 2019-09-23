using KNI_D6_web.Model.Parameters;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IParameterValuesRepository
    {
        Task ResetParameterValuesByIdAsync(int parameterId, int newValue = ParameterValue.DefaultValue);

        Task IncrementParamenterValueForUserAsync(int parameterId, string userId);

        Task DecrementParamenterValueForUserAsync(int parameterId, string userId);

        Task AddParameterValuesForParameterAsync(int parameterId);

        Task AddAllParameterValuesForUserAsync(string userId);
    }
}
