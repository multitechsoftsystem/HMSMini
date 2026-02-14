using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

Console.WriteLine("Creating sample ID card images for OCR testing...\n");

// Create sample images
CreateSampleAadhaarCard("sample_aadhaar.png");
CreateSamplePanCard("sample_pan.png");

Console.WriteLine("\n✓ Sample ID card images created successfully!\n");
Console.WriteLine("Files created:");
Console.WriteLine("  - sample_aadhaar.png");
Console.WriteLine("  - sample_pan.png\n");

Console.WriteLine("To test OCR with these images:");
Console.WriteLine("1. Upload Aadhaar:");
Console.WriteLine("   curl -X POST http://localhost:5096/api/guests/3/upload-photo -F photoNumber=1 -F file=@sample_aadhaar.png\n");
Console.WriteLine("2. Process OCR on Aadhaar:");
Console.WriteLine("   curl -X POST \"http://localhost:5096/api/guests/3/process-ocr?photoNumber=1\"\n");
Console.WriteLine("3. Upload PAN:");
Console.WriteLine("   curl -X POST http://localhost:5096/api/guests/4/upload-photo -F photoNumber=1 -F file=@sample_pan.png\n");
Console.WriteLine("4. Process OCR on PAN:");
Console.WriteLine("   curl -X POST \"http://localhost:5096/api/guests/4/process-ocr?photoNumber=1\"\n");

static void CreateSampleAadhaarCard(string filename)
{
    using var image = new Image<Rgb24>(1200, 800);

    // White background
    image.Mutate(ctx => ctx.BackgroundColor(Color.White));

    // Use larger, bold fonts for better OCR
    var largeFont = SystemFonts.CreateFont("Arial", 48, FontStyle.Bold);
    var mediumFont = SystemFonts.CreateFont("Arial", 36, FontStyle.Bold);
    var normalFont = SystemFonts.CreateFont("Arial", 32);

    image.Mutate(ctx =>
    {
        // Header
        ctx.DrawText(new RichTextOptions(mediumFont) { Origin = new PointF(50, 40) },
            "Government of India", Color.Black);
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 100) },
            "AADHAAR", Color.Black);

        // Name - large and clear
        ctx.DrawText(new RichTextOptions(largeFont) { Origin = new PointF(50, 180) },
            "RAJESH KUMAR SHARMA", Color.Black);

        // DOB and Gender
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 260) },
            "DOB 15/08/1985", Color.Black);
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(450, 260) },
            "Male", Color.Black);

        // Address - clear and spaced
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 340) },
            "S/O Mohan Lal Sharma", Color.Black);
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 390) },
            "123 MG Road Andheri West", Color.Black);
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 440) },
            "Mumbai Maharashtra 400058", Color.Black);

        // Aadhaar number - large and clear
        ctx.DrawText(new RichTextOptions(largeFont) { Origin = new PointF(50, 550) },
            "1234 5678 9012", Color.Black);

        // Mobile
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 660) },
            "Mobile 9876543210", Color.Black);
    });

    image.SaveAsPng(filename);
    Console.WriteLine($"Created: {filename}");
}

static void CreateSamplePanCard(string filename)
{
    using var image = new Image<Rgb24>(1200, 800);

    // White background for better OCR
    image.Mutate(ctx => ctx.BackgroundColor(Color.White));

    // Use larger, bold fonts for better OCR
    var largeFont = SystemFonts.CreateFont("Arial", 48, FontStyle.Bold);
    var mediumFont = SystemFonts.CreateFont("Arial", 36, FontStyle.Bold);
    var normalFont = SystemFonts.CreateFont("Arial", 32);

    image.Mutate(ctx =>
    {
        // Header
        ctx.DrawText(new RichTextOptions(mediumFont) { Origin = new PointF(50, 40) },
            "INCOME TAX DEPARTMENT", Color.Black);
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 100) },
            "GOVT OF INDIA", Color.Black);
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 150) },
            "Permanent Account Number Card", Color.Black);

        // Name
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 250) },
            "Name", Color.Black);
        ctx.DrawText(new RichTextOptions(largeFont) { Origin = new PointF(50, 300) },
            "PRIYA MEHTA", Color.Black);

        // Father's Name
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 400) },
            "Father's Name", Color.Black);
        ctx.DrawText(new RichTextOptions(mediumFont) { Origin = new PointF(50, 450) },
            "SURESH MEHTA", Color.Black);

        // DOB
        ctx.DrawText(new RichTextOptions(normalFont) { Origin = new PointF(50, 550) },
            "Date of Birth 22/03/1992", Color.Black);

        // PAN number - large and clear
        ctx.DrawText(new RichTextOptions(largeFont) { Origin = new PointF(50, 650) },
            "ABCDE1234F", Color.Black);
    });

    image.SaveAsPng(filename);
    Console.WriteLine($"Created: {filename}");
}
