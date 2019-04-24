using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AOLMapDisplay {
  // application wide basic constants
  public static class AppConsts {
    public const double d2r = 0.0174532925199433;         // degree => radian
    public const double r2d = 57.2957795130823;           // radian => degree
    // numeric constants
    public const double FLOATEPSILON = 0.000000000001;
    public const double BIG_POSITIVE_FLOAT = 1.0e100;
    public const double E = 2.71828182845904523536;
    // geographic constants
    public const double g = 9.81;
    public const double Time0 = 0.0;
    public const double SeaLevel = 0.0;
  }

  // define a point in MAPSUI world coordinates
  public class WorldCoord {
    public double x, y;
  }

  // define a geographic location - lat/long/alt
  public class GeoLocation {
    public double lat, lon;
    public double alt = 0.0;
  }

  // geographic location WITH a description
  public class NarratedGeoLocation : GeoLocation {
    public string description = "";
  }

  // enum for optional results from the getaltitude function
  public enum DTMGetErrorsEnum {
    OK = 0,
    Error = 1,
    BadMapNumber = 2,
    NoMapFits = 3,
    NoHUB = 4,
    CommError = 11
  }


}
