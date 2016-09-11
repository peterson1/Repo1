using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo1.Core.ns12.Models;

namespace Repo1.Core.ns12.DTOs.ViewsListDTOs
{
    public class ExecutablesByUserDTO : R1Executable, IViewsListDTO
    {
        public string ViewsDisplayURL => "";
    }
}
