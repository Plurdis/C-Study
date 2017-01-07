using System;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace WPF_Control_Style_Practice
{
    public class DependencyPropertyException : Exception
    {
        public DependencyPropertyException(string message) : base(message)
        {
        }
    }

    public static class DependencyHelper
    {
        private const string NameRulePattern = @"[A-Z]\w*Property";
        private const string NamePattern = @"[A-Z]\w*(?=Property)";

        private static DependencyProperty Register(
            string propName, Type ownerType,
            PropertyMetadata typeMetadata = null,
            ValidateValueCallback validateValueCallback = null)
        {
            PropertyInfo property = ownerType.GetProperty(propName);

            if (property == null)
                throw new DependencyPropertyException($"'{propName}'속성을 찾을 수 없습니다.");

            if (typeMetadata == null && validateValueCallback == null)
                return DependencyProperty.Register(propName, property.PropertyType, ownerType);
            else if (typeMetadata != null)
                if (validateValueCallback == null)
                    return DependencyProperty.Register(propName, property.PropertyType, ownerType, typeMetadata);
                else
                    return DependencyProperty.Register(propName, property.PropertyType, ownerType, typeMetadata, validateValueCallback);

            throw new ArgumentException();
        }

        public static DependencyProperty Register(
            PropertyMetadata typeMetadata = null,
            ValidateValueCallback validateValueCallback = null,
            [CallerMemberName]string dpPropName = "")
        {
            // 소유자 정보
            Type ownerType = GetDeclaringType(2);

            if (!Regex.IsMatch(dpPropName, NameRulePattern))
                throw new DependencyPropertyException("종속성 속성 명명규칙에 어긋납니다.");

            // 대상 속성
            string propName = Regex.Match(dpPropName, NamePattern).Value;

            return Register(propName, ownerType, typeMetadata);
        }

        private static Type GetDeclaringType(int depth)
        {
            var st = new StackTrace();
            var frame = st.GetFrame(depth);
            var method = frame.GetMethod();

            return method.DeclaringType;
        }
    }
}