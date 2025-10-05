// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	
	/**
	 * 
	 */
	public interface IUtilityConsideration
	{
		public bool Enabled { get;  }
		
		// display info
		public string Name { get; }

		public float GetScore(in UtilityContext context);
	}
	
}
