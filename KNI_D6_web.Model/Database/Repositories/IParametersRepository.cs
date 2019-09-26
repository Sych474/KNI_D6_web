using KNI_D6_web.Model.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IParametersRepository 
    {
        Task<int> AddParameterAsync(Parameter entity);

        Task<Parameter> FindParameterByNameAsync(string name);

        Task<Parameter> FindParameterByIdAsync(int id);

        Task UpdateParameterAsync(Parameter entity);

        Task RemoveParameterByIdAsync(int id);

        Task<List<Parameter>> GetParametersAsync();
    }
}
