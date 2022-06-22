using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface IPrincipalFileModelViewRepository
    {
        bool Save(PrincipalFileModelView data);

        bool SaveList(List<PrincipalFileModelView> data);

        PrincipalFileModelView GetDatabyPrincipal(string principal);

        List<PrincipalFileModelView> GetAllFilesViewModel();
    }
}
