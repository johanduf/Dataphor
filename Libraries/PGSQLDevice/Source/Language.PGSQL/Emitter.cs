/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/
namespace Alphora.Dataphor.DAE.Language.PGSQL
{
	using System;
	using System.Text;
	
	using Alphora.Dataphor.DAE.Language;
	using SQL = Alphora.Dataphor.DAE.Language.SQL;
	
	public class TSQLTextEmitter : SQL.SQLTextEmitter
	{

        protected override void EmitCreateIndexStatement(SQL.CreateIndexStatement AStatement)
        {
            Indent();
            AppendFormat("{0} ", SQL.Keywords.Create);
            if (AStatement.IsUnique)
                AppendFormat("{0} ", SQL.Keywords.Unique);
            //if (AStatement.IsClustered)
            //	AppendFormat("{0} ", SQL.Keywords.Clustered);
            AppendFormat("{0} ", SQL.Keywords.Index);
            if (AStatement.IndexSchema != String.Empty)
            {
                EmitIdentifier(AStatement.IndexSchema);
                Append(SQL.Keywords.Qualifier);
            }
            EmitIdentifier(AStatement.IndexName);
            AppendFormat(" {0} ", SQL.Keywords.On);
            if (AStatement.TableSchema != String.Empty)
            {
                EmitIdentifier(AStatement.TableSchema);
                Append(SQL.Keywords.Qualifier);
            }
            EmitIdentifier(AStatement.TableName);
            Append(SQL.Keywords.BeginGroup);
            for (int LIndex = 0; LIndex < AStatement.Columns.Count; LIndex++)
            {
                if (LIndex > 0)
                    EmitListSeparator();
                EmitOrderColumnDefinition(AStatement.Columns[LIndex]);
            }
            Append(SQL.Keywords.EndGroup);
        }

	}
}