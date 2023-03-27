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
    class SingleThreadedDataManager
    {
        public double raddiBranchEndSearch = -1;
        public int leafCountForExposed = -1;
        public double distForTip = -1;

        //string qsmFolderPath = "F:\\_First batch\\06A 15-02 s1 tu-kh1 o\\QSM Detailed\\detailed";
        string qsmFolderPath = "F:\\_First batch\\Tests\\QSM";
        //string folderPath;
        string[] qsmFilePaths;

        List<SingleThreadedReader> qsmReaders = new List<SingleThreadedReader>();
        List<qsmTree> qsmTrees = new List<qsmTree>();
        List<ghTree> ghTrees = new List<ghTree>();

        // string canopiesFolderPath = "F:\\_First batch\\_Leaves";
        string canopiesFolderPath = "F:\\_First batch\\Tests\\Leaves";
      string[] canopiesFilePaths;

        List<SingleThreadedCCReader> canopyReaders = new List<SingleThreadedCCReader>();
        List<qsmCanopy> qsmCanopies= new List<qsmCanopy>();

        public ghExports exports;


        public void ProcessData()
        {
            //branches
            qsmFilePaths = Directory.GetFiles(qsmFolderPath);
            foreach (string path in qsmFilePaths)
            {
                SingleThreadedReader tempRead = new SingleThreadedReader();
                qsmReaders.Add(tempRead);
                tempRead.readFile(path);
            }

            foreach (SingleThreadedReader reader in qsmReaders)
            {
                List<qsmRowInfo> QSMBranchData = reader.QsmData;
                qsmTree tempQSMTree = new qsmTree(QSMBranchData);
                qsmTrees.Add(tempQSMTree);
            }

            //outputs for grasshopper
            foreach (qsmTree _qsmTree in qsmTrees)
            {
                ghTree _ghTree = new ghTree(_qsmTree);
                
                _ghTree.leafCountForExposed = leafCountForExposed;
                _ghTree.raddiBranchEndSearch = raddiBranchEndSearch;
                _ghTree.distForTip = distForTip;

                ghTrees.Add(_ghTree);
            }

            //canopies

            canopiesFilePaths = Directory.GetFiles(canopiesFolderPath);
            foreach (string path in canopiesFilePaths)
            {
                SingleThreadedCCReader tempCCRead = new SingleThreadedCCReader();
                canopyReaders.Add(tempCCRead);
                tempCCRead.readFile(path);
            }

            foreach (SingleThreadedCCReader readerCC in canopyReaders)
            {
                List<qsmLeaf> qsmLeafInfos = readerCC.Leafdata;
                qsmCanopy tempQSMCanopy = new qsmCanopy(qsmLeafInfos);
                qsmCanopies.Add(tempQSMCanopy);
            }


            for (int i = 0; i < qsmCanopies.Count; i++)
            {
                ghCanopy tempGHCanopy = new ghCanopy(qsmCanopies[i]);
                ghTrees[i].canopy = tempGHCanopy;
            }

            foreach (ghTree tree in ghTrees)
            {
                tree.CheckDeadBranchTips();
            }
            
            //exports

            exports = new ghExports(ghTrees);
        }

        public SingleThreadedDataManager(double _raddiBranchEndSearch, double _distForTip, int _leafCountForExposed)
        {
            // raddiBranchEndSearch = _raddiBranchEndSearch;
            //  distForTip = _distForTip;
            // leafCountForExposed = _leafCountForExposed;

            raddiBranchEndSearch = 1;
            distForTip = .05;
            leafCountForExposed = 10;
        }

    }
    
    class SingleThreadedReader
    {

        private List<qsmRowInfo> qsmData = new List<qsmRowInfo>();

        public List<qsmRowInfo> QsmData { get { if (hasFinished) return qsmData; return null; } }

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
                        

                        line = reader.ReadLine();

                    }

                }
                
                hasFinished = true;

            }


        }
    }
}
