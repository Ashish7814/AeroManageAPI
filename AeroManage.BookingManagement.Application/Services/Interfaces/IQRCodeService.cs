using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Interfaces
{
    public interface IQRCodeService
    {
        Task<string> GenerateQRCodeAsync(string data, string fileName);
        Task<byte[]> GenerateQRCodeBytesAsync(string data);
        Task<string> GenerateQRCodeBase64Async(string data);
    }
}
