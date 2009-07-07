//#define SQLSTORETIMING

/*
	Alphora Dataphor
	� Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
	Simple SQL CE Store
	
	A simple storage device that uses a SQL Server Everywhere instance as it's backend.
	The store is capable of storing integers, strings, booleans, and long text and binary data.
	The store also manages logging and rollback of nested transactions to make up for the lack of savepoint support in SQL Server Everywhere.
*/

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Collections.Generic;

using Alphora.Dataphor.DAE.Connection;

namespace Alphora.Dataphor.DAE.Store.SQLCE
{
    public class SQLCEStoreConnection : SQLStoreConnection
    {
        public SQLCEStoreConnection(SQLCEStore AStore) : base(AStore)
        { }
		
        protected override DbConnection InternalCreateConnection()
        {
            return new SqlCeConnection(Store.ConnectionString);
        }

        protected override DbTransaction InternalBeginTransaction(System.Data.IsolationLevel AIsolationLevel)
        {
            // SQLServerCE does not support the ReadUncommitted isolation level because it uses versioning
            if (AIsolationLevel == System.Data.IsolationLevel.ReadUncommitted)
                AIsolationLevel = System.Data.IsolationLevel.ReadCommitted;
            return base.InternalBeginTransaction(AIsolationLevel);
        }

        public override bool HasTable(string ATableName)
        {
            return ((int)this.ExecuteScalar(String.Format("select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}'", ATableName)) != 0);
        }

        internal SqlCeResultSet ExecuteResultSet(string ATableName, string AIndexName, DbRangeOptions ARangeOptions, object[] AStartValues, object[] AEndValues, ResultSetOptions AResultSetOptions)
        {
            ExecuteCommand.CommandType = CommandType.TableDirect;
            ExecuteCommand.CommandText = ATableName;
            ExecuteCommand.IndexName = AIndexName;
            ExecuteCommand.SetRange(ARangeOptions, AStartValues, AEndValues);

			#if SQLSTORETIMING
			long LStartTicks = TimingUtility.CurrentTicks;
			try
			{
			#endif
			
	            return ExecuteCommand.ExecuteResultSet(AResultSetOptions);
			
			#if SQLSTORETIMING
			}
			finally
			{
				Store.Counters.Add(new SQLStoreCounter("ExecuteResultSet", ATableName, AIndexName, AStartValues != null && AEndValues == null, AStartValues != null && AEndValues != null, (ResultSetOptions.Updatable & AResultSetOptions) != 0, TimingUtility.TimeSpanFromTicks(LStartTicks)));
			}
			#endif
        }
		
        public new SqlCeCommand ExecuteCommand { get { return (SqlCeCommand)base.ExecuteCommand; } }

        protected override SQLStoreCursor InternalOpenCursor(string ATableName, List<string> AColumns, SQLIndex AIndex, bool AIsUpdatable)
        {
            return
                new SQLCEStoreCursor
				(
                    this,
                    ATableName,
                    AColumns,
                    AIndex,
                    AIsUpdatable
				);
        }
    }
}