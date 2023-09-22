using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BeautySim2023
{
    public class OrderedPoint
    {
        public int Cluster;


        public Point3D P3D;

        /// <summary>
        
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p"></param>

        public OrderedPoint(int c, Point3D p)
        {
            P3D = p;
            Cluster = c;
        }
    }
}
