using KNI_D6_web.Model.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IParametersRepository 
    {
        Task<bool> AddParameterAsync(Parameter entity);

        Task<bool> UpdateParameterAsync(Parameter entity);

        Task<bool> RemoveParameterByIdAsync(int id);

        Task<List<Parameter>> GetParametersAsync();
    }
}
