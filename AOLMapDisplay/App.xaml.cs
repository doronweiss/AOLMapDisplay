using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AOLMapDisplay {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {
    public bool cfgLoaded = false;

    protected override void OnStartup(System.Windows.StartupEventArgs e) {
      cfgLoaded = AOLMapDisplay.AppConfigSvc.LoadConfig("aolconfig.xml");
    }

    protected override void OnExit(System.Windows.ExitEventArgs e) {
      //AOLMapDisplay.AppConfigSvc.SaveConfig("aolconfig.xml");
    }
  }
}
