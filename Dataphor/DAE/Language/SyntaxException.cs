/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/
using System;
using System.Resources;
using System.Reflection;
using System.Runtime.Serialization;

using Alphora.Dataphor.DAE;

namespace Alphora.Dataphor.DAE.Language
{
	/// <summary>Indicates a syntactic error encountered during the lexical and syntactic analysis phases of compilation.</summary>
	/// <remarks>
	/// The SyntaxException is used to return the line number and position of the lexical analyzer when an error is encountered
	/// during lexical analysis or parsing.  Any exception encountered during these phases will be wrapped with an exception
	/// of this type.  Only the parser should throw exceptions of this type.
	/// </remarks>
	public class SyntaxException : DAEException, ILocatorException
	{
		public enum Codes : int
		{
			/// <summary>Error code 103100: "Syntax error near "{0}"."</summary>
			SyntaxError = 103100
		}
			
		// Resource manager for this exception class
		private static ResourceManager FResourceManager = new ResourceManager("Alphora.Dataphor.DAE.Language.SyntaxException", typeof(SyntaxException).Assembly);

		public SyntaxException(Lexer ALexer, Exception AInner) : base(FResourceManager, (int)Codes.SyntaxError, ErrorSeverity.Application, AInner, ALexer[0].Token)
		{
			FLine = ALexer[0, false].Line;
			FLinePos = ALexer[0, false].LinePos;
			FTokenType = ALexer[0, false].Type;
			FToken = ALexer[0, false].Token;
		}
		
		public SyntaxException(ErrorSeverity ASeverity, int ACode, string AMessage, string ADetails, string AServerContext, int ALine, int ALinePos, TokenType ATokenType, string AToken, DataphorException AInnerException) 
			: base(ASeverity, ACode, AMessage, ADetails, AServerContext, AInnerException)
		{
			FLine = ALine;
			FLinePos = ALinePos;
			FTokenType = ATokenType;
			FToken = AToken;
		}
			
		private int FLine;
		public int Line 
		{ 
			get { return FLine; } 
			set { FLine = value; }
		}
			
		private int FLinePos;
		public int LinePos 
		{ 
			get { return FLinePos; } 
			set { FLinePos = value; }
		}
			
		private TokenType FTokenType;
		public TokenType TokenType { get { return FTokenType; } }
			
		private string FToken;
		public string Token { get { return FToken; } }

		private string FLocator;
		public string Locator 
		{ 
			get { return FLocator; }
			set { FLocator = value; }
		}
	}
}