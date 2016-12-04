namespace OneDriveRestAPI.Util
{
    public static class EnumExtensions
    {
        //private const string DisplayAttributeTypeName = "System.ComponentModel.DataAnnotations.DisplayAttribute";
        //private const string DisplayAttributeGetDescriptionMethodName = "GetDescription";

        //private static readonly Func<PropertyInfo, bool> StringTypedProperty = p => p.PropertyType == typeof(string);
        //public static string GetDescription(this Enum value)
        //{
            
        //    var fi = value.GetType().GetField(value.ToString());
        //    var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute),false);
        //    return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        //}
        //// I had to add this method because PCL doesn't have DescriptionAttribute & I didn't want two versions of the code & thus the reflection
        //private static string GetCustomDescription(MemberInfo memberInfo)
        //{
        //    var attrs = memberInfo.GetCustomAttributes(true);

        //    foreach (var attr in attrs)
        //    {
        //        var attrType = attr.GetType();
        //        if (attrType.FullName == DisplayAttributeTypeName)
        //        {
        //            var method = attrType.GetMethod(DisplayAttributeGetDescriptionMethodName);
        //            if (method != null)
        //                return method.Invoke(attr, new object[0]).ToString();
        //        }
        //        var descriptionProperty =
        //            attrType.GetProperties()
        //                .Where(StringTypedProperty)
        //                .FirstOrDefault(Configurator.EnumDescriptionPropertyLocator);
        //        if (descriptionProperty != null)
        //            return descriptionProperty.GetValue(attr, null).ToString();
        //    }

        //    return null;
        //}

    }
}