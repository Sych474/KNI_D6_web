using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.Model.Achievements
{
    public enum AchievementType
    {
        [Description("Вычисляемое"), Display(Name = "Вычисляемое")]
        Custom = 0,

        [Description("Невычисляемое"), Display(Name = "Невычисляемое")]
        Calculated = 1
    }
}
