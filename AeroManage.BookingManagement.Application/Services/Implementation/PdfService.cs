using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Services.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Implementation
{
    public class PdfService : IPdfService
    {
        private readonly string _pdfPath;
        private readonly IQRCodeService _qrCodeService;
        private readonly ILogger<PdfService> _logger;

        public PdfService(
            IConfiguration configuration,
            IQRCodeService qrCodeService,
            ILogger<PdfService> logger)
        {
            _pdfPath = configuration["FileStorage:PdfPath"] ?? "wwwroot/tickets";
            _qrCodeService = qrCodeService;
            _logger = logger;

            // Ensure directory exists
            if (!Directory.Exists(_pdfPath))
            {
                Directory.CreateDirectory(_pdfPath);
            }
        }

        // ==================== GENERATE E-TICKET ====================

        public async Task<string> GenerateETicketAsync(ETicketDto eTicket)
        {
            try
            {
                var fileName = $"ETicket_{eTicket.BookingReference}_{eTicket.TicketNumber}_{DateTime.Now:yyyyMMddHHmmss}.html";
                var filePath = Path.Combine(_pdfPath, fileName);

                // ── QR code block ─────────────────────────────────────
                var qrBlock = string.Empty;
                if (!string.IsNullOrEmpty(eTicket.QRCode))
                {
                    var src = eTicket.QRCode.StartsWith("data:")
                        ? eTicket.QRCode
                        : $"data:image/png;base64,{eTicket.QRCode}";

                    qrBlock = $@"
                        <div class=""qr-wrap"">
                            <img src=""{src}"" alt=""Check-in QR Code"" class=""qr-img"" />
                            <p class=""qr-label"">Scan at self-check-in kiosk</p>
                        </div>";
                }

                // ── Flight rows ───────────────────────────────────────
                var flightRows = string.Empty;
                if (eTicket.Flight is not null)
                {
                    flightRows = $@"
                        <tr>
                            <td class=""label"">Flight</td>
                            <td class=""value mono"">{eTicket.Flight.FlightNumber ?? "—"}</td>
                        </tr>
                        <tr>
                            <td class=""label"">Departure</td>
                            <td class=""value"">{eTicket.Flight.Origin?.AirportCode} &nbsp;·&nbsp; {eTicket.Flight.DepartureDateTime:ddd, dd MMM yyyy HH:mm}</td>
                        </tr>
                        <tr>
                            <td class=""label"">Arrival</td>
                            <td class=""value"">{eTicket.Flight.Destination?.AirportCode} &nbsp;·&nbsp; {eTicket.Flight.ArrivalDateTime:ddd, dd MMM yyyy HH:mm}</td>
                        </tr>
                        <tr>
                            <td class=""label"">Seat</td>
                            <td class=""value"">As assigned at check-in</td>
                        </tr>";
                }

                var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
    <title>E-Ticket · {eTicket.BookingReference}</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"" />
    <link href=""https://fonts.googleapis.com/css2?family=DM+Serif+Display&family=DM+Sans:wght@400;500;600&family=DM+Mono&display=swap"" rel=""stylesheet"" />
    <style>
        {GetCoreStyles()}

        /* ── E-Ticket hero ────────────────────────────────── */
        .hero {{
            background: linear-gradient(135deg, #0f1b2d 0%, #1a3a5c 55%, #0d2d4a 100%);
            color: #fff;
            padding: 44px 48px 36px;
            position: relative;
            overflow: hidden;
        }}
        .hero::before {{
            content: """";
            position: absolute;
            right: -60px; top: -60px;
            width: 340px; height: 340px;
            border-radius: 50%;
            background: radial-gradient(circle, rgba(255,255,255,.06) 0%, transparent 70%);
        }}
        .hero-tag {{
            font-family: 'DM Mono', monospace;
            font-size: 10px;
            letter-spacing: 3px;
            text-transform: uppercase;
            color: #5db3f5;
            margin-bottom: 10px;
        }}
        .hero-title {{
            font-family: 'DM Serif Display', serif;
            font-size: 34px;
            line-height: 1.1;
            margin-bottom: 6px;
        }}
        .hero-sub {{
            font-size: 13px;
            color: rgba(255,255,255,.55);
            letter-spacing: .5px;
        }}
        .hero-ref {{
            position: absolute;
            right: 48px; top: 44px;
            text-align: right;
        }}
        .hero-ref .ref-label {{
            font-size: 10px;
            letter-spacing: 2px;
            text-transform: uppercase;
            color: rgba(255,255,255,.45);
        }}
        .hero-ref .ref-value {{
            font-family: 'DM Mono', monospace;
            font-size: 22px;
            font-weight: 700;
            color: #fff;
            letter-spacing: 3px;
        }}

        /* ── Route strip ──────────────────────────────────── */
        .route-strip {{
            background: #f7f9fc;
            border-bottom: 1px solid #e4e9f0;
            padding: 24px 48px;
            display: flex;
            align-items: center;
        }}
        .airport {{ flex: 1; }}
        .airport.right {{ text-align: right; }}
        .iata {{
            font-family: 'DM Serif Display', serif;
            font-size: 48px;
            line-height: 1;
            color: #0f1b2d;
        }}
        .city {{
            font-size: 12px;
            color: #8896a8;
            letter-spacing: .5px;
            margin-top: 4px;
        }}
        .plane-divider {{
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 6px;
            color: #c3cdd8;
        }}
        .plane-line {{
            width: 100%;
            height: 1px;
            background: #c3cdd8;
        }}
        .plane-icon {{
            font-size: 20px;
            color: #1a3a5c;
        }}

        /* ── QR sidebar layout ────────────────────────────── */
        .body-grid {{
            display: grid;
            grid-template-columns: 1fr 180px;
        }}
        .body-main {{ padding: 32px 48px; border-right: 1px dashed #d0d9e4; }}
        .body-sidebar {{
            padding: 32px 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            background: #f7f9fc;
        }}
        .qr-img {{
            width: 130px; height: 130px;
            border: 4px solid #fff;
            border-radius: 8px;
            box-shadow: 0 2px 12px rgba(0,0,0,.12);
        }}
        .qr-label {{
            font-size: 9px;
            text-transform: uppercase;
            letter-spacing: 1.5px;
            color: #8896a8;
            text-align: center;
            margin-top: 10px;
        }}
        .qr-wrap {{ text-align: center; }}
    </style>
</head>
<body>
    <div class=""page"">

        <!-- Hero Header -->
        <div class=""hero"">
            <div class=""hero-tag"">AeroManage · Electronic Ticket</div>
            <div class=""hero-title"">Boarding Document</div>
            <div class=""hero-sub"">Please carry a valid photo ID</div>
            <div class=""hero-ref"">
                <div class=""ref-label"">Booking Ref.</div>
                <div class=""ref-value"">{eTicket.BookingReference}</div>
            </div>
        </div>

        <!-- Route Strip -->
        <div class=""route-strip"">
            <div class=""airport"">
                <div class=""iata"">{eTicket.Flight?.Origin?.AirportCode ?? "—"}</div>
                <div class=""city"">{eTicket.Flight?.Origin?.AirportName ?? ""}</div>
            </div>
            <div class=""plane-divider"">
                <div class=""plane-line""></div>
                <div class=""plane-icon"">✈</div>
                <div class=""plane-line""></div>
            </div>
            <div class=""airport right"">
                <div class=""iata"">{eTicket.Flight?.Destination?.AirportCode ?? "—"}</div>
                <div class=""city"">{eTicket.Flight?.Destination?.AirportName ?? ""}</div>
            </div>
        </div>

        <!-- Body -->
        <div class=""body-grid"">
            <div class=""body-main"">

                <div class=""section-title"">Passenger</div>
                <table class=""info-table"">
                    <tr>
                        <td class=""label"">Full Name</td>
                        <td class=""value"">{eTicket.Passenger.FirstName} {eTicket.Passenger.LastName}</td>
                    </tr>
                    <tr>
                        <td class=""label"">Email</td>
                        <td class=""value"">{eTicket.Passenger.Email ?? "—"}</td>
                    </tr>
                    <tr>
                        <td class=""label"">Phone</td>
                        <td class=""value"">{eTicket.Passenger.Phone ?? "—"}</td>
                    </tr>
                </table>

                <div class=""divider""></div>

                <div class=""section-title"">Flight Details</div>
                <table class=""info-table"">
                    {flightRows}
                </table>

                <div class=""divider""></div>

                <div class=""section-title"">Ticket</div>
                <table class=""info-table"">
                    <tr>
                        <td class=""label"">Ticket No.</td>
                        <td class=""value mono"">{eTicket.TicketNumber}</td>
                    </tr>
                    <tr>
                        <td class=""label"">PNR</td>
                        <td class=""value mono"">{eTicket.PNR}</td>
                    </tr>
                </table>
            </div>

            <!-- QR Sidebar -->
            <div class=""body-sidebar"">
                {(string.IsNullOrEmpty(qrBlock)
                    ? "<p style='font-size:11px;color:#aaa;text-align:center'>QR code<br/>not available</p>"
                    : qrBlock)}
            </div>
        </div>

        <!-- Footer -->
        <div class=""footer"">
            <span>Arrive at the airport at least <strong>2 hours</strong> before departure</span>
            <span class=""sep"">·</span>
            <span>Valid government-issued photo ID required</span>
            <span class=""sep"">·</span>
            <span>AeroManage &copy; {DateTime.Now.Year}</span>
        </div>
    </div>
</body>
</html>";

                await File.WriteAllTextAsync(filePath, html);
                _logger.LogInformation("E-ticket generated: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating e-ticket");
                throw;
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  INVOICE
        // ══════════════════════════════════════════════════════════════

        public async Task<string> GenerateInvoiceAsync(InvoiceDto invoice)
        {
            try
            {
                var fileName = $"Invoice_{invoice.InvoiceNumber}_{DateTime.Now:yyyyMMddHHmmss}.html";
                var filePath = Path.Combine(_pdfPath, fileName);

                // ── Build line item rows ──────────────────────────────
                var rows = new System.Text.StringBuilder();

                if (invoice.Pricing is not null)
                {
                    var p = invoice.Pricing;

                    void AddRow(string label, decimal amount, bool isDiscount = false)
                    {
                        var amtHtml = isDiscount
                            ? $"<span class='discount'>-${Math.Abs(amount):F2}</span>"
                            : $"${amount:F2}";
                        rows.AppendLine($@"
                            <tr>
                                <td>{label}</td>
                                <td class=""amt"">{amtHtml}</td>
                            </tr>");
                    }

                    AddRow("Base Fare", p.BasePrice);
                    AddRow("Taxes &amp; Government Fees", p.TaxAmount);
                    AddRow("Service Fee", p.ServiceFee);
                    if (p.BaggageFee > 0) AddRow("Baggage Allowance", p.BaggageFee);
                    if (p.SeatSelectionFee > 0) AddRow("Seat Selection", p.SeatSelectionFee);
                    if (p.InsuranceFee > 0) AddRow("Travel Insurance", p.InsuranceFee);
                    if (p.DiscountAmount > 0) AddRow("Promotional Discount", p.DiscountAmount, isDiscount: true);

                    rows.AppendLine($@"
                        <tr class=""total-row"">
                            <td><strong>Total Charged</strong></td>
                            <td class=""amt""><strong>${p.TotalAmount:F2}</strong></td>
                        </tr>");
                }

                var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
    <title>Invoice · {invoice.InvoiceNumber}</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"" />
    <link href=""https://fonts.googleapis.com/css2?family=DM+Serif+Display&family=DM+Sans:wght@400;500;600&family=DM+Mono&display=swap"" rel=""stylesheet"" />
    <style>
        {GetCoreStyles()}

        /* ── Invoice header ───────────────────────────────── */
        .inv-header {{
            padding: 44px 48px 36px;
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            border-bottom: 3px solid #0f1b2d;
        }}
        .brand {{
            font-family: 'DM Serif Display', serif;
            font-size: 28px;
            color: #0f1b2d;
        }}
        .brand span {{
            font-family: 'DM Sans', sans-serif;
            font-size: 11px;
            font-weight: 500;
            letter-spacing: 2px;
            text-transform: uppercase;
            color: #8896a8;
            display: block;
            margin-top: 2px;
        }}
        .inv-meta {{ text-align: right; }}
        .inv-title {{
            font-family: 'DM Serif Display', serif;
            font-size: 36px;
            color: #0f1b2d;
            line-height: 1;
        }}
        .inv-number {{
            font-family: 'DM Mono', monospace;
            font-size: 13px;
            color: #5db3f5;
            margin-top: 6px;
            letter-spacing: 1px;
        }}

        /* ── Invoice body ─────────────────────────────────── */
        .inv-body {{ padding: 36px 48px; }}

        .meta-grid {{
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 24px;
            margin-bottom: 40px;
            padding-bottom: 32px;
            border-bottom: 1px solid #e4e9f0;
        }}
        .meta-item .mi-label {{
            font-size: 10px;
            text-transform: uppercase;
            letter-spacing: 2px;
            color: #8896a8;
            margin-bottom: 6px;
        }}
        .meta-item .mi-value {{
            font-size: 15px;
            font-weight: 600;
            color: #0f1b2d;
        }}

        /* ── Line items table ─────────────────────────────── */
        .line-items {{ width: 100%; border-collapse: collapse; }}
        .line-items thead tr {{ border-bottom: 2px solid #0f1b2d; }}
        .line-items thead th {{
            font-size: 10px;
            text-transform: uppercase;
            letter-spacing: 2px;
            color: #8896a8;
            padding: 0 0 12px;
            font-weight: 500;
            text-align: left;
        }}
        .line-items thead th.amt {{ text-align: right; }}
        .line-items tbody tr {{ border-bottom: 1px solid #f0f3f7; }}
        .line-items tbody td {{
            padding: 14px 0;
            font-size: 14px;
            color: #2c3a4a;
        }}
        .line-items .amt {{ text-align: right; font-family: 'DM Mono', monospace; }}
        .line-items .discount {{ color: #27ae60; }}
        .total-row td {{
            padding: 20px 0 0 !important;
            font-size: 16px !important;
            color: #0f1b2d !important;
            border-bottom: none !important;
            border-top: 2px solid #0f1b2d;
        }}

        /* ── Status badge ─────────────────────────────────── */
        .status-badge {{
            display: inline-block;
            background: #e8f5e9;
            color: #2e7d32;
            font-size: 11px;
            font-weight: 600;
            letter-spacing: 1.5px;
            text-transform: uppercase;
            padding: 5px 14px;
            border-radius: 20px;
            margin-top: 32px;
        }}
    </style>
</head>
<body>
    <div class=""page"">

        <!-- Invoice Header -->
        <div class=""inv-header"">
            <div class=""brand"">
                AeroManage
                <span>Aviation Services</span>
            </div>
            <div class=""inv-meta"">
                <div class=""inv-title"">Invoice</div>
                <div class=""inv-number"">#{invoice.InvoiceNumber}</div>
            </div>
        </div>

        <!-- Invoice Body -->
        <div class=""inv-body"">

            <!-- Meta grid -->
            <div class=""meta-grid"">
                <div class=""meta-item"">
                    <div class=""mi-label"">Date Issued</div>
                    <div class=""mi-value"">{invoice.InvoiceDate:dd MMM yyyy}</div>
                </div>
                <div class=""meta-item"">
                    <div class=""mi-label"">Booking Reference</div>
                    <div class=""mi-value"">{invoice.BookingReference}</div>
                </div>
                <div class=""meta-item"">
                    <div class=""mi-label"">Payment Status</div>
                    <div class=""mi-value"">Paid in Full</div>
                </div>
            </div>

            <!-- Line items -->
            <table class=""line-items"">
                <thead>
                    <tr>
                        <th>Description</th>
                        <th class=""amt"">Amount (USD)</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>

            <div>
                <span class=""status-badge"">&#10003; Payment Confirmed</span>
            </div>
        </div>

        <!-- Footer -->
        <div class=""footer"">
            <span>This is an official tax invoice issued by AeroManage</span>
            <span class=""sep"">·</span>
            <span>Retain for your records</span>
            <span class=""sep"">·</span>
            <span>AeroManage &copy; {DateTime.Now.Year}</span>
        </div>
    </div>
</body>
</html>";

                await File.WriteAllTextAsync(filePath, html);
                _logger.LogInformation("Invoice generated: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice");
                throw;
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  BOARDING PASS
        // ══════════════════════════════════════════════════════════════

        public async Task<string> GenerateBoardingPassAsync(PassengerTicketDto passenger, FlightDetailsDto flight)
        {
            try
            {
                var fileName = $"BoardingPass_{passenger.TicketNumber}_{DateTime.Now:yyyyMMddHHmmss}.html";
                var filePath = Path.Combine(_pdfPath, fileName);

                var html = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
    <title>Boarding Pass · {passenger.TicketNumber}</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"" />
    <link href=""https://fonts.googleapis.com/css2?family=DM+Serif+Display&family=DM+Sans:wght@400;500;600&family=DM+Mono&display=swap"" rel=""stylesheet"" />
    <style>
        {GetCoreStyles()}

        /* ── Boarding pass layout ─────────────────────────── */
        .bp-wrap {{
            display: flex;
            min-height: 320px;
        }}

        /* Left main section */
        .bp-main {{
            flex: 1;
            background: linear-gradient(155deg, #0f1b2d 0%, #1e3d62 100%);
            color: #fff;
            padding: 40px 44px;
            position: relative;
            overflow: hidden;
        }}
        .bp-main::after {{
            content: ""✈"";
            position: absolute;
            right: -20px; bottom: -30px;
            font-size: 180px;
            opacity: .04;
            line-height: 1;
        }}
        .bp-tag {{
            font-family: 'DM Mono', monospace;
            font-size: 9px;
            letter-spacing: 3px;
            text-transform: uppercase;
            color: #5db3f5;
            margin-bottom: 8px;
        }}
        .bp-passenger {{
            font-family: 'DM Serif Display', serif;
            font-size: 26px;
            line-height: 1.1;
            margin-bottom: 28px;
        }}
        .bp-route {{
            display: flex;
            align-items: center;
            gap: 16px;
            margin-bottom: 32px;
        }}
        .bp-iata {{
            font-family: 'DM Serif Display', serif;
            font-size: 52px;
            line-height: 1;
        }}
        .bp-arrow {{
            font-size: 22px;
            color: #5db3f5;
            flex: 1;
            text-align: center;
        }}
        .bp-details {{
            display: grid;
            grid-template-columns: repeat(3, auto);
            gap: 24px;
        }}
        .bp-detail .bd-label {{
            font-size: 9px;
            text-transform: uppercase;
            letter-spacing: 2px;
            color: rgba(255,255,255,.45);
            margin-bottom: 4px;
        }}
        .bp-detail .bd-value {{
            font-size: 15px;
            font-weight: 600;
        }}

        /* Right tear-off stub */
        .bp-stub {{
            width: 160px;
            background: #f7f9fc;
            border-left: 2px dashed #c3cdd8;
            padding: 28px 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            text-align: center;
        }}
        .stub-seat {{
            font-family: 'DM Serif Display', serif;
            font-size: 52px;
            color: #0f1b2d;
            line-height: 1;
        }}
        .stub-seat-label {{
            font-size: 9px;
            text-transform: uppercase;
            letter-spacing: 2px;
            color: #8896a8;
            margin-top: 4px;
        }}
        .stub-flight {{
            font-family: 'DM Mono', monospace;
            font-size: 14px;
            font-weight: 700;
            color: #0f1b2d;
            letter-spacing: 2px;
        }}
        .stub-date {{
            font-size: 11px;
            color: #8896a8;
            margin-top: 4px;
        }}
        .stub-logo {{
            font-family: 'DM Serif Display', serif;
            font-size: 13px;
            color: #c3cdd8;
            letter-spacing: 1px;
        }}
    </style>
</head>
<body>
    <div class=""page"" style=""padding:0;"">

        <!-- Boarding Pass Card -->
        <div class=""bp-wrap"">

            <!-- Main Section -->
            <div class=""bp-main"">
                <div class=""bp-tag"">AeroManage · Boarding Pass</div>
                <div class=""bp-passenger"">
                    {passenger.Passenger.FirstName}&nbsp;{passenger.Passenger.LastName}
                </div>

                <div class=""bp-route"">
                    <div class=""bp-iata"">{flight.Origin?.AirportCode ?? "—"}</div>
                    <div class=""bp-arrow"">&#10132;</div>
                    <div class=""bp-iata"">{flight.Destination?.AirportCode ?? "—"}</div>
                </div>

                <div class=""bp-details"">
                    <div class=""bp-detail"">
                        <div class=""bd-label"">Flight</div>
                        <div class=""bd-value"">{flight.FlightNumber ?? "—"}</div>
                    </div>
                    <div class=""bp-detail"">
                        <div class=""bd-label"">Date</div>
                        <div class=""bd-value"">{flight.DepartureDateTime:dd MMM yyyy}</div>
                    </div>
                    <div class=""bp-detail"">
                        <div class=""bd-label"">Departure</div>
                        <div class=""bd-value"">{flight.DepartureDateTime:HH:mm}</div>
                    </div>
                    <div class=""bp-detail"">
                        <div class=""bd-label"">From</div>
                        <div class=""bd-value"">{flight.Origin?.AirportName ?? flight.Origin?.AirportCode ?? "—"}</div>
                    </div>
                    <div class=""bp-detail"">
                        <div class=""bd-label"">To</div>
                        <div class=""bd-value"">{flight.Destination?.AirportName ?? flight.Destination?.AirportCode ?? "—"}</div>
                    </div>
                    <div class=""bp-detail"">
                        <div class=""bd-label"">Ticket No.</div>
                        <div class=""bd-value"" style=""font-family:'DM Mono',monospace;font-size:12px;"">{passenger.TicketNumber}</div>
                    </div>
                </div>
            </div>

            <!-- Tear-Off Stub -->
            <div class=""bp-stub"">
                <div class=""stub-logo"">AeroManage</div>
                <div>
                    <div class=""stub-seat"">{passenger.SeatNumber ?? "—"}</div>
                    <div class=""stub-seat-label"">Seat</div>
                </div>
                <div>
                    <div class=""stub-flight"">{flight.FlightNumber ?? "—"}</div>
                    <div class=""stub-date"">{flight.DepartureDateTime:dd MMM yyyy}</div>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <div class=""footer"">
            <span>Present this pass and a valid photo ID at the gate</span>
            <span class=""sep"">·</span>
            <span>Boarding closes 30 min before departure</span>
            <span class=""sep"">·</span>
            <span>AeroManage &copy; {DateTime.Now.Year}</span>
        </div>
    </div>
</body>
</html>";

                await File.WriteAllTextAsync(filePath, html);
                _logger.LogInformation("Boarding pass generated: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating boarding pass");
                throw;
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  SHARED STYLES
        //  @media print rules fire when a headless engine renders the
        //  page to PDF:
        //    • Playwright:      await page.PdfAsync(new PdfOptions { ... })
        //    • PuppeteerSharp:  await page.PdfAsync(new PdfOptions { ... })
        //    • wkhtmltopdf:     wkhtmltopdf --enable-local-file-access ...
        //    • WeasyPrint:      WeasyPrint().write_pdf(...)
        // ══════════════════════════════════════════════════════════════

        private static string GetCoreStyles() => @"
            *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

            body {
                font-family: 'DM Sans', sans-serif;
                background: #dde3ea;
                color: #2c3a4a;
                padding: 40px 20px;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            .page {
                max-width: 760px;
                margin: 0 auto;
                background: #fff;
                border-radius: 16px;
                box-shadow: 0 8px 40px rgba(15,27,45,.18), 0 2px 8px rgba(15,27,45,.08);
                overflow: hidden;
            }

            /* ── Typography ──────────────────────────────── */
            .section-title {
                font-size: 10px;
                text-transform: uppercase;
                letter-spacing: 2.5px;
                color: #8896a8;
                font-weight: 600;
                margin-bottom: 14px;
            }
            .mono { font-family: 'DM Mono', monospace; }

            /* ── Info table ──────────────────────────────── */
            .info-table { width: 100%; border-collapse: collapse; margin-bottom: 0; }
            .info-table tr { border-bottom: 1px solid #f0f3f7; }
            .info-table tr:last-child { border-bottom: none; }
            .info-table td { padding: 10px 0; font-size: 14px; vertical-align: top; }
            .info-table td.label {
                width: 38%;
                color: #8896a8;
                font-size: 12px;
                font-weight: 500;
                padding-right: 16px;
                padding-top: 11px;
            }
            .info-table td.value { color: #0f1b2d; font-weight: 500; }

            /* ── Divider ─────────────────────────────────── */
            .divider {
                border: none;
                border-top: 1px solid #e4e9f0;
                margin: 24px 0;
            }

            /* ── Footer bar ──────────────────────────────── */
            .footer {
                background: #0f1b2d;
                color: rgba(255,255,255,.5);
                font-size: 11px;
                padding: 16px 32px;
                display: flex;
                justify-content: center;
                align-items: center;
                gap: 10px;
                flex-wrap: wrap;
                letter-spacing: .3px;
            }
            .footer .sep { color: rgba(255,255,255,.2); }
            .footer strong { color: rgba(255,255,255,.8); }

            /* ── @media print ────────────────────────────── */
            @media print {
                body {
                    background: #fff;
                    padding: 0;
                    -webkit-print-color-adjust: exact;
                    print-color-adjust: exact;
                }
                .page {
                    max-width: 100%;
                    box-shadow: none;
                    border-radius: 0;
                }
                @page {
                    margin: 10mm 12mm;
                    size: A4 portrait;
                }
            }";
    }
}
