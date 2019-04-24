using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Styles;

namespace AOLMapDisplay {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    Layer drawLayer = null;
    AppManager manager = new AppManager();

    public MainWindow() {
      InitializeComponent();
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e) {
      if (!((App) Application.Current).cfgLoaded) {
        Close();
        return;
      }
      if (AppConfigSvc.appXmlCfg.baseMapSource == AppConfigSvc.BaseMapSourceType.Web)
        mapCtrl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()));
      else
        mapCtrl.Map.Layers.Add(new Mapsui.Layers.TileLayer(
          new BruTile.Web.HttpTileSource(
            new BruTile.Predefined.GlobalSphericalMercator(0, 19),
            AppConfigSvc.appXmlCfg.localMapsSrcFmt,
            new[] { "0", "0", "0" }
            )));
      // create the simulation data layer
      drawLayer = new Layer("ID") {
        Name = "SimData",
        DataSource = new Mapsui.Providers.MemoryProvider(manager.trjLyrFeatures),
        Style =
          new SymbolStyle() {
            SymbolScale = 0.8,
            Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Gray),
            Outline = { Color = Mapsui.Styles.Color.Black, Width = 1 }
          }
      };
      mapCtrl.Map.Layers.Add(drawLayer);
      //
      Mapsui.Geometries.Point tw = Mapsui.Projection.SphericalMercator.FromLonLat(34.8, 32.07);
      mapCtrl.Map.NavigateTo(tw);
      mapCtrl.Map.NavigateTo(mapCtrl.Map.Resolutions[7]);
    }

    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {

    }

    private void OnLoadDataBtnClick(object sender, RoutedEventArgs e) {

    }

    private void OnPrintMapBtnClick(object sender, RoutedEventArgs e) {
    }
  }
}
