﻿/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace Alphora.Dataphor.DAE.Runtime
{
	using Alphora.Dataphor.DAE.Schema;
	using Alphora.Dataphor.DAE.Language.D4;
	using Alphora.Dataphor.DAE.Compiling;
	using Alphora.Dataphor.DAE.Server;
	using Alphora.Dataphor.DAE.Streams;
	using Alphora.Dataphor.DAE.Runtime.Data;
	using Alphora.Dataphor.DAE.Runtime.Instructions;
	using Alphora.Dataphor.DAE.Language;
	
	/// <summary>
	/// Provides value management services for processes within the DAE.
	/// </summary>
	public interface IValueManager
	{
		Schema.DataTypes DataTypes { get; }
		IStreamManager StreamManager { get; }
		IConveyor GetConveyor(Schema.IScalarType AScalarType);
		Schema.IDataType NativeTypeToDataType(Type ANativeType);
		Schema.IDataType CompileTypeSpecifier(string ATypeSpecifier);
		Schema.Order FindClusteringOrder(Schema.TableVar ATableVar);
		Schema.Order OrderFromKey(Schema.Key AKey);
		bool OrderIncludesKey(Schema.Order AIncludingOrder, Schema.Key AIncludedKey);
		bool OrderIncludesOrder(Schema.Order AIncludingOrder, Schema.Order AIncludedOrder);
		Schema.Sort GetUniqueSort(Schema.IDataType ADataType);
		int EvaluateSort(Schema.OrderColumn AOrderColumn, object AIndexValue, object ACompareValue);
		IDataValue GetAsDataValue(Schema.Representation ARepresentation, object AValue);
		object GetAsNative(Schema.Representation ARepresentation, object AValue);
		object SetAsNative(Schema.Representation ARepresentation, object AValue, object ANewValue);
	}
	
	/// <summary>
	/// Provides value management services for processes within the DAE.
	/// </summary>
	public class ValueManager : IValueManager
	{
		public ValueManager(ServerProcess serverProcess, IServerProcess serverProcessInterface)
		{
			_serverProcess = serverProcess;
			_serverProcessInterface = serverProcessInterface;
			_plan = new Plan(_serverProcess);
		}
		
		private ServerProcess _serverProcess;
		private IServerProcess _serverProcessInterface;

		private Plan _plan;
		
		public Schema.DataTypes DataTypes { get { return _serverProcess.DataTypes; } }
		
		public IStreamManager StreamManager { get { return (IStreamManager)_serverProcessInterface; } }
		
		private Dictionary<string, IConveyor> _conveyors = new Dictionary<string, IConveyor>();
		
		public IConveyor GetConveyor(Schema.IScalarType scalarType)
		{
			IConveyor conveyor;
			if (!_conveyors.TryGetValue(scalarType.Name, out conveyor))
			{
				conveyor = (IConveyor)_serverProcessInterface.CreateObject(scalarType.ClassDefinition, null);
				var nativeConveyor = conveyor as INativeScalarConveyor;
				if (nativeConveyor != null)
					nativeConveyor.Conveyor = GetConveyor((Schema.IScalarType)CompileTypeSpecifier(nativeConveyor.SourceTypeName));
				_conveyors.Add(scalarType.Name, conveyor);
			}
			return conveyor;
		}

		private Dictionary<Type, Schema.IDataType> _nativeTypeMap = new Dictionary<Type, Schema.IDataType>();

		public Schema.IDataType DetermineDataType(object nativeValue)
		{
			if (nativeValue == null) return DataTypes.SystemGeneric;

			var nativeType = nativeValue.GetType();

			return NativeTypeToDataType(nativeType);
		}

		public Schema.IDataType NativeTypeToDataType(Type nativeType)
		{
			Schema.IDataType resultType;
			if (!_nativeTypeMap.TryGetValue(nativeType, out resultType))
			{
				resultType = LookupType(nativeType);
				_nativeTypeMap.Add(nativeType, resultType);
			}
			return resultType;
		}

		private Schema.IDataType LookupType(Type type)
		{
			// Fast lookup for the known native types
			//if (type == NativeAccessors.AsBoolean.NativeType) return DataTypes.SystemBoolean;
			//if (type == NativeAccessors.AsByte.NativeType) return DataTypes.SystemByte;
			//if (type == NativeAccessors.AsByteArray.NativeType) return DataTypes.SystemBinary;
			//if (type == NativeAccessors.AsDateTime.NativeType) return DataTypes.SystemDateTime;
			//if (type == NativeAccessors.AsDecimal.NativeType) return DataTypes.SystemDecimal;
			//if (type == NativeAccessors.AsException.NativeType) return DataTypes.SystemError;
			//if (type == NativeAccessors.AsGuid.NativeType) return DataTypes.SystemGuid;
			//if (type == NativeAccessors.AsInt16.NativeType) return DataTypes.SystemShort;
			//if (type == NativeAccessors.AsInt32.NativeType) return DataTypes.SystemInteger;
			//if (type == NativeAccessors.AsInt64.NativeType) return DataTypes.SystemLong;
			//if (type == NativeAccessors.AsString.NativeType) return DataTypes.SystemString;
			//if (type == NativeAccessors.AsTimeSpan.NativeType) return DataTypes.SystemTimeSpan;


			// Catalog lookup for any scalar type
			var resultType = DataTypes.FindScalarType(type);
			if (resultType != null)
			{
				return resultType;
			}

			var nativeListType = type.GetInterface("IList`1");
			if (nativeListType != null)
			{
				var elementType = nativeListType.GetGenericArguments()[0];
				return new ListType(LookupType(elementType));
			}

			if (type.GetInterface("IList") != null)
			{
				return new ListType(DataTypes.SystemGeneric);
			}

			// The type is a generated row type
			if (type.GetInterface("INativeRow") != null)
			{
				return type.GetProperty("RowType", System.Reflection.BindingFlags.Static).GetValue(null, null) as Schema.IRowType;
			}

			// The type is a generated table type
			if (type.GetInterface("INativeTable") != null)
			{
				throw new NotImplementedException();
			}

			return DataTypes.SystemGeneric;
		}
		
		public Schema.IDataType CompileTypeSpecifier(string typeSpecifier)
		{
			return Compiler.CompileTypeSpecifier(_plan, new Parser().ParseTypeSpecifier(typeSpecifier));
		}
		
		public Schema.Order FindClusteringOrder(Schema.TableVar tableVar)
		{
			return Compiler.FindClusteringOrder(_plan, tableVar);
		}
		
		public bool OrderIncludesKey(Schema.Order includingOrder, Schema.Key includedKey)
		{
			return Compiler.OrderIncludesKey(_plan, includingOrder, includedKey);
		}
		
		public bool OrderIncludesOrder(Schema.Order includingOrder, Schema.Order includedOrder)
		{
			return Compiler.OrderIncludesOrder(_plan, includingOrder, includedOrder);
		}
		
		public Schema.Order OrderFromKey(Schema.Key key)
		{
			return Compiler.OrderFromKey(_plan, key);
		}
		
		public Schema.Sort GetUniqueSort(Schema.IDataType type)
		{
			return Compiler.GetUniqueSort(_plan, type);
		}
		
		public int EvaluateSort(Schema.OrderColumn orderColumn, object indexValue, object compareValue)
		{
			#if USEICOMPARABLE
			IComparable indexComparable = AIndexValue as IComparable;
			if (indexComparable != null)
				return indexComparable.CompareTo(ACompareValue) * (AOrderColumn.Ascending ? 1 : -1);
			else
			{
			#endif
				// NOTE: Use currently executing program because the whole point is that this is inner loop sort code.
				// We don't want to have to use a new program, or
				Program program = _serverProcess.ExecutingProgram;
				//LProgram.Stack.PushWindow(0);
				//try
				//{
					program.Stack.Push(indexValue);
					program.Stack.Push(compareValue);
					try
					{
						return ((int)orderColumn.Sort.CompareNode.Execute(program)) * (orderColumn.Ascending ? 1 : -1);
					}
					finally
					{
						program.Stack.Pop();
						program.Stack.Pop();
					}
				//}
				//finally
				//{
				//	LProgram.Stack.PopWindow();
				//}
			#if USEICOMPARABLE
			} 
			#endif
		}
		
        private void EnsureReadNode(Plan plan, Schema.Representation representation)
        {
			if (representation.ReadNode == null)
			{
				if (representation.Properties.Count > 1)
					throw new SchemaException(SchemaException.Codes.InvalidConversionRepresentation, representation.Name, representation.ScalarType.Name);
					
				plan.Symbols.Push(new Symbol("AValue", representation.ScalarType));
				try
				{
					representation.ReadNode = Compiler.Compile(plan, new QualifierExpression(new IdentifierExpression("AValue"), new IdentifierExpression(representation.Properties[0].Name)));
				}
				finally
				{
					plan.Symbols.Pop();
				}
			}
        }
        
        public IDataValue GetAsDataValue(Schema.Representation representation, object tempValue)
        {
			EnsureReadNode(_plan, representation);
			Program program = _serverProcess.ExecutingProgram;
			program.Stack.Push(tempValue);
			try
			{
				return DataValue.FromNative(this, representation.ScalarType, representation.ReadNode.Execute(program));
			}
			finally
			{
				program.Stack.Pop();
			}
        }
        
        public object GetAsNative(Schema.Representation representation, object tempValue)
        {
			EnsureReadNode(_plan, representation);
			Program program = _serverProcess.ExecutingProgram;
			program.Stack.Push(tempValue);
			try
			{
				return representation.ReadNode.Execute(program);
			}
			finally
			{
				program.Stack.Pop();
			}
        }

        private void EnsureWriteNode(Plan plan, Schema.Representation representation)
        {
			if (representation.WriteNode == null)
			{
				if (representation.Properties.Count > 1)
					throw new SchemaException(SchemaException.Codes.InvalidConversionRepresentation, representation.Name, representation.ScalarType.Name);
					
				plan.Symbols.Push(new Symbol("AValue", representation.ScalarType));
				try
				{
					plan.Symbols.Push(new Symbol("ANewValue", representation.Properties[0].DataType));
					try
					{
						representation.WriteNode = Compiler.Compile(plan, new AssignmentStatement(new QualifierExpression(new IdentifierExpression("AValue"), new IdentifierExpression(representation.Properties[0].Name)), new IdentifierExpression("ANewValue"))).Nodes[1];
					}
					finally
					{
						plan.Symbols.Pop();
					}
				}
				finally
				{
					plan.Symbols.Pop();
				}					
			}
        }
        
        public object SetAsNative(Schema.Representation representation, object tempValue, object newValue)
        {
			EnsureWriteNode(_plan, representation);
			Program program = _serverProcess.ExecutingProgram;
			program.Stack.Push(tempValue);
			try
			{
				program.Stack.Push(newValue);
				try
				{
					return representation.WriteNode.Execute(program);
				}
				finally
				{
					program.Stack.Pop();
				}
			}
			finally
			{
				program.Stack.Pop();
			}
        }
	}
}
