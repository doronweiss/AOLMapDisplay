using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOLMapDisplay {
  // interface used to create world coordinates
  public interface CoordinatesConverter {
    void ToWorld(List<GeoLocation> geo, ref List<WorldCoord> world);
    void ToGeo(List<WorldCoord> world, ref List<GeoLocation> geo);
    WorldCoord FromGeo(GeoLocation geo);
    GeoLocation FromWorld(WorldCoord world);
  }
}
