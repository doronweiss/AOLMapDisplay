using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOLMapDisplay {
  class MappedDataStore {
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
  }
}
