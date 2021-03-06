﻿#if ORACLE
using System.Data;
using System.Data.Common;

#if DATADIRECT
using DDTek.Oracle;
#elif ODP_NET	// ODP.NET
using Oracle.DataAccess.Client;
#else			// ODP.NET.Managed
using Oracle.ManagedDataAccess.Client;
#endif

namespace DbParallel.DataAccess
{
	static partial class DerivedParametersCache
	{
		static partial void OracleDeriveParameters(DbCommand dbCmd, ref bool processed)
		{
			if (processed)
				return;

			OracleCommand oraCmd = dbCmd as OracleCommand;

			if (oraCmd != null)
			{
				ShuffleCommandTextCase(oraCmd);
				OracleCommandBuilder.DeriveParameters(oraCmd);

				processed = true;
			}
		}

		// Resolve ODP.NET caching issue (ODP.NET's DeriveParameters keeps used stored procedures' parameters descriptions in its internal Hashtable without any evict policy)
		static private void ShuffleCommandTextCase(OracleCommand oraCmd)
		{
#if !DATADIRECT		// DataDirect can be resolved by adding "Procedure Description Cache=false" in the ConnectionString
			string commandText = oraCmd.CommandText;

			if (string.IsNullOrEmpty(commandText) || commandText.StartsWith("\"") || commandText.EndsWith("\""))
				return;

			oraCmd.CommandText = commandText.ShuffleCase();
#endif
		}

		static partial void OracleOmitUnspecifiedInputParameters(DbCommand dbCmd, ref bool processed)
		{
			if (processed)
				return;

			OracleCommand oraCmd = dbCmd as OracleCommand;

			if (oraCmd != null && oraCmd.CommandType == CommandType.StoredProcedure)
			{
				int cntOmitted = 0;
				OracleParameter oraParameter;

				for (int i = oraCmd.Parameters.Count - 1; i >= 0; i--)
				{
					oraParameter = oraCmd.Parameters[i];

					if (oraParameter.Value == null && oraParameter.Direction == ParameterDirection.Input)
					{
						oraCmd.Parameters.RemoveAt(i);
						cntOmitted++;
					}
				}

				if (cntOmitted > 0 && oraCmd.BindByName == false)
					oraCmd.BindByName = true;

				processed = true;
			}
		}

		static partial void OracleGetUnderlyingDbType(DbParameter dbParameter, ref DbType underlyingDbType, ref bool processed)
		{
			if (processed)
				return;

			OracleParameter oraParameter = dbParameter as OracleParameter;

			if (oraParameter != null)
			{
				switch (oraParameter.OracleDbType)
				{
					case OracleDbType.Blob:
					case OracleDbType.Raw:
					case OracleDbType.LongRaw:
#if DATADIRECT
					case OracleDbType.Bfile:
#else
					case OracleDbType.BFile:
#endif
						underlyingDbType = DbType.Binary;
						processed = true;
						break;

					case OracleDbType.Clob:
					case OracleDbType.NClob:
					case OracleDbType.Char:
					case OracleDbType.NChar:
					case OracleDbType.Long:
#if DATADIRECT
					case OracleDbType.VarChar:
					case OracleDbType.NVarChar:
#else
					case OracleDbType.Varchar2:
					case OracleDbType.NVarchar2:
#endif
					case OracleDbType.XmlType:
						underlyingDbType = DbType.String;
						processed = true;
						break;
				}
			}
		}
	}
}

#endif

////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Copyright 2014 Abel Cheng
//	This source code is subject to terms and conditions of the Apache License, Version 2.0.
//	See http://www.apache.org/licenses/LICENSE-2.0.
//	All other rights reserved.
//	You must not remove this notice, or any other, from this software.
//
//	Original Author:	Abel Cheng <abelcys@gmail.com>
//	Created Date:		2014-12-23
//	Original Host:		http://dbParallel.codeplex.com
//	Primary Host:		http://DataBooster.codeplex.com
//	Updated Host:		https://github.com/DataBooster/DataAccess
//	Change Log:
//	Author				Date			Comment
//
//
//
//
//	(Keep code clean rather than complicated code plus long comments.)
//
////////////////////////////////////////////////////////////////////////////////////////////////////
