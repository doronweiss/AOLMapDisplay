using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOLMapDisplay {
  class MappedDataStore {
    private static int nextFeatId = 0;
    public double cntrLat, cntrLon;
    public List<MappedPoint> points = new List<MappedPoint>();
    public List<MappedLine> lines = new List<MappedLine>();

    public bool LoadData(string fileName, CoordinatesConverter coordsCvrtr, bool clearFirst) {
      if (clearFirst) {
        points?.Clear();
        lines?.Clear();
      }
      string ext = Path.GetExtension(fileName).TrimStart('.');
      switch (ext) {
        case "draw":
          return LoadBaseDrawData(fileName, coordsCvrtr);
        default:
          return true;
      }
    }

    private bool LoadBaseDrawData(string fileName, CoordinatesConverter coordsCvrtr) {
      try {
        string line;
        using (StreamReader sr = new StreamReader(fileName)) {
          line = sr.ReadLine();
          while (line != null) {
            string[] parts;
            string typeStr = line.Substring(0, 4);
            switch (typeStr) {
              case "cntr":
                parts = line.Substring(5).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                cntrLat = double.Parse(parts[0]);
                cntrLon = double.Parse(parts[1]);
                break;
              case "symb": {
                parts = line.Substring(5).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                MappedPoint mp = new MappedPoint() {featureId = nextFeatId++,
                  geo = new GeoLocation() { lat = double.Parse(parts[0]), lon = double.Parse(parts[1]), alt = 0.0 } };
                mp.world = coordsCvrtr.FromGeo(mp.geo);
                points.Add(mp);
              }
              break;
              case "line": {
                parts = line.Substring(5).Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                int sz = parts.Length / 2;
                MappedLine ml = new MappedLine() {vertices = new List<MappedPoint>()};
                for (int idx = 0; idx < sz; idx++) {
                  MappedPoint mp = new MappedPoint(){ featureId = nextFeatId++,
                    geo = new GeoLocation() {lat = double.Parse(parts[idx * 2]), lon = double.Parse(parts[idx * 2 + 1]), alt = 0.0}};
                  mp.world = coordsCvrtr.FromGeo(mp.geo);
                  ml.vertices.Add(mp);
                }
                lines.Add(ml);
              }
                break;
            }
            line = sr.ReadLine();
          }
          return true;
        }
      } catch (Exception e) {
        Console.WriteLine(e);
        return false; ;
      }
    }
  }
}
