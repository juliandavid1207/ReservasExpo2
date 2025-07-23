using AllNet.Modules.ReservasExportaciones.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllNet.Modules.ReservasExportaciones.Abstractions
{
    public interface IBookingsServices
    {
        Task<bool> UpdateState(string bl,string codigo, string id);
        Task<bool> EnviarEmail(string fromAddress, string toAddress, string ccAddress, string subjectEmail, string message);
        Task<object> GetFilePath(string id);
        Task<List<DetCargaBModel>> GetDetCargaB(int id);
        Task<bool> UpdatePesoBultos(decimal peso, decimal bultos,int bloqueo, string id);
        Task<int> GetLastIdDetCargaB();
    }
}
