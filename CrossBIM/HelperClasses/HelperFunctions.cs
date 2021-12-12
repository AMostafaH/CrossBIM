using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CrossBIM.ViewModelClasses;
using CrossBIMLib;
using CrossBIMLib.ReadDXF;
using CrossBIMLib.WriteIFC.IFCGeometry;
using CrossBIMLib.WriteIFC.IFCHelper;
using netDxf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Ifc;
using Xbim.IO;

namespace CrossBIM.HelperClasses
{
    public class HelperFunctions
    {
        public static void GetIFCFile(string DXFFilePath, string IFCFilePath, ViewModelBeams viewModelBeams, ViewModelSlabs viewModelSlabs, ViewModelColumns viewModelColumns,ViewModelStoreys viewModelStorey,string projectUnits)
        {
            double conversionFactor;

            switch (projectUnits)
            {
                case "m":
                    conversionFactor = 1000;
                    break;
                case "cm":
                    conversionFactor = 10;
                    break;
                case "mm":
                    conversionFactor = 1;
                    break;
                case "inch":
                    conversionFactor = 25.4;
                    break;
                default:
                    conversionFactor = 1000;
                    break;
            }

            viewModelColumns.ColumnHeight = viewModelStorey.Height;
            viewModelColumns.ColumnLowLevel = viewModelStorey.Elevation - viewModelStorey.Height;

            for (int i = 0; i < viewModelSlabs.SlabsLayersName.Count; i++)
            {
                viewModelSlabs.SlabsTopLevel.Add(viewModelStorey.Elevation);
            }

            for (int i = 0; i < viewModelBeams.BeamsLayersName.Count; i++)
            {
                viewModelBeams.BeamsTopLevel.Add(viewModelStorey.Elevation);
            }

            HelperStorey helperStorey = ReadDXFForColumnsAndBeamsAndSlabs(DXFFilePath, viewModelBeams, viewModelSlabs, viewModelColumns);

            var increamentInLevel = viewModelStorey.Height;
            int j = 0;

            for (int i = 0; i <= viewModelStorey.NumOfRepetitions; i++)
            {
                if (j > 1) j = 1;

                //Attributes will change
                foreach (var beam in helperStorey.BeamsList)
                {
                    beam.TopLevel += (increamentInLevel * j);
                }

                foreach (var slab in helperStorey.AllSlabsList)
                {
                    slab.TopLevel += (increamentInLevel * j);
                }

                foreach (var drop in helperStorey.AllDropsList)
                {
                    drop.TopLevel += (increamentInLevel * j);
                }

                foreach (var column in helperStorey.ColumnsList)
                {
                    column.LowLevel +=  (increamentInLevel * j);
                }

                viewModelStorey.Elevation += (increamentInLevel * j);
                j++;

                if (!(System.IO.File.Exists(IFCFilePath)))
                {
                    IfcStore model = IFCModel.Create("CrossBIM Strikers");
                    model = IFCProject.Create(model, "CrossBIM Strikers");
                    IFCSite.Create(model, "New Site");
                    IFCBuilding.Create(model, "New Building");

                    WriteIFC(model, helperStorey.BeamsList, helperStorey.ColumnsList, helperStorey.AllSlabsList, helperStorey.AllDropsList, viewModelStorey.Elevation, conversionFactor);

                    //write the Ifc File with ifc extension
                    model.SaveAs(IFCFilePath, StorageType.Ifc);

                    var FilePathXml = IFCFilePath.Replace(".ifc", ".ifcxml");
                    //write the Ifc File with ifcxml extension
                    model.SaveAs(FilePathXml, StorageType.IfcXml);
                }
                else
                {
                    IfcStore Model = IfcStore.Create(Xbim.Common.Step21.XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);

                    using (var stremReader = new StreamReader(IFCFilePath))
                    {
                        Stream stream = stremReader.BaseStream;
                        Model = IfcStore.Open(stream, StorageType.Ifc, Xbim.Common.Step21.XbimSchemaVersion.Ifc4, XbimModelType.MemoryModel, null, XbimDBAccess.ReadWrite, null, -1);

                        stremReader.Close();
                        stream.Close();

                    }

                    WriteIFC(Model, helperStorey.BeamsList, helperStorey.ColumnsList, helperStorey.AllSlabsList, helperStorey.AllDropsList, viewModelStorey.Elevation, conversionFactor);

                    //write the Ifc File with ifc extension
                    Model.SaveAs(IFCFilePath, StorageType.Ifc);

                    var FilePathXml = IFCFilePath.Replace(".ifc", ".ifcxml");
                    //write the Ifc File with ifcxml extension
                    Model.SaveAs(FilePathXml, StorageType.IfcXml);
                }
            }
        }

