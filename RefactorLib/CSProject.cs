﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Build.Evaluation;

namespace Refactor
{
	/// <summary>
	/// An override of Project that allows specific customizatoins
	/// </summary>
	public class CSProject : Project
	{
		public CSProject(string file, IDictionary<string, string> props, string toolsVersion)
			: base(file, props, toolsVersion)
		{
			Included = true;
		}

		public bool Included { get; set; }

		public string OutputPath
		{
			get
			{
				return ResolveValue(this.GetProperty("OutputPath"));
			}
		}
		
		public string ResolveValue(ProjectProperty p)
		{
			var relative = p.EvaluatedValue;
			var basepath = Path.GetDirectoryName(this.FullPath);
			var combined = Path.GetFullPath(Path.Combine(basepath, relative));
			return combined.ToLower();
		}
	}
}
