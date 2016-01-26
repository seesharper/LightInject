using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoverageToXml
{
    using System.IO;

    using Microsoft.VisualStudio.Coverage.Analysis;

    class Program
    {
        static void Main(string[] args)
        {
            var pathToCoverageFile = args[0];            

            var pathToCoverageXmlFile = Path.Combine(
                Path.GetDirectoryName(pathToCoverageFile),
                Path.GetFileNameWithoutExtension(pathToCoverageFile) + ".coveragexml");
            
            using (CoverageInfo info = CoverageInfo.CreateFromFile(pathToCoverageFile))
            {
                CoverageDS data = info.BuildDataSet();

                data.WriteXml(pathToCoverageXmlFile);
            }
           
        }
    }
}
