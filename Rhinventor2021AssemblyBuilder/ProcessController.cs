using System;
using System.Collections.Generic;
using System.Linq;
using Gh_Data = Grasshopper.Kernel.Data;
using Gh_Types = Grasshopper.Kernel.Types;
using System.IO;
using Inventor;

namespace Rhinventor2021AssemblyBuilder
{
    class ProcessController
    {
        public string rootPath { get; set; }

        public Gh_Data.GH_Structure<Gh_Types.GH_String> assemblyFile { get; set; }
        public Gh_Data.GH_Structure<Gh_Types.GH_String> label { get; set; }
        public Gh_Data.GH_Structure<Gh_Types.IGH_Goo> parameterData { get; set; }
        public Gh_Data.GH_Structure<Gh_Types.IGH_Goo> ipropertyData { get; set; }
        public Gh_Data.GH_Structure<Gh_Types.GH_Plane> plane { get; set; }
        public Gh_Data.GH_Structure<Gh_Types.GH_GeometryGroup> points { get; set; }

        public Grasshopper.Kernel.IGH_DataAccess ghOutput { get; set; }

        public CustomOptions options { get; set; }

        public void startProcess()
        {
            CustomApprenticeServer apprenticeServer = new CustomApprenticeServer(options);
            InventorAssemblySession inventorAssemblySession = new InventorAssemblySession(options);
            try
            {
                List<ChildComponent> components = createComponentInstances();
                components.ForEach(component => apprenticeServer.build(component, rootPath));
                apprenticeServer.close();

                inventorAssemblySession.openSession();
                components.ForEach(component =>
                {
                    AssemblyDocument assemblyDocument = inventorAssemblySession.openAssemblyDocument(component.compAssemblyFileTarget);
                    inventorAssemblySession.transformComponent(assemblyDocument, component);
                    inventorAssemblySession.saveDocument(assemblyDocument);
                    if (options.exportRFA)
                    {
                        string folder = System.IO.Path.GetDirectoryName(component.compAssemblyFileTarget);
                        string filename = System.IO.Path.GetFileNameWithoutExtension(component.compAssemblyFileTarget);
                        inventorAssemblySession.exportToRevitFamily(assemblyDocument, $"{folder}\\{filename}");
                        inventorAssemblySession.saveDocument(assemblyDocument);
                    }
                    inventorAssemblySession.closeDocument(assemblyDocument);
                });

                inventorAssemblySession.closeSession();
                ghOutput.SetData(0, "Building process successfully finished");
            }
            catch (Exception error)
            {
                ghOutput.SetData(0, error);
                apprenticeServer.close();
                inventorAssemblySession.closeSession();
            }
        }

        public List<ChildComponent> createComponentInstances()
        {
            List<ChildComponent> components = new List<ChildComponent>();

            if (!validateRootPath()) throw new FileNotFoundException("Given root folder not found");
            if (!validateComponentStructure()) throw new ArgumentException("Wrong tree structure in child tree");
            if (!validateComponentFiles()) throw new FileNotFoundException("Given child assembly file not exist");

            for (int i = 0; i < label.Branches.Count; i++)
            {
                components.Add(new ChildComponent()
                    {
                        compAssemblyFileSource = assemblyFile.Branches[i].First().ToString(),
                        compLabel = label.Branches[i].First().ToString(),
                        compPlane = plane.Branches[i].First().Value,
                        compRefPoints = points.Branches[i].First().Objects.Select(point =>
                            {
                                Rhino.Geometry.GeometryBase p = Grasshopper.Kernel.GH_Convert.ToGeometryBase(point);
                                return (Rhino.Geometry.Point)p;
                            }).ToList(),
                        parameterData = parameterData.Branches[i].Select(x =>
                            {
                                Dictionary<string, string> result;
                                x.CastTo<Dictionary<string, string>>(out result);
                                return result;
                            }).ToList().First(),
                        ipropertyData = ipropertyData.Branches[i].Select(x =>
                            {
                                List<Dictionary<string, object>> result;
                                x.CastTo<List<Dictionary<string, object>>>(out result);
                                return result;
                            }).ToList().First(),

                    }
                );
            }

            return components;
        }

        public bool validateComponentStructure()
        {
            HashSet<int> branchQuantity = new HashSet<int>()
            {
                assemblyFile.Branches.Count,
                label.Branches.Count,
                parameterData.Branches.Count,
                ipropertyData.Branches.Count,
                plane.Branches.Count,
                points.Branches.Count
            };

            if (branchQuantity.Count > 1) return false;


            for (int i = 0; i < label.Branches.Count; i++)
            {
                HashSet<int> branchLength = new HashSet<int>(){
                    assemblyFile.Branches[i].Count,
                    label.Branches[i].Count,
                    parameterData.Branches[i].Count,
                    ipropertyData.Branches[i].Count,
                    plane.Branches[i].Count,
                    points.Branches[i].Count};

                if (branchLength.Count > 1) return false;

            }

            return true;
        }

        public bool validateRootPath()
        {
            if (!System.IO.Directory.Exists(rootPath))
            {
                return false;
            }
            return true;
        }

        public bool validateComponentFiles()
        {
            HashSet<string> filePaths = new HashSet<string>();

            foreach (var branch in assemblyFile.Branches)
            {
                foreach (var filepath in branch.ToList())
                {
                    filePaths.Add(filepath.Value);
                }
            }

            foreach (string filepath in filePaths)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    return false;
                }
            }

            return true;
        }

    }
}

