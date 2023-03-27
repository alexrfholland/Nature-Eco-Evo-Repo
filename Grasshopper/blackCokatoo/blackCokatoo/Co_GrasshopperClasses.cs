using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using GH_IO.Types;
using Rhino.Display;

namespace blackCokatoo
{

    public enum Quality
    {
        good,
        bad
    };

    class ghTree
    {
        public List<ghRow> rows;
        public Dictionary<int, ghSegment> segments;
        public ghCanopy canopy;

        public double raddiBranchEndSearch = -1;
        public int leafCountForExposed = -1;
        public double distForTip = -1;


        public ghTree(qsmTree _qsmTree)
        {
            /*
            rows = new List<ghRow>();
            foreach (qsmRow _qsmSegment in _qsmTree.rows)
            {
                ghRow _ghSegment = new ghRow(_qsmSegment);
                ghSegment _ghSegment = new ghSegment(_qsmSegment, this);

                rows.Add(_ghSegment);
            }
            */

            rows = CreateRowsFromQsmTree(_qsmTree);
           // segments = CreateSegmentsFromRows();
        }


        public List<ghRow> CreateRowsFromQsmTree(qsmTree _qsmTree)
        {
            rows = new List<ghRow>();
            foreach (qsmRow _qsmRow in _qsmTree.rows)
            {
                ghRow _ghRow = new ghRow(_qsmRow, this);

                rows.Add(_ghRow);
            }

            return rows;

        }

        public Dictionary<int, ghSegment> CreateSegmentsFromRows()
        {
            Dictionary<int, ghSegment> segmentDic = new Dictionary<int, ghSegment>();

            foreach (ghRow row in rows)
            {
                int segmentID = row.qsmInfo.segmentID;

                if (!segmentDic.ContainsKey(segmentID))
                {
                    ghSegment newSeg = new ghSegment( row, this);
                    segmentDic.Add(segmentID, newSeg);
                }

                else
                {
                    segmentDic[segmentID].AppendSegment(row);
                }
            }

            foreach (ghSegment seg in segments.Values)
            {
                seg.GetParentChildSegment();
            }

            return segmentDic;

        }

        public void CheckDeadBranchTips()
        {
            foreach (ghRow row in rows)
            {
                row.CheckIfDeadBranchTip();
            }
        }

    }

    class ghSegment
    {
        public ghTree inputTree;
        public List<ghRow> rows = new List<ghRow>();
        public int segmentID;
        public int parentSegID;
        public ghSegment parentSegment;
        public ghSegment childSegment;

        public List<string> errorLog = new List<string>();

        public double sDev, upperQrt, lowerQrt;


        public ghSegment(ghRow _row, ghTree _inputTree)
        {
            segmentID = _row.segmentID;
            inputTree = _inputTree;
            parentSegID = _row.qsmInfo.parent_segment_ID;
            rows.Add(_row);
        }

        public void AppendSegment(ghRow _row)
        {
            rows.Add(_row);
        }

        public void GetParentChildSegment()
        {
            if (inputTree.segments.ContainsKey(parentSegID))
            {
                parentSegment = inputTree.segments[parentSegID];
                inputTree.segments[parentSegID].childSegment = this;
            }

        }
        void GetParentChildInRows()
        {
            foreach (ghRow row in rows)
            {
                row.GetParentChildRow();
            }

            List<ghRow> orderedRows = new List<ghRow>();
            bool isFirst = false;

            RowMiner(rows.First(), ref orderedRows, ref isFirst);

            rows = orderedRows;
        }

        void RowMiner(ghRow row, ref List<ghRow> orderedRows, ref bool isFirst)
        {
            if (isFirst == false && row.parentRow != null)
            {
                RowMiner(row.parentRow, ref orderedRows, ref isFirst);
            }

            else
            {
                isFirst = true;
                orderedRows.Add(row);

                if(row.childRow == null)
                {
                    return;
                }

                else
                {
                    RowMiner(row.childRow, ref orderedRows, ref isFirst);
                }
                
            }
        }

        void GetStats(int quartiles)
        {
            List<double> magDifs = new List<double>();

            foreach (ghRow row in rows)
            {
                row.GetMagDifference();

                //calculated properly
                if (row.magDif > 0)
                {
                    magDifs.Add(row.magDif);
                }


            }


            double[] magdifArray = magDifs.ToArray();

            sDev = magdifArray.StdDev(false);

            double mean, min, max;
            mean = magdifArray.Average();

            upperQrt = mean - (sDev * quartiles);
            lowerQrt = mean + (sDev * quartiles);
        }

