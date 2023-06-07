#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace ToggleLeader
{
    [Transaction(TransactionMode.Manual)]
    public class ToggleLeader : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            //Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Get the currently selected element
                ElementId selectedElementId = uiDoc.Selection.GetElementIds().FirstOrDefault();
                if (selectedElementId == null)
                {
                    TaskDialog.Show("Error", "Please select a room tag.");
                    return Result.Cancelled;
                }

                Element selectedElement = doc.GetElement(selectedElementId);
                RoomTag selectedTag = selectedElement as RoomTag;

                if (selectedTag == null)
                {
                    TaskDialog.Show("Error", "The selected element is not a room tag.");
                    return Result.Cancelled;
                }

                // Toggle the Leader parameter
                using (Transaction tx = new Transaction(doc, "Toggle Room Tag Leader"))
                {
                    tx.Start();

                    bool leaderVisible = selectedTag.HasLeader;
                    selectedTag.HasLeader = !leaderVisible;

                    tx.Commit();
                }

               // TaskDialog.Show("Success", "Room tag leader toggled successfully.");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        
        
        // Selection filter to filter out non-room tags
        public class RoomTagSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return element is RoomTag;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
