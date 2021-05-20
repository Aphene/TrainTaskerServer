using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace AphAsyncHandler
{
    public class FitPrices
    {
        public static double poly3degree(List<double> points)  // n day polynomial fit  must have degrees +1 nodes  degree 3 is x^2+x+k
        {
            int nodes = points.Count;
            List<double> vals = new List<double>();
            //  Tuple<double, double> p;
            double[] p;
            double[] ydata = new double[nodes];
            double[] xdata = new double[nodes];
            double m;
            double b;
            double s;
            double val = 0F;
            for (int i = 0; i < nodes; ++i) xdata[i] = i;
            ydata = points.ToArray();
            p = Fit.Polynomial(xdata, ydata, 2);
            b = p[0];
            m = p[1];
            s = p[2];
            val = s * nodes * nodes + m * nodes + b;
            return val;
        }

    }
}
