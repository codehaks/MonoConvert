﻿// See https://aka.ms/new-console-template for more information
using System.Drawing;
using System.Drawing.Imaging;

Console.WriteLine("Hello, World!");

var files = System.IO.Directory.GetFiles(@"F:\pics\");

Task.Run(async () => { 

var listOfTasks=new List<Task>();

foreach (var file in files)
{
    var task= ConvertAsync(file);
    listOfTasks.Add(task);
}

await Task.WhenAll(listOfTasks);

Console.WriteLine("All files processed.");
});

while (true)
{
    await Task.Delay(1000);
    Console.WriteLine("I'm working!");

}

#pragma warning disable CA1416 // Validate platform compatibility
static async Task ConvertAsync(string path)
{
    using var originalImage = new Bitmap(path);

    // Create a new Bitmap with the same dimensions as the original image
    using var blackAndWhiteImage = new Bitmap(originalImage.Width, originalImage.Height);

    Console.WriteLine("Processing Image {0} started.",path);

    // Loop through each pixel in the original image
    for (int x = 0; x < originalImage.Width; x++)
    {
        await Task.Delay(new Random().Next(1, 10));
        for (int y = 0; y < originalImage.Height; y++)
        {
            
            // Get the color of the current pixel in the original image
            Color pixelColor = originalImage.GetPixel(x, y);

            // Calculate the grayscale value (average of red, green, and blue)
            int grayValue = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);

            // Create a grayscale color
            Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);

            // Set the pixel in the black and white image to the grayscale color
            blackAndWhiteImage.SetPixel(x, y, grayColor);

           
        }
    }

    Console.WriteLine("Processing Image {0} completed.", path);

    // Save the black and white image to a file
    using var memoryStream = new MemoryStream();

    // Save the black and white image to the MemoryStream
    blackAndWhiteImage.Save(memoryStream, ImageFormat.Jpeg);

    // Do something with the MemoryStream (e.g., send it over the network or save to another location)
    // memoryStream.ToArray() contains the image data as bytes

    var fileInfo=new FileInfo(path);

    string outputPath = Path.Combine(fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "-output"+fileInfo.Extension);

    // Save the MemoryStream to disk asynchronously
    await SaveMemoryStreamToFileAsync(memoryStream, outputPath);



}
#pragma warning restore CA1416 // Validate platform compatibility

static async Task SaveMemoryStreamToFileAsync(MemoryStream memoryStream, string outputPath)
{
    using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

    memoryStream.Position = 0;
    await memoryStream.CopyToAsync(fileStream);

}