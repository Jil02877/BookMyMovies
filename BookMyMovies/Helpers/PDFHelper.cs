using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
namespace BookMyMovies.Helpers
{
    public static class PDFHelper
    {
        public static byte[] GenerateBookingPdf(string userName,string movie, string seats, string location, byte[] qrCodeImage)
        {

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            // Add title
            doc.Add(new Paragraph("🎟 BookMyMovies - Ticket Confirmation").SimulateBold().SetFontSize(18));

            // Add booking details
            doc.Add(new Paragraph($"Name: {userName}"));
            doc.Add(new Paragraph($"Movie: {movie}"));
            doc.Add(new Paragraph($"Seats: {seats}"));
            doc.Add(new Paragraph($"Location: {location}"));

            // Add QR Code image
            var imageData = ImageDataFactory.Create(qrCodeImage);
            var image = new Image(imageData).ScaleToFit(150, 150);
            doc.Add(image);

            doc.Close();
            return stream.ToArray();
        }
    }
}
