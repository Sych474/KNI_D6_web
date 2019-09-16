using System.Threading.Tasks;

namespace KNI_D6_web.Model.Achievements
{
    public interface IAchievementsCalculator
    {
        Task<bool> IsDone(int achievementId, string userId);
    }
}