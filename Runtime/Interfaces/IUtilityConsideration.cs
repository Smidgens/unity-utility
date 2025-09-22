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

		public float GetScore(in UtilityContext context);
	}
	
}
