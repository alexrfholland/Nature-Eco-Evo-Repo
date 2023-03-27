using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace blackCokatoo
{
    public class blackCokatooComponent : GH_Component
    {        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public blackCokatooComponent()
          : base("blackCokatoo", "blackCokatoo",
              "blackCokatoo",
              "blackCokatoo", "blackCokatoo")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
         //   pManager.AddNumberParameter("raddiBranchEndSearch", "raddiBranchEndSearch", "raddiBranchEndSearch", GH_ParamAccess.item);
          //  pManager.AddNumberParameter("distForTip", "distForTip", "distForTip", GH_ParamAccess.item);
          //  pManager.AddIntegerParameter("leafCountForExposed", "leafCountForExposed", "leafCountForExposed", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Output", "output", "output", GH_ParamAccess.list);

            pManager.AddGenericParameter("branchID", "branchID", "branchID", GH_ParamAccess.list);

            pManager.AddGenericParameter("branchOrder", "branchOrder", "bbranchOrderranchID", GH_ParamAccess.list);

            pManager.AddGenericParameter("segmentID", "segmentID", "segmentID", GH_ParamAccess.list);

            pManager.AddGenericParameter("lengthtoleave", "lengthtoleave", "lengthtoleave", GH_ParamAccess.list);

            pManager.AddGenericParameter("leafCount", "leafCount", "leafCount", GH_ParamAccess.list);

            pManager.AddGenericParameter("exposedBranches", "exposedBranches", "exposedBranches", GH_ParamAccess.list);

            pManager.AddGenericParameter("canopy", "canopy", "canopy", GH_ParamAccess.list);

            pManager.AddGenericParameter("pipe", "pipe", "pipe", GH_ParamAccess.list);

            pManager.AddGenericParameter("gradient", "gradient", "gradient", GH_ParamAccess.list);

            pManager.AddGenericParameter("horizontalPipes", "horizontalPipes", "horizontalPipes", GH_ParamAccess.list);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            double raddiBranchEndSearch = -1;
            double distForTip = -1;
            int leafCountForExposed = -1;


            /*if (!DA.GetData(0, ref raddiBranchEndSearch)) return;
            if (!DA.GetData(1, ref leafCountForExposed)) return;
            if (!DA.GetData(1, ref distForTip)) return;
            */


            /*
            bool isReset = false;

            if (!DA.GetData(0, ref isReset)) return;

            if (isReset)
            {
                Co_DataManager manager = new Co_DataManager();
                manager.ResetReader();
            }

            manager.Loop();
            

          */

            SingleThreadedDataManager manager = new SingleThreadedDataManager(raddiBranchEndSearch, distForTip, leafCountForExposed);
            manager.ProcessData();




            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(0, 0, 0));


            DA.SetDataList(0, manager.exports.SegmentLines);
            DA.SetDataList(1, manager.exports.branchID);
            DA.SetDataList(2, manager.exports.branchOrder);
            DA.SetDataList(3, manager.exports.segmentID);
            DA.SetDataList(4, manager.exports.lengthToLeave);
            DA.SetDataList(5, manager.exports.leafCount);
            DA.SetDataList(6, manager.exports.exposedBranch);

            DA.SetDataList(7, manager.exports.canopies);

            DA.SetDataList(8, manager.exports.pipes);
            DA.SetDataList(9, manager.exports.angles);

            DA.SetDataList(10, manager.exports.horizontalPipes);





        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("270954b6-5d02-43af-b62a-244088c2a3e6"); }
        }
    }
}
