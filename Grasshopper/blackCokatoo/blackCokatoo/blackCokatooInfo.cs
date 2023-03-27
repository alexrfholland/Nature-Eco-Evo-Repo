using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace blackCokatoo
{
    public class blackCokatooInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "blackCokatoo";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("24093d59-12ea-41fb-9a20-6c3ddc9de1e0");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
