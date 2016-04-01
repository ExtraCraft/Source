using System;

	public static class Trees
	{
		public static bool CanSpawnTree(int type,long x,long y, long z,bool debug,bool replaceItSelf )
		{
			Segment s;
			ushort cube; 
			//make truk

			//Check if there is a tree on the sides
			for (int i = 0; i < 7; i++) {
				if (ModTerrainManager.GetCube (x + 1, y+i, z) == 14)
					return false;
				if (ModTerrainManager.GetCube (x - 1, y+i, z) == 14)
					return false;
				if (ModTerrainManager.GetCube (x, y+i, z + 1) == 14)
					return false;
				if (ModTerrainManager.GetCube (x, y+i, z - 1) == 14)
					return false;
				if (ModTerrainManager.GetCube (x + 1, y+i, z+1) == 14)
					return false;
				if (ModTerrainManager.GetCube (x - 1, y+i, z-1) == 14)
					return false;
				if (ModTerrainManager.GetCube (x-1, y+i, z + 1) == 14)
					return false;
				if (ModTerrainManager.GetCube (x+1, y+i, z - 1) == 14)
					return false;
			}

			int a=0;
			if (replaceItSelf)
				a = 1;
			for(int i=a;i<7;i++)
			{
				s = WorldScript.instance.GetSegment (x, y+i, z);
				cube = s.GetCube(x, y+i, z);
				if (s != null)
				{
					if(cube!=1 && cube!=16)
					{
						if(debug)
						{
							global::GameManager.DoLocalChat("[MOD] something blocking " + cube.ToString() + " Log");
						}
						return false;
					}
				}
			}
			//Some leaves
			for(int iy=4;iy<7;iy++)
				for(int ix=-1;ix<2;ix++)
					for(int iz=-1;iz<2;iz++)
					{
						s = WorldScript.instance.GetSegment(x+ix, y+iy, z+iz);
						cube = s.GetCube(x+ix, y+iy, z+iz);
						if (s != null)
						{
							if(cube!=1 && cube!=16)
							{
								if(debug)
								{
									global::GameManager.DoLocalChat("[MOD] something blocking " + cube.ToString() + " Leave");
								}
								return false;
							}
						}


					}
			return true;
		}

		public static void SpawnTree(int type,long x,long y, long z)
		{
			Segment s = WorldScript.instance.GetSegment (x, y, z);
			//ushort oldcube;



			//Some leaves
			for(int iy=4;iy<7;iy++)
				for(int ix=-1;ix<2;ix++)
					for(int iz=-1;iz<2;iz++)
					{
						s = WorldScript.instance.GetSegment(x+ix, y+iy, z+iz);
						if (s != null)
						{

							//oldcube = s.GetCube(x+ix, y+iy, z+iz);
							//if (oldcube>0)
							//{
							WorldScript.instance.BuildOrientationFromEntity(s, x+ix, y+iy, z+iz, 16,0, 65);
							//}
						}


					}
			//make truk
			for(int i=0;i<7;i++)
			{

				if (s != null)
				{
					//oldcube = s.GetCube(x, y+i, z);
					//if (oldcube !=0)
					s = WorldScript.instance.GetSegment(x, y+i, z);
					WorldScript.instance.BuildOrientationFromEntity(s, x, y+i, z, 14, 0, 65);
				}
			}
	}
}


