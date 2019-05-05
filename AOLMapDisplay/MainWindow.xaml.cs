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
using BruTile.Wms;
using Mapsui.Layers;
using Mapsui.Styles;
using Layer = Mapsui.Layers.Layer;

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

    //https://stackoverflow.com/questions/54950439/printing-from-mapsui-skiasharp
    private void OnPrintMapBtnClick(object sender, RoutedEventArgs e) {
//      BoundingBox extents = mapCtrl.Map.Envelope;
//      //System.Drawing.Sizef size = new System.Drawing.Sizef() {Width = 100.0f, Height = 100.0f};
//      System.Drawing.SizeF size = new System.Drawing.SizeF() { Width = 100.0f, Height = 100.0f };
//      var resolution = Mapsui.Utilities.ZoomHelper.DetermineResolution(extents.Width, extents.Height, size.Width, size.Height);
//      var viewport = new Mapsui.Viewport() {
//        Center = extents.Center.ToMapsui(),
//        Resolution = resolution,
//        Width = size.Width,
//        Height = size.Height
//      };
//
//      var msMap = map.GetMapsuiMap();
//
//      var path = System.IO.Path.GetTempFileName();
//      using (var stream = new SkiaSharp.SKFileWStream(path)) {
//        using (var document = SkiaSharp.SKDocument.CreateXps(stream, dpi)) {
//
//          var canvas = document.BeginPage(size.Width, size.Height);
//          Renderer.Render(canvas, viewport, msMap.Layers, msMap.Widgets);
//          document.EndPage();
//        }
//      }

    }
  }
}