        void ErrorCheck()
        {
            GetParentChildInRows();

            GetStats(2);

            SetCondition();
        }

        void SetCondition()
        {
            foreach (ghRow row in rows)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    if (row.magDif > lowerQrt)
                    {
                        if (row.magDif < upperQrt)
                        {
                            row.quality = Quality.good;
                        }
                    }
                }
            }
        }



    }

    class ghRow
    {
        public Quality quality = Quality.bad;
        public int segmentID;
        public qsmRowInfo qsmInfo;
        public ghSegment parentSegment;

        public Point3d startPt, endPt;

        public ghRow parentRow;
        public ghRow childRow;

        public Vector3d rowVec;

        public double magDif = -2;

        public LineCurve rowCurve;

        public ghTree parentTree;

        public int nearbyLeaves = -1;

        public Mesh pipe;

        public double angle;

        public List<ghSegment> thisBranchSegs = new List<ghSegment>();
        public Mesh thisBranchSegsAsMesh;

        public ghRow(qsmRow row, ghTree _parentTree)
        {
            qsmInfo = row.qsmInfo;
            segmentID = qsmInfo.segmentID;
            parentTree = _parentTree;
    

            GetGrasshopperData();

            pipe = Mesh.CreateFromCurvePipe(rowCurve, qsmInfo.radius, 10, 1, MeshPipeCapStyle.None, false, null);

            GetAngle();


        }

        public void GetGrasshopperData()
        {
            startPt = new Point3d(qsmInfo.startX, qsmInfo.startY, qsmInfo.startZ);
            endPt = new Point3d(qsmInfo.endX, qsmInfo.endY, qsmInfo.endZ);
            rowVec = new Vector3d(endPt - startPt);
            rowCurve = new LineCurve(startPt, endPt);

        }

        public void GetParentChildRow()
        {
            foreach (ghRow row in parentSegment.rows)
            {
                if(endPt.DistanceTo(row.startPt) < 0.001)
                {
                    childRow = row;
                    row.parentRow = this;
                }
            }
        }

        public void GetMagDifference()
        {
            double mag = -1;

            if (childRow != null)
            {
                Vector3d difference = new Vector3d(childRow.rowVec - rowVec);
                mag = difference.Length;
            }

            magDif = mag;

     
        }


        public void GetAngle()
        {
            //find rise (y-y)
            double rise = Math.Abs(startPt.Y - endPt.Y);
            //find run (new pts from x and y values)
            double run = new Point2d(startPt.X, startPt.Y).DistanceTo(new Point2d(endPt.X, endPt.Y)) + 0.001;

            //divide rise by run
            angle = rise / run;
        }

        public void CheckIfDeadBranchTip()
        {
            if(qsmInfo.length_to_leave <= parentTree.distForTip)
            {
                nearbyLeaves = 0;
                //is an endtip segment

                GetBranchExposure();
            }
        }

        public void GetBranchExposure()
        {

            foreach (Point3d leafPt in parentTree.canopy.leafPts)
            {
                double maxDist = parentTree.raddiBranchEndSearch;

                double dist = endPt.DistanceToSquared(leafPt);

                if (dist < (maxDist * maxDist))
                {
                    nearbyLeaves++;
                }

            }

            // GetOtherSegmentsInBranch();
        }

        //USE TO COMBINE MESHSES IN NEW SEGMENT CODE
        /*
        public void GetOtherSegmentsInBranch()
        {
            foreach (ghSegment otherSeg in parentTree.segmentsAsList)
            {
                if (qsmInfo.segmentID == otherSeg.qsmInfo.segmentID)
                {
                    thisBranchSegs.Add(otherSeg);
                }
            }

            GetAllSegmentsAshMesh();
        }
   

        public void GetAllSegmentsAshMesh()
        {
            thisBranchSegsAsMesh = pipe.DuplicateMesh();

            foreach (ghSegment horizontalSeg in thisBranchSegs)
            {
                Mesh mesh = horizontalSeg.pipe.DuplicateMesh();

                thisBranchSegsAsMesh.Append(mesh);
            }
        }

         */


    }

    class ghLeaf
    {
        public Point3d origin;
        public Color col;
        public ColorHSL displayCol;
        public double intensity, distanceToBranch, shading;

        public ghLeaf (qsmLeaf leaf)
        {
            origin = new Point3d(leaf.x, leaf.y, leaf.z);
            col = Color.FromArgb((int)leaf.r, (int)leaf.b, (int)leaf.g);

            intensity = leaf.intensity;
            distanceToBranch = leaf.distanceToBranch;
            shading = leaf.shading;

            displayCol = new ColorHSL(Color.Green.GetHue(), Color.Green.GetSaturation(), (shading)); 
        }
    }
    class ghCanopy
    {
        public List<ghLeaf> leaves = new List<ghLeaf>();
        public List<Point3d> leafPts = new List<Point3d>();
        public PointCloud canopyCloud;

        double spacing;

        public ghCanopy(qsmCanopy _qsmCanopy)
        {
            canopyCloud = new PointCloud();

            foreach (qsmLeaf _qsmLeaf in _qsmCanopy.leaves)
            {
                leaves.Add(new ghLeaf(_qsmLeaf));
            }

            foreach (ghLeaf leaf in leaves)
            {
                leafPts.Add(leaf.origin);
                canopyCloud.Add(leaf.origin, leaf.displayCol);
            }

        }

        public ghCanopy(List<ghLeaf> _leaves)
        {          
            leaves = _leaves;
            canopyCloud = new PointCloud();

            foreach (ghLeaf leaf in leaves)
            {
                leafPts.Add(leaf.origin);

                canopyCloud.Add(leaf.origin, leaf.col);
            }
            //FindSpacing();
        }

        void FindSpacing()
        {
            double smallestDist = 9999999;

            foreach (PointCloudItem pt in canopyCloud)
            {
                double dist = canopyCloud.PointAt(0).DistanceToSquared(pt.Location);
                if (smallestDist < dist)
                {
                    smallestDist = dist;
                }
            }

            spacing = smallestDist;
        }
    }

    class ghExports
    {
        public List<Point3d> StartPts { get; set; }
        public List<Point3d> EndPts { get; set; }

        public List<Line> SegmentLines { get; set; }

        public List<int> branchID { get; set; }
        public List<int> branchOrder { get; set; }
        public List<int> segmentID { get; set; }
        public List<double> lengthToLeave { get; set; }

        public List<int> leafCount = new List<int>();

        public List<Line> exposedBranch = new List<Line>();

        public List<PointCloud> canopies = new List<PointCloud>();

        public List<Point3d> canopiesPts = new List<Point3d>();

        public List<Mesh> pipes = new List<Mesh>();
        public List<double> angles = new List<double>();

        public List<Mesh> horizontalPipes = new List<Mesh>();

        public List<int> leafCountRepeatForSegs = new List<int>();





        public ghExports(List<ghTree> _trees)
        {
            StartPts = new List<Point3d>();
            EndPts = new List<Point3d>();
            SegmentLines = new List<Line>();

            branchID = new List<int>();
            branchOrder = new List<int>();
            segmentID = new List<int>();
            lengthToLeave = new List<double>();

            foreach (ghTree tree in _trees)
            {
                foreach (ghRow _segment in tree.rows)
                {
                    StartPts.Add(_segment.startPt);
                    EndPts.Add(_segment.endPt);
                    branchID.Add(_segment.qsmInfo.branchID);
                    branchOrder.Add(_segment.qsmInfo.branch_order);
                    segmentID.Add(_segment.qsmInfo.segmentID);
                    lengthToLeave.Add(_segment.qsmInfo.length_to_leave);

                    SegmentLines.Add(new Line(_segment.startPt, _segment.endPt));
                }
                canopies.Add(tree.canopy.canopyCloud);

                canopiesPts.AddRange(tree.canopy.leafPts);

                foreach (ghRow row in tree.rows)
                {
                    StartPts.Add(row.startPt);
                    EndPts.Add(row.endPt);
                    branchID.Add(row.qsmInfo.branchID);
                    branchOrder.Add(row.qsmInfo.branch_order);
                    segmentID.Add(row.qsmInfo.segmentID);
                    lengthToLeave.Add(row.qsmInfo.length_to_leave);

                    SegmentLines.Add(new Line(row.startPt, row.endPt));

                    leafCount.Add(row.nearbyLeaves);

                    
                    if (row.nearbyLeaves >= 0 && row.nearbyLeaves <= tree.leafCountForExposed)
                    {
                        exposedBranch.Add(new Line(row.startPt, row.endPt));
                    }

                    pipes.Add(row.pipe);
                    angles.Add(row.angle);

                    if(row.thisBranchSegsAsMesh != null)
                    {
                        horizontalPipes.Add(row.thisBranchSegsAsMesh);
                    }

                }


            }

        }
    }
}
