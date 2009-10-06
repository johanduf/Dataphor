/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/
using System;
using System.Resources;

using Alphora.Dataphor.DAE;

namespace Alphora.Dataphor.DAE.Streams
{
	public class StreamsException : DAEException
	{
		public enum Codes : int
		{
			/// <summary>Error code 112100: "Cannot compare values of type StreamID to values of type {0}."</summary>
			InvalidComparison = 112100,

			/// <summary>Error code 112101: "Stream is in use and cannot be deallocated."</summary>
			StreamInUse = 112101,

			/// <summary>Error code 112102: "StreamID ({0}) not found."</summary>
			StreamIDNotFound = 112102,

			/// <summary>Error code 112103: "Copy on write overflow."</summary>
			CopyOnWriteOverflow = 112103,

			/// <summary>Error code 112104: "Offset for a cover stream must be non-negative."</summary>
			OffsetMustBeNonNegative = 112104,

			/// <summary>Error code 112106: "Invalid row stream."</summary>
			InvalidRowStream = 112106,

			/// <summary>Error code 112106: "Display accessors cannot be used to set the AValue of a stream conveyor."</summary>
			CannotSetStreamConveyorAsString = 112106,

			/// <summary>Error code 112107: "Display accessors cannot be used to set the AValue of a BOP object conveyor."</summary>
			CannotSetBOPObjectConveyorAsString = 112107,

			/// <summary>Error code 112108: "Display accessors cannot be used to set the AValue of an object conveyor."</summary>
			CannotSetObjectConveyorAsString = 112108,

			/// <summary>Error code 112109: "Cursor "{0}" not found."</summary>
			CursorNotFound = 112109,

			/// <summary>Error code 112110: "Data type of a value cannot be changed."</summary>
			DataTypeSet = 112110,

			/// <summary>Error code 112111: "Data type required."</summary>
			DataTypeRequired = 112111,

			// <summary>Error code 112112: "No value available."</summary>
			//NoValueAvailable = 112112,

			/// <summary>Error code 112113: "Invalid row data format."</summary>
			InvalidRowDataFormat = 112113,

			/// <summary>Error code 112114: "Scalar AValue size exceeds maximum byte array size."</summary>
			ScalarOverflow = 112114,

			/// <summary>Error code 112115: "Row value size exceeds maximum byte array size."</summary>
			RowValueOverflow = 112115,

			/// <summary>Error code 112116: "Minimum static byte size is required to support value overflow."</summary>
			MinimumStaticByteSizeRequired = 112116,
			
			/// <summary>Error code 112117: "Cover stream source may not be null."</summary>
			CoverStreamSourceNull = 112117
		}

		// Resource manager for this exception class
		private static ResourceManager FResourceManager = new ResourceManager("Alphora.Dataphor.DAE.Streams.StreamsException", typeof(StreamsException).Assembly);

		// Constructors
		public StreamsException(Codes AErrorCode) : base(FResourceManager, (int)AErrorCode, ErrorSeverity.System, null, null) {}
		public StreamsException(Codes AErrorCode, params object[] AParams) : base(FResourceManager, (int)AErrorCode, ErrorSeverity.System, null, AParams) {}
		public StreamsException(Codes AErrorCode, Exception AInnerException) : base(FResourceManager, (int)AErrorCode, ErrorSeverity.System, AInnerException, null) {}
		public StreamsException(Codes AErrorCode, Exception AInnerException, params object[] AParams) : base(FResourceManager, (int)AErrorCode, ErrorSeverity.System, AInnerException, AParams) {}
		public StreamsException(Codes AErrorCode, ErrorSeverity ASeverity) : base(FResourceManager, (int)AErrorCode, ASeverity, null, null) {}
		public StreamsException(Codes AErrorCode, ErrorSeverity ASeverity, params object[] AParams) : base(FResourceManager, (int)AErrorCode, ASeverity, null, AParams) {}
		public StreamsException(Codes AErrorCode, ErrorSeverity ASeverity, Exception AInnerException) : base(FResourceManager, (int)AErrorCode, ASeverity, AInnerException, null) {}
		public StreamsException(Codes AErrorCode, ErrorSeverity ASeverity, Exception AInnerException, params object[] AParams) : base(FResourceManager, (int)AErrorCode, ASeverity, AInnerException, AParams) {}
		
		public StreamsException(ErrorSeverity ASeverity, int ACode, string AMessage, string ADetails, string AServerContext, DataphorException AInnerException) 
			: base(ASeverity, ACode, AMessage, ADetails, AServerContext, AInnerException)
		{
		}
	}
}