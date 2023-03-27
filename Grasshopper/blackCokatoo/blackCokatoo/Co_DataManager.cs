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
    class Co_DataManager
    {
        static int fileReaderProgress = -1;

        List<Co_QSMreader> qsmReaders = new List<Co_QSMreader>();
        List<qsmTree> qsmTrees = new List<qsmTree>();

        string folderPath = "F:\\_First batch\\06A 15-02 s1 tu-kh1 o\\QSM Detailed\\detailed";
        //string folderPath;
        string[] filePaths;

        // output variables...


        public string Path { get; private set; }

        List<ghTree> outputTrees = new List<ghTree>();

        public List<ghTree> OutputTrees
        {
            get
            {
                if (fileReaderProgress == 3)
                {
                    return outputTrees;
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                outputTrees = value;
            }
        }

        ghExports exports;

        public ghExports Exports
        {
            get
            {
                if (fileReaderProgress == 3)
                {
                    return exports;
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                exports = value;
            }
        }

        public Co_DataManager()
        {
            // folderPath = _folderPath;
        }

        public void ResetReader()
        {
            foreach (var item in qsmReaders)
            {
                item.cancel();
            }
            qsmReaders.Clear();
            // qsmReaders = new List<Co_QSMreader>();
            fileReaderProgress = -1;
        }
        public void Loop()
        {


            if (fileReaderProgress == -1)
            {
                qsmReaders = new List<Co_QSMreader>();

                fileReaderProgress = 0;
                filePaths = Directory.GetFiles(folderPath);
                foreach (string path in filePaths)
                {
                    Co_QSMreader tempRead = new Co_QSMreader();
                    qsmReaders.Add(tempRead);
                    tempRead.readFile(path);
                }
            }

            if (fileReaderProgress == 0)
            {
                bool hasFinishedAllThreads = false;
                foreach (var item in qsmReaders)
                {
                    if (!item.hasFinished)
                    {
                        hasFinishedAllThreads = false;
                        break;
                    }
                    else
                    {
                        hasFinishedAllThreads = true;
                    }
                }
                if (hasFinishedAllThreads == true)
                {
                    fileReaderProgress = 1;
                }
            }

            if (fileReaderProgress == 1)
            {
                qsmTrees = new List<qsmTree>();

                foreach (Co_QSMreader reader in qsmReaders)
                {
                    List<qsmRowInfo> QSMBranchData = reader.GetQSMData;
                    qsmTree tempQSMTree = new qsmTree(QSMBranchData);
                    qsmTrees.Add(tempQSMTree);
                }

                fileReaderProgress = 2;
            }

            if (fileReaderProgress == 2)
            {
                outputTrees = new List<ghTree>();
                //outputs for grasshopper
                foreach (qsmTree _qsmTree in qsmTrees)
                {
                    ghTree _ghTree = new ghTree(_qsmTree);
                    outputTrees.Add(_ghTree);
                }

                Exports = new ghExports(outputTrees);

                fileReaderProgress = 3;
            }

        }
    }

       
}
