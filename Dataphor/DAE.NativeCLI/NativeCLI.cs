﻿/*
	Dataphor
	© Copyright 2000-2009 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

using Alphora.Dataphor.DAE.Listener;
using Alphora.Dataphor.DAE.Contracts;

namespace Alphora.Dataphor.DAE.NativeCLI
{
	public abstract class NativeCLIClient : ServiceClient<IClientNativeCLIService>
	{
		public const string CDefaultInstanceName = "Dataphor";
		
		public NativeCLIClient(string AHostName) : this(AHostName, CDefaultInstanceName, 0, ConnectionSecurityMode.Default, 0, ConnectionSecurityMode.Default) { }
		public NativeCLIClient(string AHostName, string AInstanceName) : this(AHostName, AInstanceName, 0, ConnectionSecurityMode.Default, 0, ConnectionSecurityMode.Default) { }
		public NativeCLIClient(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode) : this(AHostName, AInstanceName, 0, ConnectionSecurityMode.Default, 0, ConnectionSecurityMode.Default) { }
		public NativeCLIClient(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode, int AOverrideListenerPortNumber, ConnectionSecurityMode AListenerSecurityMode) : base(GetNativeServerURI(AHostName, AInstanceName, AOverridePortNumber, ASecurityMode, AOverrideListenerPortNumber, AListenerSecurityMode))
		{
			FHostName = AHostName;
			FInstanceName = AInstanceName;
			FOverridePortNumber = AOverridePortNumber;
			FSecurityMode = ASecurityMode;
			FOverrideListenerPortNumber = AOverrideListenerPortNumber;
			FListenerSecurityMode = AListenerSecurityMode;
		}
		
		private string FHostName;
		public string HostName { get { return FHostName; } }
		
		private string FInstanceName;
		public string InstanceName { get { return FInstanceName; } }

		private int FOverridePortNumber;
		public int OverridePortNumber { get { return FOverridePortNumber; } }
		
		private ConnectionSecurityMode FSecurityMode;
		public ConnectionSecurityMode SecurityMode { get { return FSecurityMode; } }
		
		private int FOverrideListenerPortNumber;
		public int OverrideListenerPortNumber { get { return FOverrideListenerPortNumber; } }
		
		private ConnectionSecurityMode FListenerSecurityMode;
		public ConnectionSecurityMode ListenerSecurityMode { get { return FListenerSecurityMode; } }
		
		public static string GetNativeServerURI(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode, int AOverrideListenerPortNumber, ConnectionSecurityMode AListenerSecurityMode)
		{
			if (AOverridePortNumber > 0)
				return DataphorServiceUtility.BuildNativeInstanceURI(AHostName, AOverridePortNumber, ASecurityMode == ConnectionSecurityMode.Transport, AInstanceName);
			else
				return ListenerFactory.GetInstanceURI(AHostName, AOverrideListenerPortNumber, AListenerSecurityMode, AInstanceName, ASecurityMode, true);
		}
	}
	
	public class NativeSessionCLIClient : NativeCLIClient
	{
		public NativeSessionCLIClient(string AHostName) : base(AHostName) { }
		public NativeSessionCLIClient(string AHostName, string AInstanceName) : base(AHostName, AInstanceName) { }
		public NativeSessionCLIClient(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode) : base(AHostName, AInstanceName, AOverridePortNumber, ASecurityMode) { }
		public NativeSessionCLIClient(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode, int AOverrideListenerPortNumber, ConnectionSecurityMode AListenerSecurityMode) : base(AHostName, AInstanceName, AOverridePortNumber, ASecurityMode, AOverrideListenerPortNumber, AListenerSecurityMode) { }
		
		public NativeSessionHandle StartSession(NativeSessionInfo ASessionInfo)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginStartSession(ASessionInfo, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				return GetInterface().EndStartSession(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public void StopSession(NativeSessionHandle ASessionHandle)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginStopSession(ASessionHandle, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				GetInterface().EndStopSession(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public void BeginTransaction(NativeSessionHandle ASessionHandle, NativeIsolationLevel AIsolationLevel)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginBeginTransaction(ASessionHandle, AIsolationLevel, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				GetInterface().EndBeginTransaction(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public void PrepareTransaction(NativeSessionHandle ASessionHandle)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginPrepareTransaction(ASessionHandle, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				GetInterface().EndPrepareTransaction(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public void CommitTransaction(NativeSessionHandle ASessionHandle)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginCommitTransaction(ASessionHandle, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				GetInterface().EndCommitTransaction(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public void RollbackTransaction(NativeSessionHandle ASessionHandle)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginRollbackTransaction(ASessionHandle, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				GetInterface().EndRollbackTransaction(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public int GetTransactionCount(NativeSessionHandle ASessionHandle)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginGetTransactionCount(ASessionHandle, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				return GetInterface().EndGetTransactionCount(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public NativeResult Execute(NativeSessionHandle ASessionHandle, string AStatement, NativeParam[] AParams, NativeExecutionOptions AOptions)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginSessionExecuteStatement(ASessionHandle, AStatement, AParams, AOptions, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				return GetInterface().EndSessionExecuteStatement(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}

		public NativeResult[] Execute(NativeSessionHandle ASessionHandle, NativeExecuteOperation[] AOperations)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginSessionExecuteStatements(ASessionHandle, AOperations, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				return GetInterface().EndSessionExecuteStatements(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
	}
	
	public class NativeStatelessCLIClient : NativeCLIClient
	{
		public NativeStatelessCLIClient(string AHostName) : base(AHostName) { }
		public NativeStatelessCLIClient(string AHostName, string AInstanceName) : base(AHostName, AInstanceName) { }
		public NativeStatelessCLIClient(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode) : base(AHostName, AInstanceName, AOverridePortNumber, ASecurityMode) { }
		public NativeStatelessCLIClient(string AHostName, string AInstanceName, int AOverridePortNumber, ConnectionSecurityMode ASecurityMode, int AOverrideListenerPortNumber, ConnectionSecurityMode AListenerSecurityMode) : base(AHostName, AInstanceName, AOverridePortNumber, ASecurityMode, AOverrideListenerPortNumber, AListenerSecurityMode) { }

		public NativeResult Execute(NativeSessionInfo ASessionInfo, string AStatement, NativeParam[] AParams, NativeExecutionOptions AOptions)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginExecuteStatement(ASessionInfo, AStatement, AParams, AOptions, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				return GetInterface().EndExecuteStatement(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
		
		public NativeResult[] Execute(NativeSessionInfo ASessionInfo, NativeExecuteOperation[] AOperations)
		{
			try
			{
				IAsyncResult LResult = GetInterface().BeginExecuteStatements(ASessionInfo, AOperations, null, null);
				LResult.AsyncWaitHandle.WaitOne();
				return GetInterface().EndExecuteStatements(LResult);
			}
			catch (FaultException<NativeCLIFault> LException)
			{
				throw NativeCLIFaultUtility.FaultToException(LException.Detail);
			}
		}
	}
}
