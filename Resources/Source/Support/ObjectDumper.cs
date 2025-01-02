// Class is not ready to use
#if false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utils
{
    public static class ObjectDumper
    {
        public static string Dump(object obj, int max_level = 3, int indent_size = 2, int? string_limit = null, int? enumerable_limit = null)
        {
            StringBuilder string_builder = new();
            Stack<object> stack = new();
            stack.Push(obj);
            while (stack.Count > 0)
            {
                obj = stack.Peek();
                switch (obj)
                {
                    case null:
                        {
                            string_builder.Append("null\n");
                        }
                        break;
                    case ValueType value:
                        {
                            string_builder.Append(value);
                        }
                        break;
                    case string str:
                        {
                            string_builder.Append("\"");
                            string_builder.Append(
                                string_limit.HasValue && str.Length > string_limit ?
                                    str.Substring(0, string_limit.Value)
                                    : str
                            );
                            string_builder.Append("\"\n");
                        }
                        break;
                    case IEnumerable ienumerable:
                        {
                            int counter = 0;
                            string_builder.Append("[");
                            foreach (var item in ienumerable)
                            {
                                if (enumerable_limit.HasValue && counter > enumerable_limit.Value)
                                {
                                    string_builder.Append("..., ");
                                    break;
                                }
                                string_builder.Append($"{item}, ");
                                counter++;
                            }
                            string_builder.Remove(string_builder.Length - 2, 2);
                            string_builder.Append("]\n");
                        }
                        break;
                    default:
                        {
                            MemberInfo[] members = obj.GetType().GetMembers(
                                BindingFlags.Public
                                | BindingFlags.NonPublic
                                | BindingFlags.Instance
                                | BindingFlags.Static
                            );
                            foreach (var member_info in members)
                            {
                                try
                                {
                                    switch (member_info)
                                    {
                                        case FieldInfo field_info:
                                            {
                                                string_builder.Append($"{member_info.Name} -> {field_info.GetValue(obj)}\n");
                                            }
                                            break;
                                        case PropertyInfo property_info:
                                            {
                                                string_builder.Append($"{member_info.Name} -> {property_info.GetValue(obj)}\n");
                                            }
                                            break;
                                    }
                                }
                                catch { }
                            }
                        }
                        break;
                }
            }
            string_builder.Remove(string_builder.Length - 2, 2);
            return string_builder.ToString();
        }
    }
}
#endif