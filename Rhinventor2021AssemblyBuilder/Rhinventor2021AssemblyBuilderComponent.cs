using System;
using Grasshopper.Kernel;

namespace Rhinventor2021AssemblyBuilder
{
    public class Rhinventor2021AssemblyBuilderComponent : GH_Component
    {
        private static System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();

        public Rhinventor2021AssemblyBuilderComponent()
          : base("RhinventorAssemblyBuilder", "rhibuilder",
              $"RhinventorAssemblyBuilder {ass.GetName().Version.ToString()} (Created by Dimitrij Minich). Build assemblies",
              "Rhinventor", "Rhinventor 2021")
        {
        }

        protected override void RegisterInputParams(
            GH_Component.GH_InputParamManager pManager
            )
        {
            pManager.AddTextParameter(
                "rootPath", "CP", "Set the root path", GH_ParamAccess.item);

            pManager.AddTextParameter(
                "assemblyFile", "AF", "Set assembly file", GH_ParamAccess.tree);
            pManager.AddTextParameter(
                "label", "L", "Set label", GH_ParamAccess.tree);
            pManager.AddGenericParameter(
                "parameterData", "MD", "Set paramter data", GH_ParamAccess.tree);
            pManager.AddGenericParameter(
                "iPropertyData", "PD", "Set iProperty data", GH_ParamAccess.tree);
            pManager.AddPlaneParameter(
                 "plane", "PL", "Set plane", GH_ParamAccess.tree);
            pManager.AddGroupParameter(
                 "points", "RP", "Set reference points", GH_ParamAccess.tree);

            pManager.AddBooleanParameter(
                "StartProcess", "B", "Start process", GH_ParamAccess.item);

            pManager.AddTextParameter(
                "OptionSeparator", "O1", "Set the separator for Inventor files, default '_'", GH_ParamAccess.item, "_");
            pManager.AddTextParameter(
                "OptionIdentifier3DSketch", "O2", "Set the 3d sketch identifier for the driver component, default 'SKELETT:1'", GH_ParamAccess.item, "SKELETT:1");
            pManager.AddTextParameter(
                "OptionIdentifierView", "O3", "Set the view identifier, default 'View1'", GH_ParamAccess.item, "View1");
            pManager.AddTextParameter(
                "OptionIdentifierLOD", "O4", "Set the LOD identifier, default 'Master'", GH_ParamAccess.item, "Master");
            pManager.AddBooleanParameter(
                "OptionExportToRFA", "O5", "Set the trigger for exporting the Assembly to Revit Family", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(
            GH_Component.GH_OutputParamManager pManager
            )
        {
            pManager.AddTextParameter(
                "Status", "S", "Present current status", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string rootPath = "";
            if (!DA.GetData(0, ref rootPath)) return;


            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_String> assemblyFile;
            if (!DA.GetDataTree(1, out assemblyFile)) return;

            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_String> label;
            if (!DA.GetDataTree(2, out label)) return;

            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo> parameterData;
            if (!DA.GetDataTree(3, out parameterData)) return;

            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo> ipropertyData;
            if (!DA.GetDataTree(4, out ipropertyData)) return;

            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Plane> plane;
            if (!DA.GetDataTree(5, out plane)) return;

            Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_GeometryGroup> points;
            if (!DA.GetDataTree(6, out points)) return;


            bool startProcess = false;
            DA.GetData(7, ref startProcess);
            if (!startProcess) return;


            string separator = "";
            if (!DA.GetData(8, ref separator)) return;

            string identifier3DSketch = "";
            if (!DA.GetData(9, ref identifier3DSketch)) return;

            string identifierView = "";
            if (!DA.GetData(10, ref identifierView)) return;

            string identifierLOD = "";
            if (!DA.GetData(11, ref identifierLOD)) return;

            bool exportRFA = false;
            DA.GetData(12, ref exportRFA);


            CustomOptions options = new CustomOptions
            {
                separator = separator,
                identifier3DSketch = identifier3DSketch,
                identifierView = identifierView,
                identifierLOD = identifierLOD,
                exportRFA = exportRFA
            };

            ProcessController mainProcess = new ProcessController()
            {
                rootPath = rootPath,
                assemblyFile = assemblyFile,
                label = label,
                parameterData = parameterData,
                ipropertyData = ipropertyData,
                plane = plane,
                points = points,
                ghOutput = DA,
                options = options
            };

            mainProcess.startProcess();


        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resources.Rhinventor2021AssemblyBuilder24;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("659b0933-5d41-4740-b280-854532fc150f"); }
        }
    }
}
