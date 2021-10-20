using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

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
            RWEachField(dataType,
                (rule, info) => rule.Write(writer, info.GetValue(obj)), 
                (info, e) => throw new Exception($"While writing value '{info.Name}': {e.Message}"));
        }

        public object[] Read(Type dataType, BinaryReader reader)
        {
            var dataList = new List<object>();
            
            RWEachField(dataType,
                (rule, _) => dataList.Add(rule.Read(reader)), 
                (info, e) => throw new Exception($"While reading value '{info.Name}': {e.Message}"));

            return dataList.ToArray();
        }

        void RWEachField(Type dataType, Action<IReadWriteRule, FieldInfo> rw, Action<FieldInfo, Exception> onException)
        {
            var fieldInfos = dataType.GetFields();

            foreach (var fieldInfo in fieldInfos)
            {
                if (!IsSavable(fieldInfo)) return;
                var success = false;
                
                foreach (var rule in readWriteRules)
                {
                    try
                    {
                        if (rule.DataType == fieldInfo.FieldType)
                        {
                            rw.Invoke(rule, fieldInfo);
                            success = true;
                        }
                    }
                    catch (Exception e)
                    { onException.Invoke(fieldInfo, e); }
                }
                
                if (!success) Debug.LogWarning($"RW Rule not found for {fieldInfo.FieldType}. Skipping...");
            }
        }

        internal static bool IsSavable(FieldInfo i)
        {
            if (i.GetCustomAttribute<SavableAttribute>() == null) return false;
            if (i.IsStatic) throw new Exception($"Value '{i.Name}' is static and Savable.");
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class SavableAttribute : Attribute { }

    public interface IReadWriteRule
    {
        public abstract Type DataType { get; }
        public abstract void Write(BinaryWriter writer, object value);
        public abstract object Read(BinaryReader reader);
    }

    public class StringRule : IReadWriteRule
    {
        public Type DataType => typeof(string);
        public void Write(BinaryWriter writer, object value) => writer.Write((string)value ?? "");
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