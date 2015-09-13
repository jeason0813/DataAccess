﻿using System;
using System.Xml;
using System.Xml.Schema;

namespace DbParallel.DataAccess
{
	internal static class XmlWriterExtensions
	{
		public static void WriteElementValue(this XmlWriter writer, string localName, object value, bool emitNullValue = true)
		{
			bool isNull = IsNull(value);

			if (emitNullValue || !isNull)
			{
				writer.WriteStartElement(localName);

				if (isNull)
					writer.WriteAttributeString("nil", XmlSchema.InstanceNamespace, "true");
				else
					writer.WriteValue(value);

				writer.WriteEndElement();
			}
		}

		public static void WriteAttributeValue(this XmlWriter writer, string localName, object value, bool emitNullValue = true)
		{
			bool isNull = IsNull(value);

			if (emitNullValue || !isNull)
			{
				writer.WriteStartAttribute(localName);

				if (isNull)
					writer.WriteString(string.Empty);
				else
					writer.WriteValue(value);

				writer.WriteEndAttribute();
			}
		}

		private static string GetBuiltInXsiType(Type type)
		{
			if (type.IsEnum)
				return null;

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean: return "boolean";
				case TypeCode.Byte: return "unsignedByte";
				case TypeCode.Char: return "char";
				case TypeCode.DateTime: return "dateTime";
				case TypeCode.Decimal: return "decimal";
				case TypeCode.Double: return "double";
				case TypeCode.Int16: return "short";
				case TypeCode.Int32: return "int";
				case TypeCode.Int64: return "long";
				case TypeCode.SByte: return "byte";
				case TypeCode.Single: return "float";
				case TypeCode.String: return "string";
				case TypeCode.UInt16: return "unsignedShort";
				case TypeCode.UInt32: return "unsignedInt";
				case TypeCode.UInt64: return "unsignedLong";

				default:
					if (type == typeof(byte[]))
						return "base64Binary";
					else if (type == typeof(Uri))
						return "anyURI";
					else if (type == typeof(XmlQualifiedName))
						return "QName";
					else if (type == typeof(TimeSpan))
						return "duration";
					else if (type == typeof(Guid))
						return "guid";
					return null;
			}
		}

		private static bool IsNull(object value)
		{
			return (value == null || Convert.IsDBNull(value));
		}
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Copyright 2015 Abel Cheng
//	This source code is subject to terms and conditions of the Apache License, Version 2.0.
//	See http://www.apache.org/licenses/LICENSE-2.0.
//	All other rights reserved.
//	You must not remove this notice, or any other, from this software.
//
//	Original Author:	Abel Cheng <abelcys@gmail.com>
//	Created Date:		2015-09-12
//	Original Host:		http://dbParallel.codeplex.com
//	Primary Host:		http://DataBooster.codeplex.com
//	Change Log:
//	Author				Date			Comment
//
//
//
//
//	(Keep code clean rather than complicated code plus long comments.)
//
////////////////////////////////////////////////////////////////////////////////////////////////////
