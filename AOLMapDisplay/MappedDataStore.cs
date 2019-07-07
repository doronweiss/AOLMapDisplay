using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOLMapDisplay {
  class MappedDataStore {
    private double cntrLat, cntrLon;
    private List<MappedPoint> points;
    private List<MappedLine> lines;

    public bool LoadData(string fileName, bool clearFirst) {
      if (clearFirst) {
        points?.Clear();
        lines?.Clear();
      }
      string ext = Path.GetExtension(fileName).TrimStart('.');
      switch (ext) {
        case "draw":
          return true;
        default:
          return true;
      }
    }

    private bool LoadBaseDrawData(string fileName) {
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
                MappedPoint mp = new MappedPoint() { geo = new GeoLocation() { lat = double.Parse(parts[0]), lon = double.Parse(parts[1]), alt = 0.0 } };
                points.Add(mp);
              }
              break;
              case "line": {
                parts = line.Substring(5).Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                int sz = parts.Length / 2;
                MappedLine ml = new MappedLine() {vertices = new List<MappedPoint>()};
                for (int idx = 0; idx < sz; idx++) {
                  MappedPoint mp = new MappedPoint()
                    {geo = new GeoLocation() {lat = double.Parse(parts[idx * 2]), lon = double.Parse(parts[idx * 2 + 1]), alt = 0.0}};
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
