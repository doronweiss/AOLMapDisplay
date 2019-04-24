using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace AOLMapDisplay {
  public enum TrajChangeOpEnum { Changed, Removed };

  public class TrajWaypoint {
    public double lat, lon, alt;
    public Vector<double> Rv;
    public double incDistance = 0.0;
    public double radius = 0.0;
    public double Vcruise = 0.0;
    public double CruiseAlt = 0.0;

    public GeoLocation GeoLocation() {
      return new GeoLocation() { lat = lat, lon = lon, alt = alt };
    }

    static public TrajWaypoint FromGeoLocation(GeoLocation geol) {
      TrajWaypoint twp = new TrajWaypoint() { lat = geol.lat * AppConsts.d2r, lon = geol.lon * AppConsts.d2r, alt = geol.alt };
      return twp;
    }
  }

  class DataLineDefs {
    public bool commited = false;
    public List<WorldCoord> verticesW = new List<WorldCoord>();         // 'mapsui' world coordinates
    public List<GeoLocation> verticesGeo = new List<GeoLocation>();     // lat/long
    public List<TrajWaypoint> wayPoints = null;
    public List<object> mapFeatures = new List<object>();               // hold the feature, as object
    public List<object> waypointsFeatures = new List<object>();         // hold the feature, as object of the generated trajectory
    public List<NarratedGeoLocation> errorLocs = new List<NarratedGeoLocation>();
    //
    int selectedVertex = -1;
    private bool isSelected = false; // selection state

    // check if way points were generated
    public bool HasWaypoints {
      get { return wayPoints != null; }
    }

    // check if has error locations from the optimizer
    public bool HasErrorLocs {
      get { return errorLocs.Count > 0; }
    }

    // add a point
    internal void AddPoint(double worldX, double worldY, double lat, double lon, double alt) {
      verticesW.Add(new WorldCoord() { x = worldX, y = worldY });
      verticesGeo.Add(new GeoLocation() { lat = lat, lon = lon, alt = alt });
      ResetWaypoints();
      ResetErrorLocs();
    }

    // insert a point
    internal void InsertPoint(int beforeIdx, double worldX, double worldY, double lat, double lon, double alt) {
      verticesW.Insert(beforeIdx, new WorldCoord() { x = worldX, y = worldY });
      verticesGeo.Insert(beforeIdx, new GeoLocation() { lat = lat, lon = lon, alt = alt });
      ResetWaypoints();
      ResetErrorLocs();
    }

    // move a point location
    internal void MovePoint(int pointIdx, double worldX, double worldY, double lat, double lon, double alt) {
      verticesW[pointIdx] = new WorldCoord() { x = worldX, y = worldY };
      verticesGeo[pointIdx] = new GeoLocation() { lat = lat, lon = lon, alt = alt };
      ResetWaypoints();
      ResetErrorLocs();
    }

    // remove a vertex
    internal void RemovePoint(int ptIdx) {
      if (ptIdx >= 0 && ptIdx < verticesW.Count) {
        verticesW.RemoveAt(ptIdx);
        verticesGeo.RemoveAt(ptIdx);
        if (selectedVertex >= ptIdx && selectedVertex > 0)
          selectedVertex--;
        ResetWaypoints();
        ResetErrorLocs();
      }
    }

    // reverse traj direction
    internal void ReverseTraj() {
      verticesW.Reverse();
      verticesGeo.Reverse();
      ResetWaypoints();
      ResetErrorLocs();
    }

    internal void ResetWaypoints() {
      wayPoints?.Clear();
      wayPoints = null;
    }

    internal void ResetErrorLocs() {
      errorLocs?.Clear();
    }

    // check if a trajectory is selected by mouse - based on distance criteria
    // return true/false and min distance
    // saves the nearest vertex index
    internal Tuple<bool, double> IsNear(WorldCoord pt, double threshold) {
      if (verticesW.Count < 2)
        return new Tuple<bool, double>(false, -1.0);
      List<double> dists = new List<double>();
      List<double> ks = new List<double>();
      for (int idx = 0; idx < verticesW.Count - 1; idx++) {
        Vector<double> A = Vector<double>.Build.Dense(new[] { verticesW[idx].x, verticesW[idx].y });
        Vector<double> B = Vector<double>.Build.Dense(new[] { verticesW[idx + 1].x, verticesW[idx + 1].y });
        Vector<double> P = Vector<double>.Build.Dense(new[] { pt.x, pt.y });
        double k = (P - A) * (B - A) / ((B - A) * (B - A));
        if (k < 1.0 && k > 0.0) { // on the line segment
          Vector<double> perp = P - (A + (B - A) * k);
          dists.Add(perp.Norm(2));
          ks.Add(k);
        } else {
          dists.Add(Double.MaxValue);
          ks.Add(0.0);
        }
      }
      double minDist = dists.Min();
      selectedVertex = dists.FindIndex(x => Math.Abs(x - minDist) < 0.001);
      if (ks[selectedVertex] > 0.5) // found in the far half of the segment
        selectedVertex++;
      return new Tuple<bool, double>(minDist < threshold, minDist);
    }

    //get selected state - INTERNAL - cannot be used directly from the UI
    internal void SetSelected(bool isSel) {
      isSelected = isSel;
      if (!isSel)
        selectedVertex = -1;
    }

    //get selected state - PUBLIC
    public bool Selected {
      get { return isSelected; }
    }

    public int SelectedVIdx {
      get { return selectedVertex; }
    }

  }
}

