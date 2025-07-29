using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication1.Application.Interfaces
{
    public interface IExcelService
    {
        Task<MemoryStream> ExportToExcel<T>(List<T> data, string sheetName);
    }
}