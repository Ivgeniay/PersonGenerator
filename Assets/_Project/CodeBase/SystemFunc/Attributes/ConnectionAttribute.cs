using System;

namespace AtomEngine.SystemFunc.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ConnectionAttribute : Attribute
    {
        public string ConnectionFieldName { get; private set; }
        public bool LastPoint { get; set; }
        public BodyChapterType BodyChapterType { get; set; }
        public ConnectionAttribute(string connectionFieldName)
        {
            ConnectionFieldName = connectionFieldName;
        } 
    }
}
