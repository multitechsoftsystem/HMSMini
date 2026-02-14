using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

namespace OcrTest;

public class Program
{
    public static void Main(string[] args)
    {
        // Create a sample ID card image for testing
        CreateSampleAadhaarCard("sample_aadhaar.png");
        CreateSamplePanCard("sample_pan.png");

        Console.WriteLine("Sample ID card images created!");
        Console.WriteLine("- sample_aadhaar.png");
        Console.WriteLine("- sample_pan.png");
        Console.WriteLine();
        Console.WriteLine("You can now test OCR by:");
        Console.WriteLine("1. Upload: curl -X POST http://localhost:5096/api/guests/3/upload-photo -F photoNumber=1 -F file=@sample_aadhaar.png");
        Console.WriteLine("2. Process OCR: curl -X POST http://localhost:5096/api/guests/3/process-ocr?photoNumber=1");
    }

    static void CreateSampleAadhaarCard(string filename)
    {
        using (var image = new Image<Rgb24>(800, 500))
        {
            // White background
            image.Mutate(ctx => ctx.BackgroundColor(Color.White));

            var font = SystemFonts.CreateFont("Arial", 24, FontStyle.Bold);
            var smallFont = SystemFonts.CreateFont("Arial", 18);

            // Add text to simulate Aadhaar card
            var textOptions = new RichTextOptions(font)
            {
                Origin = new PointF(50, 50),
                WrappingLength = 700
            };

            image.Mutate(ctx =>
            {
                // Header
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 30) },
                    "Government of India", Color.Black);
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 60) },
                    "AADHAAR", Color.Black);

                // Name
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 120) },
                    "RAJESH KUMAR SHARMA", Color.Black);

                // DOB and Gender
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 160) },
                    "DOB: 15/08/1985", Color.Black);
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(250, 160) },
                    "Male", Color.Black);

                // Address
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 200) },
                    "S/O Mohan Lal Sharma", Color.Black);
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 230) },
                    "123 MG Road, Andheri West", Color.Black);
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 260) },
                    "Mumbai, Maharashtra - 400058", Color.Black);

                // Aadhaar number
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 350) },
                    "1234 5678 9012", Color.Black);
            });

            image.SaveAsPng(filename);
        }
    }

    static void CreateSamplePanCard(string filename)
    {
        using (var image = new Image<Rgb24>(800, 500))
        {
            // Light blue background
            image.Mutate(ctx => ctx.BackgroundColor(Color.LightBlue));

            var font = SystemFonts.CreateFont("Arial", 24, FontStyle.Bold);
            var smallFont = SystemFonts.CreateFont("Arial", 18);

            image.Mutate(ctx =>
            {
                // Header
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 30) },
                    "INCOME TAX DEPARTMENT", Color.Black);
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 65) },
                    "GOVT. OF INDIA", Color.Black);
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 95) },
                    "Permanent Account Number Card", Color.Black);

                // Name
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 160) },
                    "Name", Color.Black);
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 190) },
                    "PRIYA MEHTA", Color.Black);

                // Father's Name
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 240) },
                    "Father's Name", Color.Black);
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 270) },
                    "SURESH MEHTA", Color.Black);

                // DOB
                ctx.DrawText(new RichTextOptions(smallFont) { Origin = new PointF(50, 320) },
                    "Date of Birth: 22/03/1992", Color.Black);

                // PAN number
                ctx.DrawText(new RichTextOptions(font) { Origin = new PointF(50, 380) },
                    "ABCDE1234F", Color.Black);
            });

            image.SaveAsPng(filename);
        }
    }
}