        public static void WriteIFC(IfcStore model, List<DXFBeam> beamsList, List<DXFColumn> columnsList, List<DXFSlab> AllSlabsList, List<DXFSlab> AllDropsList,double Elevation, double conversionFactor)
        {

            var numOfStoreys = model.Instances.OfType<Xbim.Ifc4.ProductExtension.IfcBuildingStorey>().Count() + 1;

            var storeyName = "Storey " + numOfStoreys + "";

            IFCStorey.Create(model, storeyName, Elevation * conversionFactor);

            #region Columns

            foreach (var column in columnsList)
            {
                IFCColumn.Create(model, storeyName, column.XDimension * conversionFactor, column.YDimension * conversionFactor,
                column.Height * conversionFactor, column.Center.X * conversionFactor, column.Center.Y * conversionFactor, column.LowLevel * conversionFactor);
            }

            #endregion

            #region Beams

            foreach (var beamCategory in beamsList) //Each Story data   
            {
                for (int j = 0; j < beamCategory.CenterLines.Count; j++) //each beam
                {
                    var currentBeamAxis = beamCategory.CenterLines.ElementAt(j);
                    var beamCentroidX = ((currentBeamAxis.StartPoint.X
                                        + currentBeamAxis.EndPoint.X) / 2) * conversionFactor;

                    var beamCentroidY = ((currentBeamAxis.StartPoint.Y
                                        + currentBeamAxis.EndPoint.Y) / 2) * conversionFactor;

                    IFCBeam.Create(model, storeyName, new netDxf.Vector2(beamCentroidX, beamCentroidY),
                        IFCUtilties.Normalize(currentBeamAxis.Direction), beamCategory.TopLevel * conversionFactor,
                        beamCategory.Length[j] * conversionFactor, beamCategory.Width * conversionFactor, beamCategory.Depth * conversionFactor
                        );
                }

            }

            #endregion

            #region Slabs

            for (int i = 0; i < AllSlabsList.Count; i++)
            {
                IFCSlab.Create(model, storeyName, AllSlabsList[i].TopLevel * conversionFactor, AllSlabsList[i].Thickness * conversionFactor, AllSlabsList[i].Vertices,
                    AllSlabsList[i].Openings, conversionFactor);
            }

            for (int i = 0; i < AllDropsList.Count; i++)
            {
                IFCSlab.Create(model, storeyName, AllDropsList[i].TopLevel * conversionFactor, AllDropsList[i].Thickness * conversionFactor,
                    AllDropsList[i].Vertices, AllDropsList[i].Openings, conversionFactor);
            }

            #endregion
 
        }

