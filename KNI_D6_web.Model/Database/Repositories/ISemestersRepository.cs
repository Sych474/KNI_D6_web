using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface ISemestersRepository
    {
        Task<bool> AddSemesterAsync(Semester entity);

        Task<List<Semester>> GetSemestersAsync();

        Task<Semester> FindSemesterByIdAsync(int id);

        Task<Semester> FindCurrentSemesterAsync();

        Task<bool> UpdateSemesterAsync(Semester entity);

        Task<bool> RemoveSemesterByIdAsync(int id);

        Task<bool> SetSemesterAsCurrentByIdAsync(int id);
    }
}
