﻿using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace msbuildrefactor
{
	/// <summary>
	/// Represents a property that has exists in a single common property sheet
	/// that many csproj or many msbuild files can reference.
	/// </summary>
	public class CommonProperty : BaseProperty
	{
		public CommonProperty(string name, string evaluated_value)
		{
			this.Name = name;
			this.EvaluatedValue = evaluated_value;
		}

		public string Name { get; set; }
		public string EvaluatedValue { get; set; }
		public override string ToString()
		{
			return string.Format("Property {0} = {1}", Name, EvaluatedValue);
		}
	}

	/// <summary>
	/// Represents a property that is duplicated many times across many project files
	/// It holds references to all the project files that define this property
	/// </summary>
	public class ReferencedProperty : BaseProperty
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="prop">A property instance from the project</param>
		public ReferencedProperty(ProjectProperty prop, CSProject proj)
		{
			this.Name = prop.Name;
			_projects.Add(proj);
		}

		/// <summary>
		/// The name of the property
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// How many times it's used in all project files
		/// </summary>
		public int UsedCount { get; set; }

		public override string ToString()
		{
			return string.Format("ReferenceProperty {0}, usedby: {1} projects", Name, UsedCount);
		}
		private List<CSProject> _projects = new List<CSProject>();
		/// <summary>
		/// The array of Projects that use this property
		/// </summary>
		public CSProject[] Projects { get { return _projects.ToArray(); } }

		/// <summary>
		/// Removes a whole bunch of projects from the list at once
		/// Updates any listeners at the end.
		/// </summary>
		/// <param name="projects"></param>
		public void RemoveProjects(List<CSProject> projects)
		{
			foreach (var removed in projects)
			{
				_projects.Remove(removed);
				UsedCount--;
			}
			OnPropertyChanged("UsedCount");
		}

		public void Add(CSProject proj)
		{
			_projects.Add(proj);
			UsedCount++;
		}
	}

	/// <summary>
	/// Represents a value that a ReferencedProperty can have
	/// as defined across many different project files. So a ReferenceProperty can hold many 
	/// of these instances. 
	/// This also has a count to indicate how many of the values it has. 
	/// It also has a field pointing back to the ReferencedProperty that 'owns' it.
	/// </summary>
	public class ReferencedValues : BaseProperty
	{
		public ReferencedValues() { }
		/// <summary>
		/// The evaluated value of the property
		/// </summary>
		public string EvaluatedValue { get; set; }

		internal string UnEvaluatedValue { get; set; }
		/// <summary>
		/// The number of occurances of the Value
		/// </summary>
		public int Count { get; set; }
		public ReferencedProperty Owner;

		public override string ToString()
		{
			return string.Format("ReferencedValues {0}, Count: {1} for {2}", EvaluatedValue, Count, Owner.Name);
		}
	}
}