        public static void IFCForIsolatedFooting(string DXFFilePath, string IFCFilePath, ViewModelIsoFooting viewModelIsoFooting,string projectUnits)
        {
            double conversionFactor;

            switch (projectUnits)
            {
                case "m":
                    conversionFactor = 1000;
                    break;
                case "cm":
                    conversionFactor = 10;
                    break;
                case "mm":
                    conversionFactor = 1;
                    break;
                case "inch":
                    conversionFactor = 25.4;
                    break;
                default:
                    conversionFactor = 1000;
                    break;
            }

            #region Foundation

            viewModelIsoFooting.BaseColumnLowLevel = viewModelIsoFooting.IsoFootingTopLevel.Min();

            viewModelIsoFooting.BaseColumnHeight = viewModelIsoFooting.Elevation - viewModelIsoFooting.BaseColumnLowLevel;

            // ========================= Create IFC RC Foundation =================//

            List<DXFIsolatedFooting> RCFootings = new List<DXFIsolatedFooting>();
            List<DXFColumn> foundationColumns = new List<DXFColumn>();
            List<DXFBeam> foundationStrips = new List<DXFBeam>();

            for (int j = 0; j < viewModelIsoFooting.FootingNumberOfCategories; j++)
            {
                var maxStripWidth = viewModelIsoFooting.StripsDepth.Max();

                RCFootings.AddRange(DXFIsolatedFooting.Read(DXFFilePath,
                                    viewModelIsoFooting.IsoFootingLayersName[j],
                                    viewModelIsoFooting.IsoFootingDepth[j], maxStripWidth,
                                    viewModelIsoFooting.IsoFootingTopLevel[j]));
            }

            foundationColumns = DXFColumn.Read(DXFFilePath, viewModelIsoFooting.BaseColumnLayerName,
                viewModelIsoFooting.BaseColumnLowLevel, viewModelIsoFooting.BaseColumnHeight);

            foundationStrips = DXFBeam.Read(DXFFilePath, viewModelIsoFooting.StripsLayerName,
                viewModelIsoFooting.StripsDepth, viewModelIsoFooting.StripsTopLevel, viewModelIsoFooting.StripsWidth);

            IfcStore model = IFCModel.Create("CrossBIM Strikers");
            model = IFCProject.Create(model, "CrossBIM Strikers");
            IFCSite.Create(model, "New Site");
            IFCBuilding.Create(model, "New Building");

            var storeyName = "Storey " + 1 + "";

            IFCStorey.Create(model, storeyName, viewModelIsoFooting.Elevation * conversionFactor);

            foreach (var RC in RCFootings)
            {
                IFCIsolatedFooting.Create(model, storeyName, RC.XDimension * conversionFactor, RC.YDimension * conversionFactor, RC.Depth * conversionFactor
                , RC.TopLevel * conversionFactor, new Vector3(RC.Center.X * conversionFactor, RC.Center.Y * conversionFactor, RC.TopLevel * conversionFactor));

                //PC
                var PC_XDIm = RC.XDimension + viewModelIsoFooting.PCOffset;
                var PC_YDIm = RC.YDimension + viewModelIsoFooting.PCOffset;
                var PC_TopLevel = RC.TopLevel - RC.Depth;

                IFCIsolatedFooting.Create(model, storeyName, PC_XDIm * conversionFactor, PC_YDIm * conversionFactor, viewModelIsoFooting.PCDepth * conversionFactor
               , PC_TopLevel * conversionFactor, new Vector3(RC.Center.X * conversionFactor, RC.Center.Y * conversionFactor, PC_TopLevel * conversionFactor));

            }

            foreach (var Column in foundationColumns)
            {
                IFCColumn.Create(model, storeyName, Column.XDimension * conversionFactor, Column.YDimension * conversionFactor, Column.Height * conversionFactor,
                    Column.Center.X * conversionFactor, Column.Center.Y * conversionFactor, Column.LowLevel * conversionFactor);
            }

            foreach (var strip in foundationStrips)
            {
                for (int j = 0; j < strip.CenterLines.Count; j++) 
                {
                    var currentStripAxis = strip.CenterLines.ElementAt(j);
                    var stripCentroidX = ((currentStripAxis.StartPoint.X
                                        + currentStripAxis.EndPoint.X) / 2) * conversionFactor;

                    var stripCentroidY = ((currentStripAxis.StartPoint.Y
                                       + currentStripAxis.EndPoint.Y) / 2) * conversionFactor;

                    IFCBeam.Create(model, storeyName, new netDxf.Vector2(stripCentroidX, stripCentroidY),
                        IFCUtilties.Normalize(currentStripAxis.Direction), strip.TopLevel * conversionFactor,
                        strip.Length[j] * conversionFactor, strip.Width * conversionFactor, strip.Depth * conversionFactor
                        );
                }
            }

            model.SaveAs(IFCFilePath, StorageType.Ifc);

            #endregion
        }
    
