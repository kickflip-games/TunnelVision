﻿/*
 * Written by Avi Vajpeyi
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{

    public static class Options
    {

        public static int ChunkLoaderRadius { get; set; }
		public static bool Lod { get; set; }
		public static bool Invert { get; set;}

        static Options()
        {
            ChunkLoaderRadius = 10;
			Lod = true;
			Invert = false;
        }
    }
}
