using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santi.Utils
{
	public static class FloatExtensions
	{
		public static float Map(this float value, float inputFrom, float inputTo, float outputFrom, float outputTo)
        {
			return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
		}
	}
}
