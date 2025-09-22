// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	/**
	 * TODO:
	 * - Consider if this can be templatized
	 */
	public ref struct UtilityContext
	{
		// target object
		public GameObject owner;
	}
	
}

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	/**
	 * TODO:
	 * - Consider if this can be templatized
	 */
	public ref struct UtilityBrainInitConfig
	{
		public IUtilityAction[] actions;
		public UtilitySelectionMethod SelectionMethod;
	}
	
}