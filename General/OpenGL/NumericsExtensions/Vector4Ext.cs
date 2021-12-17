﻿using System.Numerics;

namespace OpenGL
{
	public static class Vector4Ext
	{
		public static float Get(this Vector4 v, int index)
		{
			switch (index)
			{
				case 0: return v.X;
				case 1: return v.Y;
				case 2: return v.Z;
				case 3: return v.W;
				default: return 0;  // error case
			}
		}
	}
}
