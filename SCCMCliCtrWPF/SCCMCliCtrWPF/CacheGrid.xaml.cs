﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using sccmclictr.automation;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for CacheGrid.xaml
    /// </summary>
    public partial class CacheGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public CacheGrid()
        {
            InitializeComponent();
        }

        public SCCMAgent SCCMAgentConnection
        {
            get
            {
                return oAgent;
            }
            set
            {
                if (value.isConnected)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        oAgent = value;
                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = oAgent.Client.SWCache.CachedContent;
                        dataGrid1.EndInit();

                        uint? iTotalSize = 0;
                        foreach (sccmclictr.automation.functions.swcache.CacheInfoEx CacheItem in dataGrid1.Items)
                        {
                            if (CacheItem.ContentSize != null)
                            {
                                iTotalSize = iTotalSize + CacheItem.ContentSize;
                            }
                        }
                        sbiContentSize.Content = ((iTotalSize)/1024).ToString() + " (MB)";
                    }
                    catch { }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void imgGetCachePath2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                tbCachePath2.Text = oAgent.Client.SWCache.CachePath;
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgOpenCachePath2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Process Explorer = new Process();
                Explorer.StartInfo.FileName = "Explorer.exe";
                string sCachePath = "";
                try
                {
                    tbCachePath2.Text = oAgent.Client.SWCache.CachePath;
                    sCachePath = tbCachePath2.Text.Replace(':', '$');
                }
                catch { }
                if (!string.IsNullOrEmpty(sCachePath))
                {
                    Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + @"\" + sCachePath;
                }
                else
                {
                    Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + @"\admin$\CCM\Cache";
                }

                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgSaveCachepath2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                oAgent.Client.SWCache.CachePath = tbCachePath2.Text;
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
        }

        private void miDeleteItems_Click(object sender, RoutedEventArgs e)
        {
            foreach (sccmclictr.automation.functions.swcache.CacheInfoEx CacheItem in dataGrid1.SelectedItems)
            {
                try
                {
                    CacheItem.Delete();
                }
                catch (Exception ex)
                {
                    Listener.WriteError(ex.Message);
                }
            }

            try
            {
                uint? iTotalSize = 0;
                foreach (sccmclictr.automation.functions.swcache.CacheInfoEx CacheItem in dataGrid1.Items)
                {
                    if (CacheItem.ContentSize != null)
                    {
                        iTotalSize = iTotalSize + CacheItem.ContentSize;
                    }
                }
                sbiContentSize.Content = ((iTotalSize) / 1024).ToString() + " (MB)";
            }
            catch { }
        }

        private void miOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Process Explorer = new Process();
                Explorer.StartInfo.FileName = "Explorer.exe";
                try
                {
                    if (dataGrid1.SelectedItem != null)
                    {
                        string sPath = ((sccmclictr.automation.functions.swcache.CacheInfoEx)dataGrid1.SelectedItem).Location.Replace(':', '$'); ;
                        Explorer.StartInfo.Arguments = @"\\" + oAgent.TargetHostname + @"\" + sPath;
                    }
                }
                catch { }

                Explorer.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                Explorer.Start();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgGetCacheSize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                tbCacheSize.Text = oAgent.Client.SWCache.CacheSize.ToString();
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void imgSaveCacheSize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                oAgent.Client.SWCache.CacheSize = uint.Parse(tbCacheSize.Text);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
        }
    }
}
