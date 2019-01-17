using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace DataManagement
{
    /// <summary>
    /// Stores the Name, State,... of all Modules
    /// </summary>
    [Serializable()]
    public class ModuleManager
    {
        /// <summary>
        /// All possible Modules. Dll name, Module name, icon name
        /// </summary>
        public  Module[] Modules { get; set; }

        /// <summary>
        /// The Function id which will be auto launched
        /// </summary>
        public int AutostartId { get; set; } = 0;
    }

    /// <summary>
    /// Represents a single Module (dll), can contain multiple Functions
    /// </summary>
    public class Module
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Module()
        {
            Dll = "N/A";
            Functions = new Func[]{new Func()};
        }
        /// <summary>
        ///  constructor
        /// </summary>
        /// <param name="d">dll name</param>
        /// <param name="aF">array of all functions an this Module</param>
        public Module(int mId, string d, Func[] aF )
        {
            ModId = mId;
            Dll = d;
            Functions = aF;

        }

        public bool IsModEnabled { get; set; } = true;

        public int ModId { get; set; }

        /// <summary>
        /// name of Dll on disk
        /// </summary>
        public string Dll { get; set; }
       
        /// <summary>
        /// Represents all Functions contained in this Module
        /// </summary>
        public Func[] Functions { get; set; }
        
    }

    /// <summary>
    /// represents one function of a Module
    /// </summary>
    public class Func
    {
        /// <summary>
        /// override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public Func()
        {
            Name = "N/A";
            IconKind = PackIconKind.ErrorOutline;
            Handler = null;
            IsEnabled = false;

        }
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="id">unique id of function</param>
        /// <param name="n">Name of func</param>
        /// <param name="ic">PackIcon of func on sidebar</param>
        /// <param name="ie">If Function is visible on sidebar</param>
        public Func(int id, string n, PackIconKind ic, bool ie = true)
        {
            ID = id;
            Name = n;
            IconKind = ic;
            Handler = null;
            IsEnabled = ie;


        }

        /// <summary>
        /// identification of module function, independent of name
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        public MaterialDesignThemes.Wpf.PackIconKind IconKind { get; set; }
        /// <summary>
        /// If Func is visible in sidebar
        /// </summary>
        public bool IsEnabled { get; set; }



        /// <summary>
        /// Handle which opens func, when activated
        /// </summary>
        [XmlIgnore]
        public RoutedEventHandler Handler { get; set; }
    }

}
