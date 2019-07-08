using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapsui.Styles;
using System.Windows;

namespace AOLMapDisplay {
  class AppManager : CoordinatesConverter {
    // feature keys
    const string KeyField = "ID";
    static int featuresKey = 0;
    MappedDataStore mappedData = new MappedDataStore();

    public AppManager() {
    }

    ~AppManager() {
    }

    #region implement CoordinatesConverter
    public void ToWorld(List<GeoLocation> geo, ref List<WorldCoord> world) {
      if (world == null)
        world = new List<WorldCoord>();
      world.Clear();
      foreach (GeoLocation g in geo)
        world.Add(FromGeo(g));
    }

    public void ToGeo(List<WorldCoord> world, ref List<GeoLocation> geo) {
      if (geo == null)
        geo = new List<GeoLocation>();
      geo.Clear();
      foreach (WorldCoord w in world)
        geo.Add(FromWorld(w));
    }

    public WorldCoord FromGeo(GeoLocation g) {
      return FromGeo(g.lat, g.lon);
    }

    public GeoLocation FromWorld(WorldCoord w) {
      return FromWorld(w.x, w.y);
    }

    #endregion implement CoordinatesConverter

    #region trajectories layer source manuipulation
    public Mapsui.Providers.Features trjLyrFeatures = new Mapsui.Providers.Features(KeyField);

    // erase all drawn trajectories
    private void ClearTrajectories() {
      trjLyrFeatures.Clear();
    }

    public bool LoadData(string fileName, bool clearFirst) {
      if (clearFirst)
        trjLyrFeatures.Clear();
      if (!mappedData.LoadData(fileName, this, clearFirst))
        return false;
      // draw points
      foreach (MappedPoint mp in mappedData.points) {
        Mapsui.Providers.Feature ft = new Mapsui.Providers.Feature {
          Geometry =
            new Mapsui.Geometries.Point() {
              X = mp.world.x, Y = mp.world.y
            },
          ["Label"] = $"Hi",
          ["Type"] = "vertex"
        };
        ft.Styles.Add(new Mapsui.Styles.SymbolStyle() {
          Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Green),
          Outline = { Color = Mapsui.Styles.Color.Red, Width = 2 }
        });
        ft[KeyField] = mp.featureId;
        trjLyrFeatures.Add(ft);
      }
      // draw lines
      foreach (MappedLine ml in mappedData.lines) {
        Mapsui.Providers.Feature ft = new Mapsui.Providers.Feature {
          Geometry =
            new Mapsui.Geometries.LineString() {
              Vertices = ml.vertices.Select(v => new Mapsui.Geometries.Point() { X = v.world.x, Y = v.world.y }).ToList() as List<Mapsui.Geometries.Point>
            },
          ["Label"] = $"Hi",
          ["Type"] = "vertex"
        };
        ft.Styles.Add(new Mapsui.Styles.VectorStyle() { Line = new Pen(Mapsui.Styles.Color.Yellow, 2) });
        ft[KeyField] = ml.featureId;
        trjLyrFeatures.Add(ft);
      }
      return true;
    }
    #endregion

    #region Coordinates conversion
    WorldCoord FromGeo(double lat, double lon) {
      Mapsui.Geometries.Point pt = Mapsui.Projection.SphericalMercator.FromLonLat(lon, lat);
      return new WorldCoord() { x = pt.X, y = pt.Y };
    }

    GeoLocation FromWorld(double wrX, double wrY) {
      Mapsui.Geometries.Point pt = Mapsui.Projection.SphericalMercator.ToLonLat(wrX, wrY);
      return new GeoLocation() { lat = pt.Y, lon = pt.X, alt = 0 };
    }

    Mapsui.Geometries.Point MapsuiPtFromWaypoint(MappedPoint twp) {
      WorldCoord wr = FromGeo(twp.geo.lat * AppConsts.r2d, twp.geo.lon * AppConsts.r2d);
      Mapsui.Geometries.Point pt = new Mapsui.Geometries.Point() { X = wr.x, Y = wr.y };
      return pt;
    }

    #endregion Coordinates conversion


  }
}
