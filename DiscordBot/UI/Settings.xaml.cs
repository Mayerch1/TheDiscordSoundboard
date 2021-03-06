﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Util.IO;

namespace DiscordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl, INotifyPropertyChanged
    {
        private bool _sliderMutex = false;

        public delegate void RefreshModulesHandle();

        public RefreshModulesHandle RefreshModules;

        public delegate void OpenTutorialHandle();

        public OpenTutorialHandle OpenTutorial;


        private ObservableCollection<Swatch> primarySwatches;

        public ObservableCollection<Swatch> PrimarySwatches
        {
            get => primarySwatches;
            set
            {
                primarySwatches = value;
                OnPropertyChanged("PrimarySwatches");
            }
        }

        private ObservableCollection<Swatch> secondarySwatches;

        public ObservableCollection<Swatch> SecondarySwatches
        {
            get => secondarySwatches;
            set
            {
                secondarySwatches = value;
                OnPropertyChanged("SecondarySwatches");
            }
        }

        /// <summary>
        /// constructor for Settings class, sets Datacontext
        /// </summary>
        public Settings()
        {

            //get primary/secondary colors and sort both by sRGB values
            PrimarySwatches =
                new ObservableCollection<Swatch>(
                    new SwatchesProvider().Swatches.OrderBy(cP => cP.ExemplarHue.Color.ToString()));
            SecondarySwatches =
                new ObservableCollection<Swatch>(
                    (new SwatchesProvider().Swatches).Where(x => x.AccentExemplarHue != null)
                    .OrderBy(cS => cS.AccentExemplarHue.Color.ToString()));

            _sliderMutex = true;
            InitializeComponent();
            _sliderMutex = false;

            this.DataContext = Handle.Data.Persistent;

            updateStartupCombo();
            updateModuleSelector();
        }

        /// <summary>
        /// eventhandler for changed text in the bot-token box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void box_token_TextChanged(object sender, TextChangedEventArgs e)
        {
            Handle.Token = ((TextBox) sender).Text;
        }

        private void box_clientName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                Handle.ClientName = box.Text;
            }

        }

        private void btn_Help_Application_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#Application");
        }

        private void btn_Help_Files_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#Files");
        }

        private void btn_Help_Appearance_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#Appearance");
        }

        private void btn_Help_Modules_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#Modules");
        }

        private void btn_Help_Preferences_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#Preferences");
        }

        private void btn_Help_Setup_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#Setup");
        }

        private void btn_OpenTutorial_Click(object sender, RoutedEventArgs e)
        {
            OpenTutorial?.Invoke();
        }

        private void openHelpPage(string page)
        {
            System.Diagnostics.Process.Start(DataManagement.PersistentData.gitCompleteUrl + "wiki/" + page);
        }


        private void btn_PrimarySwatch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Tag != null)
                {
                    Handle.Data.Persistent.PrimarySwatch = btn.Tag.ToString();
                }
            }
        }


        private void btn_AccentSwatch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Tag != null)
                {
                    Handle.Data.Persistent.SecondarySwatch = btn.Tag.ToString();
                }
            }
        }

        private void dialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //DO NOT REMOVE

            //this is needed, to call CloseDialogCommand.Execute(null, null) from code
            Console.WriteLine("SAMPLE 1: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));

            //you can cancel the dialog close:
            //eventArgs.Cancel();

            if (!Equals(eventArgs.Parameter, true)) return;
        }

        

        private void btn_addMediaSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                //show dialog to add a new media-source
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    //add selected source to list
                    if (!Handle.Data.Persistent.MediaSources.Contains(dialog.SelectedPath))
                        Handle.Data.Persistent.MediaSources.Add(dialog.SelectedPath);

                    var scanCollection = new System.Collections.ObjectModel.ObservableCollection<string>();
                    scanCollection.Add(dialog.SelectedPath);

                    //rescan added files
                    Util.IO.FileWatcher.indexFiles(scanCollection, false);
                }
            }
        }

        private void btn_deleteMediaSource_Click(object sender, RoutedEventArgs e)
        {
            if (list_MediaSources.SelectedItems.Count > 0)
            {
                string sPath = list_MediaSources.SelectedItem.ToString();

                int index = Handle.Data.Persistent.MediaSources.IndexOf(sPath);

                if (index >= 0)
                {
                    Handle.Data.Persistent.MediaSources.RemoveAt(index);

                    //delete/recsan all files
                    Util.IO.FileWatcher.indexCleanFiles(Handle.Data.Persistent.MediaSources);
                }
            }
        }

        private void btn_deleteSupportedFormat_Click(object sender, RoutedEventArgs e)
        {
            if (list_SupportedFormats.SelectedItems.Count > 0)
            {
                string sPath = list_SupportedFormats.SelectedItem.ToString();

                int index = Handle.Data.Persistent.SupportedFormats.IndexOf(sPath);

                if (index >= 0)
                {
                    Handle.Data.Persistent.SupportedFormats.RemoveAt(index);
                }
            }
        }

        private void btn_SupportedFormatAdded_Click(object sender, RoutedEventArgs e)
        {
            var extensions = box_supportedFile.Text;

            extensions = extensions.Replace(" ", "");

            var extArr = extensions.Split(';');

            foreach (var element in extArr)
            {
                if (!String.IsNullOrWhiteSpace(element) && !Handle.Data.Persistent.SupportedFormats.Contains(element))
                    Handle.Data.Persistent.SupportedFormats.Add(element);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void Settings_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!dialogHost_Primary.IsMouseOver && dialogHost_Primary is DialogHost pHost)
            {
                DialogHost.CloseDialogCommand.Execute(null, pHost);

            }
            if (!dialogHost_Accent.IsMouseOver && dialogHost_Accent is DialogHost aHost)
            {
                DialogHost.CloseDialogCommand.Execute(null, aHost);

            }
            if (!dialogHost_SupportedFormat.IsMouseOver && dialogHost_SupportedFormat is DialogHost sHost)
            {
                DialogHost.CloseDialogCommand.Execute(null, sHost);

            }

            e.Handled = false;
        }


        private void Settings_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //intercept ScrollViewer of Main Scroller
            //prevent lists from capturing mouse wheel
            ScrollViewer scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 5);
            e.Handled = true;
        }

        private void module_checkForPresent()
        {
            foreach (var Module in Handle.Data.ModuleStates.Modules)
            {
                if (!String.IsNullOrEmpty(Module.Dll) && !File.Exists(Module.Dll))
                    SnackbarManager.SnackbarMessage("Could not find " + Module.Dll);
            }

            RefreshModules?.Invoke();
            updateStartupCombo();
        }

        private void updateStartupCombo()
        {
            combo_startup.Items.Clear();

            foreach (var Module in Handle.Data.ModuleStates.Modules)
            {
                //only display activated module in startup selection
                if (Module.IsModEnabled)
                {
                    foreach (var func in Module.Functions)
                    {
                        //select the old startup
                        if (func.IsEnabled)
                        {
                            combo_startup.Items.Add(func);
                            if (func.ID == Handle.Data.ModuleStates.AutostartId)
                                combo_startup.SelectedItem = func;
                        }
                    }
                }
            }
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox box && box.SelectedItem != null)
            {
                if (box.SelectedItem is DataManagement.Func fc)
                    Handle.Data.ModuleStates.AutostartId = fc.ID;
            }
        }

        private void updateModuleSelector()
        {
            foreach (var Module in Handle.Data.ModuleStates.Modules)
            {
                //create new checkbox for disabling modules
                if (!Module.HideDisableFunction)
                {
                    CheckBox box = new CheckBox();

                    box.Click += check_Module_Clicked;


                    box.Content = Module.Name;


                    box.Tag = Module.ModId;
                    box.IsChecked = Module.IsModEnabled;

                    Thickness margin = new Thickness();
                    margin.Top = 10;
                    box.Margin = margin;

                    Stack_ModuleSelector.Children.Add(box);
                }
            }
        }

        private void check_Module_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox box)
            {
                //search for changed module
                if (box.Tag is int tag)
                {
                    foreach (var Module in Handle.Data.ModuleStates.Modules)
                    {
                        if (tag == Module.ModId)
                        {
                            if (box.IsChecked != null)
                            {
                                //change enabled mode of module
                                Module.IsModEnabled = box.IsChecked.Value;
                                //iterate through every function of module
                                foreach (var func in Module.Functions)
                                {
                                    func.IsEnabled = Module.IsModEnabled;
                                }

                                break;
                            }
                        }
                    }
                }

                module_checkForPresent();
            }
        }

        private void slider_MinVisBtn_Reset(object sender, MouseButtonEventArgs e)
        {
            Handle.Data.Persistent.MinVisibleButtons = DataManagement.PersistentData.defaultMinVisBtn;
            if (sender is TextBlock box && box.Parent is StackPanel panel)
            {
                if (panel.Children.Count >= 2 && panel.Children[1] is Slider sl)
                {
                    sl.Value = Handle.Data.Persistent.MinVisibleButtons;
                }
            }
        }

        private void slider_BtnHeight_Reset(object sender, MouseButtonEventArgs e)
        {
            Handle.Data.Persistent.BtnHeight = DataManagement.PersistentData.defaultBtnHeight;
            if (sender is TextBlock box && box.Parent is StackPanel panel)
            {
                if (panel.Children.Count >= 2 && panel.Children[1] is Slider sl)
                {
                    sl.Value = Handle.Data.Persistent.BtnHeight;
                }
            }
        }

        private void slider_BtnWidth_Reset(object sender, MouseButtonEventArgs e)
        {
            Handle.Data.Persistent.BtnWidth = DataManagement.PersistentData.defaultBtnWidth;
            if (sender is TextBlock box && box.Parent is StackPanel panel)
            {
                if (panel.Children.Count >= 2 && panel.Children[1] is Slider sl)
                {
                    sl.Value = Handle.Data.Persistent.BtnWidth;
                }
            }
        }

        private void slider_VidHistoryLen_Reset(object sender, MouseButtonEventArgs e)
        {
            Handle.Data.Persistent.MaxVideoHistoryLen = DataManagement.PersistentData.defaultMaxVidHistoryLen;
            if (sender is TextBlock box && box.Parent is StackPanel panel)
            {
                if (panel.Children.Count >= 2 && panel.Children[1] is Slider sl)
                {
                    sl.Value = Handle.Data.Persistent.MaxVideoHistoryLen;
                }
            }
        }

        private void slider_HistoryLen_Reset(object sender, MouseButtonEventArgs e)
        {
            Handle.Data.Persistent.MaxHistoryLen = DataManagement.PersistentData.defaultMaxHistoryLen;
            if (sender is TextBlock box && box.Parent is StackPanel panel)
            {
                if (panel.Children.Count >= 2 && panel.Children[1] is Slider sl)
                {
                    sl.Value = Handle.Data.Persistent.MaxHistoryLen;
                }
            }
        }

        private void slider_BtnHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_sliderMutex)
            {
                _sliderMutex = true;


                //incr.button width (fixed ratio)
                if (Handle.Data.Persistent.IsFixedBtnRatio)
                {
                    var ratio = Handle.Data.Persistent.BtnWidth / Handle.Data.Persistent.BtnHeight;
                    var diff = e.NewValue - e.OldValue;

                    slider_BtnWidth.Value += Math.Round(diff * ratio, 0);
                    Console.WriteLine(diff + "/" + Math.Round(diff * ratio, 0));
                }
                _sliderMutex = false;
            }
        }

        private void slider_BtnWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_sliderMutex)
            {
                _sliderMutex = true;

      

                //incr button height (fixed ratio)
                if (Handle.Data.Persistent.IsFixedBtnRatio)
                {
                    var ratio = Handle.Data.Persistent.BtnHeight / Handle.Data.Persistent.BtnWidth;
                    var diff = e.NewValue - e.OldValue;

                    slider_BtnHeight.Value += Math.Round(diff * ratio, 0);
                    Console.WriteLine(diff + "/" + Math.Round(diff*ratio, 0));
                }
                _sliderMutex = false;
            }
            
        }
    
    }

#pragma warning restore CS1591
}