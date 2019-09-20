using KNI_D6_web.Model.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IParameterValuesRepository
    {
        Task<bool> ResetParameterValuesById(int parameterId, int newValue = ParameterValue.DefaultValue);

        Task<bool> IncrementParamenterValueForUser(int parameterId, string userId);

        Task<bool> DecrementParamenterValueForUser(int parameterId, string userId);

        Task<bool> AddParameterValues(IEnumerable<ParameterValue> entities);
    }
}
