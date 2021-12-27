using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// A class that represents the logic behind a spreadsheet application.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        private Dictionary<string, Cell> OccupiedCells; // keeps track of all occupied cells in this spreadsheets.
        private DependencyGraph CellDependencies; // Keeps track of dependencies of this spreadsheet
        private bool changed; // Represents the recorded state of this spreadsheet.

        public override bool Changed { get => changed; protected set => changed = value; }

        /// <summary>
        /// Zero parameter constructor for the Spreadsheet class. 
        /// </summary>
        public Spreadsheet() : this(s => true, s => s, "default")
        {

        }
        /// <summary>
        /// Constructor for the Spreadsheet class that creates a spreadsheet from the file denoted by a passed file path directory. 
        /// </summary>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : this(isValid, normalize, version)
        {
            if (version != GetSavedVersion(filePath))
                throw new SpreadsheetReadWriteException("File version does not correspond with this spreadsheet's version");
            MakeSpreadsheetFromFIle(filePath);
        }
        /// <summary>
        /// Constructor for the Spreadsheet class. 
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            this.OccupiedCells = new Dictionary<string, Cell>();
            this.CellDependencies = new DependencyGraph();
        }
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (Object.ReferenceEquals(name, null) || !IsValidName(name) || !IsValid(Normalize(name)))
                throw new InvalidNameException();
            if (OccupiedCells.ContainsKey(Normalize(name)))
                return OccupiedCells[Normalize(name)].contents;
            return ""; // Return an empty string if attempting to access an unoccupied cell
        }
        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            if (OccupiedCells.Count > 0)
                return OccupiedCells.Keys;
            return new HashSet<string>();
        }
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            CellDependencies.ReplaceDependees(name, new HashSet<string>());
            List<string> dependents = GetCellsToRecalculate(name).ToList();
            SetCells(name, number, number,dependents);
            return dependents;
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            CellDependencies.ReplaceDependees(name, new HashSet<string>());
            List<string> dependents = GetCellsToRecalculate(name).ToList();
            if (!text.Equals(""))
                SetCells(name, text, text, dependents);
            else
            {
                OccupiedCells.Remove(name);
                RecalculateCells(dependents);
            }
            return dependents;
        }
        /// <summary>
        /// Associates a populated Cell with a name on this spreadsheet. Recalculates the cells dependent on 'name' in terms of name's new value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        private void SetCells(string name, object contents, object value,List<string> dependents)
        {
            if (this.OccupiedCells.ContainsKey(name))
              // Replace contents if name is already occupied
                OccupiedCells[name] = new Cell(contents, value);
            else
                this.OccupiedCells.Add(name, new Cell(contents, value));

            Changed = true;
            RecalculateCells(dependents);
        }
        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            List<string> dependents;
            IEnumerable<string> prevDependents = GetDirectDependents(name);
            CellDependencies.ReplaceDependees(name, formula.GetVariables()); // Replaces the previous dependees of name with the new dependees derived from formula
            try
            {
                dependents = GetCellsToRecalculate(name).ToList();
            }
            catch (CircularException e)
            {
                CellDependencies.ReplaceDependees(name, prevDependents);
                throw e;
            }
            SetCells(name, formula, formula.Evaluate(LookUp),dependents);
            return dependents;
        }
        /// <summary>
        /// Calculates all values in reference to the first name in the collection. 
        /// </summary>
        /// <param name="ValuesToCalculate"></param>
        private void RecalculateCells(List<string> ValuesToCalculate)
        {
            if (ValuesToCalculate.Count < 2) //only name is in the list, but because name has already been calculated then all cells have been recalculated.
                return;
            for (int i = 1; i < ValuesToCalculate.Count; i++)
            {
                string otherName = ValuesToCalculate[i];
                if (OccupiedCells[otherName].contents is Formula)
                {
                    var expression = (Formula)OccupiedCells[otherName].contents;
                    OccupiedCells[otherName].value = expression.Evaluate(LookUp);
                }
            }
        }
        /// <summary>
        /// Gets the value of the cell defined by name. Throws an ArgumentException if the cell defined by name is empty or if said cell's value
        /// is not a double. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private double LookUp(string name)
        {
            if (Object.ReferenceEquals(name, null) || !IsValidName(name) || !IsValid(Normalize(name)))
                throw new ArgumentException();
            if (!OccupiedCells.ContainsKey(name))
                throw new ArgumentException();
            if (OccupiedCells[name].value is double)
            {
                return (double)OccupiedCells[name].value;
            }
            throw new ArgumentException();
        }
        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        /// 
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return CellDependencies.GetDependents(name);
        }
        /// <summary>
        /// Returns true if a name follows a "correct" character-wise pattern. A name is valid if:
        /// it follows the convention of one or more letters followed by one or more digits.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsValidName(String name)
        {
            Regex varMatcher = new Regex(@"^(?:[a-zA-Z]|[_]|$)+(?:[a-zA-Z]|[0-9]|[_]|$)+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return varMatcher.IsMatch(name);
        }
        /// <summary>
        /// A class that represents a cell for the spreadsheat that encodes values and contents.  
        /// </summary>
        private class Cell
        {
            public Object value;
            public Object contents;
            /// <summary>
            /// Constructor for a cell object. A cell has contents  and a value (e.g.formula) associated with itself. The value field will be added according to Ps5's 
            /// specifications in the future.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="contents"></param>
            public Cell(Object contents, Object value)
            {
                this.value = value;
                this.contents = contents;
            }
            /// <summary>
            /// Returns the string representation of this cell's contents. If the contents of this cell is Formula, then append '=' to the front of the expression. 
            /// </summary>
            /// <returns></returns>
            public override String ToString()
            {
                if (contents is Formula)
                    return "=" + contents.ToString();
                return contents.ToString();
            }

        }

        public override string GetSavedVersion(string filename)
        {
            if (filename == null)
                throw new SpreadsheetReadWriteException("Null filepath!");
            try
            {
                using (XmlReader file = XmlReader.Create(filename))
                {
                    file.MoveToContent();
                    if(file.IsStartElement() && file.Name == "spreadsheet")
                        return file.GetAttribute(0);
                    throw new SpreadsheetReadWriteException("File does not contain spreadsheet version in appropriate location");
                }
            }
            catch (Exception e)
            {
                if (e is System.IO.FileNotFoundException)
                    throw new SpreadsheetReadWriteException("File Not Found!");
                throw new SpreadsheetReadWriteException("Invalid file!");
            }
        }
        /// <summary>
        /// Instantiates this spreadsheet with the cells of another spreadsheet using a provided xml file. 
        /// </summary>
        private void MakeSpreadsheetFromFIle(string filepath)
        {
            try
            {
                using (XmlReader file = XmlReader.Create(filepath))
                {
                    file.MoveToContent();
                    string name = null;
                    string contents = null;
                    while (file.Read())
                    {
                        if (file.IsStartElement())
                        {
                            switch (file.Name)
                            {

                                case "cell":
                                    break;

                                case "name":
                                    file.Read();
                                    name = file.Value;
                                    break;

                                case "contents":
                                    file.Read();
                                    contents = file.Value;
                                    break;
                            }
                        }
                        else // end of an element
                        {
                            if(file.Name == "cell") //If the block read defined a single cell
                                SetContentsOfCell(name, contents);
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Invalid file!");
            }
        }

        public override void Save(string filename)
        {
            if (filename == null)
                throw new SpreadsheetReadWriteException("Null filename!");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = " ";
            try
            {
                using (XmlWriter SavedSpreadsheet = XmlWriter.Create(filename, settings))
                {
                    SavedSpreadsheet.WriteStartDocument();

                    SavedSpreadsheet.WriteStartElement("spreadsheet"); // Beginning of Spreadsheet file with version information
                    SavedSpreadsheet.WriteAttributeString("version", Version);
                    foreach (string name in GetNamesOfAllNonemptyCells())
                    {
                        SavedSpreadsheet.WriteStartElement("cell"); // Beginning of a Cell

                        SavedSpreadsheet.WriteElementString("name", name); // name element
                        // contents element
                        SavedSpreadsheet.WriteElementString("contents", OccupiedCells[name].ToString());
                        SavedSpreadsheet.WriteEndElement(); // end of a cell
                    }
                    SavedSpreadsheet.WriteEndElement(); // Ends the Spreadsheet file information
                    SavedSpreadsheet.WriteEndDocument(); //Finishes writing document
                    SavedSpreadsheet.Flush();
                }
            }catch(Exception e)
            {
                if (e is System.IO.DirectoryNotFoundException)
                    throw new SpreadsheetReadWriteException("Directory not found");
                throw new SpreadsheetReadWriteException("Write File Failure");
            }
            Changed = false;
        }

        public override object GetCellValue(string name)
        {
            if (Object.ReferenceEquals(name, null) || !IsValidName(name) || !IsValid(Normalize(name)))
                throw new InvalidNameException();
            return (OccupiedCells.ContainsKey(Normalize(name))) ? OccupiedCells[Normalize(name)].value : "";
        }
       
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (Object.ReferenceEquals(content, null))
                throw new ArgumentNullException();
            if (Object.ReferenceEquals(name, null) || !IsValidName(name))
                throw new InvalidNameException();
            name = Normalize(name);
            if (!IsValid(name)) // Check validity with caller's convention
                throw new InvalidNameException();
            if (content.Length > 1 && content.StartsWith("="))
                // Begin formula after the '=' char
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            else if (Double.TryParse(content, out double number))
                return SetCellContents(name, number);
            else
                return SetCellContents(name, content);
        }
    }
}
