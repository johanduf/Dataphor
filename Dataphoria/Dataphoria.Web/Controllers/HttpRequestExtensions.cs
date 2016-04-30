﻿/*
	From the Furore Spark FHIR Server implementation
*/

using Alphora.Dataphor.DAE.NativeCLI;
using Hl7.Fhir.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Alphora.Dataphor.Dataphoria.Web.Controllers
{
	public static class HttpRequestExtensions
	{
		public static bool Exists(this HttpHeaders headers, string key)
		{
			IEnumerable<string> values;
			if (headers.TryGetValues(key, out values))
			{
				return values.Count() > 0;
			}
			else return false;

		}
		
		public static void Replace(this HttpHeaders headers, string header, string value)
		{
			//if (headers.Exists(header)) 
			headers.Remove(header);
			headers.Add(header, value);
		}
		
		public static string Value(this HttpHeaders headers, string key)
		{
			IEnumerable<string> values;
			if (headers.TryGetValues(key, out values))
			{
				return values.FirstOrDefault();
			}
			else return null;
		}
		
		public static void ReplaceHeader(this HttpRequestMessage request, string header, string value)
		{
			request.Headers.Replace(header, value);
		}

		public static string Header(this HttpRequestMessage request, string key)
		{
			IEnumerable<string> values;
			if (request.Content.Headers.TryGetValues(key, out values))
			{
				return values.FirstOrDefault();
			}
			else return null;
		}
		
		public static string GetParameter(this HttpRequestMessage request, string key)
		{
			foreach (var param in request.GetQueryNameValuePairs())
			{
				if (param.Key == key) return param.Value;
			}
			return null;
		}

		public static List<Tuple<string, string>> TupledParameters(this HttpRequestMessage request)
		{
			var list = new List<Tuple<string, string>>();

			IEnumerable<KeyValuePair<string, string>> query = request.GetQueryNameValuePairs();
			foreach (var pair in query)
			{
				list.Add(new Tuple<string, string>(pair.Key, pair.Value));
			}
			return list;
		}

		public static IEnumerable<KeyValuePair<string, object>> NativeListedParameters(this HttpRequestMessage request)
		{
			IEnumerable<KeyValuePair<string, string>> queryPairs = request.GetQueryNameValuePairs();
			var parameters = new Dictionary<string, object>();
			foreach (var pair in queryPairs)
			{
				parameters.Add(String.Format("A{0}", pair.Key), pair.Value);
			}

			return parameters;
		}

		public static SearchParams GetSearchParams(this HttpRequestMessage request)
		{
			var parameters = request.TupledParameters();
			UriParamList actualParameters = new UriParamList(parameters);
			var searchCommand = SearchParams.FromUriParamList(parameters);
			return searchCommand;
		}
	}
}
