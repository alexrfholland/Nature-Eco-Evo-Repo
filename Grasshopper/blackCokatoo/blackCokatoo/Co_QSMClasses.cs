using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System;
using DeepDesignLab.Base;

namespace blackCokatoo
{
    class qsmTree
    {
        //public Dictionary<int, qsmSegment> segments;

        public List<qsmRow> rows;

        List<qsmRowInfo> qsmData;

        public Dictionary<int, qsmSegment> segments;



        public qsmTree(List<qsmRowInfo> _qsmData)
        {
            qsmData = _qsmData;

            rows = CreateRowsFromQSMAsList();

           // segments = CreateSegmentsFromRows();
        }

        /*
         public qsmTree(List<qsmBranchInfo> _qsmData)
         {
             qsmData = _qsmData;

             segments = CreateSegmentsFromQSM();

             foreach (qsmSegment segment in segments.Values)
             {
                 segment.GetParents(segments);
             }
         }
         */


        public List<qsmRow> CreateRowsFromQSMAsList()
        {
            List<qsmRow> _rows = new List<qsmRow>();

            foreach (qsmRowInfo qsmLine in qsmData)
            {
                _rows.Add(new qsmRow(qsmLine));
            }

            return _rows;

        }

        public Dictionary<int, qsmSegment> CreateSegmentsFromRows()
        {
            Dictionary<int, qsmSegment> segmentDic = new Dictionary<int, qsmSegment>();

            foreach (qsmRow row in rows)
            {
                int segmentID = row.qsmInfo.segmentID;

                if(!segmentDic.ContainsKey(segmentID))
                {
                    qsmSegment newSeg = new qsmSegment(segmentID, row, this);
                    segmentDic.Add(segmentID, newSeg);
                }

                else
                {
                    segmentDic[segmentID].AppendSegment(row);
                }
            }

            foreach (qsmSegment seg in segments.Values)
            {
                seg.GetParentChildSegment();
            }

            return segmentDic;

        }

    }

    class qsmRowInfo
    {
        public int
          branchID,
          branch_order,
          segmentID,
          parent_segment_ID;

        public double
            growth_volume,
            growth_length;

        public string
            detection,
            improvment;

        public double
            startX,
            startY,
            startZ,

            endX,
            endY,
            endZ,

            radius,
            length,

            length_to_leave,
            inverse_branch_order,
            length_of_segment,
            branch_order_cum;

        public qsmRowInfo(
            int _branchID,
            int _branch_order,
            int _segmentID,
            int _parent_segment_ID,

            double _growth_volume,
            double _growth_length,
            string _detection,
            string _improvment,

            double _startX,
            double _startY,
            double _startZ,

            double _endX,
            double _endY,
            double _endZ,

            double _radius,
            double _length,

            double _length_to_leave,
            double _inverse_branch_order,
            double _length_of_segment,
            double _branch_order_cum)
        {
            branchID = _branchID;
            branch_order = _branch_order;
            segmentID = _segmentID;
            parent_segment_ID = _parent_segment_ID;

            growth_volume = _growth_volume;
            growth_length = _growth_length;

            detection = _detection;
            improvment = _improvment;

            startX = _startX;
            startY = _startY;
            startZ = _startZ;

            endX = _endX;
            endY = _endY;
            endZ = _endZ;

            radius = _radius;
            length = _length;

            length_to_leave = _length_to_leave;
            inverse_branch_order = _inverse_branch_order;
            length_of_segment = _length_of_segment;
            branch_order_cum = _branch_order_cum;
        }

    }

    class qsmRow
    {
        public qsmRowInfo qsmInfo;
        public qsmRow segment;

        public qsmRow(qsmRowInfo _qsm)
        {
            qsmInfo = _qsm;
        }



    }

    class qsmSegment
    {
        public qsmTree inputTree;
        public List<qsmRow> rows = new List<qsmRow>();
        public int segmentID;
        public int parentSegID;
        public qsmSegment parentSegment;
        public qsmSegment childSegment;

        public qsmSegment(int _segmentID, qsmRow _row, qsmTree _inputTree)
        {
            segmentID = _segmentID;
            inputTree = _inputTree;
            parentSegID = _row.qsmInfo.parent_segment_ID;
            rows.Add(_row);
        }

        public void AppendSegment(qsmRow _row)
        {
            rows.Add(_row);
        }

        public void GetParentChildSegment()
        {
            if(inputTree.segments.ContainsKey(parentSegID))
            {
                parentSegment = inputTree.segments[parentSegID];
                inputTree.segments[parentSegID].childSegment = this;
            }

        }
    }

    class qsmLeaf
    {
        public double x, y, z, r, g, b, intensity, distanceToBranch, shading;

        public qsmLeaf (double _x, double _y, double _z, double _r, double _b, double _g, double _intensity, double _distanceToBranch, double _shading)
        {
            x = _x;
            y = _y;
            z = _z;

            r = _r;
            b = _b;
            g = _g;

            intensity = _intensity;

            distanceToBranch = _distanceToBranch;

            shading = _shading;
        }
    }

    class qsmCanopy
    {
        public List<qsmLeaf> leaves;

        public qsmCanopy (List<qsmLeaf> _leaves)
        {
            leaves = _leaves;
        }
    }
}
