using System;
using System.Reflection;

namespace OneDriveRestAPI.Model
{
    public enum PictureSize
    {
        [Description("small")]
        Small,
        [Description("medium")]
        Medium,
        [Description("large")]
        Large
    }

    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetRuntimeField(value.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute),false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();

        }
    }
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }
       
        public DescriptionAttribute(string value)
        {

            Description = value;
        }
    }
}