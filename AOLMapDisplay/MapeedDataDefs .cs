using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace AOLMapDisplay {
  public enum TrajChangeOpEnum { Changed, Removed };

  public class MappedPoint {
    public WorldCoord world;
    public GeoLocation geo;
    public double radius = 0.0;

    public GeoLocation GeoLocation() {
      return geo;
    }

    public static MappedPoint FromGeoLocation(GeoLocation geol) {
      MappedPoint twp = new MappedPoint() {
        geo = new GeoLocation() {lat = geol.lat * AppConsts.d2r, lon = geol.lon * AppConsts.d2r, alt = geol.alt},
        world = null
      };
      return twp;
    }
  }

  class MappedLine {
    public List<MappedPoint> vertices = null;
    public List<object> mapFeatures = new List<object>();               // hold the feature, as object
    //
    int selectedVertex = -1;
    private bool isSelected = false; // selection state

    // check if a trajectory is selected by mouse - based on distance criteria
    // return true/false and min distance
    // saves the nearest vertex index
    internal Tuple<bool, double> IsNear(WorldCoord pt, double threshold) {
      if (vertices.Count < 2)
        return new Tuple<bool, double>(false, -1.0);
      List<double> dists = new List<double>();
      List<double> ks = new List<double>();
      for (int idx = 0; idx < vertices.Count - 1; idx++) {
        Vector<double> A = Vector<double>.Build.Dense(new[] { vertices[idx].world.x, vertices[idx].world.y });
        Vector<double> B = Vector<double>.Build.Dense(new[] { vertices[idx + 1].world.x, vertices[idx + 1].world.y });
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

