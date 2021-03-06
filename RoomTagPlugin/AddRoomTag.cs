//  Плагин автоматической нумерации помещений в модели
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

            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            using (Transaction tran = new Transaction(doc))
            {
                int ln = 0;
                int rn = 0;

                tran.Start("tran1");
                foreach (Level level in levels)
                {
                    ln += 1;
                    PlanTopology topology = doc.get_PlanTopology(level);
                    PlanCircuitSet circuitSet = topology.Circuits;
                    foreach (PlanCircuit circuit in circuitSet)
                    {
                        if (!circuit.IsRoomLocated)
                        {
                            rn += 1;
                            Room room = doc.Create.NewRoom(null, circuit);
                            room.Number = (ln*100 + rn).ToString();
                        }
                    }
                    rn = 0;
                }
                tran.Commit();
            }

            List<Room> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfType<Room>()
                .ToList();

            //int i = 0;
            //Transaction ts = new Transaction(doc, "Room Number Transaction");
            //ts.Start();
            string names = String.Empty;
            foreach (var room in rooms)
            {
                //i += 1;
                //room.Number = i.ToString();
                names += $"\"{room.Number}\" - \"{room.Name}\"{Environment.NewLine}";
            }
            //ts.Commit();

            TaskDialog.Show($"{rooms.Count} rooms:", names);
            return Result.Succeeded;
        }
    }
}
