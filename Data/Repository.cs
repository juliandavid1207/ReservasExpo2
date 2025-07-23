using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using DataTables;
using DotNetNuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Data
{
    public class Repository : IRepository
    {
        private readonly string connection_string = Config.GetConnectionString("BD_AGP");

        public void UpdateBookingState(Dictionary<string, dynamic> state, Dictionary<string, dynamic> condition)
        {     
            try
            {
                using (var db = new Database("sqlserver", connection_string))
                {   

                    db.Update("BOOKINGS_EXPO_DOCS", state, condition);
                  
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        public object GetFilePath(string id)
        {
            try
            {
                var where = new Dictionary<string, dynamic> { { "id", id } };
                var fields = new[] { "system_path", "filename" };              
               
                using (var db = new Database("sqlserver", connection_string))
                {
                    var result = db.Select("ARCHIVOS_BOOKINGS", fields, where);
                    if (result.Count() > 0)
                    {
                        var rows = result.FetchAll();
                        var row = rows[0];
                        if (row.ContainsKey("system_path"))
                        {
                            return new { path = row["system_path"].ToString(), name = row["filename"].ToString() };
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DetCargaBModel> GetDetCargaB(int id)
        {
            try
            {
                using (var db = new Database("sqlserver", connection_string))
                {
                    var where = new Dictionary<string, object> { { "ID_BOOKINGS_EXPO", id } };
                   
                    var result = db.Select("DETCARGAB", new[] { "*" }, where);

                    if (result.Count() > 0)
                    {

                        var list = new List<DetCargaBModel>();

                        foreach (var row in result.FetchAll())
                        {
                            list.Add(new DetCargaBModel
                            {
                                ID_DETCARGA = Convert.ToInt32(row["ID_DETCARGA"]),
                                IDEN = row["IDEN"]?.ToString(),
                                PESO_ORIG = Convert.ToDecimal(row["PESO_ORIG"]),
                                BULTOS = Convert.ToDecimal(row["BULTOS"]),
                                ID_BOOKINGS_EXPO = Convert.ToInt32(row["ID_BOOKINGS_EXPO"])
                            });
                        }

                        return list;
                    }
                    return null;
                    
                }
            }
            catch (Exception ex)
            {              
                return null;
            }
        }

        public int GetLastId()
        {
            try
            {
                using (var db = new Database("sqlserver", connection_string))
                {
                    var result =  db.Select("DETCARGAB", new[] { "ID_DETCARGA" }, (Dictionary<string,dynamic>)null, new[] { "ID_DETCARGA DESC" });
                    if (result.Count() > 0)
                    {
                        var row = result.Fetch();
                        return Convert.ToInt32(row["ID_DETCARGA"]);
                    }

                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}