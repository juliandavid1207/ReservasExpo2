using AllNet.Modules.ReservasExportaciones.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllNet.Modules.ReservasExportaciones.Abstractions
{
    public interface IRepository
    {
        void UpdateBookingState(Dictionary<string, dynamic> state, Dictionary<string, dynamic> condition);
        object GetFilePath(string id);
        List<DetCargaBModel> GetDetCargaB(int id);
        int GetLastId();


    }
}
