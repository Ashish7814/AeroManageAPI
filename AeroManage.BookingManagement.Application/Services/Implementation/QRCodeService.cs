using AeroManage.BookingManagement.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Implementation
{
    public class QRCodeService : IQRCodeService
    {
        private readonly string _qrCodePath;
        private readonly ILogger<QRCodeService> _logger;

        public QRCodeService(IConfiguration configuration, ILogger<QRCodeService> logger)
        {
            _qrCodePath = configuration["FileStorage:QRCodePath"] ?? "wwwroot/qrcodes";
            _logger = logger;

            // Ensure directory exists
            if (!Directory.Exists(_qrCodePath))
            {
                Directory.CreateDirectory(_qrCodePath);
            }
        }

        public async Task<string> GenerateQRCodeAsync(string data, string fileName)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

                using var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(20);

                var filePath = Path.Combine(_qrCodePath, fileName);
                await File.WriteAllBytesAsync(filePath, qrCodeImage);

                _logger.LogInformation($"QR code generated: {filePath}");

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                throw;
            }
        }

        public async Task<byte[]> GenerateQRCodeBytesAsync(string data)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

                using var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(20);

                return await Task.FromResult(qrCodeImage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code bytes");
                throw;
            }
        }

        public async Task<string> GenerateQRCodeBase64Async(string data)
        {
            try
            {
                var qrCodeBytes = await GenerateQRCodeBytesAsync(data);
                var base64String = Convert.ToBase64String(qrCodeBytes);

                return $"data:image/png;base64,{base64String}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code base64");
                throw;
            }
        }
    }
}
