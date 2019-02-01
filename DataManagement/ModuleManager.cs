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

        /// <summary>
        /// The Version of the Save, for detecting new Modules
        /// </summary>
        public int Version { get; set; } = 0;
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
            HideDisableFunction = false;
            IsModEnabled = true;
        }
        /// <summary>
        ///  constructor
        /// </summary>
        /// <param name="mId">ID of this Module, must be unique</param>
        /// <param name="n">Displayed Name of Module</param>
        /// <param name="d">dll name</param>
        /// <param name="aF">array of all functions an this Module</param>
        /// <param name="ie">Is Module Enabled</param>
        /// <param name="hide">Hides Module from Lists to Disable/Enable this Module. Is still visible in sidebar</param>
        public Module(int mId ,string n,  string d, Func[] aF,bool ie = true, bool hide = false )
        {
            ModId = mId;
            Name = n;
            Dll = d;
            Functions = aF;
            IsModEnabled = ie;
            HideDisableFunction = hide;

        }

        /// <summary>
        /// will hide the module from dropdown menue in Settings
        /// </summary>
        public bool HideDisableFunction { get; set; }

        /// <summary>
        /// Disable the entire Module
        /// </summary>
        public bool IsModEnabled { get; set; }

        /// <summary>
        /// unique id for module
        /// </summary>
        public int ModId { get; set; }


        /// <summary>
        /// Name of the Module
        /// </summary>
        public string Name { get; set; }

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
        /// identification of module function, independent of name.
        /// Needs to be same as in assignHandleToModules
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
