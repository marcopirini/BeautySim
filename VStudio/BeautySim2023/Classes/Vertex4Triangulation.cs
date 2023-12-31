using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySim2023
{
    public class Vertex4Triangulation : IVertex
    {
        public double[] Position { get; set; }

        // Constructor for easy creation of vertices
        public Vertex4Triangulation(double x, double y, double z)
        {
            Position = new double[] { x, y, z };
        }
    }
}
