using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace KNI_D6_web.Model
{
    public enum UserPosition
    {
        [Description("Игрок")]
        Member = 0,
        [Description("Почетный член КНИ")]
        HonoraryMember = 1,
        [Description("Админ")]
        Admin = 2,
        [Description("Сектерать")]
        Secretary = 3,
        [Description("Председатель")]
        Chairman = 4
    }

    public static class UserPositionExtention
    {
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }
    }
}
