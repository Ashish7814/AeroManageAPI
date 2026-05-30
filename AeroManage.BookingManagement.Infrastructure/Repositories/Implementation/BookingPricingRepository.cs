using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class BookingPricingRepository : IBookingPricingRepository
    {
        private readonly string _connectionString;

        public BookingPricingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<BookingPricing> CreatePricingAsync(BookingPricing pricing)
        {
            using var connection = CreateConnection();

            var sql = @"
                INSERT INTO BookingPricing (
                    BookingId, BasePrice, TaxAmount, ServiceFee, BaggageFee,
                    SeatSelectionFee, InsuranceFee, DiscountAmount, PromoCode,
                    TotalAmount, Currency
                )
                VALUES (
                    @BookingId, @BasePrice, @TaxAmount, @ServiceFee, @BaggageFee,
                    @SeatSelectionFee, @InsuranceFee, @DiscountAmount, @PromoCode,
                    @TotalAmount, @Currency
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var pricingId = await connection.ExecuteScalarAsync<int>(sql, pricing);
            pricing.PricingId = pricingId;

            return pricing;
        }

        public async Task<BookingPricing> GetPricingByBookingIdAsync(int bookingId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    PricingId, BookingId, BasePrice, TaxAmount, ServiceFee,
                    BaggageFee, SeatSelectionFee, InsuranceFee, DiscountAmount,
                    PromoCode, TotalAmount, Currency, CreatedAt
                FROM BookingPricing
                WHERE BookingId = @BookingId";

            var pricing = await connection.QueryFirstOrDefaultAsync<BookingPricing>(
                sql,
                new { BookingId = bookingId }
            );

            return pricing;
        }

        public async Task<decimal> CalculateTotalPriceAsync(
            List<int> flightIds,
            List<(string PassengerType, string SeatClass, int ExtraBaggage, bool TravelInsurance)> passengers,
            string promoCode = null)
        {
            using var connection = CreateConnection();

            // Get flight prices
            decimal totalFlightPrice = 0;

            foreach (var flightId in flightIds)
            {
                var flightPrices = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT EconomyPrice, BusinessPrice, FirstClassPrice FROM Flights WHERE FlightId = @FlightId",
                    new { FlightId = flightId }
                );

                if (flightPrices != null)
                {
                    foreach (var passenger in passengers)
                    {
                        decimal passengerPrice = passenger.SeatClass switch
                        {
                            "Economy" => (decimal)flightPrices.EconomyPrice,
                            "Business" => (decimal)flightPrices.BusinessPrice,
                            "FirstClass" => (decimal)flightPrices.FirstClassPrice,
                            _ => (decimal)flightPrices.EconomyPrice
                        };

                        // Apply passenger type discount
                        if (passenger.PassengerType == "Child")
                            passengerPrice *= 0.75m; // 25% discount for children
                        else if (passenger.PassengerType == "Infant")
                            passengerPrice *= 0.10m; // 90% discount for infants

                        totalFlightPrice += passengerPrice;
                    }
                }
            }

            // Add baggage fees
            decimal baggageFee = passengers.Sum(p => p.ExtraBaggage * 50m); // $50 per extra bag

            // Add insurance
            decimal insuranceFee = passengers.Count(p => p.TravelInsurance) * 25m; // $25 per person

            // Add taxes and fees
            decimal taxAmount = totalFlightPrice * 0.12m; // 12% tax
            decimal serviceFee = 15m; // Flat $15 service fee

            decimal totalPrice = totalFlightPrice + baggageFee + insuranceFee + taxAmount + serviceFee;

            return totalPrice;
        }

        public async Task<bool> UpdatePricingAsync(BookingPricing pricing)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE BookingPricing
                SET BasePrice = @BasePrice,
                    TaxAmount = @TaxAmount,
                    ServiceFee = @ServiceFee,
                    BaggageFee = @BaggageFee,
                    SeatSelectionFee = @SeatSelectionFee,
                    InsuranceFee = @InsuranceFee,
                    DiscountAmount = @DiscountAmount,
                    PromoCode = @PromoCode,
                    TotalAmount = @TotalAmount,
                    Currency = @Currency
                WHERE PricingId = @PricingId";

            var rowsAffected = await connection.ExecuteAsync(sql, pricing);

            return rowsAffected > 0;
        }

        public async Task<ClaculatedPrice> CalculatePricingAsync(PricingCalculation request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var response = new ClaculatedPrice
                {
                    Currency = "USD",
                    Breakdown = new PricingBreakdown()
                };

                // 1. Get flight base prices
                var flightPrices = await GetFlightPricesAsync(connection, request.FlightIds);
                response.Breakdown.FlightPrices = flightPrices;

                // 2. Calculate passenger pricing with seat class multipliers
                decimal totalBasePrice = 0;
                var passengerPrices = new List<PassengerPrice>();

                for (int i = 0; i < request.Passengers.Count; i++)
                {
                    var passenger = request.Passengers[i];
                    var passengerPrice = CalculatePassengerPrice(
                        flightPrices.Sum(f => f.BasePrice),
                        passenger.PassengerType,
                        passenger.SeatClass,
                        i + 1
                    );
                    passengerPrices.Add(passengerPrice);
                    totalBasePrice += passengerPrice.FinalPrice;
                }
                response.Breakdown.PassengerPrices = passengerPrices;
                response.BasePrice = totalBasePrice;

                // 3. Calculate fees
                response.BaggageFee = request.ExtraBaggageCount * 50m; // $50 per bag
                response.InsuranceFee = request.TravelInsurance ? (request.Passengers.Count * 25m) : 0; // $25 per passenger
                response.ServiceFee = 15m; // Flat service fee
                response.SeatSelectionFee = request.Passengers.Any(p => p.SeatClass != "Economy")
                    ? request.Passengers.Count(p => p.SeatClass != "Economy") * 30m
                    : 0;

                // 4. Calculate tax (12% of base + fees before discount)
                var taxableAmount = response.BasePrice + response.BaggageFee + response.InsuranceFee + response.SeatSelectionFee;
                response.TaxAmount = Math.Round(taxableAmount * 0.12m, 2);

                // 5. Calculate subtotal before discount
                response.Subtotal = response.BasePrice + response.TaxAmount + response.ServiceFee
                    + response.BaggageFee + response.SeatSelectionFee + response.InsuranceFee;

                // 6. Apply promo code discount
                if (!string.IsNullOrEmpty(request.PromoCode))
                {
                    var discount = await GetPromoCodeDiscountAsync(connection, request.PromoCode, response.Subtotal);
                    response.DiscountAmount = discount;
                    response.PromoCode = request.PromoCode;
                }

                // 7. Calculate final total
                response.TotalAmount = response.Subtotal - response.DiscountAmount;

                // 8. Build fee breakdown
                response.Breakdown.Fees = BuildFeeBreakdown(response, request);

               // _logger.LogInformation($"Pricing calculated: ${response.TotalAmount} for {request.Passengers.Count} passengers on {request.FlightIds.Count} flights");

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating pricing" + ex.Message);
            }
        }


        public async Task<(List<BookingHistory> Bookings, int TotalCount)> GetBookingHistoryAsync(BookingHistoryFilter filter)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", filter.UserId);
                parameters.Add("@BookingReference", filter.BookingReference);
                parameters.Add("@PNR", filter.PNR);
                parameters.Add("@Status", filter.Status);
                parameters.Add("@FromDate", filter.FromDate);
                parameters.Add("@ToDate", filter.ToDate);
                parameters.Add("@Page", filter.Page);
                parameters.Add("@PageSize", filter.PageSize);
                parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var bookings = await connection.QueryAsync<BookingHistory>(
                    "sp_GetBookingHistory",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var totalCount = parameters.Get<int>("@TotalCount");

                // Load related data for each booking
                var bookingList = bookings.ToList();
                foreach (var booking in bookingList)
                {
                    booking.Flights = await GetBookingFlightsAsync(connection, booking.BookingId);
                    booking.Passengers = await GetBookingPassengersAsync(connection, booking.BookingId);
                }

                return (bookingList, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking history" + ex.Message);
            }
        }

        /*  public async Task<(List<BookingHistory> Bookings, int TotalCount)> GetBookingHistoryAsync(BookingHistoryFilter filter)
          {
              try
              {
                  using var connection = new SqlConnection(_connectionString);

                  var parameters = new DynamicParameters();
                  parameters.Add("@UserId", filter.UserId);
                  parameters.Add("@BookingReference", filter.BookingReference);
                  parameters.Add("@PNR", filter.PNR);
                  parameters.Add("@Status", filter.Status);
                  parameters.Add("@FromDate", filter.FromDate);
                  parameters.Add("@ToDate", filter.ToDate);
                  parameters.Add("@Page", filter.Page);
                  parameters.Add("@PageSize", filter.PageSize);
                  parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                  var bookings = await connection.QueryAsync<BookingHistory>(
                      "sp_GetBookingHistory",
                      parameters,
                      commandType: CommandType.StoredProcedure
                  );

                  var totalCount = parameters.Get<int>("@TotalCount");

                  // Load related data for each booking
                  var bookingList = bookings.ToList();
                  foreach (var booking in bookingList)
                  {
                      booking.Flights = await GetBookingFlightsAsync(connection, booking.BookingId);
                      booking.Passengers = await GetBookingPassengersAsync(connection, booking.BookingId);
                  }

                  return (bookingList, totalCount);
              }
              catch (Exception ex)
              {
                  throw new Exception("Error retrieving booking history" + ex.Message);
              }
          }*/

        public async Task<BookingStatistics> GetBookingStatisticsAsync(int? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@FromDate", fromDate);
                parameters.Add("@ToDate", toDate);

                var stats = await connection.QueryFirstOrDefaultAsync<BookingStatistics>(
                    "sp_GetBookingStatistics",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return stats ?? new BookingStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking statistics" + ex.Message);
            }
        }

        private async Task<List<FlightPrice>> GetFlightPricesAsync(IDbConnection connection, List<int> flightIds)
        {
            const string sql = @"
                SELECT 
                    f.FlightId,
                    f.FlightNumber,
                    CONCAT(orig.AirportCode, ' → ', dest.AirportCode) AS Route,
                    f.BasePrice
                FROM Flights f
                INNER JOIN Airports orig ON f.OriginAirportId = orig.AirportId
                INNER JOIN Airports dest ON f.DestinationAirportId = dest.AirportId
                WHERE f.FlightId IN @FlightIds";

            var flights = await connection.QueryAsync<FlightPrice>(sql, new { FlightIds = flightIds });
            return flights.ToList();
        }

        private PassengerPrice CalculatePassengerPrice(decimal baseFlightPrice, string passengerType, string seatClass, int passengerNumber)
        {
            decimal seatMultiplier = seatClass switch
            {
                "Economy" => 1.0m,
                "Business" => 2.5m,
                "FirstClass" => 4.0m,
                _ => 1.0m
            };

            decimal typeDiscount = passengerType switch
            {
                "Adult" => 0m,      // No discount
                "Child" => 0.25m,   // 25% discount
                "Infant" => 0.90m,   // 90% discount
                _ => 0m
            };

            var priceWithClass = baseFlightPrice * seatMultiplier;
            var discountAmount = priceWithClass * typeDiscount;
            var finalPrice = priceWithClass - discountAmount;

            return new PassengerPrice
            {
                PassengerNumber = passengerNumber,
                PassengerType = passengerType,
                SeatClass = seatClass,
                BasePrice = priceWithClass,
                DiscountApplied = discountAmount,
                FinalPrice = finalPrice
            };
        }

        private async Task<decimal> GetPromoCodeDiscountAsync(IDbConnection connection, string promoCode, decimal subtotal)
        {
            const string sql = @"
                SELECT DiscountType, DiscountValue, MaxDiscountAmount
                FROM PromoCodes
                WHERE Code = @Code 
                  AND IsActive = 1
                  AND (ExpiryDate IS NULL OR ExpiryDate >= GETDATE())
                  AND (UsageLimit IS NULL OR UsageCount < UsageLimit)";

            var promo = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Code = promoCode });

            if (promo == null) return 0;

            decimal discount = promo.DiscountType == "Percentage"
                ? subtotal * (promo.DiscountValue / 100m)
                : promo.DiscountValue;

            if (promo.MaxDiscountAmount != null && discount > promo.MaxDiscountAmount)
                discount = promo.MaxDiscountAmount;

            return Math.Round(discount, 2);
        }

        private List<FeeLineItem> BuildFeeBreakdown(ClaculatedPrice response, PricingCalculation request)
        {
            var fees = new List<FeeLineItem>();

            if (response.BaggageFee > 0)
                fees.Add(new FeeLineItem { Name = "Extra Baggage", Description = $"{request.ExtraBaggageCount} additional bags", Amount = response.BaggageFee });

            if (response.InsuranceFee > 0)
                fees.Add(new FeeLineItem { Name = "Travel Insurance", Description = $"{request.Passengers.Count} passengers covered", Amount = response.InsuranceFee });

            if (response.SeatSelectionFee > 0)
                fees.Add(new FeeLineItem { Name = "Premium Seats", Description = "Business/First class selection", Amount = response.SeatSelectionFee });

            fees.Add(new FeeLineItem { Name = "Service Fee", Description = "Booking processing", Amount = response.ServiceFee });
            fees.Add(new FeeLineItem { Name = "Taxes & Levies", Description = "Government charges (12%)", Amount = response.TaxAmount });

            if (response.DiscountAmount > 0)
                fees.Add(new FeeLineItem { Name = "Discount", Description = $"Promo code: {response.PromoCode}", Amount = -response.DiscountAmount });

            return fees;
        }

        private async Task<List<FlightSummary>> GetBookingFlightsAsync(IDbConnection connection, int bookingId)
        {
            const string sql = @"
                SELECT 
                    f.FlightNumber,
                    orig.AirportCode AS OriginCode,
                    orig.AirportName AS OriginName,
                    dest.AirportCode AS DestinationCode,
                    dest.AirportName AS DestinationName,
                    f.DepartureDateTime,
                    f.ArrivalDateTime
                FROM BookingFlights bf
                INNER JOIN Flights f ON bf.FlightId = f.FlightId
                INNER JOIN Airports orig ON f.OriginAirportId = orig.AirportId
                INNER JOIN Airports dest ON f.DestinationAirportId = dest.AirportId
                WHERE bf.BookingId = @BookingId
                ORDER BY f.DepartureDateTime";

            var flights = await connection.QueryAsync<FlightSummary>(sql, new { BookingId = bookingId });
            return flights.ToList();
        }

        private async Task<List<PassengerSummary>> GetBookingPassengersAsync(IDbConnection connection, int bookingId)
        {
            const string sql = @"
                SELECT 
                    p.FirstName,
                    p.LastName,
                    p.PassengerType,
                    pt.SeatNumber,
                    pt.TicketNumber
                FROM Passengers p
                INNER JOIN PassengerTickets pt ON p.PassengerId = pt.PassengerId
                WHERE p.BookingId = @BookingId
                ORDER BY p.PassengerId";

            var passengers = await connection.QueryAsync<PassengerSummary>(sql, new { BookingId = bookingId });
            return passengers.ToList();
        }

        
    }
}
