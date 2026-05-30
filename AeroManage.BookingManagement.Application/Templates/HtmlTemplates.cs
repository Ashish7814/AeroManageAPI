using AeroManage.BookingManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Templates
{
    public class HtmlTemplates
    {
       /* public string GenerateETicketHtml(ETicketDto t)
        {
            return $@"
                <html>
                <head>
                <style>
                body {{
                    font-family: Arial, Helvetica, sans-serif;
                    margin:0;
                    padding:20px;
                    font-size:12px;
                    color:#222;
                }}

                .header {{
                    display:flex;
                    justify-content:space-between;
                    align-items:center;
                }}

                .logo {{
                    font-size:26px;
                    font-weight:bold;
                    color:#b30000;
                }}

                .title {{
                    font-size:22px;
                    font-weight:bold;
                }}

                .barcode {{
                    text-align:right;
                }}

                .notice {{
                    margin-top:10px;
                    line-height:1.5;
                }}

                .section-title {{
                    background:#cfcfcf;
                    padding:6px;
                    font-weight:bold;
                    margin-top:18px;
                }}

                table {{
                    width:100%;
                    border-collapse:collapse;
                    margin-top:5px;
                }}

                th, td {{
                    padding:6px;
                    border-bottom:1px solid #ccc;
                }}

                th {{
                    text-align:left;
                    font-weight:bold;
                }}

                .right {{ text-align:right; }}

                .footer {{
                    margin-top:25px;
                    font-size:11px;
                    display:flex;
                    justify-content:space-between;
                }}

                .small {{ font-size:11px; color:#555; }}
                </style>
                </head>

                <body>

                <div class='header'>
                    <div class='logo'>AeroManage</div>
                    <div class='title'>e-Ticket Receipt & Itinerary</div>
                    <div class='barcode'>
                        <img src='{t.QRCode}' height='70'/><br/>
                        <b>{t.TicketNumber}</b>
                    </div>
                </div>

                <div class='notice small'>
                Your electronic ticket is stored in our reservation system. This receipt is your record.
                Please arrive at airport at least 2 hours before departure.
                </div>

                <div class='section-title'>PASSENGER AND TICKET INFORMATION</div>

                <table>
                <tr>
                <td><b>Passenger Name</b></td>
                <td>{t.Passenger.FirstName} {t.Passenger.LastName}</td>
                <td><b>Booking Ref</b></td>
                <td>{t.BookingReference}</td>
                </tr>

                <tr>
                <td><b>E-Ticket Number</b></td>
                <td>{t.TicketNumber}</td>
                <td><b>Issued</b></td>
                <td>{DateTime.Now:ddMMM yyyy}</td>
                </tr>
                </table>

                <div class='section-title'>TRAVEL INFORMATION</div>

                <table>
                <tr>
                <th>Flight</th>
                <th>Depart/Arrive</th>
                <th>Airport</th>
                <th>Class</th>
                <th>Status</th>
                </tr>

                <tr>
                <td>{t.Flight?.FlightNumber}</td>
                <td>{t.Flight?.DepartureDateTime:dd MMM HH:mm}</td>
                <td>{t.Flight?.Origin?.Code}</td>
                <td>Economy</td>
                <td>Confirmed</td>
                </tr>

                <tr>
                <td></td>
                <td>{t.Flight?.ArrivalDateTime:dd MMM HH:mm}</td>
                <td>{t.Flight?.Destination?.Code}</td>
                <td colspan='2'>Baggage Allowance: 30kg</td>
                </tr>

                </table>

                <div class='section-title'>FARE AND ADDITIONAL INFORMATION</div>

                <table>
                <tr>
                <td>Base Fare</td>
                <td class='right'>{t.Pricing?.BasePrice:F2}</td>
                </tr>

                <tr>
                <td>Taxes</td>
                <td class='right'>{t.Pricing?.TaxAmount:F2}</td>
                </tr>

                <tr>
                <td>Service Fee</td>
                <td class='right'>{t.Pricing?.ServiceFee:F2}</td>
                </tr>

                <tr>
                <td><b>Total</b></td>
                <td class='right'><b>{t.Pricing?.TotalAmount:F2}</b></td>
                </tr>

                <tr>
                <td>Form of Payment</td>
                <td class='right'>Card</td>
                </tr>
                </table>

                <div class='section-title'>FARE CALCULATION</div>
                <div class='small'>
                Fare calculated automatically based on booking class, route, and airline fare rules.
                </div>

                <div class='footer'>
                <div>© AeroManage. All rights reserved.</div>
                <div>Page 1 of 1</div>
                </div>

                </body>
                </html>";
        }*/

        public string GenerateInvoiceHtml(InvoiceDto i)
        {
            return $@"
        <html>
        <body style='font-family:Arial'>
            <h1 style='text-align:center'>INVOICE</h1>

            <table width='100%' border='1' cellpadding='5'>
                <tr><td><b>Invoice</b></td><td>{i.InvoiceNumber}</td></tr>
                <tr><td><b>Date</b></td><td>{i.InvoiceDate:yyyy-MM-dd}</td></tr>
                <tr><td><b>Booking Ref</b></td><td>{i.BookingReference}</td></tr>
            </table>

            <h3>Pricing</h3>
            <table width='100%' border='1' cellpadding='5'>
                <tr><td>Base</td><td align='right'>{i.Pricing.BasePrice:F2}</td></tr>
                <tr><td>Tax</td><td align='right'>{i.Pricing.TaxAmount:F2}</td></tr>
                <tr><td>Fee</td><td align='right'>{i.Pricing.ServiceFee:F2}</td></tr>
                <tr><td><b>Total</b></td><td align='right'><b>{i.Pricing.TotalAmount:F2}</b></td></tr>
            </table>
        </body>
        </html>";
        }

        public string GenerateBoardingPassHtml(PassengerTicketDto p, FlightDetailsDto f)
        {
            return $@"
        <html>
        <body style='font-family:Arial'>
            <h1 style='text-align:center'>🎫 BOARDING PASS</h1>

            <table width='100%' border='1' cellpadding='5'>
                <tr><td><b>Name</b></td><td>{p.Passenger.FirstName} {p.Passenger.LastName}</td></tr>
                <tr><td><b>Flight</b></td><td>{f.FlightNumber}</td></tr>
                <tr><td><b>Route</b></td><td>{f.Origin?.AirportCode} → {f.Destination?.AirportCode}</td></tr>
                <tr><td><b>Departure</b></td><td>{f.DepartureDateTime}</td></tr>
                <tr><td><b>Seat</b></td><td>{p.SeatNumber}</td></tr>
            </table>
        </body>
        </html>";
        }

    }
}
