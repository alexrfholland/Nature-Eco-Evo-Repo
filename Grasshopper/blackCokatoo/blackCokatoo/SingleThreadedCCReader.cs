using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System;
using DeepDesignLab.Base;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace blackCokatoo
{
    class SingleThreadedCCReader
    {
        private List<qsmLeaf> leafData = new List<qsmLeaf>();

        public List<qsmLeaf> Leafdata { get { if (hasFinished) return leafData; return null; } }

        public List<string> Headers;
        public int nHeaders;

        bool hasFinished = false;


        public void readFile(string path)
        {

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs, 4 * 1024 * 1024))
            using (StreamReader reader = new StreamReader(bs))
            {

                double ReadValue;
                string line;
                line = reader.ReadLine();
                string[] headerNames = line.Split(';');
                for (int i = 0; i < headerNames.Length; i++)
                {
                    headerNames[i] = headerNames[i].Replace("/r", "");
                }


                Headers = new List<string>(headerNames);
                nHeaders = Headers.Count;

                string[] rowTexts;
                // read header lines, now to read body.
                line = reader.ReadLine();
                while (!reader.EndOfStream)
                {

                    rowTexts = line.Split(';');
                    if (rowTexts.Length == nHeaders)
                    {

                        double[] rowValues = new double[rowTexts.Length];

                        for (int k = 0; k < rowTexts.Length; k++)
                        {
                            if (double.TryParse(rowTexts[k], out ReadValue))
                            {
                                rowValues[k] = ReadValue;
                            }
                            else
                            {
                                rowValues[k] = double.NaN;
                            }
                        }


                        qsmLeaf thisRowLeaf = new qsmLeaf(
                           rowValues[0],
                           rowValues[1],
                           rowValues[2],
                           rowValues[3],
                           rowValues[4],
                           rowValues[5],
                           rowValues[6],
                           rowValues[7],
                           rowValues[7]);


                        leafData.Add(thisRowLeaf);


                        line = reader.ReadLine();

                    }

                }

                hasFinished = true;

            }


        }
    }
}

