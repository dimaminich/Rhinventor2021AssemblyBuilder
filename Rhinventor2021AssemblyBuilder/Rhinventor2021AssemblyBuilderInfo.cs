using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Rhinventor2021AssemblyBuilder
{
    public class Rhinventor2021AssemblyBuilderInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Rhinventor2021AssemblyBuilder";
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
                return new Guid("290dcb72-e6ca-4d4c-8bcf-c955c33881f3");
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
