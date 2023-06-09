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
            //UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            //Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Get the currently selected room tags
                IList<ElementId> selectedElementIds = (IList<ElementId>)uiDoc.Selection.GetElementIds();
                if (selectedElementIds.Count == 0)
                {
                    TaskDialog.Show("Error", "Please select one or more room tags.");
                    return Result.Cancelled;
                }

                using (TransactionGroup txGroup = new TransactionGroup(doc, "Toggle Room Tag Leader"))
                {
                    txGroup.Start();

                    foreach (ElementId selectedElementId in selectedElementIds)
                    {
                        Element selectedElement = doc.GetElement(selectedElementId);
                        RoomTag selectedTag = selectedElement as RoomTag;

                        if (selectedTag != null)
                        {
                            // Toggle the Leader parameter for each selected room tag
                                bool leaderVisible = selectedTag.HasLeader;
                                selectedTag.HasLeader = !leaderVisible;
                        }
                    }

                    txGroup.Assimilate();
                }

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
