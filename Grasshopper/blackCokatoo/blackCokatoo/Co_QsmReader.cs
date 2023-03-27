using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System;
using DeepDesignLab.Base;

namespace blackCokatoo
{
    class Co_QSMreader : BackgroundFileReader {

        // Object files, once read this will store the information.
        private List<string> Headers = new List<string>();
        private List<qsmRowInfo> qsmData = new List<qsmRowInfo>();

        public List<string> GetHeaders { get { if (hasFinished) return Headers; return null; } }
        public List<qsmRowInfo> GetQSMData { get { if (hasFinished) return qsmData; return null; } }

        // The characters for CSV file
        private string ret = "\r";
        private char columnChar = ';';

        // Temp variables for each line read.
        private double ReadValue;
        private int nHeaders;
        private string[] rowTexts;

        public Co_QSMreader() : base() {
            base.headerFinish = "1"; //one header line
        }


        protected override void ProcessHeaderLine(string line, int lineNumber, BackgroundWorker worker, DoWorkEventArgs e) {

            string[] headerNames = line.Split(columnChar);
            for (int i = 0; i < headerNames.Length; i++) {
                headerNames[i] = headerNames[i].Replace(ret, "");
            }
            Headers = new List<string>(headerNames);
            nHeaders = Headers.Count;

            base.ProcessHeaderLine(line, lineNumber, worker, e); // This is a blank function.
        }

        protected override void ProcessLine(string line, int lineNumber, BackgroundWorker worker, DoWorkEventArgs e) {

            rowTexts = line.Split(columnChar);
            if (rowTexts.Length == nHeaders) {

                double[] rowValues = new double[rowTexts.Length];

                for (int k = 0; k < rowTexts.Length; k++) {
                    if (double.TryParse(rowTexts[k], out ReadValue)) {
                        rowValues[k] = ReadValue;
                    }
                    else {
                        rowValues[k] = double.NaN;
                    }
                }

                qsmRowInfo thisRowQSM = new qsmRowInfo(
                    (int)rowValues[0],
                    (int)rowValues[1],
                    (int)rowValues[2],
                    (int)rowValues[3],
                    rowValues[4],
                    rowValues[5],
                    rowTexts[6],
                    rowTexts[7],
                    rowValues[8],
                    rowValues[9],
                    rowValues[10],
                    rowValues[11],
                    rowValues[12],
                    rowValues[13],
                    rowValues[14],
                    rowValues[15],
                    rowValues[18],
                    (int)rowValues[19],
                    rowValues[20],
                    rowValues[21]);
                qsmData.Add(thisRowQSM);
            }
            base.ProcessLine(line, lineNumber, worker, e); // This is a blank function.
        }

    }
}
