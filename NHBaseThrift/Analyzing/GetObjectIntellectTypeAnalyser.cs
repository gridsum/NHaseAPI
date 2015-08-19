using System;
using System.Collections.Generic;
using System.Linq;
using Gridsum.NHBaseThrift.Attributes;
using KJFramework.Helpers;

namespace Gridsum.NHBaseThrift.Analyzing
{
    /// <summary>
    ///     ��ת��Ϊ������������ͷ��������ṩ����صĻ���������
    /// </summary>
    internal class GetObjectIntellectTypeAnalyser : ThriftProtocolTypeAnalyser<Dictionary<short, GetObjectAnalyseResult>, Type>
    {
        #region Methods.

        /// <summary>
        ///     ����һ�������е�������������
        /// </summary>
        /// <param name="type">Ҫ����������</param>
        /// <returns>���ط����Ľ��</returns>
        public override Dictionary<short, GetObjectAnalyseResult> Analyse(Type type)
        {
            if (type == null) return null;
            Dictionary<short, GetObjectAnalyseResult> result = GetObject(type.FullName);
            if (result != null) return result;
            var targetProperties = type.GetProperties().AsParallel().Where(property => AttributeHelper.GetCustomerAttribute<ThriftPropertyAttribute>(property) != null);
            if (!targetProperties.Any()) return null;
            result = targetProperties.Select(property => new GetObjectAnalyseResult
                                                        {
                                                            VTStruct = GetVT(property.PropertyType),
                                                            Property = property,
                                                            TargetType = type,
                                                            Nullable = Nullable.GetUnderlyingType(property.PropertyType) != null,
                                                            Attribute = AttributeHelper.GetCustomerAttribute<ThriftPropertyAttribute>(property)
                                                        }.Initialize()).DefaultIfEmpty().OrderBy(property => property.Attribute.Id).ToDictionary(property => property.Attribute.Id);
            RegistAnalyseResult(type.FullName, result);
            return result;
        }

        #endregion
    }
}