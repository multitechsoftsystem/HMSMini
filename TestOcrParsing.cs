using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

public class OcrParsingTest
{
    public static void Main()
    {
        // Sample text from the actual Aadhar OCR output
        string sampleText = @"[TUTY
ARG AR
Government of India
AR fafdree 3fe@ wrfereor
[O1a 16 TVT=M [ (Yol 1 iler=YileTg WaNU1 (g TeT 413"" il oo [E=
Aieuft shmieh:/ Enrolment No.: 2856/02001/00434
o
e gt
Kshama Prashan: Kulkari
BEHIND RT O OFFICE,
C103,DREAM HOUSE PURNA NAGAR,
VIC: CHINCHWAD,
PO: Chinchuad East,
Disrct: Pune,
St Maharashia,
PIN Code: 411019,
Mobile: 9422005779
SgnatyeTiovertied
[ v
IS TYR AT / Your Aadhaar No. :
XXXX XXXX 8722
VID : 9198 9929 34347024
ATSY HTUR, ATSY Mes
g Government of India Srare
H
3
e et
Kshama Prashant Kulkami
o i DOB: 02/11/1972
e FEMALE
? MR 51 ol G onR, AR e o A
o R TR TSI AT TR (3T S e o 1
T AT
Aadhaar is proof of identity, not of citizenship
or date of birth, It should be used with verification (online|
authentication, or scanning of QR code / offine XML).
XXXX XXXX 8722
FTST TN, HIST IS";

        Console.WriteLine("=== Testing OCR Parsing ===\n");

        var guestInfo = ExtractAadhaarInfo(sampleText);

        Console.WriteLine($"Name: {guestInfo.GuestName}");
        Console.WriteLine($"Address: {guestInfo.Address}");
        Console.WriteLine($"City: {guestInfo.City}");
        Console.WriteLine($"State: {guestInfo.State}");
        Console.WriteLine($"Country: {guestInfo.Country}");
        Console.WriteLine($"Mobile: {guestInfo.MobileNo}");
        Console.WriteLine($"ID Number: {guestInfo.IdNumber}");

        Console.WriteLine("\n=== Test Complete ===");
    }

    public class GuestInfoDto
    {
        public string GuestName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? MobileNo { get; set; }
        public string? IdNumber { get; set; }
    }

    private static GuestInfoDto ExtractAadhaarInfo(string text)
    {
        var guestInfo = new GuestInfoDto();

        // Extract Aadhaar number (12 digits, may have spaces, or masked with X)
        var aadhaarMatch = Regex.Match(text, @"(\d{4}\s*\d{4}\s*\d{4})");
        if (aadhaarMatch.Success)
        {
            guestInfo.IdNumber = aadhaarMatch.Value.Replace(" ", "");
        }

        // Process lines to extract information
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var addressLines = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Extract Mobile (pattern: Mobile: 9422005779 or Mobile 9422005779)
            var mobileMatch = Regex.Match(line, @"Mobile[\s:]+(\d{10})", RegexOptions.IgnoreCase);
            if (mobileMatch.Success)
            {
                guestInfo.MobileNo = mobileMatch.Groups[1].Value;
            }

            // Extract State (pattern: St Maharashtra, or State: Maharashtra, or just Maharashtra)
            var stateMatch = Regex.Match(line, @"(?:St|State)[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
            if (stateMatch.Success)
            {
                var stateName = stateMatch.Groups[1].Value.Trim().TrimEnd(',', '.');
                // Handle OCR errors like "Maharashia" -> "Maharashtra"
                stateName = FixStateNameOcrErrors(stateName);
                guestInfo.State = stateName;
            }

            // Extract District/City (pattern: Disrct: Pune, or District: Pune)
            var districtMatch = Regex.Match(line, @"(?:Disrct|District)[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
            if (districtMatch.Success)
            {
                guestInfo.City = districtMatch.Groups[1].Value.Trim().TrimEnd(',', '.');
            }

            // Extract PO (Post Office) as additional city info if not already found
            if (string.IsNullOrWhiteSpace(guestInfo.City))
            {
                var poMatch = Regex.Match(line, @"PO[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
                if (poMatch.Success)
                {
                    guestInfo.City = poMatch.Groups[1].Value.Trim().TrimEnd(',', '.');
                }
            }

            // Extract VIC (vicinity/area) info
            var vicMatch = Regex.Match(line, @"VIC[\s:]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
            if (vicMatch.Success && string.IsNullOrWhiteSpace(guestInfo.City))
            {
                guestInfo.City = vicMatch.Groups[1].Value.Trim().TrimEnd(',', '.');
            }
        }

        // Extract Name - look for capitalized name pattern after enrollment number or near top
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Look for name pattern - capitalized words (first letter caps)
            // Example: "Kshama Prashant Kulkami"
            if (Regex.IsMatch(line, @"^[A-Z][a-z]+\s+[A-Z][a-z]+\s+[A-Z][a-z]+") &&
                !line.Contains("Government") &&
                !line.Contains("India") &&
                line.Length > 5 && line.Length < 50)
            {
                // Clean up name - remove colons and extra text
                var cleanName = Regex.Replace(line, @"[:\(\)]", "").Trim();
                if (Regex.IsMatch(cleanName, @"^[A-Za-z\s]+$"))
                {
                    guestInfo.GuestName = cleanName;
                    break;
                }
            }
        }

        // Extract Address - collect lines between name and structured fields
        int nameLineIndex = -1;

        // Find where name appears
        for (int i = 0; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(guestInfo.GuestName) && lines[i].Contains(guestInfo.GuestName.Split(' ')[0]))
            {
                nameLineIndex = i;
                break;
            }
        }

        // Collect address lines after name
        if (nameLineIndex >= 0)
        {
            for (int i = nameLineIndex + 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                // Stop at structured fields
                if (Regex.IsMatch(line, @"(VIC|PO|Disrct|District|St |State|PIN|Mobile|DOB|Aadhaar|XXXX)", RegexOptions.IgnoreCase))
                {
                    break;
                }

                // Collect address lines
                if (line.Length > 5 && Regex.IsMatch(line, @"[A-Za-z]"))
                {
                    addressLines.Add(line.TrimEnd(','));
                }
            }
        }

        // Combine address lines
        if (addressLines.Any())
        {
            guestInfo.Address = string.Join(", ", addressLines.Take(3));
        }

        guestInfo.Country = "India";

        return guestInfo;
    }

    private static string FixStateNameOcrErrors(string stateName)
    {
        // Handle common OCR errors in state names
        var stateMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Maharashia", "Maharashtra" },
            { "Karnatka", "Karnataka" },
            { "Tamilnadu", "Tamil Nadu" },
            { "TamilNadu", "Tamil Nadu" }
        };

        foreach (var kvp in stateMap)
        {
            if (stateName.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }

        return stateName;
    }
}
