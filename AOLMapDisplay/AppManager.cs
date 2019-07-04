using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapsui.Styles;
using System.Windows;

namespace AOLMapDisplay {
  class AppManager  {
    // feature keys
    const string KeyField = "ID";
    static int featuresKey = 0;

    public AppManager() {
    }

    ~AppManager() {
    }

    internal enum InitRes { Bad, Ok, NeedsCfg };
    public InitRes Init(out string errorStr) {
      errorStr = "";
      return InitRes.Ok;
    }

    public void UnInit() {
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
    #endregion implement CoordinatesConverter

    #region trajectories layer source manuipulation
    public Mapsui.Providers.Features trjLyrFeatures = new Mapsui.Providers.Features(KeyField);

    // erase all drawn trajectories
    private void ClearTrajectories() {
      trjLyrFeatures.Clear();
    }

/*
    // draw a trajectory - simple version called on change
    private void DrawTrajectory(DataLineDefs traj, TrajChangeOpEnum changeType) {
      DrawTrajectory(traj, changeType, false);
    }

    // draws the trajectory with waypo
    public void DrawWithWP(DataLineDefs traj) {
      DrawTrajectory(traj, TrajChangeOpEnum.Changed, true);
    }


    private void DrawTrajectory(DataLineDefs traj, TrajChangeOpEnum changeType, bool drawWaypoints) {
      // set style
      bool applyStyle = traj.Selected || traj.HasWaypoints;
      Mapsui.Styles.Color styleColor = Mapsui.Styles.Color.Black;
      if (applyStyle) {
        if (traj.Selected) {
          if (traj.HasWaypoints)
            styleColor = Mapsui.Styles.Color.Yellow;
          else
            styleColor = Mapsui.Styles.Color.Violet;
        } else {
          if (traj.HasWaypoints)
            styleColor = Mapsui.Styles.Color.Green;
        }
      }
      // draw
      Mapsui.Providers.Feature ft = null;
      // clear current
      foreach (object o in traj.mapFeatures) // traj features
        trjLyrFeatures.Delete(o);
      traj.mapFeatures.Clear();
      foreach (object o in traj.waypointsFeatures) // mislul features
        trjLyrFeatures.Delete(o);
      traj.waypointsFeatures.Clear();
      switch (changeType) {
        case TrajChangeOpEnum.Changed:
          #region draw vertices
          for (int idx = 0; idx < traj.verticesW.Count; idx++) {
            WorldCoord w = traj.verticesW[idx];
            ft = new Mapsui.Providers.Feature {
              Geometry =
                new Mapsui.Geometries.Point() {
                  X = w.x, Y = w.y
                },
              ["Label"] = $"Vertex {idx}",
              ["Type"] = "Traj vertex"
            };
            if (traj.Selected && idx == traj.SelectedVIdx) {
              ft.Styles.Add(new Mapsui.Styles.SymbolStyle() {
                Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Green),
                Outline = { Color = Mapsui.Styles.Color.Red, Width = 2 }
              });
            } else {
              ft.Styles.Add(new Mapsui.Styles.SymbolStyle() {
                SymbolScale = idx == 0 ? 1.1 : 0.8,
                Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Gray),
                Outline = { Color = Mapsui.Styles.Color.Black, Width = 2 }
              });
            }
            ft[KeyField] = featuresKey;
            trjLyrFeatures.Add(ft);
            traj.mapFeatures.Add(featuresKey++);
          }
          #endregion draw vertices
          #region draw line string - traj
          if (traj.verticesW.Count > 1) {
            ft = new Mapsui.Providers.Feature {
              Geometry =
                new Mapsui.Geometries.LineString() {
                  Vertices = traj.verticesW.Select(x => new Mapsui.Geometries.Point() { X = x.x, Y = x.y }).ToList() as List<Mapsui.Geometries.Point>
                }
            };
            ft[KeyField] = featuresKey;
            if (applyStyle)
              ft.Styles.Add(new Mapsui.Styles.VectorStyle() { Line = new Pen(styleColor, 2) });
            trjLyrFeatures.Add(ft);
            traj.mapFeatures.Add(featuresKey++);
          }
          #endregion draw line string - traj
          #region draw waypoints
          if (drawWaypoints && traj.HasWaypoints && traj.wayPoints.Count > 1) {
            ft = new Mapsui.Providers.Feature {
              Geometry =
                new Mapsui.Geometries.LineString() {
                  Vertices = traj.wayPoints.Select(x => MapsuiPtFromWaypoint(x)).ToList() as List<Mapsui.Geometries.Point>
                },
              ["Label"] = $"Error waypoint",
              ["Type"] = ""
            };
            ft[KeyField] = featuresKey;
            ft.Styles.Add(new Mapsui.Styles.VectorStyle() { Line = new Pen(Mapsui.Styles.Color.Red, 2) });
            trjLyrFeatures.Add(ft);
            traj.waypointsFeatures.Add(featuresKey++);
          }
          #endregion draw waypoints
          #region draw error locations
          if (drawWaypoints && traj.HasErrorLocs) {
            for (int idx = 0; idx < traj.errorLocs.Count; idx++) {
              WorldCoord w = FromGeo(traj.errorLocs[idx]);
              ft = new Mapsui.Providers.Feature {
                Geometry =
                  new Mapsui.Geometries.Point() {
                    X = w.x, Y = w.y
                  },
                ["Label"] = traj.errorLocs[idx].description,
                ["Type"] = ""
              };
              if (traj.Selected && idx == traj.SelectedVIdx)
                ft.Styles.Add(new Mapsui.Styles.SymbolStyle() {
                  SymbolScale = 0.8,
                  Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
                  Outline = { Color = Mapsui.Styles.Color.Red, Width = 5 }
                });
              ft[KeyField] = featuresKey;
              trjLyrFeatures.Add(ft);
              traj.waypointsFeatures.Add(featuresKey++);
            }
          }
          #endregion draw error locations
          break;
        case TrajChangeOpEnum.Removed:
          break;
      }
    }
*/
    #endregion

    #region Coordinates conversion
    WorldCoord FromGeo(GeoLocation g) {
      return FromGeo(g.lat, g.lon);
    }

    WorldCoord FromGeo(double lat, double lon) {
      Mapsui.Geometries.Point pt = Mapsui.Projection.SphericalMercator.FromLonLat(lon, lat);
      return new WorldCoord() { x = pt.X, y = pt.Y };
    }

    GeoLocation FromWorld(WorldCoord w) {
      return FromWorld(w.x, w.y);
    }

    GeoLocation FromWorld(double wrX, double wrY) {
      Mapsui.Geometries.Point pt = Mapsui.Projection.SphericalMercator.ToLonLat(wrX, wrY);
      return new GeoLocation() { lat = pt.Y, lon = pt.X, alt = 0 };
    }

    Mapsui.Geometries.Point MapsuiPtFromWaypoint(Waypoint twp) {
      WorldCoord wr = FromGeo(twp.lat * AppConsts.r2d, twp.lon * AppConsts.r2d);
      Mapsui.Geometries.Point pt = new Mapsui.Geometries.Point() { X = wr.x, Y = wr.y };
      return pt;
    }
    #endregion Coordinates conversion


  }
}
