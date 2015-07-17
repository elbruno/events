using System;

namespace ElBruno.LegoLeapWpf.Directions.Annotations
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AspMvcAreaPartialViewLocationFormatAttribute : Attribute
    {
        public AspMvcAreaPartialViewLocationFormatAttribute(string format) { }
    }
}