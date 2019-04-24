using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace AOLMapDisplay {
  public static class AppConfigSvc {

    public static readonly int[] SelectionProximities = new int[] { 1, 10, 100, 1000, 10000 };
    // optional map sources
    public enum BaseMapSourceType { Web, Local };

    // describe a folder to load maps from
    [Serializable]
    public class DTMLoadCmd {
      public string folder;
      public bool debug;
    }

    [Serializable]
    [XmlRoot("AppXMLConfig")]
    public class AppXMLConfig {
      public int proximityIdx = 1;
      public bool useDTM = true;
      // map
      public BaseMapSourceType baseMapSource = BaseMapSourceType.Web;
      public string localMapsSrcFmt = "http://192.168.1.150/hot/{0}/{1}/{2}.png";
      public List<DTMLoadCmd> mapFolders;
    }


    public static AppXMLConfig appXmlCfg;
    private static string fileName = null;
    public static bool LoadConfig(string cfgFileName) {
      try {
        XmlSerializer s = new XmlSerializer(typeof(AppXMLConfig));
        using (TextReader r = new StreamReader(cfgFileName)) {
          appXmlCfg = (AppXMLConfig)s.Deserialize(r);
        }
      } catch (Exception e) {
        System.Diagnostics.Debug.WriteLine(e.Message);
        return false; // did not initialize
      }
      fileName = cfgFileName;
      return true;
    }

    public static bool SaveConfig(string cfgFileName) {
      if (cfgFileName == null) {
        if (fileName == null)
          return false;
        else
          cfgFileName = fileName;
      }
      try {
        XmlSerializer s = new XmlSerializer(typeof(AppXMLConfig));
        using (TextWriter w = new StreamWriter(cfgFileName)) {
          s.Serialize(w, appXmlCfg);
        }
      } catch {
        return false;
      }
      return true;
    }

    // serialize and deserialize the configuration into a string for use by the configuration editor
    public static string SerializeConfig(AppXMLConfig cfg) {
      try {
        XmlSerializer s = new XmlSerializer(typeof(AppXMLConfig));
        using (StringWriter sw = new StringWriter()) {
          s.Serialize(sw, cfg);
          return sw.ToString();
        }
      } catch (Exception e) {
        System.Diagnostics.Debug.WriteLine(e.Message);
        return null; // did not initialize
      }
    }

    public static AppXMLConfig DeserializeConfig(string cfgString) {
      try {
        XmlSerializer s = new XmlSerializer(typeof(AppXMLConfig));
        using (StringReader sr = new StringReader(cfgString)) {
          AppXMLConfig settings = (AppXMLConfig)s.Deserialize(sr);
          return settings;
        }
      } catch {
        return null;
      }
    }
  }
}