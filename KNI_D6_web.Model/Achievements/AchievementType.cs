using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.Model.Achievements
{
    public enum AchievementType
    {
        [Description("Невычисляемое"), Display(Name = "Невычисляемое")]
        Custom = 0,

        [Description("Вычисляемое"), Display(Name = "Вычисляемое")]
        Calculated = 1
    }
}
