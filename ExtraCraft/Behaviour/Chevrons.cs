using System;


	public class Chevrons
	{
		public Chevrons ()
		{
		}
	public static bool CheckSquareSize(long mnX,long mnY,long mnZ,int maxSize,out int xmin,out int xmax,out int zmin,out int zmax)
		{
			int xMin = 0, xMax = 0, zMin = 0, zMax = 0;
			#region Check Size
			//Might be on X Side
			//lets start searching the blocks around
			if (ModTerrainManager.GetCube (mnX + 1, mnY - 1, mnZ) == 124 &&
				ModTerrainManager.GetCube (mnX - 1, mnY - 1, mnZ) == 124) {

				for (int x = 0; x < maxSize; x++) {
					if (ModTerrainManager.GetCube (mnX + x, mnY - 1, mnZ) == 124) {
						if (xMax < x)
							xMax = x;
					} else
						break;
				}

				for (int x = 0; x > -maxSize; x--) {
					if (ModTerrainManager.GetCube (mnX + x, mnY - 1, mnZ) == 124) {
						if (xMin > x)
							xMin = x;
					} else
						break;
				}

				//check if its Z+ or Z-
				int range = Math.Abs (xMax - xMin);
				if (ModTerrainManager.GetCube (mnX + xMax, mnY - 1, mnZ + 1) == 124) {
					zMin = 0;
					zMax = range;

				} else if (ModTerrainManager.GetCube (mnX + xMax, mnY - 1, mnZ - 1) == 124) {
					zMin = -range;
					zMax = 0;

				} else {
					xmin=xmax=zmin=zmax=0;
					return false;
					//whatsbroken = "On x side nothing at z-1 or z+1 ";
				}
				//PopUpString = "On the X Side with " + xMin.ToString () + "/" + xMax.ToString () + "\nAnd range is " + range.ToString ();


			} else {

				//Might be on Z Side
				if (ModTerrainManager.GetCube (mnX, mnY - 1, mnZ + 1) == 124 &&
					(ModTerrainManager.GetCube (mnX, mnY - 1, mnZ + 1) == 124)) {

					for (int z = 0; z < maxSize; z++) {

						if (ModTerrainManager.GetCube (mnX, mnY - 1, mnZ + z) == 124) {
							if (zMax < z)
								zMax = z;
						} else
							break;
					}

					for (int z = 0; z > -maxSize; z--) {
						if (ModTerrainManager.GetCube (mnX, mnY - 1, mnZ + z) == 124) {
							if (zMin > z)
								zMin = z;
						} else
							break;
					}
					int range = Math.Abs (zMax - zMin);
					if (ModTerrainManager.GetCube (mnX + 1, mnY - 1, mnZ + zMax) == 124) {
						xMin = 0;
						xMax = range;

					} else if (ModTerrainManager.GetCube (mnX - 1, mnY - 1, mnZ + zMax) == 124) {
						xMin = -range;
						xMax = 0;

					} else {
						xmin=xmax=zmin=zmax=0;
						return false;
						//whatsbroken = "On z side nothing at x-1 or x+1 ";
					}

					//PopUpString = "On the Z Side with " + zMin.ToString () + "/" + zMax.ToString () + "\nAnd range is " + range.ToString ();
				}

			}
			#endregion
			xmin=xMin;
			xmax=xMax;
			zmin=zMin;
			zmax=zMax;
			return true;
		}

	}

