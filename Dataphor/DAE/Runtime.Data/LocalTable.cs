/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.Collections;

namespace Alphora.Dataphor.DAE.Runtime.Data
{
	using Alphora.Dataphor.DAE.Language.D4;
	using Schema = Alphora.Dataphor.DAE.Schema;
	using Alphora.Dataphor.DAE.Runtime.Instructions;

    public class LocalTable : Table
    {
		public LocalTable(TableNode ATableNode, Program AProgram, TableValue ATableValue) : base(ATableNode, AProgram)
		{
			FNativeTable = (INativeTable)ATableValue.AsNative;
			FKey = AProgram.OrderFromKey(AProgram.FindClusteringKey(FNativeTable.TableVar));
		}
		
		public LocalTable(TableNode ATableNode, Program AProgram) : base(ATableNode, AProgram)
		{
			FNativeTable = new NativeTable(AProgram.ValueManager, ATableNode.TableVar);
			FKey = AProgram.OrderFromKey(AProgram.FindClusteringKey(ATableNode.TableVar));
		}
		
		protected override void Dispose(bool ADisposing)
		{
			Close();
			if (FNativeTable != null)
			{
				FNativeTable.Drop(Program.ValueManager);
				FNativeTable = null;
			}
		}
		
		public new TableNode Node { get { return (TableNode)FNode; } }
		
		protected internal INativeTable FNativeTable;
		private Schema.Order FKey;
		private Scan FScan;

		protected override void InternalOpen()
		{
			if (FNativeTable.ClusteredIndex.Key.Equivalent(FKey))
				FScan = new Scan(Manager, FNativeTable, FNativeTable.ClusteredIndex, ScanDirection.Forward, null, null);
			else
				FScan = new Scan(Manager, FNativeTable, FNativeTable.NonClusteredIndexes[FKey], ScanDirection.Forward, null, null);
			FScan.Open();
		}
		
		protected override void InternalClose()
		{
			FScan.Dispose();
			FScan = null;
		}
		
		protected override void InternalReset()
		{
			FScan.Reset();
		}
		
		protected override void InternalSelect(Row ARow)
		{
			FScan.GetRow(ARow);
		}
		
		protected override void InternalFirst()
		{
			FScan.First();
		}
		
		protected override bool InternalPrior()
		{
			return FScan.Prior();
		}
		
		protected override bool InternalNext()
		{
			return FScan.Next();
		}
		
		protected override void InternalLast()
		{
			FScan.Last();
		}
		
		protected override bool InternalBOF()
		{
			return FScan.BOF();
		}
		
		protected override bool InternalEOF()
		{
			return FScan.EOF();
		}

		// Bookmarkable

		protected override Row InternalGetBookmark()
		{
			return FScan.GetKey();
		}

		protected override bool InternalGotoBookmark(Row ABookmark, bool AForward)
		{
			return FScan.FindKey(ABookmark);
		}
        
		protected override int InternalCompareBookmarks(Row ABookmark1, Row ABookmark2)
		{
			return FScan.CompareKeys(ABookmark1, ABookmark2);
		}

		// Searchable

		protected override Schema.Order InternalGetOrder()
		{
			return FKey;
		}
		
		protected override Row InternalGetKey()
		{
			return FScan.GetKey();
		}

		protected override bool InternalFindKey(Row AKey, bool AForward)
		{
			return FScan.FindKey(AKey);
		}
		
		protected override void InternalFindNearest(Row AKey)
		{
			FScan.FindNearest(AKey);
		}
		
		protected override bool InternalRefresh(Row AKey)
		{
			return FScan.FindNearest(AKey);
		}

		protected override void InternalInsert(Row AOldRow, Row ANewRow, BitArray AValueFlags, bool AUnchecked)
		{
			FNativeTable.Insert(Manager, ANewRow);
		}
		
		protected override void InternalUpdate(Row ARow, BitArray AValueFlags, bool AUnchecked)
		{
			FNativeTable.Update(Manager, Select(), ARow);
		}
		
		protected override void InternalDelete(bool AUnchecked)
		{
			FNativeTable.Delete(Manager, Select());
		}
    }
}