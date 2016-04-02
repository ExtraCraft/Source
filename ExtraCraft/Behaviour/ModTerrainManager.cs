using System;
using UnityEngine;

	public class ModTerrainManager : FortressCraftMod
	{
		public static uint GetCube(long x,long y,long z)
		{
			Segment s = WorldScript.instance.GetSegment (x ,y, z);
			return s.GetCube 					(x, y, z);
		}
		public static CubeData GetCubeData(long x,long y,long z)
		{
			Segment s = WorldScript.instance.GetSegment (x ,y, z);
			return s.GetCubeData					(x, y, z);
		}

	public static string GetKeyfromCube(uint cube)
	{
		foreach (ModCubeMap cubeMap in ModManager.mModMappings.CubeTypes)
		{
			if (cubeMap.CubeType == cube) 
			{
				return cubeMap.Key;
			}
		}
		return null;
	}

	public static ushort GetCubeFromKey(string key)
	{
		foreach (ModCubeMap cubeMap in ModManager.mModMappings.CubeTypes)
		{
			if (cubeMap.Key == key) 
			{
				return cubeMap.CubeType;
			}
		}
		return 0;
	}
		
	}


