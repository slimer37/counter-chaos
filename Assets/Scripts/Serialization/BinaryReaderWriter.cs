using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Serialization
{
    internal class BinaryReaderWriter
    {
        readonly IReadWriteRule[] readWriteRules =
        {
            new StringRule(),
            new IntRule(),
            new FloatRule(),
            new BoolRule(),
            new DateTimeRule()
        };
	
        public void Write(Type dataType, object obj, BinaryWriter writer)
        {
            var fieldInfos = dataType.GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetCustomAttribute<ExcludeFromWrite>() != null) continue;
                foreach (var rule in readWriteRules)
                {
                    if (rule.DataType == fieldInfo.FieldType)
                        rule.Write(writer, fieldInfo.GetValue(obj));
                }
            }
        }

        public object[] Read(Type dataType, BinaryReader reader)
        {
            var fieldInfos = dataType.GetFields();
		
            var dataList = new List<object>();
		
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetCustomAttribute<ExcludeFromWrite>() != null) continue;
			
                foreach (var rule in readWriteRules)
                {
                    if (rule.DataType == fieldInfo.FieldType)
                        dataList.Add(rule.Read(reader));
                }
            }

            return dataList.ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ExcludeFromWrite : Attribute { }

    public interface IReadWriteRule
    {
        public abstract Type DataType { get; }
        public abstract void Write(BinaryWriter writer, object value);
        public abstract object Read(BinaryReader reader);
    }

    public class StringRule : IReadWriteRule
    {
        public Type DataType => typeof(string);
        public void Write(BinaryWriter writer, object value) => writer.Write((string)value);
        public object Read(BinaryReader reader) => reader.ReadString();
    }
    public class IntRule : IReadWriteRule
    {
        public Type DataType => typeof(int);
        public void Write(BinaryWriter writer, object value) => writer.Write((int)value);
        public object Read(BinaryReader reader) => reader.ReadInt32();
    }
    public class FloatRule : IReadWriteRule
    {
        public Type DataType => typeof(float);
        public void Write(BinaryWriter writer, object value) => writer.Write((float)value);
        public object Read(BinaryReader reader) => reader.ReadSingle();
    }
    public class BoolRule : IReadWriteRule
    {
        public Type DataType => typeof(bool);
        public void Write(BinaryWriter writer, object value) => writer.Write((float)value);
        public object Read(BinaryReader reader) => reader.ReadBoolean();
    }
    public class DateTimeRule : IReadWriteRule
    {
        public Type DataType => typeof(DateTime);
        public void Write(BinaryWriter writer, object value) => writer.Write(((DateTime)value).ToBinary());
        public object Read(BinaryReader reader) => DateTime.FromBinary(reader.ReadInt64());
    }
}