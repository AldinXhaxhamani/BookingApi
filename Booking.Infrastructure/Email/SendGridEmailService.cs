using Booking.Application.Email;
using Booking.Domain.Email;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Booking.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {

            private readonly SendGridSettings _settings;

            public SendGridEmailService(IOptions<SendGridSettings> settings)
            {
                _settings = settings.Value;
            }

            public async Task SendBookingConfirmationToGuestAsync(
                BookingEmailDto booking,
                CancellationToken ct = default)
            {
                var client = new SendGridClient(_settings.ApiKey);
                var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
                var to = new EmailAddress(booking.GuestEmail, booking.GuestName);
                var subject = $"Booking Confirmed — {booking.PropertyName}";


                var plainText = BuildPlainTextEmail(booking);


                var htmlContent = BuildHtmlEmail(booking);

                var message = MailHelper.CreateSingleEmail(
                    from, to, subject, plainText, htmlContent);

                var response = await client.SendEmailAsync(message, ct);

                // throw if SendGrid returns an error
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Body.ReadAsStringAsync(ct);
                    throw new InvalidOperationException(
                        $"Failed to send email. Status: {response.StatusCode}. " +
                        $"Body: {body}");
                }
            }


            private static string BuildPlainTextEmail(BookingEmailDto b)
            {
                return $"""
                Dear {b.GuestName},

                Your booking request has been submitted successfully.

                BOOKING DETAILS
                ───────────────
                Booking ID:   {b.BookingId}
                Property:     {b.PropertyName}
                Location:     {b.PropertyCity}, {b.PropertyCountry}
                Check-in:     {b.CheckIn:dd MMM yyyy}
                Check-out:    {b.CheckOut:dd MMM yyyy}
                Nights:       {b.Nights}
                Guests:       {b.GuestCount}

                PRICE BREAKDOWN
                ───────────────
                Period price:      ${b.PriceForPeriod:F2}
                Cleaning fee:      ${b.CleaningFee:F2}
                Amenities charge:  ${b.AmenitiesUpCharge:F2}
                Service fee:       ${b.ServiceFee:F2}
                Tax (8%):          ${b.TaxAmount:F2}
                ───────────────────────────────
                Total:             ${b.TotalPrice:F2}

                STATUS
                ───────────────
                Your booking is currently PENDING.
                The owner has until {b.ExpiresAtUtc:dd MMM yyyy HH:mm} UTC
                to confirm or reject your request.

                If not confirmed within 24 hours, the booking will
                expire automatically and your dates will be freed.

                Thank you for choosing our platform.
                """;
            }


            private static string BuildHtmlEmail(BookingEmailDto b)
            {
                return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; color: #333; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; padding: 20px; }}
        .header {{ background-color: #2c3e50; color: white; padding: 20px; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 22px; }}
        .body {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .section {{ margin-bottom: 24px; }}
        .section h2 {{ font-size: 16px; color: #2c3e50; border-bottom: 1px solid #ddd; padding-bottom: 8px; }}
        .row {{ display: flex; justify-content: space-between; padding: 6px 0; font-size: 14px; }}
        .row.total {{ font-weight: bold; font-size: 16px; border-top: 2px solid #2c3e50; margin-top: 8px; padding-top: 8px; }}
        .status-badge {{ display: inline-block; background: #f39c12; color: white; padding: 6px 14px; border-radius: 20px; font-size: 13px; font-weight: bold; }}
        .footer {{ text-align: center; font-size: 12px; color: #999; margin-top: 20px; }}
        .expiry {{ background: #fff3cd; border: 1px solid #ffc107; padding: 12px; border-radius: 6px; font-size: 13px; margin-top: 16px; }}
    </style>
</head>
<body>
<div class=""container"">
    <div class=""header"">
        <h1>🏠 Booking Request Submitted</h1>
        <p style=""margin:8px 0 0 0; font-size:14px; opacity:0.85;"">
            We have received your booking request
        </p>
    </div>
    <div class=""body"">

        <div class=""section"">
            <p>Dear <strong>{b.GuestName}</strong>,</p>
            <p>Your booking request for <strong>{b.PropertyName}</strong>
               has been submitted. The owner will review and confirm shortly.</p>
            <span class=""status-badge"">⏳ PENDING CONFIRMATION</span>
        </div>

        <div class=""section"">
            <h2>📋 Booking Details</h2>
            <div class=""row""><span>Booking ID</span><span>: {b.BookingId}</span></div>
            <div class=""row""><span>Property</span><span>: {b.PropertyName}</span></div>
            <div class=""row""><span>Location</span><span>: {b.PropertyCity}, {b.PropertyCountry}</span></div>
            <div class=""row""><span>Check-in</span><span>: {b.CheckIn:dd MMM yyyy}</span></div>
            <div class=""row""><span>Check-out</span><span>: {b.CheckOut:dd MMM yyyy}</span></div>
            <div class=""row""><span>Nights</span><span>: {b.Nights}</span></div>
            <div class=""row""><span>Guests</span><span>: {b.GuestCount}</span></div>
        </div>

        <div class=""section"">
            <h2>💰 Price Breakdown</h2>
            <div class=""row""><span>Period price (: {b.Nights} nights)</span><span>${b.PriceForPeriod:F2}</span></div>
            <div class=""row""><span>Cleaning fee</span><span>$: {b.CleaningFee:F2}</span></div>
            <div class=""row""><span>Amenities charge</span><span> :${b.AmenitiesUpCharge:F2}</span></div>
            <div class=""row""><span>Service fee (10%)</span><span>$: {b.ServiceFee:F2}</span></div>
            <div class=""row""><span>Tax (8%)</span><span>$: {b.TaxAmount:F2}</span></div>
            <div class=""row total""><span>Total</span><span>$: {b.TotalPrice:F2}</span></div>
        </div>

        <div class=""expiry"">
            ⚠️ <strong>Important:</strong> The owner must confirm your booking by
            <strong>: {b.ExpiresAtUtc:dd MMM yyyy HH:mm} UTC</strong>.
            If not confirmed within 24 hours, the booking will expire automatically.
        </div>

    </div>
    <div class=""footer"">
        <p>This is an automated message. Please do not reply to this email.</p>
        <p>© : {DateTime.UtcNow.Year} Booking App. All rights reserved.</p>
    </div>
</div>
</body>
</html>";
            }
        
    }

}