/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/
using System;

namespace Alphora.Dataphor.DAE.Runtime.Data
{
	public struct NativeCursor
	{
		public NativeCursor(Schema.ICursorType cursorType, int id)
		{
			CursorType = cursorType;
			ID = id;
		}

		public Schema.ICursorType CursorType { get; }

		public int ID { get; }
	}

	public class CursorValue : DataValue, ICursor
	{
		public CursorValue(IValueManager manager, Schema.ICursorType cursorType, int iD) : base(manager, cursorType)
		{
			_nativeCursor = new NativeCursor(cursorType, iD);
		}

		private NativeCursor _nativeCursor;

		public int ID { get { return _nativeCursor.ID; } }
		
		public new Schema.ICursorType DataType { get { return (Schema.ICursorType)base.DataType; } }
		
		public override object AsNative
		{
			get { return _nativeCursor; }
			set { _nativeCursor = (NativeCursor)value; }
		}
		
		public override bool IsNil { get { return false; } }

		internal class CursorWriteContext : IWriteContext
		{
			public int Size { get; set; }
		}

		public static object CopyNativeAs(ICursor cursor, Schema.IDataType dataType)
		{
			return new NativeCursor(cursor.DataType, cursor.ID);
		}

		public static IWriteContext GetPhysicalSize(ICursor cursor, bool expandStreams)
		{
			return new CursorWriteContext { Size = sizeof(int) };
		}

		public static void WriteToPhysical(ICursor cursor, IWriteContext context, byte[] buffer, int offset, bool expandStreams)
		{
			ByteArrayUtility.WriteInt32(buffer, offset, cursor.ID);
		}

		public static void ReadFromPhysical(ICursor cursor, byte[] buffer, int offset)
		{
			cursor.AsNative = new NativeCursor(cursor.DataType, ByteArrayUtility.ReadInt32(buffer, offset));
		}
	}

	// Cursors
    public class Cursor : Disposable
    {
		protected internal Cursor(CursorManager manager, int iD, ITable table) : base()
		{
			_iD = iD;
			_table = table;
			_manager = manager;
			SnapshotContext(table.Program);
		}
		
		protected override void Dispose(bool disposing)
		{
			try
			{
				_iD = -1;
				
				try
				{
					if (_table != null)
					{
						_table.Dispose();
						_table = null;
					}
				}
				finally
				{
					if (_context != null)
						_context = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		protected Stack _context;
		
		public void SnapshotContext(Program program)
		{
			if (program != null)
			{
				_context = new Stack(program.Stack.MaxStackDepth, program.Stack.MaxCallDepth);
				_context.PushWindow(0);
				for (int index = program.Stack.Count - 1; index >= 0; index--)
					_context.Push(DataValue.CopyValue(program.ValueManager, program.Stack.Peek(index)));
			}
		}
		
		public void SwitchContext(Program program)
		{
			if (program != null)
				_context = program.SwitchContext(_context);
		}
		
		// ID
		protected int _iD = -1;
		public int ID { get { return _iD; } }
		
		// Table
		protected ITable _table;
		public ITable Table { get { return _table; } }
		
		// Manager
		protected CursorManager _manager;
 		public CursorManager Manager { get { return _manager; } }
    }
    
    public class Cursors : DisposableList<Cursor>
    {		
		public Cursors() : base(true) { }
		
		public Cursor GetCursor(int cursorID)
		{
			foreach (Cursor cursor in this)
				if (cursor.ID == cursorID)
					return cursor;
			throw new RuntimeException(RuntimeException.Codes.CursorNotFound, cursorID.ToString());
		}
    }
    
    public class CursorManager : Disposable
    {
		public CursorManager() : base(){}
		
		protected int _nextCursorID = -1;
		protected Cursors _cursors = new Cursors();
		
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (_cursors != null) 
				{
					while (_cursors.Count > 0)
						_cursors[0].Dispose();
					_cursors.Dispose();
					_cursors = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		public int CreateCursor(ITable table)
		{
			table.Open(); // JIC
			_nextCursorID++;
			Cursor cursor = new Cursor(this, _nextCursorID, table);
			try
			{
				_cursors.Add(cursor);
				return cursor.ID;
			}
			catch
			{
				cursor.Dispose();
				throw;
			}
		}
		
		public Cursor GetCursor(int cursorID)
		{
			return _cursors.GetCursor(cursorID);
		}
		
		public void CloseCursor(int cursorID)
		{
			GetCursor(cursorID).Dispose();
		}
	}
}