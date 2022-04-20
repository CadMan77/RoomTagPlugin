﻿//  Плагин автоматической нумерации помещений в модели
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomTagPlugin
{
    [TransactionAttribute (TransactionMode.Manual)]
    public class AddRoomTag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            string names = String.Empty;

            //List<ViewPlan> projectFloorPlans = new FilteredElementCollector(doc)
            //    .OfClass(typeof(ViewPlan))
            //    .OfType<ViewPlan>()
            //    .Where(x=>!x.IsTemplate)
            //    .Where(x=>x.ViewType == ViewType.FloorPlan)
            //    .ToList();

            //List<Level> levels = new FilteredElementCollector(doc)
            //    .OfClass(typeof(Level))
            //    .OfType<Level>()
            //    .ToList();

            List<Room> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfType<Room>()
                .ToList();

            int i = 0;
            Transaction ts = new Transaction(doc, "Room Number Transaction");
            ts.Start();
            foreach (var room in rooms)
            {
                i += 1;
                //names += item.Name + Environment.NewLine;
                room.Number = i.ToString();
                names += $"\"{room.Number}\" - \"{room.Name}\"{Environment.NewLine}";
            }
            ts.Commit();

            TaskDialog.Show($"{rooms.Count} rooms:", names);
            return Result.Succeeded;
        }
    }
}