        public static HelperStorey ReadDXFForColumnsAndBeamsAndSlabs(string DXFFilePath, ViewModelBeams viewModelBeams, ViewModelSlabs viewModelSlabs, ViewModelColumns viewModelColumns)
        {

            HelperStorey helperStorey = new HelperStorey();

            #region Beams

            helperStorey.BeamsList.AddRange(DXFBeam.Read(DXFFilePath,
                viewModelBeams.BeamsLayersName, viewModelBeams.BeamsDepth,
                viewModelBeams.BeamsTopLevel, viewModelBeams.BeamsWidth));

            #endregion

            #region Slabs

            List<DXFSlab> slabsList = new List<DXFSlab>();
            slabsList.AddRange(DXFSlab.Read(DXFFilePath, viewModelSlabs.SlabsLayersName,
                viewModelSlabs.Slabsthickness, viewModelSlabs.SlabsTopLevel));

            var openingPolygons = DXFOpening.GetOpenings(DXFFilePath, viewModelSlabs.OpeningLayerName);
            var dropPolygons = DXFOpening.GetOpenings(DXFFilePath, viewModelSlabs.DropLayerName);

            // Assign opening to its floor if the floor contains it.
            slabsList = slabsList.Select(eachSlab => DXFSlab.SetOpenings(eachSlab, openingPolygons)).ToList();
            slabsList = slabsList.Select(eachSlab => DXFSlab.SetOpenings(eachSlab, dropPolygons)).ToList();

            //Check which slab inside other 
            //List<LwPolyline> slabsAsPolyline = new List<LwPolyline>();
            //foreach (var dxfSlab in slabsList)
            //{
            //    slabsAsPolyline.Add(new LwPolyline(dxfSlab.Vertices));
            //}
            //slabsList = slabsList.Select(eachSlab => DXFSlab.SetOpenings(eachSlab, slabsAsPolyline)).ToList();


            //Drops as slab //we assumed that it has no opening till now

            var names = new List<string>();
            names.Add(viewModelSlabs.DropLayerName);
            var thicknesses = new List<double>();
            thicknesses.Add(viewModelSlabs.DropThickness);
            var levels = new List<double>();
            levels.Add(viewModelSlabs.DropTopLevel);

            List<DXFSlab> drops = DXFSlab.Read(DXFFilePath, names, thicknesses, levels);

            //AllSlabsList ====> eachStory ==> List of Slabs -Each story in list-  ===> Listof Slabs categories example .24 m thickness ==>  List of instances 
            //                             |                                       |===> Listof Slabs categories example .22 m thickness |==> List of instances 

            helperStorey.AllSlabsList.AddRange(slabsList);
            helperStorey.AllDropsList.AddRange(drops);

            #endregion

            #region Columns

            helperStorey.ColumnsList.AddRange(DXFColumn.Read(DXFFilePath, viewModelColumns.ColumnLayerName,
                    viewModelColumns.ColumnLowLevel, viewModelColumns.ColumnHeight));

            #endregion

            return helperStorey;
        }

        public static void DeleteCloudibaryFilesAfterOneHour()
        {
            /*
            Account account = new Account(
                  "Cloud",//user name
                  "APIKey",
                  "APISecert");
            */

            Account account = _Cloudinary.GetCloudinaryAccount();
            Cloudinary cloudinary = new Cloudinary(account);

            var listResourcesParams = new ListResourcesParams()
            {
                ResourceType = ResourceType.Raw,
            };

            var listResourcesResult = cloudinary.ListResources(listResourcesParams);

            var timeNow = DateTime.UtcNow;

            DelResParams delResParams = new DelResParams();
            delResParams.ResourceType = ResourceType.Raw;

            foreach (var resource in listResourcesResult.Resources)
            {
                var time = resource.CreatedAt;
                TimeSpan interval = default(TimeSpan);

                try
                {
                    var month = int.Parse(time.Substring(0, 2));
                    var day = int.Parse(time.Substring(3, 2));
                    var year = int.Parse(time.Substring(6, 4));

                    var hour = int.Parse(time.Substring(11, 2));
                    var minute = int.Parse(time.Substring(14, 2));
                    var seconds = int.Parse(time.Substring(17, 2));

                    DateTime createdTime = new DateTime(year, month, day, hour, minute, seconds);

                    interval = timeNow - createdTime;
                    var totalMinutes = Math.Round(interval.TotalMinutes, 0);

                    if (totalMinutes > 60)
                    {
                        delResParams.PublicIds.Add(resource.PublicId);
                    }

                }
                catch
                {

                }
            }

            if (delResParams.PublicIds.Count > 0)
                cloudinary.DeleteResources(delResParams);
        }
    }
}
